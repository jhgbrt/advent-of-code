using System.Collections.Immutable;

namespace Registers
{
    class Cpu
    {
        private readonly IImmutableList<Instruction> _instructions;
        private readonly IDictionary<string, int> _memory;
        private int _m = int.MinValue;
        public Cpu(IImmutableList<Instruction> instructions)
        {
            _instructions = instructions;
            _memory = instructions.Select(i => i.Register).Distinct().ToDictionary(s => s, _ => 0);
        }

        public Cpu Run()
        {
            _m = _instructions.Select(i => i.Apply(_memory)).Max();
            return this;
        }

        public int MaxCurrentValue() => _memory.Values.Max();
        public int MaxValueEver() => _m;
    }
}