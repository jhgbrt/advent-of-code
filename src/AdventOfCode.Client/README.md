# Advent of Code in C# helper

## Overview

This library provides a helper toolkit for the advent of code. You can use it to 

- automatically download puzzle input
- post puzzle answers from the command line
- export puzzle code to a standalone C# project
- verify puzzle code
- list puzzle answers and status

## Getting started

* Create a console application and install the nuget package. The console application will be containing your Advent of Code puzzle logic.
* In a terminal, go to the root folder of your console application and run 'dotnet run -- init -y YYYY -d DD'. This will initialize a folder in your project for solving the puzzle of the specified day.
* In your favourite editor, open the generated 'AoC.cs' file. You will see 2 methods: `Part1()` and `Part2()`. This is where you put your logic. 
* After you solved the puzzle, you can post a solution using `dotnet run -- post -y YYYY -d DD -v [VALUE]`. You will need to set the variable AOC_SESSION either as a user secret in your project, or as an environment variable to the value of your logged in session on adventofcode.com (use your browser devtools to grab this value)
* Then you can sync the puzzle data for part 2, and download the answer using `dotnet run -- sync -y YYYY -d DD`

## Overall usage

As mentioned, you can use `dotnet run -- [command]` (recommended) or `your-exe` [command]` in the root of your console project folder (if you put your output folder in the PATH to run your application). Here you find an overview of the supported commands:

```
Usage:
  dotnet run -- [command] [options]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  run          Run the code for a specific puzzle.
  init         Initialize the code for a specific puzzle. 
               Requires AOC_SESSION set as an environment variable
  sync         Sync the data (specifically the posted answers) for a puzzle. 
               Requires AOC_SESSION set as an environment variable.
  post         Post an answer for a puzzle part. 
               Requires AOC_SESSION set as an environment variable.
  export       Export the code for a puzzle to a stand-alone C# project
  report       Show a list of all puzzles, their status (unlocked,
               answered), and the answers posted.
  leaderboard  Show some stats from the configured private leaderboard.
```