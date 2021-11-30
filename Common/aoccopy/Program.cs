
//var content = File.ReadAllText(Path.Combine("Common", "AdventOfCode.Template.CSharp", "AdventOfCode.Template.CSharp.csproj"));
//var answers = Path.Combine("Common", "AdventOfCode.Template.CSharp", "Answers.cs");
//var aoc = Path.Combine("Common", "AdventOfCode.Template.CSharp", "AoC.cs");

for (var year = 2015; year <= DateTime.Now.Year - 1; year++)
{
    for (var day = 1; day <= 25; day++)
    {
        var source = new DirectoryInfo(Path.Combine(year.ToString(), $"Day{day:00}"));
        var target = new DirectoryInfo(Path.Combine("src", $"Year{year}", $"Day{day:00}"));

        if (!target.Exists) target.Create();

        //foreach (var file in source.GetFiles("*.cs"))
        //{
        //    if (file.Name == "Program.cs") continue;
        //    file.CopyTo(Path.Combine(target.FullName, file.Name));
        //}

        foreach (var file in source.GetFiles("*.json"))
        {
            file.CopyTo(Path.Combine(target.FullName, file.Name));
        }

        //if (Directory.Exists(dir))
        //{
        //var csproj = Path.Combine(year.ToString(), $"Day{day:00}", $"AoC.{year}.Day{day:00}.csproj");
        //File.WriteAllText(csproj, content);
        //File.Copy(aoc, Path.Combine(dir, "AoC.cs"), true);
        //}
        //      if (File.Exists(answers)) File.Delete(answers);
    }

}
//var target = new DirectoryInfo(@$"c:\users\jeroe\source\repos\ae-nv\aedvent-code-{year}");

//foreach (var day in Enumerable.Range(1, 25))
//{
//    Console.WriteLine(day);
//    var sourcedir = source.GetDirectories($"day {day:00}").SingleOrDefault()?.GetDirectories("jeroen*").FirstOrDefault();
//    if (sourcedir == null) 
//    {
//        Console.WriteLine($"NO SOURCE FOUND for {year}/{day}");
//        continue;
//    }
//    var targetdir = target.GetDirectories($"Day{day:00}").SingleOrDefault();
//    if (targetdir == null)
//    {
//        Console.WriteLine($"TARGET NOT FOUND: {target}");
//        continue;
//    }

//    foreach (var f in sourcedir.GetFiles($"*.csproj", new EnumerationOptions{RecurseSubdirectories = true}))
//    {
//        var destination = Path.Combine(targetdir.FullName, $"Day{day:00}.csproj");
//        Console.WriteLine($"{destination}");
//        f.CopyTo(destination, true);
//    }
//    foreach (var f in sourcedir.GetFiles($"*.cs", new EnumerationOptions{RecurseSubdirectories = true}))
//    {
//        var destination = Path.Combine(targetdir.FullName, f.Name);
//        Console.WriteLine($"{destination}");
//        f.CopyTo(destination, true);
//    }
//    foreach (var f in sourcedir.GetFiles($"input.txt", new EnumerationOptions{RecurseSubdirectories = true}))
//    {
//        var destination = Path.Combine(targetdir.FullName, f.Name);
//        Console.WriteLine($"{destination}");
//        f.CopyTo(destination, true);
//    }

//}