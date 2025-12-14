using System.Numerics;

namespace AdventOfCode.Year2019.Day22;

public class AoC201922(string[] input)
{
    readonly Shuffle[] shuffles = ParseInput(input);

    public AoC201922() : this(Read.InputLines()) { }

    public long Part1() => ComposeTransforms(shuffles, 10007).Apply(2019);

    public long Part2() => ComposeTransforms(shuffles, 119315717514047L).Power(101741582076661L).Inverse().Apply(2020L);

    internal static Shuffle[] ParseInput(string[] lines)
    {
        var shuffles = new Shuffle[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            ReadOnlySpan<char> line = lines[i];
            var shuffle = line switch
            {
                ['d', 'e', 'a', 'l', ' ', 'i', ..] => new Shuffle(ShuffleType.DealIntoNewStack, 0),
                ['c', 'u', 't', ' ', .. var nStr] => new Shuffle(ShuffleType.Cut, int.Parse(nStr)),
                ['d', 'e', 'a', 'l', ' ', 'w', 'i', 't', 'h', ' ', 'i', 'n', 'c', 'r', 'e', 'm', 'e', 'n', 't', ' ', .. var mStr] 
                    => new Shuffle(ShuffleType.DealWithIncrement, int.Parse(mStr)),   
            };
            shuffles[i] = shuffle;
        }
        return [.. shuffles];
    }

    internal static LinearMod ComposeTransforms(Shuffle[] shuffles, long deckSize)
    {
        var transform = new LinearMod(1, 0, deckSize); // Identity: f(x) = x
        
        foreach (var shuffle in shuffles)
        {
            var next = shuffle.Type switch
            {
                ShuffleType.DealIntoNewStack => new LinearMod(-1, deckSize - 1, deckSize),
                ShuffleType.Cut => new LinearMod(1, -shuffle.N, deckSize),
                ShuffleType.DealWithIncrement => new LinearMod(shuffle.N, 0, deckSize),
                _ => throw new ArgumentException($"Unknown shuffle type: {shuffle.Type}")
            };
            transform = next.Compose(transform);
        }
        
        return transform;
    }

    internal static int[] SimulateShuffle(Shuffle[] shuffles, int deckSize)
    {
        var deck = Enumerable.Range(0, deckSize).ToArray();
        
        foreach (var shuffle in shuffles)
        {
            deck = shuffle.Type switch
            {
                ShuffleType.DealIntoNewStack => deck.Reverse().ToArray(),
                ShuffleType.Cut => Cut(deck, shuffle.N),
                ShuffleType.DealWithIncrement => DealWithIncrement(deck, shuffle.N),
                _ => throw new ArgumentException($"Unknown shuffle type: {shuffle.Type}")
            };
        }
        
        return deck;
    }

    static int[] Cut(int[] deck, int n)
    {
        var cutPosition = n >= 0 ? n : deck.Length + n;
        return deck[cutPosition..].Concat(deck[..cutPosition]).ToArray();
    }

    static int[] DealWithIncrement(int[] deck, int increment)
    {
        var result = new int[deck.Length];
        var pos = 0;
        for (int i = 0; i < deck.Length; i++)
        {
            result[pos] = deck[i];
            pos = (pos + increment) % deck.Length;
        }
        return result;
    }
}

enum ShuffleType { DealIntoNewStack, Cut, DealWithIncrement }

record Shuffle(ShuffleType Type, int N);

record struct LinearMod(long A, long B, long M)
{
    public LinearMod Compose(LinearMod other)
    {
        // this(other(x)) = this(other.A*x + other.B) = A*(other.A*x + other.B) + B
        // = (A*other.A)*x + (A*other.B + B)
        var newA = (long)((BigInteger)A * other.A % M);
        var newB = (long)(((BigInteger)A * other.B + B) % M);
        newA = newA < 0 ? newA + M : newA;
        newB = newB < 0 ? newB + M : newB;
        return new LinearMod(newA, newB, M);
    }

    public long Apply(long x) => (long)(((BigInteger)A * x + B) % M + M) % M;

    // Apply this transformation n times using fast exponentiation
    // f^n(x) can be computed in O(log n) time
    public LinearMod Power(long n)
    {
        if (n == 0) return new LinearMod(1, 0, M); // Identity
        if (n == 1) return this;
        
        var half = Power(n / 2);
        var squared = half.Compose(half);
        
        return (n % 2 == 0) ? squared : squared.Compose(this);
    }

