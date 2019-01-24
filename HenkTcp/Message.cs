/* HenkTcp
 * Copyright (C) 2019  henkje (henkje@pm.me)
 * 
 * MIT license
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System;

namespace HenkTcp
{
    public class Message
    {
        public readonly TcpClient TcpClient;

        private readonly byte[] _EncryptionKey;
        private readonly SymmetricAlgorithm _Algorithm;
        private readonly Encoding _Encoding;

        public Message(byte[] Data, TcpClient TcpClient, SymmetricAlgorithm Algorithm, byte[] EncryptionKey, Encoding Encoding)
        {
            this.Data = Data;
            this.TcpClient = TcpClient;

            _Encoding = Encoding;
            _EncryptionKey = EncryptionKey;
            _Algorithm = Algorithm;
        }

        public readonly byte[] Data;
        public byte[] DecryptedData
        {
            get
            {
                if (_EncryptionKey == null || _Algorithm == null) throw new Exception("Alghoritm/Key not set");
                return Encryption.Decrypt(_Algorithm, Data, _EncryptionKey);
            }
        }

        public string MessageString { get { return _Encoding.GetString(Data); } }
        public string DecryptedMessageString { get { return _Encoding.GetString(DecryptedData); } }

        public string ClientIP { get { return ((IPEndPoint)TcpClient.Client.RemoteEndPoint).Address.ToString(); } }

        public void Reply(string data)=> Reply(_Encoding.GetBytes(data));
        public void Reply(byte[] data)=> TcpClient.GetStream().Write(data, 0, data.Length);

        public void ReplyEncrypted(string Data) => ReplyEncrypted(_Encoding.GetBytes(Data)); 
        public void ReplyEncrypted(byte[] Data)
        {
            if (_EncryptionKey == null || _Algorithm == null) throw new Exception("Alghoritm/Key not set");
            Reply(Encryption.Encrypt(_Algorithm, Data, _EncryptionKey));
        }
    }
}
