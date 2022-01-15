using System;
using System.Threading;
using System.Net.Sockets;

namespace Clab
{
    public static partial class Network
    {
        static string serverIP;
        static TcpClient server;
        static NetworkStream serverStream;
        static readonly object clientLock = new object();

        /// <summary>accepts incoming client messages</summary>
        public static void client_inbox()
        {
            //  notify other members about connecting to the chat 
            Network.send_message($"{Network.username} has joined the Clab!", system: true);

            Generation.History.Message objMessage;
            int msgCount = Convert.ToInt32(Clab.history.get()["count"]);

            Logging.handler("info", "Client Thread Initiated", true);

            while (true)
            {
                lock (clientLock)
                {
                    if (config["isServer"] == false)
                    {
                        try
                        {
                            if (serverStream.DataAvailable)
                            {
                                objMessage = get_message_packet(serverStream);

                                if (Generation.History.check_date_paths(DateTime.Now.ToString("yyyy.MM.dd")))  //  counter was updated
                                    msgCount = Convert.ToInt32(Clab.history.get()["count"]);

                                Generation.History.handle_inbox_message(objMessage, ref msgCount);

                                Logging.handler("info", "Message Received", true);
                            }
                        }
                        catch (Exception)
                        {
                            Logging.handler("error", "Client Inbox Read Closed Socket", true);
                        }
                    }
                    else break;
                }
                
                Thread.Sleep(50);
            }
        }
    }
}