    // Find the inverse transformation
    // If f(x) = Ax + B, then f^-1(y) = A^-1 * (y - B) = A^-1 * y - A^-1 * B
    public LinearMod Inverse()
    {
        var aInv = ModInverse(A, M);
        var newB = (long)((-(BigInteger)aInv * B) % M + M) % M;
        return new LinearMod(aInv, newB, M);
    }

    // Returns x such that (a * x) % m == 1
    static long ModInverse(long a, long m)
    {
        long m0 = m, x0 = 0, x1 = 1;
        
        if (m == 1) return 0;
        
        while (a > 1)
        {
            long q = a / m;
            (m, a) = (a % m, m);
            (x0, x1) = (x1 - q * x0, x0);
        }
        
        return x1 < 0 ? x1 + m0 : x1;
    }
}

public class AoC201922Tests
{
    [Fact]
    public void Example1_DealWithIncrement7_ThenDealIntoNewStack_Twice()
    {
        var input = new[]
        {
            "deal with increment 7",
            "deal into new stack",
            "deal into new stack"
        };
        var shuffles = AoC201922.ParseInput(input);
        var result = AoC201922.SimulateShuffle(shuffles, 10);
        
        Assert.Equal([0, 3, 6, 9, 2, 5, 8, 1, 4, 7], result);
    }

    [Fact]
    public void Example2_Cut6_DealWithIncrement7_DealIntoNewStack()
    {
        var input = new[]
        {
            "cut 6",
            "deal with increment 7",
            "deal into new stack"
        };
        var shuffles = AoC201922.ParseInput(input);
        var result = AoC201922.SimulateShuffle(shuffles, 10);
        
        Assert.Equal([3, 0, 7, 4, 1, 8, 5, 2, 9, 6], result);
    }

    [Fact]
    public void Example3_DealWithIncrement7And9_CutMinus2()
    {
        var input = new[]
        {
            "deal with increment 7",
            "deal with increment 9",
            "cut -2"
        };
        var shuffles = AoC201922.ParseInput(input);
        var result = AoC201922.SimulateShuffle(shuffles, 10);
        
        Assert.Equal([6, 3, 0, 7, 4, 1, 8, 5, 2, 9], result);
    }

    [Fact]
    public void Example4_ComplexSequence()
    {
        var input = new[]
        {
            "deal into new stack",
            "cut -2",
            "deal with increment 7",
            "cut 8",
            "cut -4",
            "deal with increment 7",
            "cut 3",
            "deal with increment 9",
            "deal with increment 3",
            "cut -1"
        };
        var shuffles = AoC201922.ParseInput(input);
        var result = AoC201922.SimulateShuffle(shuffles, 10);
        
        Assert.Equal([9, 2, 5, 8, 1, 4, 7, 0, 3, 6], result);
    }

    [Fact]
    public void LinearTransform_MatchesSimulation_Example1()
    {
        var input = new[]
        {
            "deal with increment 7",
            "deal into new stack",
            "deal into new stack"
        };
        var shuffles = AoC201922.ParseInput(input);
        var simulated = AoC201922.SimulateShuffle(shuffles, 10);
        var transform = AoC201922.ComposeTransforms(shuffles, 10);
        
        for (int i = 0; i < 10; i++)
        {
            var transformedPos = transform.Apply(i);
            Assert.Equal(simulated[transformedPos], i);
        }
    }

    [Fact]
    public void LinearTransform_MatchesSimulation_Example4()
    {
        var input = new[]
        {
            "deal into new stack",
            "cut -2",
            "deal with increment 7",
            "cut 8",
            "cut -4",
            "deal with increment 7",
            "cut 3",
            "deal with increment 9",
            "deal with increment 3",
            "cut -1"
        };
        var shuffles = AoC201922.ParseInput(input);
        var simulated = AoC201922.SimulateShuffle(shuffles, 10);
        var transform = AoC201922.ComposeTransforms(shuffles, 10);
        
        // Expected: [9, 2, 5, 8, 1, 4, 7, 0, 3, 6]
        // Card 0 is at position 7, Card 1 is at position 4, etc.
        Assert.Equal([9, 2, 5, 8, 1, 4, 7, 0, 3, 6], simulated);
        
        for (int i = 0; i < 10; i++)
        {
            var transformedPos = transform.Apply(i);
            Assert.Equal(simulated[transformedPos], i);
        }
    }

