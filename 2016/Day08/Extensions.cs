using System;
using System.Collections.Generic;
using System.Linq;

namespace Jeroen
{
    public static class Extensions
    {
        public static IEnumerable<T> Rotate<T>(this IList<T> input, int offset)
        {
            return input.Skip(input.Count - offset).Concat(input.Take(input.Count - offset));
        }

        public static void RotateRow<T>(this T[,] array, int row, int n)
        {
            array.ReplaceRow(row, array.Row(row).ToList().Rotate(n).ToArray());
        }
        public static void RotateCol<T>(this T[,] array, int col, int n)
        {
            array.ReplaceCol(col, array.Column(col).ToList().Rotate(n).ToArray());
        }

        public static IEnumerable<T> Row<T>(this T[,] array, int row)
        {
            for (int i = 0; i < array.GetLength(1); i++)
                yield return array[row, i];
        }
        public static IEnumerable<T> Column<T>(this T[,] array, int column)
        {
            for (int i = 0; i < array.GetLength(0); i++)
                yield return array[i, column];
        }

        public static void ReplaceRow<T>(this T[,] array, int row, T[] replacement)
        {
            for (int col = 0; col < replacement.Length; col++)
            {
                array[row, col] = replacement[col];
            }
        }
        public static void ReplaceCol<T>(this T[,] array, int col, T[] replacement)
        {
            for (int row = 0; row < replacement.Length; row++)
            {
                array[row, col] = replacement[row];
            }
        }


        public static void Display<T>(this T[,] array, Func<T,string> tostring = null)
        {
            if (tostring == null) tostring = t => t.ToString();

            for (int row = 0; row <= array.GetUpperBound(0); row++)
            {
                for (int col = 0; col <= array.GetUpperBound(1); col++)
                {
                    Console.Write(tostring(array[row, col]));
                }
                Console.WriteLine();
            }
        }


    }
}