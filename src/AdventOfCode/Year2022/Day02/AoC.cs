namespace AdventOfCode.Year2022.Day02;
#pragma warning disable CS8524, CS8509 

using static RPS;
using static Result;

enum RPS { Rock, Paper, Scissors}
enum Result { Win, Draw, Lose }
public class AoC202202
{
    static string[] input = Read.InputLines();

    public int Part1() => (from line in input
                           let opponentMove = ToMove(line[0])
                           let myMove = ToMove(line[2])
                           select CalculateScore(myMove, Play(opponentMove, myMove))).Sum();

    public int Part2() => (from line in input
                           let opponentMove = ToMove(line[0])
                           let result = ToResult(line[2])
                           let myMove = DefineMove(opponentMove, result)
                           select CalculateScore(myMove, result)).Sum();
    
    static int CalculateScore(RPS move, Result result)
    {
        var value = move switch { Rock => 1, Paper => 2, Scissors => 3 };
        var score = result switch { Lose => 0, Draw => 3, Win => 6 };
        return value + score;
    }

    static RPS ToMove(char input) => input switch
    {
        'A' or 'X' => Rock,
        'B' or 'Y' => Paper,
        'C' or 'Z' => Scissors
    };

    static Result ToResult(char input) => input switch
    {
        'X' => Lose,
        'Y' => Draw,
        'Z' => Win
    };

    static Result Play(RPS opponentMove, RPS myMove) => (opponentMove, myMove) switch
    {
        (Rock, Rock) => Draw,
        (Rock, Paper) => Win,
        (Rock, Scissors) => Lose,
        (Paper, Rock) => Lose,
        (Paper, Paper) => Draw,
        (Paper, Scissors) => Win,
        (Scissors, Rock) => Win,
        (Scissors, Paper) => Lose,
        (Scissors, Scissors) => Draw
    };

    static RPS DefineMove(RPS opponentMove, Result outcome) => outcome switch
    {
        Lose => opponentMove switch
        {
            Rock => Scissors,
            Paper => Rock,
            Scissors => Paper
        },
        Draw => opponentMove,
        Win => opponentMove switch
        {
            Rock => Paper,
            Paper => Scissors,
            Scissors => Rock
        }
    };
}
