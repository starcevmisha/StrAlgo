﻿using System;
using System.Linq;
using System.IO;

namespace task_LZW
{
	class Program
	{
		static void Main(string[] args)
		{
			string prefix = @"..\..\data-files\";
			foreach (var filename in new []{ "program.txt", "lotr.txt", "img.bmp" })
			{
				var data = File.ReadAllBytes(prefix + filename);
				var compressed = LZW.Compress(data);
				File.WriteAllBytes(prefix + filename + ".compessed", compressed);
				Console.WriteLine($"'{filename}' {data.Length:n0} B to {compressed.Length:n0} B");
				var a = LZW.Decompress(compressed);
				if (!Enumerable.SequenceEqual(LZW.Decompress(compressed), data))
					Console.WriteLine("Decompression error");
			}
		}
	}
}
