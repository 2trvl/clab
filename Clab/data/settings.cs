namespace Clab
{
    public static partial class Generation
    {
        public static class Settings
        {
            public static void generate_template(JsonFile settings)
            {
                settings.add(Network.IP, "username");
                settings.add(false, "autoDeleteLog");
                settings.add(false, "autoDeleteHistory");
                settings.add(Common.get_filepath(true, "downloads"), "downloadPath");
            }

            public static void load_values(JsonFile settings)
            {
                Network.username = (string)settings.get()["username"];
                Logging.delete = (bool)settings.get()["autoDeleteLog"];
                Network.downloadPath = (string)settings.get()["downloadPath"];
                Clab.history.delete = (bool)settings.get()["autoDeleteHistory"];
            }
        }
    }
}
