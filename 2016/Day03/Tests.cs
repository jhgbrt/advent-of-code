using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Jeroen
{

    public class Tests
    {
        private ITestOutputHelper output;

        public Tests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData(5, 10, 25, false)]
        [InlineData(5, 25, 10, false)]
        [InlineData(25, 5, 10, false)]
        [InlineData(25, 10, 5, false)]
        [InlineData(10, 25, 5, false)]
        [InlineData(10, 5, 25, false)]
        [InlineData(5, 10, 12, true)]
        [InlineData(5, 12, 10, true)]
        [InlineData(12, 5, 10, true)]
        [InlineData(12, 10, 5, true)]
        [InlineData(10, 12, 5, true)]
        [InlineData(10, 5, 12, true)]
        [InlineData(3, 4, 5, true)]
        public void Test(int x, int y, int z, bool expected)
        {
            Assert.Equal(IsPossibleTriangle((x, y, z)), expected);
        }

        public bool IsPossibleTriangle((int x, int y, int z) triangle)
        {
            return triangle.x + triangle.y > triangle.z
                && triangle.y + triangle.z > triangle.x
                && triangle.x + triangle.z > triangle.y;
        }

        IEnumerable<string> ReadInputs1()
        {
            var stream = File.OpenRead("input.txt");
            using (var reader = new StreamReader(stream))
            {
                while (reader.Peek() > 0)
                {
                    yield return reader.ReadLine();
                }
            }
        }

        IEnumerable<(int x, int y, int z)> GetTriangles()
        {
            return from line in ReadInputs1()
            let triangle = (
                int.Parse(line.Substring(2, 3).Trim()),
                int.Parse(line.Substring(7, 3).Trim()),
                int.Parse(line.Substring(12, 3).Trim())
                )
            select triangle;
        }

        [Fact]
        public void Day3Part1()
        {
            var triangles = from triangle in GetTriangles()
                            where IsPossibleTriangle(triangle)
                            select triangle;
            var count = triangles.Count();
            output.WriteLine(count.ToString());
        }

        [Fact]
        public void Day3Part2()
        {
            var integers = from line in ReadInputs1()
                           let x = int.Parse(line.Substring(2, 3).Trim())
                           let y = int.Parse(line.Substring(7, 3).Trim())
                           let z = int.Parse(line.Substring(12, 3).Trim())
                           select (x, y, z);

            var triangles = from chunk in GetTriangles().Chunk(3)
                            from triangle in Transpose(chunk)
                            where IsPossibleTriangle(triangle)
                            select triangle;

            var count = triangles.Count();
            output.WriteLine(count.ToString());
        }

        private IEnumerable<(int x,int y,int z)> Transpose((int x, int y, int z)[] chunk)
        {
            yield return (chunk[0].x, chunk[1].x, chunk[2].x);
            yield return (chunk[0].y, chunk[1].y, chunk[2].y);
            yield return (chunk[0].z, chunk[1].z, chunk[2].z);
        }
    }

    public static class Extensions
    {
        public static IEnumerable<T[]> Chunk<T>(this IEnumerable<T> items, int size)
        {
            T[] array = items as T[] ?? items.ToArray();
            for (int i = 0; i < array.Length; i += size)
            {
                T[] chunk = new T[Math.Min(size, array.Length - i)];
                Array.Copy(array, i, chunk, 0, chunk.Length);
                yield return chunk;
            }
        }
    }
}
