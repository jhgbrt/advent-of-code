using System.ComponentModel;

namespace AdventOfCode.Client.Commands;

interface ICommand<TOptions> where TOptions : IOptions
{
    Task Run(TOptions options);
}
interface IOptions
{
}


