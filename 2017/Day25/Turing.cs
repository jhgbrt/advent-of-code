using System.Collections.Generic;
using System.Linq;

static class Turing
{
    public static int CalculateChecksum(this (char beginState, int steps, string code) input)
    {
        var instructions = input.code.ReadLines().Select(line =>
            (
            currentstate: line[0],
            currentvalue: int.Parse(line.Substring(1, 1)),
            newvalue: int.Parse(line.Substring(2, 1)),
            delta: line[3] == '+' ? +1 : -1,
            newstate: line[4])
        ).ToDictionary(x => (x.currentstate, x.currentvalue));

        var tape = new Dictionary<int, int>();

        int GetValue(int i)
        {
            return tape.ContainsKey(i) ? tape[i] : 0;
        }

        var cursor = 0;
        var state = input.beginState;
        for (int i = 0; i < input.steps; i++)
        {
            var value = GetValue(cursor);
            var instruction = instructions[(state, value)];
            tape[cursor] = instruction.newvalue;
            cursor += instruction.delta;
            state = instruction.newstate;
        }

        var actual = tape.Values.Sum();
        return actual;
    }

}