using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Libra.orm.ExpressionExtend
{
    public static class ExpressionsToParameter
    {

        /// <summary>
        /// 目录树表达式 a => 1 转数据库参数化
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public static SqlParameter[] ToDbParameterExtend(this Expression<Func<string, object>>[] expressions)
        {
            if (expressions.Length == 0) return null;

            List<SqlParameter> parameters = new List<SqlParameter>();
            ExpressionToSql visit = new ExpressionToSql();
            foreach (var item in expressions)
            {
                var kv = visit.VisitLambda(item);
                parameters.Add(new SqlParameter($"@{kv.Item1}", kv.Item2));
            }
            return parameters.ToArray();
        }
    }
}
