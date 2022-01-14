using System;
using System.IO;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Clab
{
    /// <summary>common JSON methods</summary>
    public static class Json
    {
        /// <summary>get ExpandoObject slice with IDictionary</summary>
        public static IDictionary<string, object> get(dynamic data, params string[] path)
        {
            var slice = data as IDictionary<string, object>;

            for (int i = 0; i < path.Length; ++i)
                slice = slice[path[i]] as IDictionary<string, object>;

            return slice;
        }

        public static string ObjToStr(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T StrToObj<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
    }

    /// <summary>allows to work with JSON files</summary>
    public class JsonFile
    {
        string filename;
        string filepath;

        public bool delete;  //  auto delete after exit ?
        public bool init;    //  file was created ?
        dynamic data = new ExpandoObject();

        public JsonFile(string filename, bool delete = false)
        {
            this.delete = delete;
            this.filename = filename;
            filepath = Common.get_filepath(true, filename);

            if (!File.Exists(filepath))
            {
                save();
                init = true;
            }
            else
            {
                init = false;
                reload();
            }

            Logging.handler("info", $"{filename}: Json loaded", true);
        }

        /// <summary>get value by its keywords path</summary>
        public IDictionary<string, object> get(params string[] path)
        {
            return Json.get(data, path);
        }

        /// <summary>get list reference by its keywords path</summary>
        public List<object> get_list(params string[] path)
        {
            return (List<object>)get(path[..^1])[path[^1]];
        }

        public void save()
        {
            try
            {
                string buffer = JsonConvert.SerializeObject(data);
                File.WriteAllText(filepath, buffer);
            }
            catch (Exception) { }
        }

        /// <summary>add new value or overwrite old under keyword</summary>
        public void add(object value, params string[] path)
        {
            string name = path[^1];
            var dataDict = get(path[..^1]);

            if (dataDict.ContainsKey(name))
                dataDict[name] = value;
            else
                dataDict.Add(name, value);

            save();
            Logging.handler("info", $"{filename}: Object \"{string.Join(".", path)}\" was added to json", true);
        }

        /// <summary>add new value to list under keyword</summary>
        public void add_to_list<T>(T value, params string[] path)
        {
            var dataList = get_list(path);
            dataList.Add(value);
            save();
            Logging.handler("info", $"{filename}: Object \"{string.Join(".", path)}[{dataList.Count-1}]\" was added to json", true);
        }

        /// <summary>remove object under keyword from JSON</summary>
        public void remove(params string[] path)
        {
            get(path[..^1]).Remove(path[^1]);
            save();
            Logging.handler("info", $"{filename}: Removed \"{string.Join(".", path)}\" from json", true);
        }

        /// <summary>read file from drive and load it to RAM</summary>
        public void reload()
        {
            string buffer = File.ReadAllText(filepath);
            data = JsonConvert.DeserializeObject<ExpandoObject>(buffer, new ExpandoObjectConverter());
            Logging.handler("info", $"{filename}: Reloaded", false);
        }

        /// <summary>executes on program exit</summary>
        public void destructor()
        {
            if (delete)
                File.Delete(filepath);
        }
    }
}
