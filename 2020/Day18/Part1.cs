using Sprache;

using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

using Xunit;
namespace Part1;
public static class Runner
{
    public static void Run()
    {
        var result = (
            from line in File.ReadLines("input.txt")
            select Calculator.Calculate(line)
            ).Aggregate(0L, (x, y) => x + y);

        Console.WriteLine(result);

    }
}


static class Calculator
{
    public static long Calculate(string input) => ParseExpression(input).Compile().Invoke();
    public static Expression<Func<long>> ParseExpression(string text) => Lambda.Parse(text) as Expression<Func<long>>;
    static Parser<Expression> Constant => from i in Parse.Number select Expression.Constant(long.Parse(i));
    static Parser<ExpressionType> Operator(string op, ExpressionType opType) => Parse.String(op).Token().Return(opType);
    static Parser<ExpressionType> Add => Operator("+", ExpressionType.AddChecked);
    static Parser<ExpressionType> Multiply => Operator("*", ExpressionType.MultiplyChecked);
    static Parser<Expression> ExpressionInParentheses =>
        from lparen in Parse.Char('(')
        from expr in Expr
        from rparen in Parse.Char(')')
        select expr;
    static Parser<Expression> Term => ExpressionInParentheses.XOr(Constant);
    static Parser<Expression> Expr => Parse.ChainOperator(Add.Or(Multiply), Term, Expression.MakeBinary);
    static Parser<LambdaExpression> Lambda => Expr.End().Select(body => Expression.Lambda<Func<long>>(body));
}

public class Tests
{
    [Fact]
    public void SimpleNumber()
    {
        var result = Calculator.Calculate("123");
        Assert.Equal(123, result);
    }
    [Fact]
    public void SimpleSum()
    {
        var result = Calculator.Calculate("1 + 2");
        Assert.Equal(3, result);
    }
    [Fact]
    public void SimpleProduct()
    {
        var result = Calculator.Calculate("2 * 3");
        Assert.Equal(6, result);
    }
    [Fact]
    public void SimpleProductAndSum()
    {
        var result = Calculator.Calculate("1 + 2 * 3 + 4");
        Assert.Equal(13, result);
    }
    [Fact]
    public void AddBraces()
    {
        var result = Calculator.Calculate("1 + (2 * 3) + 4");
        Assert.Equal(11, result);
    }
    [Fact]
    public void StartWithDoubleBrace()
    {
        var result = Calculator.Calculate("((1 + 5) * (2 * 3) + 4) * 2");
        Assert.Equal(80, result);
    }

    [Fact]
    public void SomeInput()
    {
        var result = Calculator.Calculate("4 * 8 * 9 + (6 * 7 * 8 * (6 * 5 * 2 + 8 + 5)) + 7");
        Assert.Equal(24823, result);
    }

}
