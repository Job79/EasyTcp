using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using EasyEncrypt2;
using EasyTcp3.Protocols.Tcp;

namespace EasyTcp3.Encryption.Protocols.Tcp
{
    /// <summary>
    /// Protocol that determines the length of a message based on a small header
    /// Header is an ushort as byte[] with the length of the encrypted incoming message
    ///
    /// All data is encrypted before sending to remote host
    /// All received data is decrypted before triggering OnDataReceive
    /// </summary>
    public class EncryptedPrefixLengthProtocol : PrefixLengthProtocol
    {
        /// <summary>
        /// Encrypter instance, used to encrypt and decrypt data 
        /// </summary>
        protected readonly EasyEncrypt Encrypter;

        /// <summary></summary>
        /// <param name="encrypter"></param>
        /// <param name="maxMessageLength">TODO</param>
        public EncryptedPrefixLengthProtocol(EasyEncrypt encrypter, int maxMessageLength = ushort.MaxValue) : base(maxMessageLength)
            => Encrypter = encrypter;

        /// <summary>
        /// Get receiving/sending stream
        /// </summary>
        /// <returns></returns>
        public override Stream GetStream(EasyTcpClient client)
        {
            Debug.WriteLine("EasyTcp: Stream encryption in currently not supported, continuing with not-encrypted stream");
            return new NetworkStream(client.BaseSocket); 
        }

        /// <summary>
        /// Create a new message from 1 or multiple byte arrays
        /// returned data will be send to remote host
        /// </summary>
        /// <param name="data">data of message</param>
        /// <returns>data to send to remote host</returns>
        public override byte[] CreateMessage(params byte[][] data)
        {
            if (data == null || data.Length == 0) throw new ArgumentException("Could not create message: Data array is empty");

            // Calculate length of message
            var dataLength = data.Sum(t => t?.Length ?? 0);
            if (dataLength == 0) throw new ArgumentException("Could not create message: Data array only contains empty arrays");
            byte[] mergedData = new byte[dataLength];

            // Add data to message
            int offset = 0;
            foreach (var d in data)
            {
                if (d == null) continue;
                Buffer.BlockCopy(d, 0, mergedData, offset, d.Length);
                offset += d.Length;
            }
            
            // Encrypt and create message
            var encryptedData = Encrypter.Encrypt(mergedData);
            var message = new byte[(Extended ? 4 : 2) + encryptedData.Length];

            if(Extended) Buffer.BlockCopy(BitConverter.GetBytes((int) encryptedData.Length), 0, message, 0, 4);
            else Buffer.BlockCopy(BitConverter.GetBytes((ushort) encryptedData.Length), 0, message, 0, 2);
            Buffer.BlockCopy(encryptedData, 0, message, Extended ? 4 : 2, encryptedData.Length);

            if (message.Length > ushort.MaxValue)
                throw new ArgumentException("Could not create message: Message can't be created & send because it is too big. Send message with the LargeArrayUtil, StreamUtil or use another protocol.");
            return message;
        }
 
        /// <summary>
        /// Handle received data
        /// </summary>
        /// <param name="data">received data, has size of clients buffer</param>
        /// <param name="receivedBytes">amount of received bytes</param>
        /// <param name="client"></param>
        public override async Task DataReceive(byte[] data, int receivedBytes, EasyTcpClient client)
        {
            if(ReceivingLength)
            {
                BufferSize = Extended ? BitConverter.ToInt32(data, 0) : BitConverter.ToUInt16(data, 0);
                if (BufferSize == 0) client.Dispose();
                BufferCount = Math.Min(BufferSize, MaxBufferCount);
                ReceivingLength = false;
            }
            else
            {
                if(BufferOffset + receivedBytes == BufferSize)
                {
                    BufferSize = Extended ? 4 : 2;
                    BufferOffset = 0;
                    BufferCount = BufferSize;
                    ReceivingLength = true;
                    try { await client.DataReceiveHandler(new Message(data, client).Decrypt(Encrypter)); }
                    catch { OnDecryptionError(client); }
                }
                else
                {
                    BufferOffset += receivedBytes;
                    BufferCount = Math.Min(BufferSize - BufferOffset, MaxBufferCount);
                }
            }
        }
        
        /// <summary>
        /// Dispose instance of Encrypter
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            Encrypter?.Dispose();
        }

        /// <summary>
        /// Return new instance of protocol 
        /// </summary>
        /// <returns>new object</returns>
        public override object Clone() => new EncryptedPrefixLengthProtocol(new EasyEncrypt(key: Encrypter.GetKey()));

        /*
         * Internal
         */
        
        /// <summary>
        /// Handle decryption error 
        /// </summary>
        /// <param name="client"></param>
        protected virtual void OnDecryptionError(EasyTcpClient client) => HandleDisconnect(client);
    }
}
