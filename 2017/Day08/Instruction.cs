using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Registers
{
    struct Instruction
    {
        internal static Regex r = new Regex(
            @"^(?<register>[a-z]+) (?<operation>inc|dec) (?<amount>-{0,1}\d+) if (?<testregister>[a-z]+) (?<operator>[<>=!]{1,2}) (?<comparisonValue>-{0,1}[\d]+)", RegexOptions.Compiled);

        public static Instruction Parse(string input)
        {
            var match = r.Match(input);
            return new Instruction(
                match.Groups["register"].Value,
                ParseOperation(match.Groups["register"].Value, match.Groups["operation"].Value, int.Parse(match.Groups["amount"].Value)),
                ParsePredicate(match.Groups["testregister"].Value, match.Groups["operator"].Value, match.Groups["comparisonValue"].Value)
            );
        }

        private static Func<IDictionary<string,int>, bool> ParsePredicate(string s, string comparisonOperator, string value)
        {
            int i = int.Parse(value);
            switch (comparisonOperator)
            {
                case "<":
                    return d => d[s] < i;
                case "<=":
                    return d => d[s] <= i;
                case ">":
                    return d => d[s] > i;
                case ">=":
                    return d => d[s] >= i;
                case "==":
                    return d => d[s] == i;
                case "!=":
                    return d => d[s] != i;
            }
            throw new ArgumentException(nameof(comparisonOperator));
        }

        private static Action<IDictionary<string, int>> ParseOperation(string s, string operation, int amount)
        {
            switch (operation)
            {
                case "dec": return d => d[s] -= amount;
                case "inc": return d => d[s] += amount;
            }
            throw new ArgumentException();
        }

        public readonly string Register;
        public readonly Action<IDictionary<string,int>> Action;
        public readonly Func<IDictionary<string, int>, bool> Predicate;
        public Instruction(string register, Action<IDictionary<string,int>> action, Func<IDictionary<string, int>, bool> predicate)
        {
            Register = register;
            Action = action;
            Predicate = predicate;
        }

        public int Apply(IDictionary<string, int> dictionary)
        {
            if (Predicate(dictionary))
                Action(dictionary);
            return dictionary[Register];
        }
    }
}