using System;
using System.Collections.Generic;

public enum Operator
{
    OperatorMin = 11000000,
    
    Add,          // +
    Sub,          // -
    Mul,          // *
    Div,          // /
    Mod,          // %
    Left,         // (
    Right,        // )
    Neg,          // Unary - (negation)
    Less,         // <
    Greater,      // >
    LessEqual,    // <=
    GreaterEqual, // >=
    Equal,        // ==
    NotEqual,     // !=
    Or,           // ||
    And,          // &&
    Not,          // ! (unary negation)
    
    OperatorMax
}

public static class ExpressionEvaluator
{
    public static int Evaluate(List<int> tokens)
    {
        // Step 1: Convert to postfix notation (Shunting-yard algorithm)
        var postfix = ConvertToPostfix(tokens);

        // Step 2: Evaluate the postfix expression
        return EvaluatePostfix(postfix);
    }

    private static bool IsOperator(int token) => token is > (int)Operator.OperatorMin and < (int)Operator.OperatorMax;

    private static List<int> ConvertToPostfix(List<int> tokens)
    {
        var output = new List<int>();
        var operators = new Stack<int>();

        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];

            if (!IsOperator(token)) // It's a number
            {
                output.Add(token);
            }
            else if (token == (int)Operator.Left) // (
            {
                operators.Push(token);
            }
            else if (token == (int)Operator.Right) // )
            {
                var op = operators.Pop();
                while (op != (int)Operator.Left)
                {
                    output.Add(op);
                    op = operators.Pop();
                }
            }
            else if (token == (int)Operator.Sub && (i == 0 || (IsOperator(tokens[i - 1]) && tokens[i - 1] != (int)Operator.Right))) // Unary -
            {
                operators.Push((int)Operator.Neg);
            }
            else // It's a binary operator
            {
                while (operators.Count > 0 &&
                       GetPrecedence(operators.Peek()) >= GetPrecedence(token))
                {
                    output.Add(operators.Pop());
                }
                operators.Push(token);
            }
        }

        // Pop all remaining operators
        while (operators.Count > 0)
        {
            output.Add(operators.Pop());
        }

        return output;
    }

    private static int EvaluatePostfix(List<int> postfix)
    {
        var stack = new Stack<int>();

        foreach (var token in postfix)
        {
            if (!IsOperator(token)) // It's a number
            {
                stack.Push(token);
            }
            else if (token is (int)Operator.Neg or (int)Operator.Not) // Unary - or !
            {
                var a = stack.Pop();
                stack.Push(token == (int)Operator.Neg ? -a : a == 0 ? 1 : 0);
            }
            else // It's a binary operator
            {
                var b = stack.Pop();
                var a = stack.Pop();
                stack.Push(ApplyOperator(a, b, token));
            }
        }

        return stack.Pop();
    }

    private static int ApplyOperator(int a, int b, int op)
    {
        return op switch
        {
            (int)Operator.Add => a + b,
            (int)Operator.Sub => a - b,
            (int)Operator.Mul => a * b,
            (int)Operator.Div => a / b,
            (int)Operator.Mod => a % b,
            (int)Operator.Less => a < b ? 1 : 0,
            (int)Operator.Greater => a > b ? 1 : 0,
            (int)Operator.LessEqual => a <= b ? 1 : 0,
            (int)Operator.GreaterEqual => a >= b ? 1 : 0,
            (int)Operator.Equal => a == b ? 1 : 0,
            (int)Operator.NotEqual => a != b ? 1 : 0,
            (int)Operator.And => a != 0 && b != 0 ? 1 : 0,
            (int)Operator.Or => a != 0 || b != 0 ? 1 : 0,
            _ => throw new InvalidOperationException("Invalid operator")
        };
    }

    private static int GetPrecedence(int op)
    {
        return op switch
        {
            (int)Operator.Or => 0,        // ||
            (int)Operator.And => 1,       // &&
            (int)Operator.Less => 2,      // <
            (int)Operator.Greater => 2,   // >
            (int)Operator.LessEqual => 2, // <=
            (int)Operator.GreaterEqual => 2, // >=
            (int)Operator.Equal => 2,     // ==
            (int)Operator.NotEqual => 2,  // !=
            (int)Operator.Add => 3,       // +
            (int)Operator.Sub => 3,       // -
            (int)Operator.Mul => 4,       // *
            (int)Operator.Div => 4,       // /
            (int)Operator.Mod => 4,       // %
            (int)Operator.Neg => 5,       // Unary -
            (int)Operator.Not => 5,       // Unary !
            _ => 0                        // '(' has lowest precedence
        };
    }
}
