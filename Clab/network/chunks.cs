using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Clab
{
    public partial class Network
    {
		/// <summary>NetworkStream read/write in delegates form</summary>
		public static class IOHandlers
		{
			public static Func<NetworkStream, byte[], int, int, int> NetRead = (NetworkStream stream, byte[] data, int bytesProcessed, int curDataSize)
																		     => { return stream.Read(data, bytesProcessed, curDataSize); };
			public static Func<NetworkStream, byte[], int, int, int> NetWrite = (NetworkStream stream, byte[] data, int bytesProcessed, int curDataSize)
																		      => { stream.Write(data, bytesProcessed, curDataSize); return curDataSize; };
		}

		/// <summary>read message from the stream and return it by ref</summary>
		public static void read_chunks(NetworkStream sender, ref byte[] message)
		{
			while (!sender.DataAvailable) { }
			handle_chunk(sender, ref message, IOHandlers.NetRead);
		}

		/// <summary>write data to streams</summary>
		public static bool write_chunks(ref NetworkStream[] recipients, byte[] data)
        {
			bool errors = false;
			List<int> exclude = new List<int>();

			for (int i = 0; i < recipients.Length; ++i)
			{
				try
				{
					handle_chunk(recipients[i], ref data, IOHandlers.NetWrite);
				}
				catch (Exception)
				{
					exclude.Add(i);
					Logging.handler("warning", "Skipped Closed Stream", true);
				}
			}

			if (exclude.Count != 0)
			{
				foreach (int i in exclude)
				{
					recipients = recipients.Where((source, index) => index != i).ToArray();
				}
				errors = true;
			}

			return errors;
        }

		/// <summary>read the message and forward it</summary>
		public static void forward_chunks(NetworkStream sender, ref byte[] message, NetworkStream[] recipients)
        {
			read_chunks(sender, ref message);
			write_chunks(ref recipients, message);
		}

		/// <summary>read file from the stream</summary>
		public static void read_file_chunks(NetworkStream sender, FileStream file)
		{
			long bufferSize = 100 * 1024;
			long bytesLeft = file.Length;
			int curDataSize;
			byte[] data;

			while (bytesLeft > 0)
			{
				curDataSize = (int)Math.Min(bufferSize, bytesLeft);

				data = new byte[curDataSize];
				read_chunks(sender, ref data);

				file.Write(data, 0, curDataSize);
				file.Flush();

				bytesLeft -= curDataSize;
			}

			file.Close();
		}

		/// <summary>write file to streams</summary>
		public static void write_file_chunks(NetworkStream[] recipients, FileStream file)
		{
			long bufferSize = 100 * 1024;
			long bytesLeft = file.Length;
			int curDataSize;
			byte[] data;

			while (bytesLeft > 0)
			{
				curDataSize = (int)Math.Min(bufferSize, bytesLeft);

				data = new byte[curDataSize];
				file.Read(data, 0, curDataSize);

				write_chunks(ref recipients, data);

				bytesLeft -= curDataSize;
			}

			file.Close();
		}

		/// <summary>read file from the stream and forward it</summary>
		public static void forward_file_chunks(NetworkStream sender, FileStream file, NetworkStream[] recipients)
		{
			long bufferSize = 100 * 1024;
			long bytesLeft = file.Length;
			int curDataSize;
			byte[] data;

			while (bytesLeft > 0)
			{
				curDataSize = (int)Math.Min(bufferSize, bytesLeft);

				data = new byte[curDataSize];
				read_chunks(sender, ref data);

				file.Write(data, 0, curDataSize);
				file.Flush();

				write_chunks(ref recipients, data);

				bytesLeft -= curDataSize;
			}

			file.Close();
		}

		/// <summary>universal chunk handler</summary>
		public static void handle_chunk(NetworkStream stream, ref byte[] data, Func<NetworkStream, byte[], int, int, int> netHandler)
		{
			int bufferSize = 1024;
			int bytesProcessed = 0;
			int bytesLeft = data.Length;
			int curDataSize;

			while (bytesLeft > 0)
			{
				curDataSize = Math.Min(bufferSize, bytesLeft);

				curDataSize = netHandler(stream, data, bytesProcessed, curDataSize);

				bytesProcessed += curDataSize;
				bytesLeft -= curDataSize;
			}
		}
	}
}
