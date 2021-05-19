using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Libra.orm.ExpressionExtend
{
    internal static class ExpressionOperator
    {
        /// <summary>
        /// <remarks>解析操作类型</remarks>
        /// </summary>
        /// <param name="expressionType">操作类型：例如（>,=）</param>
        /// <returns></returns>
        internal static string ToSqlOperator(this ExpressionType expressionType)
        {
            // 将操作类型解析为对应的表达方式
            return expressionType switch
            {
                ExpressionType.GreaterThan => ">",
                ExpressionType.LessThan => "<",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThanOrEqual => "<=",
                ExpressionType.Equal => "=",
                ExpressionType.NotEqual => "<>",
                ExpressionType.Not => "NOT",
                ExpressionType.And => "AND",
                ExpressionType.AndAlso => "AND",
                ExpressionType.Or => "OR",
                ExpressionType.OrElse => "OR",
                _ => throw new NotSupportedException(expressionType.ToString() + " is not supported!"),
            };
        }
    }
}
