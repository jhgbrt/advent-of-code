using System;
using System.Collections.Generic;
using System.Linq;

class MazeRunner
{
    private enum Direction { Up, Down, Left, Right }
    private readonly string[] _maze;

    public MazeRunner(string[] maze)
    {
        _maze = maze;
    }

    public (string code, int steps) Traverse()
    {
        var path = TraversePrivate().ToArray();
        return (new string(path.Where(char.IsLetter).ToArray()), path.Length);
    }

    IEnumerable<char> TraversePrivate()
    {
        var ((x, y), direction, c) = ((_maze[0].IndexOf('|'), 0), Direction.Down, '|');

        while (true)
        {
            (x, y) = Step();
            c = this[(x, y)];
            yield return c;
            if (c == '+')
                direction = Turn();
            if (c == ' ')
                yield break;
        }


        IEnumerable<((int x, int y) pos, Direction d)> Neighbors()
        {
            if (x > 0) yield return ((x - 1, y), Direction.Left);
            if (y < _maze.Length - 1) yield return ((x, y + 1), Direction.Down);
            if (x < _maze[y].Length - 1) yield return ((x + 1, y), Direction.Right);
            if (y > 0) yield return ((x, y - 1), Direction.Up);
        }

        Direction Turn() => Neighbors().Single(_ => _.d != Reverse() && !char.IsWhiteSpace(this[_.pos])).d;

        (int x, int y) Step()
        {
            switch (direction)
            {
                case Direction.Up: return (x, y - 1);
                case Direction.Down: return (x, y + 1);
                case Direction.Left: return (x - 1, y);
                case Direction.Right: return (x + 1, y);
                default: throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        Direction Reverse()
        {
            switch (direction)
            {
                case Direction.Up: return Direction.Down;
                case Direction.Down: return Direction.Up;
                case Direction.Left: return Direction.Right;
                case Direction.Right: return Direction.Left;
                default: throw new ArgumentOutOfRangeException();
            }
        }

    }
    char this[(int x, int y) pos] => _maze[pos.y][pos.x];
}