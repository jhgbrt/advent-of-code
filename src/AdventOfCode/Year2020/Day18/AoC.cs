using Sprache;

using System.Linq.Expressions;

namespace AdventOfCode.Year2020.Day18;

public class AoC202018
{
    static string[] input = Read.InputLines();
    public object Part1() => (
            from line in input
            select Part1Calculator.Calculate(line)
            ).Aggregate(0L, (x, y) => x + y);
    public object Part2() => (
            from line in input
            select Part2Calculator.Calculate(line)
            ).Aggregate(0L, (x, y) => x + y);
}


static class Part1Calculator
{
    public static long Calculate(string input) => ParseExpression(input).Compile().Invoke();
    public static Expression<Func<long>> ParseExpression(string text) => (Expression<Func<long>>)Lambda.Parse(text);
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

static class Part2Calculator
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


