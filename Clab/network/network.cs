using System;
using System.IO;
using System.Net;
using System.Linq;
using NetFwTypeLib;
using System.Reflection;
using System.Net.Sockets;
using System.Diagnostics;
using System.Windows.Forms;
using System.Security.Principal;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace Clab
{
    public static partial class Network
    {
        public static string username;
        public static string downloadPath;

        static Dictionary<string, int> ports = new Dictionary<string, int> {
                                     { "signals", 15056 },
                                     { "messages", 15057 },
                                     { "files", 15058 } };

        public static string IP = Common.get_local_ip();
        static List<string> IPGroup = Common.get_local_ip_group();

        /// <summary>checking access to needed ports with firewall rules</summary>
        public static void grant_firewall_rules()
        {
            Type tNetFwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            INetFwPolicy2 fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(tNetFwPolicy2);
            var currentProfiles = fwPolicy2.CurrentProfileTypes;

            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

            bool firewallRules = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => (x.Name == "ClabSignals") || 
                                                                                      (x.Name == "ClabMessages") || 
                                                                                      (x.Name == "ClabFiles")).Count() == 3;

            if (!firewallRules)
            {
                Common.run_as_admin();

                INetFwRule firewallRule = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name == "ClabSignals").FirstOrDefault();

                if (firewallRule == null)
                {
                    INetFwRule2 clabSignals = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                    clabSignals.Enabled = true;
                    clabSignals.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                    clabSignals.Protocol = 17;
                    clabSignals.LocalPorts = ports["signals"].ToString();
                    clabSignals.Name = "ClabSignals";
                    clabSignals.Profiles = currentProfiles;
                    firewallPolicy.Rules.Add(clabSignals);
                    Logging.handler("warning", "Clab Signals Firewall Rule Added", true);
                }

                firewallRule = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name == "ClabMessages").FirstOrDefault();

                if (firewallRule == null)
                {
                    INetFwRule2 clabMessages = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                    clabMessages.Enabled = true;
                    clabMessages.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                    clabMessages.Protocol = 6;
                    clabMessages.LocalPorts = ports["messages"].ToString();
                    clabMessages.Name = "ClabMessages";
                    clabMessages.Profiles = currentProfiles;
                    firewallPolicy.Rules.Add(clabMessages);
                    Logging.handler("warning", "Clab Messages Firewall Rule Added", true);
                }

                firewallRule = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name == "ClabFiles").FirstOrDefault();

                if (firewallRule == null)
                {
                    INetFwRule2 clabFiles = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                    clabFiles.Enabled = true;
                    clabFiles.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                    clabFiles.Protocol = 6;
                    clabFiles.LocalPorts = ports["files"].ToString();
                    clabFiles.Name = "ClabFiles";
                    clabFiles.Profiles = currentProfiles;
                    firewallPolicy.Rules.Add(clabFiles);
                    Logging.handler("warning", "Clab Files Firewall Rule Added", true);
                }
            }
        }
    }

    public static partial class Common
    {
        public static bool is_admin()
        {
            bool admin;

            try
            {
                WindowsIdentity id = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(id);
                admin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception)
            {
                admin = false;
            }

            return admin;
        }

        public static void run_as_admin()
        {
            if (!is_admin())
            {
                Process proc = new Process();
                proc.StartInfo.Verb = "runas";
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.FileName = Path.ChangeExtension(Assembly.GetEntryAssembly().Location, ".exe");

                try
                {
                    proc.Start();
                }
                catch (Exception)
                {
                    MessageBox.Show(new Form { TopMost = true }, 
                                    "Please run Clab as administrator next time. This is required to configure the firewall",
                                    "Firewall Configuration [Access Denied]",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);
                }

                Environment.Exit(0);
            }
        }

        /// <summary>Returns only physical local IP</summary>
        public static string get_local_ip()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                var addr = ni.GetIPProperties().GatewayAddresses.FirstOrDefault();

                if (addr != null && !addr.Address.ToString().Equals("0.0.0.0"))
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                return ip.Address.ToString();
                            }
                        }
                    }
                }
            }

            Clab.chat.add_message_to_box("\nPlease connect to local network", AppMessages.error);
            Logging.handler("error", "Please connect to local network", true);

            return "127.0.0.1";
        }

        /// <summary>Returns all PC local IP's, including virtual hardware</summary>
        public static List<string> get_local_ip_group()
        {
            List<string> localIPGroup = new List<string>();
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIPGroup.Add(ip.ToString());
                }
            }

            if (localIPGroup.Count == 0)
                localIPGroup.Add("127.0.0.1");

            return localIPGroup;
        }

        /// <summary>opens a file, safe open means creating a new file without overwriting</summary>
        public static (FileStream, string) open_file(string filepath, bool safe = false)
        {
            FileStream file;
            string filename = Path.GetFileName(filepath);

            Directory.CreateDirectory(Path.GetDirectoryName(filepath));

            if (safe)
            {
                if (!File.Exists(filepath))
                {
                    file = File.Open(filepath, FileMode.CreateNew);
                }

                else
                {
                    for (int i = 1; ; ++i)
                    {
                        string copy = Path.Combine(
                               Network.downloadPath,
                               string.Format("{0}({1}){2}",
                               Path.GetFileNameWithoutExtension(filename),
                               i,
                               Path.GetExtension(filename)));

                        if (!File.Exists(copy))
                        {
                            file = File.Open(copy, FileMode.CreateNew);
                            filename = Path.GetFileName(copy);
                            break;
                        }
                    }
                }
            }
            else
            {
                try
                {
                    file = File.Open(filepath, FileMode.Open);
                }
                catch (Exception)
                {
                    file = null;
                    Logging.handler("warning", $"File Used By Another Process. Can't Open {filepath}", true);
                }
            }

            return (file, filename);
        }
    }
}