    [Fact]
    public void LinearMod_Inverse_WorksCorrectly()
    {
        var transform = new LinearMod(7, 3, 10);
        var inverse = transform.Inverse();
        
        // f(f^-1(x)) should equal x for all x
        for (int i = 0; i < 10; i++)
        {
            var result = transform.Apply(inverse.Apply(i));
            Assert.Equal(i, result);
        }
    }

    [Fact]
    public void LinearMod_Power_WorksCorrectly()
    {
        var transform = new LinearMod(3, 5, 11);
        
        // f^3(x) should equal f(f(f(x)))
        var power3 = transform.Power(3);
        
        for (int i = 0; i < 11; i++)
        {
            var manual = transform.Apply(transform.Apply(transform.Apply(i)));
            var calculated = power3.Apply(i);
            Assert.Equal(manual, calculated);
        }
    }

    [Fact]
    public void LinearMod_Power_LargeExponent()
    {
        var transform = new LinearMod(2, 1, 97);
        var power1000 = transform.Power(1000);
        
        // Just verify it doesn't crash and produces a result
        var result = power1000.Apply(42);
        Assert.True(result >= 0 && result < 97);
    }

    [Fact]
    public void Part2_Logic_SmallExample()
    {
        // Test the Part 2 logic on a small example
        var input = new[]
        {
            "deal with increment 7",
            "deal into new stack",
            "deal into new stack"
        };
        const int deckSize = 10;
        const int shuffleCount = 3;
        const int position = 5;
        
        var shuffles = AoC201922.ParseInput(input);
        var transform = AoC201922.ComposeTransforms(shuffles, deckSize);
        
        // Apply shuffle 3 times
        var repeated = transform.Power(shuffleCount);
        
        // Find which card ends up at position 5
        var inverse = repeated.Inverse();
        var card = inverse.Apply(position);
        
        // Verify by forward simulation
        var verifyPos = repeated.Apply(card);
        Assert.Equal(position, verifyPos);
    }

    [Fact]
    public void VerifyTransformSemantics()
    {
        // Start with deck [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
        // "deal into new stack" gives [9, 8, 7, 6, 5, 4, 3, 2, 1, 0]
        // Card 0 is at position 9, card 9 is at position 0
        var input = new[] { "deal into new stack" };
        var shuffles = AoC201922.ParseInput(input);
        var simulated = AoC201922.SimulateShuffle(shuffles, 10);
        var transform = AoC201922.ComposeTransforms(shuffles, 10);
        
        // simulated[pos] = card at that position
        Assert.Equal(9, simulated[0]); // Position 0 has card 9
        Assert.Equal(0, simulated[9]); // Position 9 has card 0
        
        // transform.Apply(card) = position where card ends up
        Assert.Equal(9, transform.Apply(0)); // Card 0 ends up at position 9
        Assert.Equal(0, transform.Apply(9)); // Card 9 ends up at position 0
        
        // To find which card is at position X, we use inverse
        var inverse = transform.Inverse();
        Assert.Equal(9, inverse.Apply(0)); // Position 0 has card 9
        Assert.Equal(0, inverse.Apply(9)); // Position 9 has card 0
    }

    [Fact]
    public void VerifyPowerAndInverseCommute()
    {
        // Verify that (f^n)^-1 == (f^-1)^n
        var input = new[]
        {
            "deal with increment 7",
            "cut 3"
        };
        var shuffles = AoC201922.ParseInput(input);
        var transform = AoC201922.ComposeTransforms(shuffles, 97); // Prime number for better testing
        
        const int n = 100;
        
        // Method 1: Power then inverse
        var method1 = transform.Power(n).Inverse();
        
        // Method 2: Inverse then power
        var method2 = transform.Inverse().Power(n);
        
        // They should be equal
        Assert.Equal(method1.A, method2.A);
        Assert.Equal(method1.B, method2.B);
        
        // Verify they give same results
        for (int i = 0; i < 10; i++)
        {
            Assert.Equal(method1.Apply(i), method2.Apply(i));
        }
    }

  
}