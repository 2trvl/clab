using System;
using System.IO;
using NetFwTypeLib;
using System.Reflection;
using System.Diagnostics;

namespace ClabUninstall
{
    class Uninstaller
    {
        static void remove_firewall_rules()
        {
            Type tNetFwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            INetFwPolicy2 fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(tNetFwPolicy2);
            var currentProfiles = fwPolicy2.CurrentProfileTypes;

            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

            foreach (INetFwRule rule in fwPolicy2.Rules)
            {                    
                if (rule.Name == "ClabSignals" || rule.Name == "ClabMessages" || rule.Name == "ClabFiles")
                {
                    try
                    {
                        firewallPolicy.Rules.Remove(rule.Name);
                        Console.WriteLine($"{rule.Name} has been remove from Firewall Policy");
                    }
                    catch (Exception) { }
                }
            }
        }

        static void run_cmd_uninstaller()
        {
            string directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            Process proc = new Process();
            proc.StartInfo.Verb = "runas";
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.Arguments = $"{directory}";
            proc.StartInfo.FileName = Path.Combine(directory, "uninstall.cmd");
            proc.Start();
        }

        static void Main()
        {
            remove_firewall_rules();
            run_cmd_uninstaller();
        }
    }
}
