using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Clab
{
    public static partial class Generation
    {
        public static class History
        {
            public static string date;
            static readonly Regex datePattern = new Regex(@"^....\...\...$", RegexOptions.Compiled);  //  or ^[0-9]{4}\.[0-9]{2}\.[0-9]{2}$

            public static string[] COUNT;
            public static string[] DATE;
            public static string[] DATE_SENT;
            public static string[] DATE_INBOX;

            public static void generate_paths()
            {
                date = DateTime.Now.ToString("yyyy.MM.dd");

                DATE = new string[] { date };
                COUNT = new string[] { "count" };
                DATE_SENT = new string[] { date, "sent" };
                DATE_INBOX = new string[] { date, "inbox" };

                Logging.handler("warning", "Paths regenerated", true);
            }

            public static void generate_template(JsonFile history)
            {
                history.add(new ExpandoObject(), DATE);
                history.add(new List<Sent>(), DATE_SENT);
                history.add(new List<Inbox>(), DATE_INBOX);
                history.add(0, COUNT);
                history.reload();
            }

            public static bool check_date_paths(string messageDate)
            {
                if (messageDate != date)
                {
                    generate_paths();
                    Clab.history.add(new ExpandoObject(), DATE);
                    Clab.history.add(new List<Sent>(), DATE_SENT);
                    Clab.history.add(new List<Inbox>(), DATE_INBOX);
                    Clab.history.add(0, COUNT);
                    Clab.history.reload();  //  implicit reload due DLR particularity ?

                    Logging.handler("warning", "History date updated", true);
                    return true;
                }
                return false;
            }

            public static bool isToday(JsonFile history)
            {
                bool today = false;
                IDictionary<string, object> dates = history.get();
                List<string> expired = new List<string>();

                foreach (var date in dates)
                {
                    if (datePattern.IsMatch(date.Key))
                    {
                        if (date.Key.Equals(History.date))
                            today = true;
                        else
                            expired.Add(date.Key);
                    }
                }

                foreach (string date in expired)
                    history.remove(date);

                return today;
            }

            public static void load_history(JsonFile history)
            {
                if (isToday(history))
                {
                    var sent = history.get_list(DATE_SENT);
                    var inbox = history.get_list(DATE_INBOX);

                    int sentBound = sent.Count;
                    int inboxBound = inbox.Count;

                    int sentIndex = 0;
                    int inboxIndex = 0;

                    IDictionary<string, object> sentMsg;  //  ExpandoObject doesn't support as Class cast, so i gave up with List<Object>
                    IDictionary<string, object> inboxMsg;

                    while (sentIndex < sentBound && inboxIndex < inboxBound)
                    {
                        sentMsg = sent[sentIndex] as IDictionary<string, object>;
                        inboxMsg = inbox[inboxIndex] as IDictionary<string, object>;

                        if (Convert.ToInt32(sentMsg["id"]) < Convert.ToInt32(inboxMsg["id"]))
                        {
                            Clab.chat.add_message_to_box($"\n{Network.username}: {sentMsg["message"]}");
                            sentIndex++;
                        }
                        else
                        {
                            Clab.chat.add_message_to_box($"\n{inboxMsg["name"]}: {inboxMsg["message"]}");
                            inboxIndex++;
                        }
                    }

                    //  one of the lists has come to an end, load the remaining messages

                    while (sentIndex < sentBound)
                    {
                        sentMsg = sent[sentIndex++] as IDictionary<string, object>;
                        Clab.chat.add_message_to_box($"\n{Network.username}: {sentMsg["message"]}");
                    }
                    
                    while (inboxIndex < inboxBound)
                    {
                        inboxMsg = inbox[inboxIndex++] as IDictionary<string, object>;
                        Clab.chat.add_message_to_box($"\n{inboxMsg["name"]}: {inboxMsg["message"]}");
                    }
                }
                else check_date_paths("");

                Logging.handler("warning", "History loaded", true);
            }

            public class Message
            {
                public string name { get; set; }
                public string message { get; set; }
                public bool attachedFile { get; set; }
            }

            public class Inbox
            {
                public string name { get; set; }
                public string message { get; set; }
                public string time { get; set; }
                public int id { get; set; }
            }

            public class Sent
            {
                public string message { get; set; }
                public string time { get; set; }
                public int id { get; set; }
            }

            public static void handle_inbox_message(Message objMessage, ref int msgCount)
            {
                //  system message have name ""
                //  users can't set their names to ""
                if (objMessage.name == "")
                    Clab.chat.add_message_to_box($"\n{objMessage.message}", AppMessages.warning);
                else
                    add_message<Generation.History.Inbox>(objMessage, ref msgCount);
            }

            public static void add_message<T>(Message objMessage, ref int msgCount)
            {
                if (typeof(T) == typeof(Inbox))
                    Clab.history.add_to_list<Inbox>(new Inbox
                    {
                        name = objMessage.name,
                        message = objMessage.message,
                        time = DateTime.Now.ToString("HH:mm"),
                        id = msgCount++
                    },
                    DATE_INBOX);

                else if (typeof(T) == typeof(Sent))
                    Clab.history.add_to_list<Sent>(new Sent
                    {
                        message = objMessage.message,
                        time = DateTime.Now.ToString("HH:mm"),
                        id = msgCount++
                    },
                    DATE_SENT);

                Clab.history.get()["count"] = msgCount;
                Clab.history.save();

                Clab.chat.add_message_to_box($"\n{objMessage.name}: {objMessage.message}");
            }
        }
    }
}
