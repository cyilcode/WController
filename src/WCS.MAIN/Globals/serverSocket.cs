using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WCS.MAIN.Functions;

namespace WCS.MAIN.Globals
{
    public class serverSocket
    {
        private readonly FunctionHandler                 _functions;
        private readonly Dictionary<int, Func<object>>   _actionTable;
        private readonly GlobalHelper                    _gHelper;
        private byte[] _globalBuffer                     = new byte[1];
        private const sbyte g_bArrayOffset               = 0;
        private Socket g_sckServer                       = new Socket(AddressFamily.InterNetwork, 
                                                                      SocketType.Stream, 
                                                                      ProtocolType.Tcp);

        public serverSocket(FunctionHandler fnc, GlobalHelper hlp)
        {
            _gHelper        = hlp;
            _functions      = fnc;
            _actionTable    = _gHelper.InitActionTable(_functions);
        }

        public void startServer(IPEndPoint ipEndpoint)
        {
            try
            {
                g_sckServer.Bind(ipEndpoint);
                g_sckServer.Listen(0);
                g_sckServer.BeginAccept(SocketAcceptCallback, null);
            }
            catch (SocketException e)
            {
                _gHelper.coloredLine(string.Format("Socket Error: {0}", e.Message), ConsoleColor.Red);
            }
        }

        private void SocketAcceptCallback(IAsyncResult ar)
        {
            var inSocket = g_sckServer.EndAccept(ar);
            inSocket.BeginReceive(_globalBuffer, 
                                  g_bArrayOffset, 
                                  _globalBuffer.Length, 
                                  SocketFlags.None, 
                                  SocketRecieveCallback, inSocket);
            g_sckServer.BeginAccept(SocketAcceptCallback, null);
        }

        private void SocketRecieveCallback(IAsyncResult ar)
        {
            var inSocket = (Socket)ar.AsyncState;
            var recievedBytes = inSocket.EndReceive(ar);
            // TODO: Apply commands \\
            _gHelper.coloredLine("Data arrived", ConsoleColor.White);
            executeCommand(_globalBuffer[0]);
            // TEST
            var testResponse = Encoding.UTF8.GetBytes("TEST");
            inSocket.BeginSend(testResponse, 
                               g_bArrayOffset, 
                               testResponse.Length, 
                               SocketFlags.None, 
                               SocketSendCallback, inSocket);
            // TEST
        }

        private void executeCommand(byte pktHeader)
        {
            if (_actionTable.Keys.Contains(pktHeader))
                _actionTable[pktHeader].Invoke();
            else
            {
                string message = string.Format("Couldn't find any function mapped to {0}", pktHeader);
                GlobalHelper.log(message);
                _gHelper.coloredLine(message, ConsoleColor.Yellow); // for now..
            }
        }

        private void SocketSendCallback(IAsyncResult ar)
        {
            var inSocket = (Socket)ar.AsyncState;
            var result = inSocket.EndSend(ar);
            inSocket.Dispose();
        }
    }
}
