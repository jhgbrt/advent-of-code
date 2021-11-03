namespace Jeroen.Day2
{
    public class Keypad
    {
        char?[,] _keys;
        Coordinate _coordinate;

        public Keypad(char?[,] keys, Coordinate coordinate)
        {
            _keys = keys;
            _coordinate = coordinate;
        }

        public char? Current => _keys[_coordinate.Row, _coordinate.Column];

        public void Move(char direction)
        {
            var next = _coordinate.Move(direction);
            if (
                next.Row >= 0 && next.Row < _keys.GetLength(0)
                && next.Column >= 0 && next.Column < _keys.GetLength(1)
                && _keys[next.Row, next.Column].HasValue
                )
                _coordinate = next;
        }
    }
}