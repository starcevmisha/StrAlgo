using System;
using System.Collections.Generic;
using System.Linq;

namespace task_Zfunc
{
	class Matcher
	{
		public static int[] CountPrefixMatches<TChar>(IList<TChar> pattern, IList<TChar> text)
		{
			var m = pattern.Count;
			var n = text.Count;
			var prefixMatchCount = new int[m];
			var z = Zfunction(pattern);

			//prefixOccurrences[i] = число вхождений pattern[0..i] в text
			//Для сравнения TChar a и TChar b используйте Equal(a, b) (см. ниже)


			return prefixMatchCount;
		}

		// То же что CountPrefixMatches, но работает за O(|text| * |text|).
		public static int[] NaiveCountPrefixMatches<TChar>(IList<TChar> pattern, IList<TChar> text)
		{
			var prefixMatchCount = new int[pattern.Count];
			for (int i = 0; i < text.Count; i++)
			{
				int match = text.Skip(i)  //Skip работает за O(n) - очень медленно
					.TakeWhile((c, j) => j < pattern.Count && Equal(pattern[j], c)).Count();
				if (match > 0)
					prefixMatchCount[match - 1]++;
			}
			for (int i = pattern.Count - 2; i >= 0; i--)
				prefixMatchCount[i] += prefixMatchCount[i + 1];
			return prefixMatchCount;
		}
		private static bool Equal<TChar>(TChar a, TChar b)
		{
			return EqualityComparer<TChar>.Default.Equals(a, b);
		}
		public static int[] Zfunction<TChar>(IList<TChar> str)
		{
			var z = new int[str.Count];
			z[0] = str.Count;
			int rightBoundIdx = 1, rightBound = -1000;
			for (int i = 1; i < str.Count; i++)
			{
				if (i + z[i - rightBoundIdx] > rightBound)//Если мы выйдем за пределы
				{
					z[i] = Math.Max(0, rightBound - i + 1);
					while (i + z[i] < str.Count && Equal(str[z[i]], str[i + z[i]]))
						z[i]++;
				}
				else//Если текущая позиция лежит внутри отрезка
				{
					z[i] = z[i - rightBoundIdx];
				}
				if (i + z[i] - 1 >= rightBound)//Расширяем наш отразок совпадения
				{
					rightBoundIdx = i;
					rightBound = i + z[i] - 1;
				}
			}
			return z;
		}
		
		public static void ZfunctionSearch<TChar>(IList<TChar> str, IList<TChar> pattern)
		{
			var patternZFunc = Zfunction(pattern);
			var res = new int[patternZFunc.Length];
			int rightBoundIdx = 0, rightBound = -1000;
			
			for (int i = 0; i < str.Count; i++)
			{
				var counter = 0;
				if (i + patternZFunc[i - rightBoundIdx] > rightBound)//Если мы выйдем за пределы
				{
					counter = Math.Max(0, rightBound - i + 1);
					while (i + counter < str.Count && counter < pattern.Count && Equal(pattern[counter], str[i + counter]))
						counter++;
				}
				else//Если текущая позиция лежит внутри отрезка
				{
					counter = patternZFunc[i - rightBoundIdx];
				}
				if (i + counter - 1 >= rightBound)//Расширяем наш отразок совпадения
				{
					rightBoundIdx = i;
					rightBound = i + counter - 1;
				}

				if (counter == pattern.Count)
				{
					Console.WriteLine(i);
				}
			}
			return ;
		}
		
	}
}
