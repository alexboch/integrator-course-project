using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    public static class ExpressionsExtensions
    {
      
        /// <summary>
        /// Вычисляет результат арифметического выражения
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static double Eval(this string expr, double x=0)
        {
            return new RPNExpression(expr).Evaluate(x);
        }

        /// <summary>
        /// Переводит выражение в инфиксной записи в обратную польскую нотацию
        /// </summary>
        public static RPNExpression ToRPN(this string expr)
        {
            RPNExpression q = new RPNExpression(expr);
            return q;
        }
    }

}
