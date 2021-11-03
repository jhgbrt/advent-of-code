using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    class Grid
    {
        Point[] _points;

        public Grid(IEnumerable<Point> points, int ticks = 0)
        {
            _points = points.ToArray();
            Ticks = ticks;
        }

        public Grid Move(int ticks) => new Grid(_points.Select(p => p.Move(ticks)), Ticks + ticks);

        int MinX => _points.Min(p => p.X);
        int MaxX => _points.Max(p => p.X);
        int MinY => _points.Min(p => p.Y);
        int MaxY => _points.Max(p => p.Y);
        public int Ticks { get; }
        public int Width => MaxX - MinX + 1;
        public int Height => MaxY - MinY + 1;

        public override string ToString()
        {
            var grid = Enumerable.Range(0, Height)
                .Select(i => Enumerable.Repeat('.', Width).ToArray())
                .ToArray();

            foreach (var point in _points)
            {
                grid[point.Y - MinY][point.X - MinX] = '#';
            }

            return string.Join(Environment.NewLine, grid.Select(a => new string(a)));
        }
    }
}
