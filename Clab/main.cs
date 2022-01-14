using System;
using System.Windows.Forms;

namespace Clab
{
    class Clab
    {
        public static JsonFile history, settings;

        public static Chat chat;

        static Clab()
        {
            Network.grant_firewall_rules();

            Logging.init("clab.log");
            history = new JsonFile("history.json");
            settings = new JsonFile("settings.json");

            if (settings.init)
                Generation.Settings.generate_template(settings);

            Generation.Settings.load_values(settings);

            Generation.History.generate_paths();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            chat = new Chat();  //  create chat, so load_history will can add messages

            if (history.init)
                Generation.History.generate_template(history);
            else
                Generation.History.load_history(history);

            Network.run();
        }

        [STAThread]
        static void Main()
        {
            Application.Run(chat);
        }
    }
}
