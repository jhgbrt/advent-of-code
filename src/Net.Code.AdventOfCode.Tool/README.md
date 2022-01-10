# Advent of Code in C# helper

## Overview

This library provides a helper toolkit for the advent of code. You can use it to 

- automatically download puzzle input
- post puzzle answers from the command line
- export puzzle code to a standalone C# project
- run or verify puzzle code
- show a report of all your puzzle answers and status
- show your private leaderboard
- and more

## Getting started

* Create a standard console application and install the nuget package. The console application will be containing your Advent of Code puzzle logic.
* For many commands, you need to set the variable AOC_SESSION either as a user secret in your project, or as an environment variable to the value of your logged in session on adventofcode.com (use your browser devtools to grab this value)
* In a terminal, go to the root folder of your console application and run 'dotnet run -- init YYYY DD'. This will initialize a folder in your project for solving the puzzle of the specified day. (During the AoC, you don't need to specify the year and day explicitly)
* In your favourite editor, open the generated 'AoC.cs' file. You will see 2 methods: `Part1()` and `Part2()`. This is where you put your logic. 
* To run the code, use `dotnet run -- exec`. This will print your solution on standard output.
* After you solved the puzzle, you can post a solution using `dotnet run -- post SOLUTION [YYYY] [DD]`. 
* To verify the code when you have solved a puzzle, you can use `dotnet run -- verify [YYYY] [DD]`. This will check if your code still returns the same answer.

## Overall usage

As mentioned, you can use `dotnet run -- [command]` (recommended) or `your-exe` [command]` in the root of your console project folder (if you put your output folder in the PATH to run your application). Here you find an overview of the supported commands. Use the `-h` switch for more info.

```
USAGE:
    aoc.dll [OPTIONS] <COMMAND>

OPTIONS:
    -h, --help       Prints help information
    -v, --version    Prints version information

COMMANDS:
    exec               Run the code for a specific puzzle
    verify             Verify the results for the given puzzle(s)
    init               Initialize the code for a specific puzzle. Requires AOC_SESSION set as an environment variable
    sync               Sync the data (specifically the posted answers) for a puzzle. Requires AOC_SESSION set as an environment variable
    show               Show the puzzle instructions
    post <SOLUTION>    Post an answer for a puzzle part. Requires AOC_SESSION set as an environment variable
    export             Export the code for a puzzle to a stand-alone C# project
    report             Show a list of all puzzles, their status (unlocked, answered), and the answers posted
    leaderboard        Show some stats from the configured private leaderboard. Set AOC_LEADERBOARD_ID as a environment variable
```

