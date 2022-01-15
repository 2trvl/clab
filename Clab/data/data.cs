using System.IO;

namespace Clab
{
    public static partial class Common
    {
        /// <summary>absolute path from string[] or relative from cwd</summary>
        public static string get_filepath(bool cwd, params string[] filename)
        {
            string filepath = "";
            
            if (cwd)
                filepath = Directory.GetCurrentDirectory();

            foreach (string path in filename)
                filepath = Path.Combine(filepath, path);

            return filepath;
        }
    }
}
