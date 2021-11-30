using Sprache;

using System.Linq.Expressions;

namespace AdventOfCode.Year2020.Day18.Part2;
public static class Runner
{
    public static object Run()
    {
        var result = (
            from line in Read.InputLines(typeof(AoC202018))
            select Calculator.Calculate(line)
            ).Aggregate(0L, (x, y) => x + y);
        return result;

    }
}



static class Calculator
{
    public static long Calculate(string input) => ParseExpression(input).Compile().Invoke();
    static Expression<Func<long>> ParseExpression(string text) => (Expression<Func<long>>)Lambda.Parse(text);
    static Parser<Expression> Constant => from i in Parse.Number select Expression.Constant(long.Parse(i));
    static Parser<ExpressionType> Operator(string op, ExpressionType opType) => Parse.String(op).Token().Return(opType);
    static Parser<ExpressionType> Add => Operator("+", ExpressionType.AddChecked);
    static Parser<ExpressionType> Multiply => Operator("*", ExpressionType.MultiplyChecked);
    static Parser<Expression> ExpressionInParentheses =>
        from lparen in Parse.Char('(')
        from expr in Expr
        from rparen in Parse.Char(')')
        select expr;
    static Parser<Expression> InnerTerm => ExpressionInParentheses.XOr(Constant);
    static Parser<Expression> Term => Parse.ChainOperator(Add, InnerTerm, Expression.MakeBinary);
    static Parser<Expression> Expr => Parse.ChainOperator(Multiply, Term, Expression.MakeBinary);
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
        Assert.Equal(21, result);
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
        Assert.Equal(120, result);
    }

    [Fact]
    public void SomeInput()
    {
        var result = Calculator.Calculate("4 * 8 * 9 + (6 * 7 * 8 * (6 * 5 * 2 + 8 + 5)) + 7");
        Assert.Equal(4838912, result);
    }

}
