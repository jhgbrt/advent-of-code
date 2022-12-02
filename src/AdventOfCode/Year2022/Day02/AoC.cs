namespace AdventOfCode.Year2022.Day02;
#pragma warning disable CS8524, CS8509 

using static RPS;
enum RPS { Rock = 1, Paper = 2, Scissors = 3}

public class AoC202202
{
    static string[] input = Read.InputLines();

    public int Part1() => (from line in input
                           let l = Convert(line[0])
                           let r = Convert(line[2])
                           select (int)r + Score(l, r)).Sum();

    public int Part2() => (from line in input
                           let l = Convert(line[0])
                           let r = DefineMove(l, line[2])
                           select (int)r + Score(l, r)).Sum();
    static RPS Convert(char input) => input switch
    {
        'A' or 'X' => Rock,
        'B' or 'Y' => Paper,
        'C' or 'Z' => Scissors
    };

    private int Score(RPS l, RPS r) => (l, r) switch
    {
        (Rock, Rock) => 3,
        (Rock, Paper) => 6,
        (Rock, Scissors) => 0,
        (Paper, Rock) => 0,
        (Paper, Paper) => 3,
        (Paper, Scissors) => 6,
        (Scissors, Rock) => 6,
        (Scissors, Paper) => 0,
        (Scissors, Scissors) => 3
    };

    static RPS DefineMove(RPS opponent, char outcome) => outcome switch
    {
        'X' /* lose */ => opponent switch
        {
            Rock => Scissors,
            Paper => Rock,
            Scissors => Paper
        },
        'Y' => opponent,
        'Z' /* win */ => opponent switch
        {
            Rock => Paper,
            Paper => Scissors,
            Scissors => Rock
        }
    };

}
