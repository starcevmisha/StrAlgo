//using System.Linq;
//
//namespace task_Match2d
//{
//    public static class ArrayExtensions<T>
//    {
//        public T[] GetColumns(T[,] matrix)
//        {
//            for (int i = 0; i < matrix.GetLength(1); i++)
//            {
//                matrix.GetColumn()
//            }
//
//        }
//        public static  T[] GetColumn(this T[,] matrix, int columnNumber)
//        {
//            return Enumerable.Range(0, matrix.GetLength(0))
//                .Select(x => matrix[x, columnNumber])
//                .ToArray();
//        }
//
////        public T[] GetRow(T[,] matrix, int rowNumber)
////        {
////            return Enumerable.Range(0, matrix.GetLength(1))
////                .Select(x => matrix[rowNumber, x])
////                .ToArray();
////        }
//    }
//}