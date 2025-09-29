using System;

namespace Calculator
{
    public class Expression
    {
        public virtual int Evaluate() => 0;
    }

    class BinaryExpression(char op, Expression lhs, Expression rhs) : Expression
    {
        private readonly char _op = op;
        private readonly Expression _lhs = lhs;
        private readonly Expression _rhs = rhs;

        public override int Evaluate()
        {
            int l = _lhs.Evaluate();
            int r = _rhs.Evaluate();
            if (_op == '+') return l + r;
            if (_op == '-') return l - r;
            if (_op == '*') return l * r;
            if (_op == '/') return l / r;
            return 0;
        }

        public override string ToString() => $"({_op} {_lhs} {_rhs})";
    }

    public class IntegerExpression(int number) : Expression
    {
        public override int Evaluate() => number;
        public override string ToString() => $"{number}";
    }

    public class Parser
    {
        private readonly string _expression;
        private int _position;
        private readonly int _length;

        public Parser(string expression)
        {
            _expression = expression;
            _position = 0;
            _length = _expression.Length;
        }

        public Expression Parse()
        {
            Console.WriteLine($"Expression: {_expression}");

            return ExprAdd();
        }

        private Expression ExprAdd()
        {
            Expression left = ExprMul();
            while (!End && (CurrentChar == '+' || CurrentChar == '-'))
            {                
                char op = NextChar;
                Expression right = ExprMul();
                left = new BinaryExpression(op, left, right);
            }
            return left;
        }

        private Expression ExprMul()
        {
            Expression left = ParseIntegerExpression();
            while (!End && (CurrentChar == '*' || CurrentChar == '/'))
            {
                char op = NextChar;
                Expression right = ParseIntegerExpression();
                left = new BinaryExpression(op, left, right);
            }
            return left;
        }

        private IntegerExpression ParseIntegerExpression()
        {
            int number = 0;
            while (!End && char.IsAsciiDigit(CurrentChar))
            {
                number = number * 10 + (NextChar - '0');
            }

            return new IntegerExpression(number);
        }

        private bool End => _position >= _length;
        private char NextChar => _expression[_position++];
        private char CurrentChar => _expression[_position];
    }

    public static class Calculator
    {
        public static void Main()
        {
            Console.WriteLine("Enter expression:");
            var expression = Console.ReadLine();

            var parser = new Parser(expression);
            var node = parser.Parse();

            Console.WriteLine($"S expr: {node}");
            Console.WriteLine($"Eval: {node.Evaluate()}");
        }
    }
}