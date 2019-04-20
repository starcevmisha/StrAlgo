using System;
using System.Collections.Generic;
using System.Linq;

namespace task_Match2d
{
	class Program
	{
		static void PrepareMatrixAndPattern(out int[][] matrix, out int[][] pattern, Random rand)
		{
			matrix = Enumerable.Range(0, 100).Select(_ => Enumerable.Range(0,100)
						.Select(__ => rand.Next() % 3).ToArray()).ToArray();
			pattern = Enumerable.Range(0, 9).Select(_ => Enumerable.Range(0, 9)
						.Select(__ => rand.Next() % 3).ToArray()).ToArray();
			for (int occNum = rand.Next() % 20; occNum >= 0; occNum--)
			{
				int x = rand.Next() % (matrix.Length - pattern.Length + 1);
				int y = rand.Next() % (matrix[0].Length - pattern[0].Length + 1);
				for (int i = 0; i < pattern.Length; i++)
					for (int j = 0; j < pattern[0].Length; j++)
						matrix[x + i][y + j] = pattern[i][j];
			}
		}
		static void Main(string[] args)
		{
//			var resMatrix = new int[3,3];
//			Action<int, int> reportAction = (position, strId) => resMatrix[position/4,position%4]=strId;
//
//
////			
//			var patterns = new List<List<char>> { "bc".ToList(), "ef".ToList()};
//			var ahoCorasick = new AhoCorasick<char>(patterns, out var stringIds);
////			var res = ahoCorasick.Find("abc$def$ghi");
//			ahoCorasick.ReportOccurrencesIds("abc$def$efe", reportAction);
//
//			for (int i = 0; i < 3; i++)
//			{
//				for (int j = 0; j < 3; j++)
//				{
//					Console.Write(resMatrix[i,j]);
//				}
//
//				Console.WriteLine();
//			}
//			
//			var b = new AhoCorasick<int>(stringIds.ToList());
////			foreach (var column in resMatrix.GetColumns())
//			{
//				b.ReportOccurrencesIds(new List<int>{0,1,3}, (i, i1) => { Console.WriteLine(i); });
//			}


			var rand = new Random();
			for (int i = 0; i < 1000; i++)
			{
				int[][] matrix, pattern;
				PrepareMatrixAndPattern(out matrix, out pattern, rand);
				var occs = Matcher2d.PatternMatches(pattern, matrix).OrderBy(t => t.Item1).ThenBy(t => t.Item2).ToList();
				var occs_expected = Matcher2d.NaivePatternMatches(pattern, matrix).OrderBy(t => t.Item1).ThenBy(t => t.Item2).ToList();
				if (!Enumerable.SequenceEqual(
						occs.OrderBy(t => t.Item1).ThenBy(t => t.Item2), 
						occs_expected.OrderBy(t => t.Item1).ThenBy(t => t.Item2)))
				{
					Console.WriteLine("TEST FAILED!");
					return;
				}
			}
			Console.WriteLine("All tests passed successfully");
		}
	}
}
