var input = File.ReadAllText("input.txt");
var result1 = input.Select(c => c switch { '(' => +1, ')' => -1 }).Sum();

Console.WriteLine(result1);
var sum = 0;

for (int i = 0; i < input.Length; i++)
{
    sum += input[i] switch { '(' => +1, ')' => -1 };
    if (sum == -1)
    {
        Console.WriteLine(i + 1);
        break;
    }
}
