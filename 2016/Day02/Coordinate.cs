using System;

namespace Jeroen.Day2
{
    public struct Coordinate
    {
        public readonly int Row;
        public readonly int Column;

        public Coordinate(int row, int column)
        {
            Row = row;
            Column = column;
        }
        public Coordinate Left() => new Coordinate(Row, Column - 1);
        public Coordinate Right() => new Coordinate(Row, Column + 1);
        public Coordinate Up() => new Coordinate(Row - 1, Column);
        public Coordinate Down() => new Coordinate(Row + 1, Column);
        public Coordinate Move(char direction)
        {
            switch (direction)
            {
                case 'U': return Up();
                case 'D': return Down();
                case 'L': return Left();
                case 'R': return Right();
            }
            throw new InvalidOperationException();
        }
    }
}