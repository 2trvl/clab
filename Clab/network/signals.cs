using System;
using System.Net;
using System.Text;
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Clab
{
    /// <summary>signals exchanged between PCs to discover each other and build a client/server chain</summary>
    public abstract class Signals
    {
        public static string connect = "Connect";
        public static string request = "Request";
        public static string callback = "Callback";
        public static string disconnect = "Disconnect";
    }

    public static partial class Network
    {
        static IPEndPoint sender;
        static UdpClient signals;
        static IPAddress signalsIP = IPAddress.Parse("224.100.0.1");

        static List<string> members = new List<string> { IP };
        static Dictionary<string, bool?> config = new Dictionary<string, bool?> {
                                       { "isServer", null },
                                       { "exit", false } };

        static TcpListener filesListener;
        static List<TcpClient> files;
        static readonly object filesLock = new object();

        static Network()
        {
            signals = new UdpClient();
            signals.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            signals.Client.Bind(new IPEndPoint(IPAddress.Any, ports["signals"]));
            signals.JoinMulticastGroup(signalsIP, IPAddress.Parse(Network.IP));
        }

        public static void run()
        {
            //  if the network is silent for 5 seconds, the PC becomes a server
            Task.Delay(5000).ContinueWith(timer => run_server());

            new Thread(new ThreadStart(signals_handler)).Start();
            send_signal(Signals.connect);
            Logging.handler("info", "Searching For Devices On The Net", true);
        }

        public static void run_server()
        {
            if (config["isServer"] == null)
            {
                config["isServer"] = true;
                serverIP = IP;
                run_files();
                clientsListener = new TcpListener(IPAddress.Any, ports["messages"]);
                clients = new Dictionary<string, TcpClient>();
                clientsListener.Start();

                new Thread(new ThreadStart(server_connections_handler)).Start();
                new Thread(new ThreadStart(server_broadcast_service)).Start();

                send_signal(Signals.callback);
                Clab.chat.add_message_to_box("\nWow, you are the server now!", AppMessages.info);
            }
        }

        public static void run_client(string address)
        {
            config["isServer"] = false;
            serverIP = address;
            run_files();
            server = new TcpClient(address, ports["messages"]);
            serverStream = server.GetStream();

            new Thread(new ThreadStart(client_inbox)).Start();
            Clab.chat.add_message_to_box("\nYou're connected to chat", AppMessages.info);
        }

        /// <summary>starts the file sharing system</summary>
        public static void run_files()
        {
            filesListener = new TcpListener(IPAddress.Any, ports["files"]);
            files = new List<TcpClient>();
            filesListener.Start();

            new Thread(new ThreadStart(files_connections_handler)).Start();
        }

        public static void signals_handler()
        {
            while (config["exit"] == false)
            {
                if (signals.Available > 0)
                {
                    byte[] data = signals.Receive(ref sender);
                    string signal = Encoding.UTF8.GetString(data);
                    string address = sender.Address.ToString();

                    if (!is_my_IP(address))
                    {
                        if (signal == Signals.connect)
                        {
                            if (!members.Contains(address))
                            {
                                members.Add(address);
                                send_signal(Signals.connect);

                                if (config["isServer"] == true)
                                    send_signal(Signals.callback);
                            }

                            else if (config["isServer"] == null)
                                send_signal(Signals.request);

                            Logging.handler("info", "Connect Signal Processed", true);
                        }

                        else if (signal == Signals.disconnect)
                        {
                            if (serverIP == address)
                            {
                                lock (clientLock) lock (filesLock)
                                    {
                                        config["isServer"] = null;
                                        serverIP = null;
                                        server.Close();
                                        filesListener.Stop();
                                    }
                                send_signal(Signals.request);
                                //  if there is no one on the network, PC will become a server 
                                Task.Delay(5000).ContinueWith(timer => run_server());
                            }
                            members.Remove(address);

                            if (config["isServer"] == true)
                            {
                                lock (serverLock)
                                {
                                    clients[address].Close();
                                    clients.Remove(address);
                                }
                            }

                            Logging.handler("info", $"Disconnect [{address}] Signal Processed", true);
                        }

                        else if (signal == Signals.callback && config["isServer"] == null)
                        {
                            run_client(address);
                            Logging.handler("info", $"Callback [{address}] Signal Processed", true);
                        }

                        else if (signal == Signals.request && config["isServer"] == null)
                        {
                            members = members.Select(Version.Parse)
                                             .OrderBy(arg => arg)
                                             .Select(arg => arg.ToString())
                                             .ToList();

                            if (members[0] == IP)
                                run_server();

                            Logging.handler("info", $"Request Signal Processed. New Server {members[0]}", true);
                        }
                    }
                }
                Thread.Sleep(50);
            }
        }

        public static void send_signal(string signal)
        {
            byte[] data = Encoding.UTF8.GetBytes(signal);
            signals.Send(data, data.Length, signalsIP.ToString(), ports["signals"]);
        }

        public static bool is_my_IP(string address)
        {
            foreach (string IP in IPGroup)
            {
                if (IP == address)
                    return true;
            }
            return false;
        }

        /// <summary>accepts file upload requests</summary>
        public static void files_connections_handler()
        {
            TcpClient file;

            Logging.handler("info", "Files Connections Handler Thread Initiated", true);

            while (true)
            {
                lock (filesLock)
                {
                    if (config["isServer"] != null)
                    {
                        if (filesListener.Pending())
                        {
                            file = filesListener.AcceptTcpClient();
                            files.Add(file);
                            Logging.handler("info", $"{((IPEndPoint)file.Client.RemoteEndPoint).Address} Sending You A File", true);
                        }
                    }
                    else break;
                }
                Thread.Sleep(50);
            }
        }

        /// <summary>runs when the chat window is closing</summary>
        public static void destructor()
        {
            Network.send_message($"{Network.username} left the Clab", system: true);

            Thread.Sleep(50);  //  give members some time to read the disconnect message

            lock (clientLock) lock (serverLock) lock(filesLock)
            {
                if (config["isServer"] == true)
                    clientsListener.Stop();

                if (config["isServer"] != null)
                    filesListener.Stop();

                config["isServer"] = null;
                config["exit"] = true;
                
                send_signal(Signals.disconnect);
                signals.DropMulticastGroup(signalsIP);
                signals.Close();
            }
        }
    }
}
