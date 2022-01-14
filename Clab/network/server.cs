using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace Clab
{
    public static partial class Network
    {
		static TcpListener clientsListener;
		static Dictionary<string, TcpClient> clients;
		static readonly object serverLock = new object();

		/// <summary>establishes TCP connections with other chat participants</summary>
		public static void server_connections_handler()
		{
			TcpClient client;

			Logging.handler("info", "Server Connections Handler Thread Initiated", true);

			while (true)
			{
				lock (serverLock)
				{
					if (config["isServer"] == true)
					{
						if (clientsListener.Pending())
                        {
							client = clientsListener.AcceptTcpClient();
							clients.Add(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(), client);
						}
					}
					else break;
				}
				Thread.Sleep(50);
			}
		}

		/// <summary>accepts incoming messages and broadcasts them by TCP</summary>
		public static void server_broadcast_service()
		{
			NetworkStream clientStream;
			Generation.History.Message objMessage;
			int msgCount = Convert.ToInt32(Clab.history.get()["count"]);

			Logging.handler("info", "Server Broadcast Service Thread Initiated", true);

			while (true)
			{
				lock (serverLock)
				{
					if (config["isServer"] == true)
					{
						try
						{
							foreach (KeyValuePair<string, TcpClient> client in clients)
							{
								clientStream = client.Value.GetStream();

								if (clientStream.DataAvailable)
								{
									objMessage = get_message_packet(clientStream, true, get_recipients(client.Key));  //  auto forwarding

									if (Generation.History.check_date_paths(DateTime.Now.ToString("yyyy.MM.dd")))     //  counter was updated
										msgCount = Convert.ToInt32(Clab.history.get()["count"]);

									Generation.History.handle_inbox_message(objMessage, ref msgCount);
									Logging.handler("info", "Message Broadcasted", true);
								}
							}
						}
						catch (Exception)
						{
							Logging.handler("error", "Broadcast Service Read Closed Socket", true);
						}
					}
					else break;
				}

				Thread.Sleep(50);
			}
		}
	}
}
