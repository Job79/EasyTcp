using EasyTcp3.Protocols;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Protocols
{
    /// <summary>
    /// This class contains examples of how to use different protocols with EasyTcp
    /// ! With protocols is not meant UDP etc...
    /// This class is not intended to execute
    ///
    /// See EasyTcp3/Protocols/IEasyTcpProtocol when implementing a new protocol
    /// Code is well documented, but feel free to open an issue when still unclear
    /// </summary>
    public class ProtocolsExample
    {
        private const ushort Port = 5_102;
        
        public void Start()
        {
            /*
             * The default protocol,
             * prefixes all data with its length. Length is a ushort as byte[2]
             * Example data: [4 : ushort as byte[2]] ["data"]
             */
            var defaultProtocol = new PrefixLengthProtocol();
            
            /*
             * Delimiter protocol,
             * adds a sequence of bytes to the end of every message.
             * Determines the end of a message based on the delimiter.
             * Example data (Delimiter: "\n"): ["Data\r\n"]
             */
            bool autoAddDelimiter = true; // Determines whether to automatically add the delimiter to the end of a message while sending
            bool autoRemoveDelimiter = true; // Determines whether to automatically remove the delimiter when triggering the OnDataReceive event
            var delimiterProtocol = new DelimiterProtocol("\r\n", autoAddDelimiter, autoRemoveDelimiter);

            /*
             * NoneProtocol,
             * This 'protocol' doesn't add any data to the message.
             * It has a maximum data size (Because the receiving end does not know the message size)
             * BufferSize gets allocated every time receiving a message, even when message is smaller (so do not set this extremely high when not needed)
             * ! When data is bigger then the max size it is split into 2 messages
             * ! Data gets merged when sending very fast in a row
             * ! This protocol has some issues with sendStream. Bytes are captured in buffer when possible.
             * This protocol is handy when communicating with an already existing server
             */
            int bufferSize = 1024; // Max data size 
            var nonProtocol = new NoneProtocol(bufferSize);
            
            // Create client that uses a specific protocol
            var client = new EasyTcpClient(nonProtocol);
            
            // Create a server that uses a specific protocol
            var server = new EasyTcpServer(nonProtocol).Start(Port);
        }
    }
}