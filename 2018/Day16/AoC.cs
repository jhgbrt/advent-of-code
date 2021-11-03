using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    static class AoC
    {
        public static int Part1(string[] input)
        {
            var q = from sample in input.Parse()
                    from opcode in OpCode.All
                    let result = OpCode.apply(sample.before, sample.instruction, opcode)
                    where result.SequenceEqual(sample.after)
                    group sample by sample into g
                    where g.Count() >= 3
                    select g;

            return q.Count();
        }

        public static int Part2(string[] input)
        {
            var notmatching = (
                from sample in input.Parse()
                let code = sample.instruction.code
                group sample by code into sampleGroup
                from candidate in OpCode.All
                from sample in sampleGroup
                let result = OpCode.apply(sample.before, sample.instruction, candidate)
                where !result.SequenceEqual(sample.after)
                select (sampleGroup.Key, candidate)
                ).Distinct().ToLookup(x => x.Key, x => x.candidate);

            var opcodes = OpCode.All.ToList();
            var actualOpcodes = new Dictionary<int, Action<int[], int, int, int, int>>();
            do
            {
                var grp = notmatching.First(g => g.Count() == opcodes.Count - 1);
                var opcode = opcodes.Except(grp).Single();
                opcodes = opcodes.Where(o => o != opcode).ToList();
                notmatching = (from sample in notmatching
                               where sample.Key != grp.Key
                               from item in sample
                               where item != opcode
                               select (sample.Key, item))
                    .ToLookup(x => x.Key, x => x.item);
                actualOpcodes[grp.Key] = opcode;
            } while (notmatching.Any());

            var missing = Enumerable.Range(0, 16).Where(i => !actualOpcodes.ContainsKey(i)).Single();

            actualOpcodes[missing] = opcodes.Single();

            var registers = new int[4];
            foreach (var instruction in input.GetInstructions())
            {
                var opcode = actualOpcodes[instruction.code];
                registers = OpCode.apply(registers, instruction, opcode);
            }


            return registers[0];
        }
        static IEnumerable<(int code, int a, int b, int c)> GetInstructions(this string[] input)
        {
            int blank = 0;
            int i = 0;
            for (i = 0; i < input.Length; i++)
            {
                if (string.IsNullOrEmpty(input[i])) blank++;
                else blank = 0;
                if (blank == 3)
                    break;
            }
            for (i = i + 1; i < input.Length; i++)
            {
                var instruction = input[i].Split(' ').Select(int.Parse).ToArray();
                yield return (instruction[0], instruction[1], instruction[2], instruction[3]);
            }
        }

        static IEnumerable<(int[] before, (int code, int a, int b, int c) instruction, int[] after)> Parse(this string[] input)
        {
            for (int i = 0; i < input.Length; i += 4)
            {
                if (input[i].StartsWith("Before"))
                {
                    var before = input[i][9..^1].Split(',').Select(int.Parse).ToArray();
                    var instruction = input[i+1].Split(' ').Select(int.Parse).ToArray();
                    var after = input[i+2][9..^1].Split(',').Select(int.Parse).ToArray();
                    yield return (before, (instruction[0], instruction[1], instruction[2], instruction[3]), after);
                }
                else
                {
                    break;
                }
            }
        }

    }

    class OpCode
    {
        public static IList<Action<int[], int, int, int, int>> All = new Action<int[], int, int, int, int>[]
        {
            addr,
            addi,
            mulr,
            muli,
            banr,
            bani,
            borr,
            bori,
            setr,
            seti,
            gtir,
            gtri,
            gtrr,
            eqir,
            eqri,
            eqrr,
        };

        public static int[] apply(int[] input, (int code, int a, int b, int c) instruction, Action<int[], int, int, int, int> opcode)
        {
            int[] result = input.ToArray();
            opcode(result, instruction.code, instruction.a, instruction.b, instruction.c);
            return result;
        }

        public static void addr(int[] r, int code, int a, int b, int c) => r[c] = r[a] + r[b];
        public static void addi(int[] r, int code, int a, int b, int c) => r[c] = r[a] + b;
        public static void mulr(int[] r, int code, int a, int b, int c) => r[c] = r[a] * r[b];
        public static void muli(int[] r, int code, int a, int b, int c) => r[c] = r[a] * b;
        public static void banr(int[] r, int code, int a, int b, int c) => r[c] = r[a] & r[b];
        public static void bani(int[] r, int code, int a, int b, int c) => r[c] = r[a] & b;
        public static void borr(int[] r, int code, int a, int b, int c) => r[c] = r[a] | r[b];
        public static void bori(int[] r, int code, int a, int b, int c) => r[c] = r[a] | b;
        public static void setr(int[] r, int code, int a, int b, int c) => r[c] = r[a];
        public static void seti(int[] r, int code, int a, int b, int c) => r[c] = a;
        public static void gtir(int[] r, int code, int a, int b, int c) => r[c] = a > r[b] ? 1 : 0;
        public static void gtri(int[] r, int code, int a, int b, int c) => r[c] = r[a] > b ? 1 : 0;
        public static void gtrr(int[] r, int code, int a, int b, int c) => r[c] = r[a] > r[b] ? 1 : 0;
        public static void eqir(int[] r, int code, int a, int b, int c) => r[c] = a == r[b] ? 1 : 0;
        public static void eqri(int[] r, int code, int a, int b, int c) => r[c] = r[a] == b ? 1 : 0;
        public static void eqrr(int[] r, int code, int a, int b, int c) => r[c] = r[a] == r[b] ? 1 : 0;

    }
}

