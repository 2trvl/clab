using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Diagnostics;
using System.Collections.Generic;

namespace Clab
{
    public static partial class Network
    {
		public static void send_message(string message, string filepath = null, bool system = false)
		{
			FileStream file = null;
			string filename = null;
			int msgCount = Convert.ToInt32(Clab.history.get()["count"]);

			Generation.History.Message objMessage = new Generation.History.Message
			{
				name = username,
				message = message,
				attachedFile = false
			};

			if (filepath != null)
			{
				(file, filename) = Common.open_file(filepath);

				if (file != null)
				{
					if (objMessage.message != "")
						objMessage.message += " ";

					objMessage.message += $"{filename}";
					objMessage.attachedFile = true;
				}
				else
				{
					filepath = null;
					Logging.handler("warning", "File Sending Cancelled", true);
					Clab.chat.add_message_to_box("\nFile Sending Cancelled. It Used By Another Process", AppMessages.error);

					if (objMessage.message == "")
						return;
				}
			}

			if (system)
				objMessage.name = "";

			lock (clientLock) lock (serverLock)
			{
				if (config["isServer"] != null)
				{
					NetworkStream[] recipients;

					recipients = get_recipients(Network.IP);
					send_message_packet(recipients, Json.ObjToStr(objMessage));

					if (filepath != null)
					{
						recipients = get_recipients(Network.IP, true);
						new Thread(() => send_file_packet(recipients, file, Encoding.UTF8.GetBytes(filename))).Start();
					}
				}
			}

			if (Generation.History.check_date_paths(DateTime.Now.ToString("yyyy.MM.dd")))  //  counter was changed
				msgCount = Convert.ToInt32(Clab.history.get()["count"]);

			if (!system)
				Generation.History.add_message<Generation.History.Sent>(objMessage, ref msgCount);

			Logging.handler("info", "Message Sent", true);
		}

		public static void send_message_packet(NetworkStream[] recipients, string message)
		{
			byte[] binMessage = Encoding.UTF8.GetBytes(message);

			//  message up to 2 GB
			byte[] messageSize = new byte[4];
			Array.Copy(BitConverter.GetBytes(binMessage.Length), 0, messageSize, 0, 4);

			byte[] messagePacket = new byte[messageSize.Length + binMessage.Length];

			Array.Copy(messageSize, 0, messagePacket, 0, messageSize.Length);
			Array.Copy(binMessage, 0, messagePacket, messageSize.Length, binMessage.Length);

			write_chunks(ref recipients, messagePacket);
		}

		public static void send_file_packet(NetworkStream[] recipients, FileStream file, byte[] name)
        {
			byte[] fileName = new byte[2 + name.Length];
			Array.Copy(BitConverter.GetBytes(Convert.ToInt16(name.Length)), 0, fileName, 0, 2);
			Array.Copy(name, 0, fileName, 2, name.Length);

			write_chunks(ref recipients, fileName);

			//  file up to 9 exabytes
			byte[] fileSize = new byte[8];  
			Array.Copy(BitConverter.GetBytes(file.Length), 0, fileSize, 0, 8);

			write_chunks(ref recipients, fileSize);

			write_file_chunks(recipients, file);

			Logging.handler("info", "File Sent", true);
			Clab.chat.add_message_to_box("\nFile Sent", AppMessages.info);
		}

		//  Ok, it must throw Exception to be seen by client_inbox and server_broadcast_service
		public static Generation.History.Message get_message_packet(NetworkStream sender, bool forwarding = false, NetworkStream[] recipients = null)
		{
			Generation.History.Message objMessage;

			byte[] messageSize = new byte[4];
			sender.Read(messageSize, 0, 4);

			byte[] binMessage = new byte[BitConverter.ToInt32(messageSize, 0)];

			if (forwarding)
            {
				write_chunks(ref recipients, messageSize);
				forward_chunks(sender, ref binMessage, recipients);
			}
			else
				read_chunks(sender, ref binMessage);

			string message = Encoding.UTF8.GetString(binMessage);
			objMessage = Json.StrToObj<Generation.History.Message>(message);

			if (objMessage.attachedFile == true)
            {
				if (forwarding)
					new Thread(() => get_file_packet(true, recipients)).Start();
				else
					new Thread(() => get_file_packet()).Start();
			}

			return objMessage;
		}

