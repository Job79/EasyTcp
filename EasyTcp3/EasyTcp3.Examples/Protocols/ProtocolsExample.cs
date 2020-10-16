using System.Security.Cryptography.X509Certificates;
using EasyEncrypt2;
using EasyTcp3.Encryption.Protocols.Tcp;
using EasyTcp3.Encryption.Protocols.Tcp.Ssl;
using EasyTcp3.Protocols.Tcp;
using EasyTcp3.ServerUtils;

namespace EasyTcp3.Examples.Protocols
{
    /* This class contains examples of how to use different protocols with EasyTcp
     * A protocol defines all the behavior of an EasyTcpClient and EasyTcpServer
     * Currently EasyTcp has only protocols with different types of framing and encryption
     * See this example for all different types of protocols included in EasyTcp & EasyTcp.Encryption
     *
     * See code of EasyTcp when implementing a new protocol (Protocols/IEasyTcpProtocol & Protocols/Tcp/DefaultTcpProtocol) 
     */
    public class ProtocolsExample
    {
        private const ushort Port = 5_102;
        
        public void Start()
        {
            /* Prefix length protocol, (Default when not specified)
             * prefixes all data with its length. Length is a ushort as byte[2]
             * Max data size is 65.535 bytes. See LargeArray or streams examples for large data
             *
             * Example message:
             *     data: "data"
             *     length: 4 bytes
             * 
             *     message: (ushort as byte[2]) 4 + "data"
             */
            using var defaultProtocol = new PrefixLengthProtocol();
            
            /* Delimiter protocol, 
             * determines the end of a message based on a sequence of bytes 
             *
             * Example message:
             *     data: "data"
             *     delimiter: "\r\n"
             * 
             *     message: "data" + "\r\n"
             */
            bool autoAddDelimiter = true; // Determines whether to automatically add the delimiter to the end of a message before sending
            bool autoRemoveDelimiter = true; // Determines whether to automatically remove the delimiter when triggering the OnDataReceive event
            using var delimiterProtocol = new DelimiterProtocol("\r\n", autoAddDelimiter, autoRemoveDelimiter);

            /* None protocol,
             * doesn't determine the end of a message
             * Reads all available bytes into 1 byte[]
             * Doesn't work with ReceiveStream/ReceiveLargeArray
             *
             * Example message:
             *     data: "data"
             *     message: "data"
             */
            int bufferSize = 1024; // Max data(chunk) size 
            var nonProtocol = new NoneProtocol(bufferSize);
            
            // Create client that uses a specific protocol
            using var client = new EasyTcpClient(nonProtocol);
            
            // Create a server that uses a specific protocol
            using var server = new EasyTcpServer(nonProtocol).Start(Port);
            
            /*             EasyTcp.Encryption
             * 
             * Every protocol above is available with ssl
             * PrefixLengthSslProtocol, DelimiterSslProtocol & NoneSslProtocol
             * All ssl protocols have some extra parameters
             */
            
            // Client ssl protocol
            using var defaultClientSslProtocol = new PrefixLengthSslProtocol("localhost", acceptInvalidCertificates: false);
            
            // Server ssl protocol
            using var certificate = new X509Certificate2("certificate.pfx", "password");
            using var defaultServerSslProtocol = new PrefixLengthSslProtocol(certificate);
            
            /* The helper method client/server.UseSsl() as seen in Encryption.SslExample uses PrefixLengthSslProtocol
             * Use constructor with custom ssl protocol for DelimiterSslProtocol & NoneSslProtocol
             *
             * 
             * EncryptedPrefixLengthProtocol,
             * this protocol encrypts all data with EasyEncrypt
             * See Encryption/CustomAlgorithmProtocol for more info
             * There is no Delimiter/None protocol for encryption with EasyEncrypt
             */
            
            var encrypter = new EasyEncrypt();
            using var encryptedPrefixLengthProtocol = new EncryptedPrefixLengthProtocol(encrypter);
            
        }
    }
}