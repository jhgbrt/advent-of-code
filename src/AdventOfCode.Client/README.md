# Advent of Code in C# helper

## Overview

This library provides a helper toolkit for the advent of code. You can use it to 

- automatically download puzzle input
- post puzzle answers from the command line
- export puzzle code to a standalone C# project
- verify puzzle code
- list puzzle answers and status

## Usage

Create a console application and install the nuget package. The console application will contain your Advent of Code puzzle logic.

Now you can use `dotnet run -- [command]` (recommended) or `your-exe` [command]`, if you put your output folder in the PATH to run your application.
```
Usage:
  [your-cli] [options] [command]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  run          Run the code for a specific puzzle.
  init         Initialize the code for a specific puzzle. Requires AOC_SESSION set as an environment variable
  sync         Sync the data (specifically the posted answers) for a puzzle. Requires AOC_SESSION set as an environment
               variable.
  post         Post an answer for a puzzle part. Requires AOC_SESSION set as an environment variable.
  export       Export the code for a puzzle to a stand-alone C# project
  report       Show a list of all puzzles, their status (unlocked, answered), and the answers posted.
  leaderboard  Show some stats from the configured private leaderboard. Set AOC_LEADERBOARD_ID as a environment
               variable.
```