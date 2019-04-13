using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace task_LZW
{
	class LZW
	{
		private static int[] DataToLZW(byte[] data)
		{
			throw new NotImplementedException();
		}
		
		private static byte[] LZWtoData(int[] lzw)
		{
			throw new NotImplementedException();
		}
		
		public static byte[] Compress(byte[] data)
		{
			var lzw = DataToLZW(data);
			var output = new List<byte>();
			int id_bits = 8, curr_bit = 0;
			for (int id = 0; id < lzw.Length; id++)
			{
				int new_size = (curr_bit + id_bits + 7) / 8 + 8;
				output.AddRange(new byte[new_size - output.Count]);
				var bytes = BitConverter.GetBytes((ulong)lzw[id] << curr_bit % 8);
				for (int k = 0; k < 2 + id_bits/8; k++)
					output[curr_bit/8 + k] |= bytes[k];
				curr_bit += id_bits;
				if (256 + id >= (1 << id_bits))
					id_bits++;
			}
			output.RemoveRange((curr_bit + 7) / 8, 8);
			return output.ToArray();
		}
		
		public static byte[] Decompress(byte[] data)
		{
			var lzw = new List<int>();
			int id_bits = 8, curr_bit = 0, last_bit = data.Length * 8;
			data = data.Concat(new byte[8]).ToArray();
			for (int id = 0; curr_bit + id_bits <= last_bit; id++)
			{
				ulong x = BitConverter.ToUInt64(data, curr_bit / 8);
				lzw.Add((int)(x >> curr_bit % 8) & (1 << id_bits) - 1);
				curr_bit += id_bits;
				if (256 + id >= (1 << id_bits))
					id_bits++;
			}
			return LZWtoData(lzw.ToArray());
		}
	}
}
