using AdventOfCode2019.Helpers;
using AdventOfCode2019.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Year2019.Day07
{
    public class Day072019
    {
        private List<string> _Input { get; set; }
        private List<string> Amplifiers { get; set; } = new List<string>();

        public Day072019(List<string> input)
        {
            _Input = input;
        }

        public string Part1()
        {
            Permute("01234", 0, "01234".Length - 1);
            return Amplifiers.Select(amp =>
            {
                List<int> datalist = new List<int>();
                datalist.AddRange(amp.Select(c => int.Parse(c.ToString())));
                return LastOutput(datalist);
            }).Max().ToString();
        }

        private int LastOutput(List<int> ampli)
        {
            int input = 0;
            ampli.ForEach(amp => input = ModifyArray(_Input, amp, input));
            return input;
        }

        public string Part2()
        {
            Permute("56789", 0, "56789".Length - 1);
            return Amplifiers.Select(amp =>
            {
                List<int> datalist = new List<int>();
                datalist.AddRange(amp.Select(c => int.Parse(c.ToString())));
                return LastOutputLoop(datalist);
            }).Max().ToString();
        }

        private int LastOutputLoop(List<int> phaseSesttings)
        {
            int input = 0;
            var runningAmp = new List<AmplifierState>
            {
                new AmplifierState(5, _Input[0].Split(",").Select(val => long.Parse(val)).ToList()),
                new AmplifierState(6, _Input[0].Split(",").Select(val => long.Parse(val)).ToList()),
                new AmplifierState(7, _Input[0].Split(",").Select(val => long.Parse(val)).ToList()),
                new AmplifierState(8, _Input[0].Split(",").Select(val => long.Parse(val)).ToList()),
                new AmplifierState(9, _Input[0].Split(",").Select(val => long.Parse(val)).ToList()),
            };
            while (!runningAmp.All(amp => amp.Finished))
            {
                foreach (var amp in phaseSesttings)
                {
                    input = ModifyArrayMemory(runningAmp[amp-5], amp, input).Output;
                }
            }

            return input;
        }

        private void Permute(string str,
                                    int l, int r)
        {
            if (l == r)
                Amplifiers.Add(str);
            else
            {
                for (int i = l; i <= r; i++)
                {
                    str = Swap(str, l, i);
                    Permute(str, l + 1, r);
                    str = Swap(str, l, i);
                }
            }
        }

        public static string Swap(string a,
                                  int i, int j)
        {
            char temp;
            char[] charArray = a.ToCharArray();
            temp = charArray[i];
            charArray[i] = charArray[j];
            charArray[j] = temp;
            string s = new string(charArray);
            return s;
        }

        public static AmplifierState ModifyArrayMemory(AmplifierState ampli, int input1, int input2)
        {

            while (true)
            {
                var currentInstruction = ampli.Array[ampli.Index].ToString();
                long mode1 = 0;
                long mode2 = 0;
                if (ampli.Array[ampli.Index].ToString().Length > 1)
                {
                    currentInstruction = ampli.Array[ampli.Index].ToString().Last().ToString();
                    if (ampli.Array[ampli.Index].ToString().Length == 3)
                    {
                        mode1 = long.Parse(ampli.Array[ampli.Index].ToString().First().ToString());
                    }
                    else
                    {
                        mode2 = 1;
                        mode1 = long.Parse(ampli.Array[ampli.Index].ToString().ElementAt(1).ToString());
                    }
                }
                switch (currentInstruction)
                {
                    case "1":
                        Utils.Adds(ampli.Array, ampli.Index, mode1, mode2, 0, 0);
                        ampli.Index += 4;
                        break;
                    case "2":
                        Utils.Multiply(ampli.Array, ampli.Index, mode1, mode2, 0, 0);
                        ampli.Index += 4;
                        break;
                    case "3":
                        if (ampli.InstIndex == 0)
                        {
                            Utils.Replace(ampli.Array, ampli.Index, input1, mode1, 0);
                        }
                        else
                        {
                            Utils.Replace(ampli.Array, ampli.Index, input2, mode1, 0);
                        }
                        ampli.InstIndex++;
                        ampli.Index += 2;
                        break;
                    case "4":
                        ampli.Output = (int)Utils.Output(ampli.Array, ampli.Index, mode1, 0);
                        ampli.Index += 2;
                        return ampli;
                    case "5":
                        ampli.Index = (int)Utils.JumpIfTrue(ampli.Array, ampli.Index, mode1, mode2, 0);
                        break;
                    case "6":
                        ampli.Index = (int)Utils.JumpIfFalse(ampli.Array, ampli.Index, mode1, mode2, 0);
                        break;
                    case "7":
                        Utils.LessThan(ampli.Array, ampli.Index, mode1, mode2, 0, 0);
                        ampli.Index += 4;
                        break;
                    case "8":
                        Utils.Equals(ampli.Array, ampli.Index, mode1, mode2, 0, 0);
                        ampli.Index += 4;
                        break;
                    default:
                        ampli.Finished = true;
                        return ampli;
                }
            }
        }

        public static int ModifyArray(List<string> code, int input1, int input2)
        {
            var line = code[0];
            var array = line.Split(",").Select(val => long.Parse(val)).ToList();

            var result = new List<long>();
            int inst = 0;
            int inputInst = 0;
            while (true)
            {
                var currentInstruction = array[inst].ToString();
                long mode1 = 0;
                long mode2 = 0;
                if (array[inst].ToString().Length > 1)
                {
                    currentInstruction = array[inst].ToString().Last().ToString();
                    if (array[inst].ToString().Length == 3)
                    {
                        mode1 = int.Parse(array[inst].ToString().First().ToString());
                    }
                    else
                    {
                        mode2 = int.Parse(array[inst].ToString().First().ToString());
                        mode1 = int.Parse(array[inst].ToString().ElementAt(1).ToString());
                    }
                }
                switch (currentInstruction)
                {
                    case "1":
                        Utils.Adds(array, inst, mode1, mode2, 0, 0);
                        inst += 4;
                        break;
                    case "2":
                        Utils.Multiply(array, inst, mode1, mode2, 0, 0);
                        inst += 4;
                        break;
                    case "3":
                        if (inputInst == 0)
                        {
                            Utils.Replace(array, inst, input1, mode1, 0);
                        }
                        else
                        {
                            Utils.Replace(array, inst, input2, mode1, 0);
                        }
                        inputInst++;
                        inst += 2;
                        break;
                    case "4":
                        result.Add(Utils.Output(array, inst, mode1, 0));
                        inst += 2;
                        break;
                    case "5":
                        inst = (int)Utils.JumpIfTrue(array, inst, mode1, mode2, 0);
                        break;
                    case "6":
                        inst = (int)Utils.JumpIfFalse(array, inst, mode1, mode2, 0);
                        break;
                    case "7":
                        Utils.LessThan(array, inst, mode1, mode2, 0, 0);
                        inst += 4;
                        break;
                    case "8":
                        Utils.Equals(array, inst, mode1, mode2, 0, 0);
                        inst += 4;
                        break;
                    default:
                        return (int)result.Last();
                }
            }
        }
    }
}

