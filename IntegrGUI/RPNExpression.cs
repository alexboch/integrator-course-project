using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expressions
{
    /// <summary>
    /// Выражение в обратной польской нотации
    /// </summary>
    public class RPNExpression
    {
        private Queue<string> expr_queue = new Queue<string>();
        private string infixString = "";
        public string InfixString
        {
            get
            {
                return infixString;
            }
        }

        private static string NextToken(StringBuilder strb)
        {
            StringBuilder result = new StringBuilder("");
            char c = strb[0];
            if (c == 'x')
            {
                result.Append(c);
                strb.Remove(0, 1);
            }
            else
                if (Char.IsDigit(c))//Число
            {
                int i = 0;
                for (i = 0; i < strb.Length; i++)
                {

                    if (!Char.IsDigit(strb[i]) && strb[i] != '.')
                    {
                        strb.Remove(0, i);
                        break;
                    }
                    result.Append(strb[i]);
                    if (i == strb.Length - 1)
                        strb.Remove(0, i + 1);
                }


            }
            else
            {
                if (Char.IsLetter(c))//Функция
                {
                    for (int i = 0; i < strb.Length; i++)
                    {
                        if (!Char.IsLetterOrDigit(strb[i]))
                        {
                            strb.Remove(0, i);
                            break;
                        }
                        result.Append(strb[i]);
                    }
                }
                else
                {
                    result = new StringBuilder(c.ToString());
                    strb.Remove(0, 1);
                }
            }
            return result.ToString();
        }

        const int maxStackSize = 1000;
        /// <summary>
        /// Создает из строки выражение в обратной польской нотации, используя алгоритм сортировочной станции
        /// </summary>
        /// <param name="str">Выражение в инфиксной записи</param>
        public RPNExpression(string str)
        {
            infixString = str;
            Stack<string> s = new Stack<string>();
            string PrevToken = "";
            StringBuilder expr_b = new StringBuilder(str);
            while (expr_b.Length > 0)
            {

                string token = NextToken(expr_b);

                char c = token[0];
                if (c == 'x' || Char.IsDigit(c))
                    expr_queue.Enqueue(token);
                else
                if (c == '!')//Унарный постфиксный оператор
                    expr_queue.Enqueue(token);
                else
                    if ((PrevToken == "" || !(Char.IsDigit(PrevToken[0]) || PrevToken[0] == ')' || PrevToken == "x"))
                    && PrevToken != "!" && (token == "-" || token == "+"))//Унарный префиксный оператор
                {
                    expr_queue.Enqueue("0");

                    s.Push(token);
                }
                else
                        if (Char.IsLetter(token[0]))//Функция
                    s.Push(token);
                else
                            if (token == ",")//Разделитель аргументов функции
                    while (s.Peek() != "(")
                        expr_queue.Enqueue(s.Pop());
                else
                if (token == "+" || token == "-")//Бинарный левоассоциативный оператор
                {
                    if (s.Count != 0)
                    {
                        string sp = s.Peek();
                        char ch = sp[0];
                        while (s.Count > 0 && (ch == '/' || ch == '*' || ch == '+' || ch == '-' || ch == '^'))//Если на вершине стека
                        {
                            expr_queue.Enqueue(s.Pop());//Вытолкнуть из стека и присоединить к выходной очереди
                            if (s.Count > 0)
                            {
                                sp = s.Peek();
                                ch = sp[0];
                            }
                        }
                    }
                    s.Push(token);
                }
                else
                if (token == "/" || token == "*")
                {
                    if (s.Count != 0)
                    {
                        string sp = s.Peek();
                        char ch = sp[0];
                        while ((s.Count > 0) && (ch == '/' || ch == '*' || ch == '^'))
                        {
                            expr_queue.Enqueue(s.Pop());//Вытолкнуть из стека и присоединить к выходной очереди

                            if (s.Count > 0)
                            {
                                sp = s.Peek();
                                ch = sp[0];
                            }
                        }
                    }
                    s.Push(token);
                }
                else
                if (token == "^")//Бинарный правоассоциативный оператор
                {

                    s.Push(token);
                }
                else
                if (token == "(")
                    s.Push(token);
                else if (token == ")")
                {
                    while (s.Peek() != "(")
                        expr_queue.Enqueue(s.Pop());
                    s.Pop();
                    if (s.Count != 0)
                        if (Char.IsLetter(s.Peek()[0]))
                            expr_queue.Enqueue(s.Pop());
                }
                PrevToken = token;
                if (s.Count > maxStackSize) throw new FormatException();
            }
            while (s.Count != 0)//Пока в стеке есть операторы
                expr_queue.Enqueue(s.Pop());
        }

        public override string ToString()
        {
            StringBuilder sBuilder = new StringBuilder(expr_queue.Count);
            foreach (string str in expr_queue)
            {
                sBuilder.Append(str);
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// Вычисляет факториал
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private static double fact(double a)
        {
            if (a <= 1)
                return 1;
            double f = 1;
            for (int b = 1; b <= a; b++)
                f *= b;
            return f;
        }

        /// <summary>
        /// Вычисляет значение выражения
        /// </summary>
        /// <param name="x">значение переменной</param>
        /// <returns></returns>
        public double Evaluate(double x = 0)
        {
            Queue<string> q = new Queue<string>(expr_queue);

            string s;
            Stack<double> ts = new Stack<double>();
            while (q.Count != 0)
            {
                s = q.Dequeue();
                char c = s[0];
                if (c == 'x')
                {
                    ts.Push(x);
                }
                else
                {
                    if (Char.IsDigit(c))//Если число
                    {
                        s = s.Replace('.', ',');
                        ts.Push(Convert.ToDouble(s));
                    }
                    else
                    {
                        double a = ts.Pop();
                        if (Char.IsLetter(c))//Функция
                        {
                            switch (s)
                            {
                                case "cos":
                                    a = Math.Cos(a);
                                    break;
                                case "sin":
                                    a = Math.Sin(a);
                                    break;
                                case "tan":
                                    a = Math.Tan(a);
                                    break;
                                case "cot":
                                    a = Math.Cos(a) / Math.Sin(a);
                                    break;
                                case "ln":
                                    a = Math.Log(a);
                                    break;
                                case "lg":
                                    a = Math.Log10(a);
                                    break;
                                default:
                                    throw new FormatException();

                            }
                        }
                        else
                        {
                            double b = 0;
                            if (c != '!')
                                b = ts.Pop();
                            switch (c)
                            {
                                case '-':
                                    a = b - a;
                                    break;
                                case '+':
                                    a = b + a;
                                    break;
                                case '*':
                                    a = b * a;
                                    break;
                                case '/':
                                    a = b / a;
                                    break;
                                case '^':
                                    a = Math.Pow(b, a);
                                    break;
                                case '!':
                                    a = fact(a);
                                    break;
                            }
                        }
                        ts.Push(a);
                    }
                }
            }
            return ts.Pop();
        }
    }

}
