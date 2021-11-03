using static AoC;

var lines = ReadInput(false);

(var row, var column) = (3010, 3019);
(long code, long m, long d) = (20151125, 252533, 33554393);

Part1(() =>
{
    (var r, var c) = (1, 1);
    while (true)
    {
        (r, c) = (r - 1, c + 1);
        if (r == 0) (r, c) = (c, 1);
        code = (m * code) % d;
        if ((r, c) == (row, column)) return code;
    }
});

Part2(() => 0);
