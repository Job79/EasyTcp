using System;
using System.Linq;
using EasyEncrypt2;
using EasyTcp3;
using EasyTcp3.Protocols;

namespace EasyTcp.Encryption.Protocols
{
    /// <summary>
    /// This protocol extends PrefixLengthProtocol
    /// It works the same, but encrypts all data before sending it and decrypts before triggering events
    /// Example:
    /// [(SIZE DEPENDS ON ALGORITHM) : ushort as byte[2]] Encrypted(["data"]),
    /// [SIZE DEPENDS ON ALGORITHM : ushort as byte[2]] Encrypted(["exampleData"])
    /// </summary>
    public class EncryptedPrefixLengthProtocol : PrefixLengthProtocol
    {
        /// <summary>
        /// encrypter instance, used 
        /// </summary>
        private readonly EasyEncrypt _encrypter;

        /// <summary>
        /// </summary>
        /// <param name="encrypter"></param>
        public EncryptedPrefixLengthProtocol(EasyEncrypt encrypter)
            => _encrypter = encrypter;

        /// <summary>
        /// Create a new message from 1 or multiple byte arrays
        ///
        /// [length of data[][] : ushort as byte[2]] Encrypted([data[] + data1[] + data2[]...])
        /// </summary>
        /// <param name="data">data to send to server</param>
        /// <returns>encrypted byte array with merged data + length: [data length : ushort as byte[2]] Encrypted([data])</returns>
        /// <exception cref="ArgumentException">could not create message: Data array is empty</exception>
        public override byte[] CreateMessage(params byte[][] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Could not create message: Data array is empty");

            // Calculate length of message
            var dataLength = data.Sum(t => t?.Length ?? 0);
            if (dataLength == 0)
                throw new ArgumentException("Could not create message: Data array only contains empty arrays");
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
            var encryptedData = _encrypter.Encrypt(mergedData);
            var message = new byte[2 + encryptedData.Length];
            Buffer.BlockCopy(BitConverter.GetBytes((ushort) encryptedData.Length), 0, message, 0, 2);
            Buffer.BlockCopy(encryptedData, 0, message, 2, encryptedData.Length);
            return message;
        }

        /// <summary>
        /// Handle received data, trigger event and set new bufferSize determined by ReceivingData 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="receivedBytes">ignored</param>
        /// <param name="client"></param>
        public override void DataReceive(byte[] data, int receivedBytes, EasyTcpClient client)
        {
            ushort dataLength = 2;

            if (ReceivingLength) dataLength = BitConverter.ToUInt16(client.Buffer, 0);
            else
            {
                try
                {
                    client.DataReceiveHandler(new Message(client.Buffer, client).Decrypt(_encrypter));
                }
                catch
                {
                   OnDecryptionError(client); 
                }
            }

            ReceivingLength = !ReceivingLength;

            if (dataLength == 0) client.Dispose();
            else BufferSize = dataLength;
        }

        /// <summary>
        /// Dispose client
        /// </summary>
        /// <param name="client"></param>
        protected virtual void OnDecryptionError(EasyTcpClient client) => client.Dispose();

        /// <summary>
        /// Return new instance of this protocol 
        /// </summary>
        /// <returns>new object</returns>
        public override object Clone() => new EncryptedPrefixLengthProtocol(_encrypter);
    }
}