namespace AdventOfCode2019.Models
{
    public class AmplifierState
    {
        public int Key { get; set; }

        public List<long> Array { get; set; }

        public int Output { get; set; }

        public int Index { get; set; } = 0;

        public int InstIndex { get; set; } = 0;

        public bool Finished { get; set; } = false;

        public AmplifierState(int key, List<long> array)
        {
            Key = key;
            Array = array;
        }
    }
}



namespace AdventOfCode2019.Helpers
{
    public static class Utils
    {

        public static int CountOccurence(string str, char count)
        {
            return str.Count(character => character == count);
        }

        public static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }

        public static void Multiply(List<long> array, int idx, long mode1, long mode2, long mode3, long relative)
        {
            long firstParam = 0;
            long secondParam = 0;
            if (mode1 != 1)
            {
                firstParam = array[(int)(array[idx + 1] + (mode1 == 2 ? relative : 0))];
            }
            else
            {
                firstParam = array[idx + 1];
            }
            if (mode2 != 1)
            {
                secondParam = array[(int)(array[idx + 2] + (mode2 == 2 ? relative : 0))];
            }
            else
            {
                secondParam = array[idx + 2];
            }
            var multiply = firstParam * secondParam;
            array[(int)(array[idx + 3] + (mode3 == 2 ? relative : 0))] = multiply;
        }

