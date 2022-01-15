using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace Clab
{
    public static class Logging
    {
        public static bool delete;
        static string filepath;

        public static void init(string filename, bool delete = false)
        {
            Logging.delete = delete;
            filepath = Common.get_filepath(true, filename);
            File.Create(filepath).Close();
            handler("info", "Log created", false);
        }

        static string get_method_path(MethodBase methodInfo)
        {
            return methodInfo.DeclaringType.Name + "." + methodInfo.Name;
        }

        static string format(string level, string sender, string message)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return $"{date} [{level.ToUpper()}] {sender}: {message}\n";
        }

        public static void handler(string level, string message, bool printing)
        {
            MethodBase methodInfo = new StackFrame(1).GetMethod();
            string line = get_method_path(methodInfo);
            line = format(level, line, message);

            try
            {
                File.AppendAllText(filepath, line);
            }
            catch (Exception) { }  //  few threads access at the same time

            if (printing)
            {
                //  extend logger with custom runtime levels
                //  System.NullReferenceException can be catched only with [HandleProcessCorruptedStateExceptions]
                FieldInfo field = typeof(Levels).GetField(level);
                ConsoleColor color = field != null ? (ConsoleColor)field.GetValue(null) : Levels.info;
                color_print(line, color);
            }
        }

        public static class Levels
        {
            public static ConsoleColor error = ConsoleColor.Red;
            public static ConsoleColor warning = ConsoleColor.Yellow;
            public static ConsoleColor info = Console.ForegroundColor;
        }

        public static void color_print(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }

        public static void destructor()
        {
            if (delete)
                File.Delete(filepath);
        }
    }
}