		public static void get_file_packet(bool forwarding = false, NetworkStream[] recipients = null)
        {
			TcpClient sender;
			NetworkStream senderStream;

			while (files.Count == 0) { }

			lock (filesLock)
			{
				sender = files[0];
				files.RemoveAt(0);
			}

			if (!sender.Connected)
            {
				Logging.handler("warning", "File Processing Cancelled. User Disconnected", true);
				Clab.chat.add_message_to_box("\nFile Processing Cancelled. User Disconnected", AppMessages.error);
				return;
			}

			FileStream file = null;
			string filepath = null;

			try
            {
				senderStream = sender.GetStream();

				while (!senderStream.DataAvailable) { }

				byte[] fileNameSize = new byte[2];
				senderStream.Read(fileNameSize, 0, 2);

				byte[] fileName = new byte[BitConverter.ToInt16(fileNameSize, 0)];
				senderStream.Read(fileName, 0, fileName.Length);

				filepath = Common.get_filepath(false, Network.downloadPath, Encoding.UTF8.GetString(fileName));
				(file, filepath) = Common.open_file(filepath, true);
				Logging.handler("info", $"Getting File \"{filepath}\"", true);

				filepath = Common.get_filepath(false, Network.downloadPath, filepath);

				byte[] fileSize = new byte[8];
				senderStream.Read(fileSize, 0, 8);

				file.SetLength(BitConverter.ToInt64(fileSize, 0));
				Logging.handler("info", $"File Size {file.Length}", true);

				if (forwarding)
                {
					write_chunks(ref recipients, fileNameSize);
					write_chunks(ref recipients, fileName);
					write_chunks(ref recipients, fileSize);
					forward_file_chunks(senderStream, file, recipients);
				}
				else
					read_file_chunks(senderStream, file);

				Logging.handler("info", $"File Saved {filepath}", true);
				Clab.chat.add_message_to_box($"\nFile Saved {filepath}", AppMessages.info);
			}

			catch (Exception)
            {
				if (filepath != null)
                {
					file.Close();
					File.Delete(filepath);
				}

				Logging.handler("error", "Can't Read File", true);
				Clab.chat.add_message_to_box("\nCan't Read File", AppMessages.error);
			}

			sender.Close();
		}

		/// <summary>creates an array of data recipients, for files establishes new connections</summary>
		public static NetworkStream[] get_recipients(string sender, bool create = false)
		{
			TcpClient recipient;
			Stopwatch timer = new Stopwatch();
			NetworkStream[] recipients = { };

			if (config["isServer"] == false)
			{
				if (create)
                {
					recipient = new TcpClient(serverIP, ports["files"]);
					connection_timer(ref timer, ref recipient);
					recipients = new NetworkStream[] { recipient.GetStream() };
				}
				else
					recipients = new NetworkStream[] { serverStream };
			}

			else if (config["isServer"] == true)
			{
				if (sender != IP)
					recipients = new NetworkStream[clients.Count - 1];
				else
					recipients = new NetworkStream[clients.Count];

				int i = 0;

				foreach (KeyValuePair<string, TcpClient> client in clients)
				{
					if (client.Key != sender)
					{
						if (create)
						{
							recipient = new TcpClient(client.Key, ports["files"]);
							connection_timer(ref timer, ref recipient);
							recipients[i++] = recipient.GetStream();
						}
						else
							recipients[i++] = client.Value.GetStream();
					}
				}
			}

			return recipients;
		}

		/// <summary>timer for establishing new connections</summary>
		public static void connection_timer(ref Stopwatch timer, ref TcpClient recipient)
		{
			timer.Start();

			while (true)
			{
				if (timer.ElapsedMilliseconds > 50 || recipient.Connected)
				{
					timer.Reset();
					break;
				}
			}
		}
	}
}