        public static void Adds(List<long> array, int idx, long mode1, long mode2, long mode3, long relative)
        {
            long firstParam = 0;
            long secondParam = 0;
            if (mode1 != 1)
            {
                firstParam = array[(int)(array[idx + 1] + (mode1 == 2 ? relative : 0))];
            }
            else
            {
                firstParam = array[idx + 1];
            }
            if (mode2 != 1)
            {
                secondParam = array[(int)(array[idx + 2] + (mode2 == 2 ? relative : 0))];
            }
            else
            {
                secondParam = array[idx + 2];
            }
            var add = firstParam + secondParam;
            array[(int)(array[idx + 3] + (mode3 == 2 ? relative : 0))] = add;
        }

        public static void Replace(List<long> array, int idx, long val, long mode1, long relative)
        {
            if (mode1 != 1)
            {
                array[(int)(array[idx + 1] + (mode1 == 2 ? relative : 0))] = val;
            }
            else
            {
                array[idx + 1] = val;
            }
        }

        public static long Output(List<long> array, int idx, long mode1, long relative)
        {
            if (mode1 != 1)
            {
                return array[(int)(array[idx + 1] + (mode1 == 2 ? relative : 0))];
            }
            else
            {
                return array[idx + 1];
            }
        }

        public static long JumpIfTrue(List<long> array, int idx, long mode1, long mode2, long relative)
        {
            long firstParam = 0;
            long secondParam = 0;
            if (mode1 != 1)
            {
                firstParam = array[(int)(array[idx + 1] + (mode1 == 2 ? relative : 0))];
            }
            else
            {
                firstParam = array[idx + 1];
            }
            if (mode2 != 1)
            {
                secondParam = array[(int)(array[idx + 2] + (mode2 == 2 ? relative : 0))];
            }
            else
            {
                secondParam = array[idx + 2];
            }
            if (firstParam != 0)
            {
                return secondParam;
            }
            return idx + 3;
        }

        public static long JumpIfFalse(List<long> array, int idx, long mode1, long mode2, long relative)
        {
            long firstParam = 0;
            long secondParam = 0;
            if (mode1 != 1)
            {
                firstParam = array[(int)(array[idx + 1] + (mode1 == 2 ? relative : 0))];
            }
            else
            {
                firstParam = array[idx + 1];
            }
            if (mode2 != 1)
            {
                secondParam = array[(int)(array[idx + 2] + (mode2 == 2 ? relative : 0))];
            }
            else
            {
                secondParam = array[idx + 2];
            }
            if (firstParam == 0)
            {
                return secondParam;
            }
            return idx + 3;
        }

        public static void LessThan(List<long> array, int idx, long mode1, long mode2, long mode3, long relative)
        {
            long firstParam = 0;
            long secondParam = 0;
            var res = 0;
            if (mode1 != 1)
            {
                firstParam = array[(int)(array[idx + 1] + (mode1 == 2 ? relative : 0))];
            }
            else
            {
                firstParam = array[idx + 1];
            }
            if (mode2 != 1)
            {
                secondParam = array[(int)(array[idx + 2] + (mode2 == 2 ? relative : 0))];
            }
            else
            {
                secondParam = array[idx + 2];
            }
            if (firstParam < secondParam)
            {
                res = 1;
            }
            array[(int)(array[idx + 3] + (mode3 == 2 ? relative : 0))] = res;
        }

        public static void Equals(List<long> array, int idx, long mode1, long mode2, long mode3, long relative)
        {
            long firstParam = 0;
            long secondParam = 0;
            var res = 0;
            if (mode1 != 1)
            {
                firstParam = array[(int)(array[idx + 1] + (mode1 == 2 ? relative : 0))];
            }
            else
            {
                firstParam = array[idx + 1];
            }
            if (mode2 != 1)
            {
                secondParam = array[(int)(array[idx + 2] + (mode2 == 2 ? relative : 0))];
            }
            else
            {
                secondParam = array[idx + 2];
            }
            if (firstParam == secondParam)
            {
                res = 1;
            }
            array[(int)(array[idx + 3] + (mode3 == 2 ? relative : 0))] = res;
        }

        public static long Relative(List<long> array, int idx, long mode, long relative)
        {
            if (mode != 1)
            {
                return relative + array[(int)(array[idx + 1] + (mode == 2 ? relative : 0))];
            }
            else
            {
                return relative + array[idx + 1];
            }
        }
    }

}