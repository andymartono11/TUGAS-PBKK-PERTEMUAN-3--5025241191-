using System;
using System.Collections.Generic;
using System.Globalization;

namespace GlassCalculator
{
    public class CalculatorEngine
    {
        public List<string> History { get; } = new List<string>();

        public double Evaluate(string expression, bool degreeMode, double ans)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return 0;

            string original = expression.Trim();

            expression = expression
                .Replace("×", "*")
                .Replace("÷", "/")
                .Replace("−", "-")
                .Replace("π", "pi")
                .Replace("√", "sqrt")
                .Replace("%", "/100");

            Parser parser = new Parser(expression.Trim(), degreeMode, ans);
            double result = parser.ParseExpression();

            if (double.IsNaN(result) || double.IsInfinity(result))
                throw new Exception("Hasil tidak valid.");

            string formatted = FormatNumber(result);
            History.Add($"{original} = {formatted}");
            if (History.Count > 50)
                History.RemoveAt(0);

            return result;
        }

        public static string FormatNumber(double value)
        {
            if (Math.Abs(value) < 1e-12)
                value = 0;
            if (value != 0 && (Math.Abs(value) >= 1e15 || Math.Abs(value) < 1e-6))
                return value.ToString("E6", CultureInfo.InvariantCulture);
            return value.ToString("G14", CultureInfo.InvariantCulture);
        }

        // ─── Nested Parser ────────────────────────────────────────────────────

        private class Parser
        {
            private readonly string text;
            private readonly bool degreeMode;
            private readonly double ans;
            private int pos;

            public Parser(string text, bool degreeMode, double ans)
            {
                this.text       = text;
                this.degreeMode = degreeMode;
                this.ans        = ans;
                this.pos        = 0;
            }

            private void SkipSpaces()
            {
                while (pos < text.Length && char.IsWhiteSpace(text[pos]))
                    pos++;
            }

            private bool Match(char c)
            {
                SkipSpaces();
                if (pos < text.Length && text[pos] == c)
                {
                    pos++;
                    return true;
                }
                return false;
            }

            // Top-level: also verifies entire string consumed
            public double ParseExpression()
            {
                double value = ParseExpressionInner();
                SkipSpaces();
                if (pos < text.Length)
                    throw new Exception($"Karakter tidak dikenal: '{text[pos]}'");
                return value;
            }

            // Inner: used inside parentheses — does NOT check remaining string
            private double ParseExpressionInner()
            {
                double value = ParseTerm();
                while (true)
                {
                    SkipSpaces();
                    if (Match('+'))
                        value += ParseTerm();
                    else if (Match('-'))
                        value -= ParseTerm();
                    else
                        break;
                }
                return value;
            }

            private double ParseTerm()
            {
                double value = ParsePower();
                while (true)
                {
                    SkipSpaces();
                    if (Match('*'))
                    {
                        value *= ParsePower();
                    }
                    else if (Match('/'))
                    {
                        double d = ParsePower();
                        if (Math.Abs(d) < 1e-15)
                            throw new DivideByZeroException();
                        value /= d;
                    }
                    else
                        break;
                }
                return value;
            }

            private double ParsePower()
            {
                double value = ParseUnary();
                SkipSpaces();
                if (Match('^'))
                {
                    double exp = ParsePower(); // right-associative
                    value = Math.Pow(value, exp);
                }
                // implicit multiplication: 2pi, 3sin(30), (2+3)(4+1)
                SkipSpaces();
                if (pos < text.Length)
                {
                    char next = text[pos];
                    if (char.IsLetter(next) || next == '(')
                        value *= ParseUnary();
                }
                return value;
            }

            private double ParseUnary()
            {
                SkipSpaces();
                if (Match('+')) return ParseUnary();
                if (Match('-')) return -ParseUnary();
                return ParsePostfix();
            }

            private double ParsePostfix()
            {
                double value = ParsePrimary();
                while (true)
                {
                    SkipSpaces();
                    if (Match('!'))
                        value = Factorial(value);
                    else
                        break;
                }
                return value;
            }

            private double ParsePrimary()
            {
                SkipSpaces();
                if (pos >= text.Length)
                    throw new Exception("Ekspresi tidak lengkap.");

                // Parentheses
                if (Match('('))
                {
                    double val = ParseExpressionInner();
                    if (!Match(')'))
                        throw new Exception("Kurung tidak ditutup.");
                    return val;
                }

                // Number
                if (char.IsDigit(text[pos]) || text[pos] == '.')
                    return ParseNumber();

                // Identifier: constant or function
                if (char.IsLetter(text[pos]))
                {
                    string name = ParseName();
                    if (name == "pi")  return Math.PI;
                    if (name == "e")   return Math.E;
                    if (name == "ans") return ans;
                    return ParseFunction(name);
                }

                throw new Exception($"Token tidak dikenal: '{text[pos]}'");
            }

