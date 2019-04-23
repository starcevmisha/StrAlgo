using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace task_LZW
{
    class ListComparer : IComparer<List<byte>>
    {
        public int Compare(List<byte> x, List<byte> y)
        {
            var len = Math.Min(x.Count, y.Count);
            for (var i = 0; i < len; i++)
            {
                var c = x[i].CompareTo(y[i]);
                if (c != 0)
                {
                    return c;
                }
            }

            return x.Count.CompareTo(y.Count);
        }
    }

    class LZW
    {
        private static int[] DataToLZW(byte[] data)
        {
            var dict = new SortedDictionary<List<byte>, int>(new ListComparer());
            for (int i = 0; i < 256; i++)
            {
                dict.Add(new List<byte> {(byte) i}, i);
            }

            var outArray = new List<int>();
            var w = new List<byte>();
            foreach (var c in data)
            {
                var wc = new List<byte>(w) {c};
                if (dict.ContainsKey(wc))
                {
                    w.Clear();
                    w.AddRange(wc);
                }
                else
                {
                    outArray.Add(dict[w]);
                    dict.Add(wc, dict.Count);

                    w.Clear();
                    w.Add(c);
                }
            }

            if (w.Count != 0)
                outArray.Add(dict[w]);

            return outArray.ToArray();
        }

        private static byte[] LZWtoData(int[] lzw)
        {
            var dict = new Dictionary<int, List<byte>>();
            for (int i = 0; i < 256; i++)
            {
                dict.Add(i, new List<byte> {(byte) i});
            }

            var window = dict[lzw[0]];
            var outArray = new List<byte>(window);

            foreach (var i in lzw.Skip(1))
            {
                var entry = new List<byte>();
                if (dict.ContainsKey(i))
                    entry.AddRange(dict[i]);
                else if (i == dict.Count)
                {
                    entry.AddRange(window);
                    entry.Add(window[0]);
                }

                if (entry.Count > 0)
                {
                    outArray.AddRange(entry);

                    window.Add(entry[0]); // b ab. Мы должны добавить ba в словарь
                    dict.Add(dict.Count, new List<byte>(window));

                    window = entry;
                }
            }


            return outArray.ToArray();
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
                var bytes = BitConverter.GetBytes((ulong) lzw[id] << curr_bit % 8);
                for (int k = 0; k < 2 + id_bits / 8; k++)
                    output[curr_bit / 8 + k] |= bytes[k];
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
                lzw.Add((int) (x >> curr_bit % 8) & (1 << id_bits) - 1);
                curr_bit += id_bits;
                if (256 + id >= (1 << id_bits))
                    id_bits++;
            }

            return LZWtoData(lzw.ToArray());
        }
    }
}