using System;
using System.Linq.Expressions;

namespace Common
{
    public static class FormulaBuilder
    {
        public static Func<float, float> Build(string formula)
        {
            if (string.IsNullOrWhiteSpace(formula))
            {
                return (x) => x;
            }
            
            var parameter = Expression.Parameter(typeof(float), "x");

            var body = ParseExpression(formula, parameter);

            return Expression.Lambda<Func<float, float>>(body, parameter).Compile();
        }

        private static Expression ParseExpression(string formula, ParameterExpression param)
        {
            // Remove spaces
            formula = formula.Replace(" ", "");

            return ParseAddSubtract(formula, param);
        }

        private static Expression ParseAddSubtract(string expr, ParameterExpression param)
        {
            int index = FindOperator(expr, '+', '-');

            if (index != -1)
            {
                var left = ParseAddSubtract(expr.Substring(0, index), param);
                var right = ParseMultiplyDivide(expr.Substring(index + 1), param);

                return expr[index] == '+'
                    ? Expression.Add(left, right)
                    : Expression.Subtract(left, right);
            }

            return ParseMultiplyDivide(expr, param);
        }

        private static Expression ParseMultiplyDivide(string expr, ParameterExpression param)
        {
            int index = FindOperator(expr, '*', '/');

            if (index != -1)
            {
                var left = ParseMultiplyDivide(expr.Substring(0, index), param);
                var right = ParseValue(expr.Substring(index + 1), param);

                return expr[index] == '*'
                    ? Expression.Multiply(left, right)
                    : Expression.Divide(left, right);
            }

            return ParseValue(expr, param);
        }

        private static Expression ParseValue(string expr, ParameterExpression param)
        {
            if (expr == "x")
                return param;

            if (float.TryParse(expr, out float value))
                return Expression.Constant(value);

            throw new Exception($"Invalid token: {expr}");
        }

        private static int FindOperator(string expr, params char[] ops)
        {
            for (int i = expr.Length - 1; i >= 0; i--)
            {
                foreach (var op in ops)
                {
                    if (expr[i] == op)
                        return i;
                }
            }
            return -1;
        }
    }
}