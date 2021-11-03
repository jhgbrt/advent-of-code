using System;
var (key1, key2) = (2084668L, 3704642L);
long prime = 20201227, value = 1, result = 1;
while (value != key2)
{
    (value, result) = (value * 7 % prime, result * key1 % prime);
}
Console.WriteLine(result);