            private double ParseNumber()
            {
                int start = pos;
                while (pos < text.Length && (char.IsDigit(text[pos]) || text[pos] == '.'))
                    pos++;
                // scientific notation: 1e10 / 1E-3
                if (pos < text.Length && (text[pos] == 'e' || text[pos] == 'E'))
                {
                    pos++;
                    if (pos < text.Length && (text[pos] == '+' || text[pos] == '-'))
                        pos++;
                    while (pos < text.Length && char.IsDigit(text[pos]))
                        pos++;
                }
                string num = text.Substring(start, pos - start);
                if (!double.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                    throw new Exception($"Angka tidak valid: '{num}'");
                return value;
            }

            private string ParseName()
            {
                int start = pos;
                while (pos < text.Length && char.IsLetterOrDigit(text[pos]))
                    pos++;
                return text.Substring(start, pos - start).ToLowerInvariant();
            }

            private double ParseFunction(string name)
            {
                // Two-argument functions
                if (name == "max" || name == "min" || name == "log")
                {
                    SkipSpaces();
                    // log with base: log(x) = log10, log(x,b) = logb(x)
                    if (name == "log")
                    {
                        if (Match('('))
                        {
                            double a = ParseExpressionInner();
                            if (Match(','))
                            {
                                double b = ParseExpressionInner();
                                if (!Match(')')) throw new Exception("Kurung tidak ditutup.");
                                if (a <= 0 || b <= 0 || Math.Abs(b - 1) < 1e-15)
                                    throw new Exception("Argumen log tidak valid.");
                                return Math.Log(a) / Math.Log(b);
                            }
                            if (!Match(')')) throw new Exception("Kurung tidak ditutup.");
                            if (a <= 0) throw new Exception("log membutuhkan argumen > 0.");
                            return Math.Log10(a);
                        }
                        double arg = ParseUnary();
                        if (arg <= 0) throw new Exception("log membutuhkan argumen > 0.");
                        return Math.Log10(arg);
                    }
                    // max / min
                    if (!Match('(')) throw new Exception($"{name} butuh kurung.");
                    double p1 = ParseExpressionInner();
                    if (!Match(',')) throw new Exception($"{name} butuh dua argumen.");
                    double p2 = ParseExpressionInner();
                    if (!Match(')')) throw new Exception("Kurung tidak ditutup.");
                    return name == "max" ? Math.Max(p1, p2) : Math.Min(p1, p2);
                }

                // Single-argument functions
                double argument;
                SkipSpaces();
                if (Match('('))
                {
                    argument = ParseExpressionInner();
                    if (!Match(')'))
                        throw new Exception("Kurung tidak ditutup.");
                }
                else
                {
                    argument = ParseUnary();
                }

                switch (name)
                {
                    case "sqrt":
                        if (argument < 0) throw new Exception("sqrt tidak bisa negatif.");
                        return Math.Sqrt(argument);
                    case "cbrt":
                        return Math.Cbrt(argument);
                    case "ln":
                        if (argument <= 0) throw new Exception("ln membutuhkan argumen > 0.");
                        return Math.Log(argument);
                    case "log2":
                        if (argument <= 0) throw new Exception("log2 membutuhkan argumen > 0.");
                        return Math.Log2(argument);
                    case "sin":
                        return Math.Sin(ToRadians(argument));
                    case "cos":
                        return Math.Cos(ToRadians(argument));
                    case "tan":
                        return Math.Tan(ToRadians(argument));
                    case "asin":
                        if (argument < -1 || argument > 1) throw new Exception("asin: argumen harus -1 sampai 1.");
                        return FromRadians(Math.Asin(argument));
                    case "acos":
                        if (argument < -1 || argument > 1) throw new Exception("acos: argumen harus -1 sampai 1.");
                        return FromRadians(Math.Acos(argument));
                    case "atan":
                        return FromRadians(Math.Atan(argument));
                    case "sinh":
                        return Math.Sinh(argument);
                    case "cosh":
                        return Math.Cosh(argument);
                    case "tanh":
                        return Math.Tanh(argument);
                    case "abs":
                        return Math.Abs(argument);
                    case "exp":
                        return Math.Exp(argument);
                    case "inv":
                        if (Math.Abs(argument) < 1e-15) throw new DivideByZeroException();
                        return 1.0 / argument;
                    case "sq":
                        return argument * argument;
                    case "cube":
                        return argument * argument * argument;
                    case "ten":
                        return Math.Pow(10, argument);
                    case "floor":
                        return Math.Floor(argument);
                    case "ceil":
                        return Math.Ceiling(argument);
                    case "round":
                        return Math.Round(argument);
                    case "rand":
                        return new Random().NextDouble();
                    default:
                        throw new Exception($"Fungsi tidak dikenal: '{name}'");
                }
            }

            private double ToRadians(double v) => degreeMode ? v * Math.PI / 180.0 : v;
            private double FromRadians(double v) => degreeMode ? v * 180.0 / Math.PI : v;

            private double Factorial(double value)
            {
                if (value < 0 || Math.Abs(value - Math.Round(value)) > 1e-10)
                    throw new Exception("n! membutuhkan bilangan bulat >= 0.");
                if (value > 170)
                    throw new Exception("Angka terlalu besar untuk faktorial.");
                double result = 1;
                for (int i = 2; i <= (int)value; i++)
                    result *= i;
                return result;
            }
        }
    }
}
