using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    internal static class AoC
    {
        public static int Part1(string[] input)
        {
            var points = input.ToCoordinates().OrderBy(c => c.x).ThenBy(c => c.y);
            var maxX = points.Max(c => c.x);
            var maxY = points.Max(c => c.y);
            var q = from location in Grid(maxX, maxY)
                    let cd = (
                        from c in points
                        let d = Math.Abs(c.x - location.x) + Math.Abs(c.y - location.y)
                        orderby d
                        group (c, d) by d
                        )
                    let first = cd.First()
                    where first.Count() == 1
                    select (location, point: first.First().c, first.First().d);

            var items = q.ToList();

            var excluded = new HashSet<(int, int)>(
                    from item in items
                    where item.location.x == 0 || item.location.x == maxX || item.location.y == 0 || item.location.y == maxY
                    select item.point
                );

            var largest = (
                from item in items
                where !excluded.Contains(item.point)
                group item by item.point into x
                select (coordinate: x.Key, area: x.Count()) into result
                orderby result.area descending
                select result
                    ).First();

            return largest.area;
        }

        public static int Part2(string[] input) => Part2(input, 10000);

        public static int Part2(string[] input, int max)
        {
            var points = input.ToCoordinates().OrderBy(c => c.x).ThenBy(c => c.y);
            var maxX = points.Max(c => c.x);
            var maxY = points.Max(c => c.y);
            var q = from location in Grid(maxX, maxY)
                    let d = (
                        from point in points
                        select Math.Abs(location.x - point.x) + Math.Abs(location.y - point.y)
                    ).Sum()
                    where d < max
                    select (location.x, location.y, d);

            return q.Count();
        }

        private static IEnumerable<(int x, int y)> Grid(int maxX, int maxY) => from x in Enumerable.Range(0, maxX)
                                                                               from y in Enumerable.Range(0, maxY)
                                                                               select (x, y);

        private static readonly Regex _regex = new Regex(@"(\d+), (\d+)", RegexOptions.Compiled);
        public static (int x, int y) ToCoordinate(this string input)
        {
            var match = _regex.Match(input);
            return (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
        }

        public static IEnumerable<(int x, int y)> ToCoordinates(this IEnumerable<string> input) => input.Select(ToCoordinate);

    }
}
