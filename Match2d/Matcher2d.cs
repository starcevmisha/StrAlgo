using System;
using System.Collections.Generic;
using System.Linq;

namespace task_Match2d
{
    class Matcher2d
    {
        public static List<Tuple<int, int>> PatternMatches<TChar>(
            IList<IList<TChar>> pattern, IList<IList<TChar>> matrix)
            where TChar : IComparable<TChar>
        {
            if (pattern[0].Count == 0 || pattern.Any(row => row.Count != pattern[0].Count))
                throw new ArgumentException();
            if (matrix[0].Count == 0 || matrix.Any(row => row.Count != matrix[0].Count))
                throw new ArgumentException();
            int p = pattern.Count, q = pattern[0].Count;
            int m = matrix.Count, n = matrix[0].Count;
            var result = new List<Tuple<int, int>>();

            //Используя Ахо-Корасик, реализуйте поиск двумерных паттернов за O((pq + mn)log s)

//            foreach (var str in matrix)
//            {
//                Console.WriteLine(String.Concat(str));
//            }
//            
//            foreach (var str in pattern)
//            {
//                Console.WriteLine(String.Concat(str));
//            }
            
            var resMatrix = new int[n,m];
            Action<int, int> reportAction = (position, strId) => resMatrix[position/(n+1),position%(n+1)]=strId;

            var strPatterns = pattern.Select(pat => String.Concat(pat).ToList()).ToList();

            var strMatrix = "";
            foreach (var str in matrix)
            {
                strMatrix += String.Concat(str);
                strMatrix += "$";
            }
            
            var ahoCorasick = new AhoCorasick<char>(strPatterns, out var stringIds);
            ahoCorasick.ReportOccurrencesIds(strMatrix, reportAction);


//            for (int i = 0; i < n; i++)
//            {
//                for (int j = 0; j < m; j++)
//                {
//                    Console.Write(resMatrix[i, j]);
//                }
//
//                Console.WriteLine();
//            }
//            
            var resultTuples = new List<Tuple<int, int>>();
            
            
            var columns = "";
            for (int i = 0; i < n; i++)
            {
                var column = String.Concat(Enumerable.Range(0, resMatrix.GetLength(0))
                        .Select(x => resMatrix[x, i]))
                    .ToList();
               
                columns += String.Concat(column);
                columns += "$";
            }
            
            var ahoCorasickForFindAnswer = new AhoCorasick<char>(String.Concat(stringIds).ToList());
                ahoCorasickForFindAnswer.ReportOccurrencesIds(columns, 
                    (position, strId) 
                        => resultTuples.Add(Tuple.Create( position%(n+1), position/(n+1)))/*Console.WriteLine($"{position/(n+1)},{position%(n+1)}")*/);
            
            
            
            return resultTuples;
        }

        public static List<Tuple<int, int>> NaivePatternMatches<TChar>(
            IList<IList<TChar>> pattern, IList<IList<TChar>> matrix)
            where TChar : IComparable<TChar>
        {
            if (pattern[0].Count == 0 || pattern.Any(row => row.Count != pattern[0].Count))
                throw new ArgumentException();
            if (matrix[0].Count == 0 || matrix.Any(row => row.Count != matrix[0].Count))
                throw new ArgumentException();
            int p = pattern.Count, q = pattern[0].Count;
            int m = matrix.Count, n = matrix[0].Count;
            var result = new List<Tuple<int, int>>();
            for (int i = 0; i <= m - p; i++)
            for (int j = 0; j <= n - q; j++)
                if (pattern.Select((row, k) => Enumerable.SequenceEqual(
                    matrix[i + k].Skip(j).Take(q), pattern[k])).All(b => b))
                {
                    result.Add(Tuple.Create(i, j));
                }

            return result;
        }
    }
}