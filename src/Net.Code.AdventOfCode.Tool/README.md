# Advent of Code in C# helper

## Overview

This library provides a helper toolkit for the advent of code. You can use it to 

- initialize a template folder for a puzzle & download puzzle input
- post puzzle answers from the command line
- export puzzle code to a standalone C# project
- run or verify puzzle code
- show a report of all your puzzle answers and status
- show your private leaderboard
- and more

## Authentication

For many commands, you need to set the variable `AOC_SESSION` either as a user secret in your project, 
or as an environment variable value. You can find this value via a webbrowser when logged in to adventofcode.com. 
When you're logged in to the Advent of Code website, look in the network tab of the devtools window 
under Request headers. There should be a header called cookie, with a value that starts with `session=`. 
Copy everything after the equal sign.

![Get cookie value](content/f12devtools-cookie.png)

## Getting started

* Create a standard console application and install the nuget package. The console application will be containing your Advent of Code puzzle logic.
* In a terminal, go to the root folder of your console application and run 'dotnet run -- init YYYY DD'. This will initialize a folder in your project for solving the puzzle of the specified day. (During the AoC, you don't need to specify the year and day explicitly)
* In your favourite editor, open the generated 'AoC.cs' file. You will see 2 methods: `Part1()` and `Part2()`. This is where you put your logic. By default, they have the return type 'object', but you can change that.
* To run the code, use `dotnet run -- run`. This will print your solution on standard output.
* After you solved the puzzle, you can post a solution using `dotnet run -- post SOLUTION [YYYY] [DD]`. 
* To verify the code when you have solved a puzzle, you can use `dotnet run -- verify [YYYY] [DD]`. This will check if your code still returns the same answer.

## Typical workflow on a day in december
```
# initialize
dotnet run -- init

# code, test puzzle 1
dotnet run -- run

# when satisfied, post solution 1
dotnet run -- post 12345

# optional: refactor
dotnet run -- run
dotnet run -- verify

# code, test puzzle 2
dotnet run -- run

# post solution 2
dotnet run -- post 12345
```

## Overall usage

As mentioned, you can use `dotnet run -- [command]` (recommended) or `your-exe [command]` in the root of your console project folder (if you put your output folder in the PATH to run your application). Here you find an overview of the supported commands. Use the `-h` switch for more info.

```
USAGE:
    aoc.dll [OPTIONS] <COMMAND>

OPTIONS:
    -h, --help       Prints help information
    -v, --version    Prints version information

COMMANDS:
    run                Run the code for a specific puzzle
    verify             Verify the results for the given puzzle(s)
    init               Initialize the code for a specific puzzle. Requires AOC_SESSION set as an environment variable
    sync               Sync the data (specifically the posted answers) for a puzzle. Requires AOC_SESSION set as an environment variable
    show               Show the puzzle instructions
    post <SOLUTION>    Post an answer for a puzzle part. Requires AOC_SESSION set as an environment variable
    export             Export the code for a puzzle to a stand-alone C# project
    report             Show a list of all puzzles, their status (unlocked, answered), and the answers posted
    leaderboard        Show some stats from the configured private leaderboard. Set AOC_LEADERBOARD_ID as a environment variable
```

