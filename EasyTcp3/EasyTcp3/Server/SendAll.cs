using System;
using System.Text;
using EasyTcp3.Client;

namespace EasyTcp3.Server
{
    public static class _SendAll
    {
        /// <summary>
        /// Send data (byte[]) to all connected clients
        /// </summary>
        /// <param name="data">Data to send to all connected clients</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid server</exception>
        ///
        /// <example>
        /// ushort port = TestHelper.GetPort();
        /// using var server = new EasyTcpServer();
        /// server.Start(IPAddress.Any, port);
        ///    
        /// var client = new EasyTcpClient();
        /// client.Connect(IPAddress.Any, port);
        ///
        /// byte[] data = new byte[100];
        /// server.SendAll(data);
        /// </example>
        public static void SendAll(this EasyTcpServer server, byte[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Could not send data: Data array is empty");
            if (server == null || !server.IsRunning)
                throw new Exception("Could not send data: Server not running or null");
            
            foreach (var client in server.GetConnectedClients()) client.Send(data);
        }
        
        /// <summary>
        /// Send data (ushort) to all connected clients
        /// </summary>
        /// <param name="data">Data to send to all connected clients</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid server</exception>
        ///
        /// <example>
        /// ushort port = TestHelper.GetPort();
        /// using var server = new EasyTcpServer();
        /// server.Start(IPAddress.Any, port);
        ///    
        /// var client = new EasyTcpClient();
        /// client.Connect(IPAddress.Any, port);
        ///
        /// ushort data = 123;
        /// server.SendAll(data);
        /// </example>
        public static void SendAll(this EasyTcpServer server, ushort data) => server.SendAll(BitConverter.GetBytes(data));
        
        /// <summary>
        /// Send data (short) to all connected clients
        /// </summary>
        /// <param name="data">Data to send to all connected clients</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid server</exception>
        ///
        /// <example>
        /// ushort port = TestHelper.GetPort();
        /// using var server = new EasyTcpServer();
        /// server.Start(IPAddress.Any, port);
        ///    
        /// var client = new EasyTcpClient();
        /// client.Connect(IPAddress.Any, port);
        ///
        /// short data = 123;
        /// server.SendAll(data);
        /// </example>
        public static void SendAll(this EasyTcpServer server, short data) => server.SendAll(BitConverter.GetBytes(data));
        
        /// <summary>
        /// Send data (uint) to all connected clients
        /// </summary>
        /// <param name="data">Data to send to all connected clients</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid server</exception>
        ///
        /// <example>
        /// ushort port = TestHelper.GetPort();
        /// using var server = new EasyTcpServer();
        /// server.Start(IPAddress.Any, port);
        ///    
        /// var client = new EasyTcpClient();
        /// client.Connect(IPAddress.Any, port);
        ///
        /// uint data = 123;
        /// server.SendAll(data);
        /// </example>
        public static void SendAll(this EasyTcpServer server, uint data) => server.SendAll(BitConverter.GetBytes(data));
        
        /// <summary>
        /// Send data (int) to all connected clients
        /// </summary>
        /// <param name="data">Data to send to all connected clients</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid server</exception>
        ///
        /// <example>
        /// ushort port = TestHelper.GetPort();
        /// using var server = new EasyTcpServer();
        /// server.Start(IPAddress.Any, port);
        ///    
        /// var client = new EasyTcpClient();
        /// client.Connect(IPAddress.Any, port);
        ///
        /// int data = 123;
        /// server.SendAll(data);
        /// </example>
        public static void SendAll(this EasyTcpServer server, int data) => server.SendAll(BitConverter.GetBytes(data));
        
        /// <summary>
        /// Send data (ulong) to all connected clients
        /// </summary>
        /// <param name="data">Data to send to all connected clients</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid server</exception>
        ///
        /// <example>
        /// ushort port = TestHelper.GetPort();
        /// using var server = new EasyTcpServer();
        /// server.Start(IPAddress.Any, port);
        ///    
        /// var client = new EasyTcpClient();
        /// client.Connect(IPAddress.Any, port);
        ///
        /// ulong data = 123;
        /// server.SendAll(data);
        /// </example>
        public static void SendAll(this EasyTcpServer server, ulong data) => server.SendAll(BitConverter.GetBytes(data));
        
        /// <summary>
        /// Send data (long) to all connected clients
        /// </summary>
        /// <param name="data">Data to send to all connected clients</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid server</exception>
        ///
        /// <example>
        /// ushort port = TestHelper.GetPort();
        /// using var server = new EasyTcpServer();
        /// server.Start(IPAddress.Any, port);
        ///    
        /// var client = new EasyTcpClient();
        /// client.Connect(IPAddress.Any, port);
        ///
        /// long data = 123;
        /// server.SendAll(data);
        /// </example>
        public static void SendAll(this EasyTcpServer server, long data) => server.SendAll(BitConverter.GetBytes(data));
        
        /// <summary>
        /// Send data (double) to all connected clients
        /// </summary>
        /// <param name="data">Data to send to all connected clients</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid server</exception>
        ///
        /// <example>
        /// ushort port = TestHelper.GetPort();
        /// using var server = new EasyTcpServer();
        /// server.Start(IPAddress.Any, port);
        ///    
        /// var client = new EasyTcpClient();
        /// client.Connect(IPAddress.Any, port);
        ///
        /// double data = 123.0;
        /// server.SendAll(data);
        /// </example>
        public static void SendAll(this EasyTcpServer server, double data) => server.SendAll(BitConverter.GetBytes(data));
        
        /// <summary>
        /// Send data (bool) to all connected clients
        /// </summary>
        /// <param name="data">Data to send to all connected clients</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid server</exception>
        ///
        /// <example>
        /// ushort port = TestHelper.GetPort();
        /// using var server = new EasyTcpServer();
        /// server.Start(IPAddress.Any, port);
        ///    
        /// var client = new EasyTcpClient();
        /// client.Connect(IPAddress.Any, port);
        ///
        /// bool data = true;
        /// server.SendAll(data);
        /// </example>
        public static void SendAll(this EasyTcpServer server, bool data) => server.SendAll(BitConverter.GetBytes(data));

        /// <summary>
        /// Send data (string) to all connected clients
        /// </summary>
        /// <param name="data">Data to send to all connected clients</param>
        /// <param name="encoding">Encoding type (Default: UTF8)</param>
        /// <exception cref="ArgumentException">Data array is empty or invalid server</exception>
        /// 
        /// <example>
        /// ushort port = TestHelper.GetPort();
        /// using var server = new EasyTcpServer();
        /// server.Start(IPAddress.Any, port);
        ///    
        /// var client = new EasyTcpClient();
        /// client.Connect(IPAddress.Any, port);
        /// 
        /// string data = "Data";
        /// server.SendAll(data);
        /// server.SendAll(data, Encoding.UTF32); //Send with different encoding
        /// </example>
        public static void SendAll(this EasyTcpServer server, string data, Encoding encoding = null)
            => server.SendAll((encoding??Encoding.UTF8).GetBytes(data));
    }
}