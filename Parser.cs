using System;
using System.Collections.Generic;
namespace Parser
{
    public static class Parser
    {
        public Parser()
        {
        }
        static string[] functions = { "sin", "cos", "tan", "ctg", "log", "lg", "sqrt" };
        static int FunctionIndexes(string exp, ref string function, ref string arg, ref int first, ref int last)
        {
            int x = -1;
            for (int i = 0; i < functions.Length; i++)
            {
                x = exp.IndexOf(functions[i]);
                if (x != -1)
                {
                    function = functions[i];
                    break;
                }
            }
            if (x != -1)
            {
                int x1 = x + function.Length;
                int x2;
                for (x2 = x1 + 1; x2 < exp.Length; x2++)
                {
                    if (exp[x2] == 42 || exp[x2] == 43 || exp[x2] == 47 || exp[x2] == 45 || exp[x2] == 28 || exp[x2] == 29)
                        break;
                }
                arg = exp.Substring(x1, x2 - x1);
                first = x;
                last = x2 - 1;
            }

            return x;
        }
        static int Indexes(string exp, ref int first, ref int last, char[] delims)
        {

            int index = exp.IndexOfAny(delims, 0);
            if (index != -1)
            {
                for (int i = index - 1; i >= 0; i--)
                {
                    if (exp[i] == 42 || exp[i] == 43 || exp[i] == 47 || exp[i] == 45 || exp[i] == 28 || exp[i] == 29)
                    {
                        first = i + 1;
                        break;
                    }
                    if (i == 0)
                    {
                        first = i;

                    }
                }
                for (int i = index + 1; i < exp.Length; i++)
                {
                    if (exp[i] == 42 || exp[i] == 43 || exp[i] == 47 || exp[i] == 45 || exp[i] == 28 || exp[i] == 29)
                    {
                        last = i - 1;
                        break;
                    }
                    if (i == exp.Length - 1)
                    {
                        last = i;

                    }
                }
            }

            return index;
        }
        public static string Parse(string exp)
        {
            bool brackets = false;
            bool functions = false;
            string function = "", arg = "";
            double d;
            exp = exp.Replace("-N", "+");
            exp = exp.Replace("N", "-");
            exp = exp.Replace("--", "+");
            if (Double.TryParse(exp, out d)) return d.ToString();

            exp = exp.Replace("(-", "(N");
            exp = exp.Replace("*-", "*N");
            exp = exp.Replace("/-", "/N");
            exp = exp.Replace("+-", "+N");
            exp = exp.Replace("^-", "^N");
            if (exp[0] == '-')
            {
                exp = exp = exp.Remove(0, 1);
                exp = exp.Insert(0, "N");
            }



            int first = -1;
            for (int i = 0; i < exp.Length; i++)
                if (exp[i] == '(')
                {
                    first = i;

                }
            int last = -1;
            string sub;
            if (first != -1)
            {
                brackets = true;
                for (int i = first; i < exp.Length; i++)
                    if (exp[i] == ')')
                    {

                        last = i;
                        break;

                    }

            }
            else
            {
                if (FunctionIndexes(exp, ref function, ref arg, ref first, ref last) == -1)
                {
                    if (Indexes(exp, ref first, ref last, new char[] { '^' }) == -1)
                        if (Indexes(exp, ref first, ref last, new char[] { '*', '/' }) == -1)
                        {
                            if (Indexes(exp, ref first, ref last, new char[] { '+', '-' }) == -1)
                            {

                                exp = exp.Replace('N', '-');
                                exp = exp.Replace('(', ')');
                                exp = exp.Replace(")", "");
                                return exp.ToString();
                            }
                        }
                }
                else functions = true;
            }
            string result;


            sub = exp.Substring(first, (last - first) + 1);
            if (brackets)
            {

                sub = sub.Replace('(', ')');
                sub = sub.Replace(")", "");
                result = Parse(sub);

            }
            else if (functions)
                result = CalcFunction(function, arg).ToString();
            else
                result = eval(sub).ToString();




            exp = exp.Remove(first, (last - first) + 1);
            exp = exp.Insert(first, result);


            return Parse(exp);

        }

        static double eval(string exp)
        {
            char sign = ' ';
            List<char> signs = new List<char>();
            bool contains = false;

            int x = -1;
            do
            {
                x = exp.IndexOfAny(new char[] { '(', ')' });
                if (x != -1)
                    exp = exp.Remove(x, 1);
            }
            while (x != -1);
            for (int i = 0; i < exp.Length; i++)
                if (exp[i] == 42 || exp[i] == 43 || exp[i] == 47 || exp[i] == 45 || exp[i] == 94)
                {
                    sign = exp[i];
                    signs.Add(sign);
                    contains = true;
                }
            // 
            if (!contains) return Convert.ToDouble(exp);
            string[] arr = exp.Split('+', '-', '*', '/', '^');
            exp = exp.Replace('N', '-');
            if (arr.Length == 1) return Convert.ToDouble(exp);

            /*for (int i = 0; i < arr.Length; i++)
            {
                int x = arr[i].IndexOfAny(new char[] { '(', ')' });
                if (x != -1)
                    arr[i] = arr[i].Remove(x, 1);
                if (arr[i][0] == 'N')
                {
                    arr[i] = arr[i].Replace('N', '-');
                }
            }*/

            for (int i = 0; i < arr.Length - 1; i++)
            {
                sign = signs[i];
                arr[i + 1] = calc(arr[i], arr[i + 1], sign).ToString();
            }
            return Convert.ToDouble(arr.Last());
        }
        static double CalcFunction(string function, string arg)
        {
            arg = arg.Replace('N', '-');
            double a = Convert.ToDouble(arg);

            switch (function)
            {
                case "lg": return Math.Log10(a);
                case "log": return Math.Log(a);
                case "sin": return Math.Sin(a);
                case "cos": return Math.Cos(a);
                case "tan": return Math.Tan(a);
                case "ctg": return Math.Cos(a) / Math.Sin(a);
                case "sqrt": return Math.Sqrt(a);

            }
            return -1;
        }
        static double calc(string s1, string s2, char sign)
        {

            s1 = s1.Replace('N', '-');
            s2 = s2.Replace('N', '-');
            double a = Convert.ToDouble(s1);
            double b = Convert.ToDouble(s2);
            switch (sign)
            {
                case '+': return a + b;
                case '-': return a - b;
                case '*': return a * b;
                case '/': return a / b;
                case '^': return Math.Pow(a, b);
            }
            return -1;
        }
    }
}