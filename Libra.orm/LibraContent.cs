using Libra.orm.DbCache;
using Libra.orm.LibraAttributes.DbFilter;
using Libra.orm.LibraAttributes.DbMapping;
using Libra.orm.DbModel;
using Libra.orm.LibraAttributes.DbValidata;
using Libra.orm.ExpressionExtend;
using Libra.orm.LibraBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Libra.orm.LibraAttributes.DbValiformat;

namespace Libra.orm
{
    public class LibraContent : ILibraContent
    {
        public LibraContent(LibraConfigure configure)
        {
            // 初始化注入配置文件
            LibraConnectionStringPool.Configure = configure;
            LibraConnectionStringPool.PoolInitialization();
        }

        /// <summary>
        /// 执行单模型查询 实时查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">目录树表达式</param>
        /// <returns></returns>
        public IEnumerable<T> QueryRealtime<T>(Expression<Func<T, bool>> expression) where T : LibraBaseModel, new()
        {
            string sql = LibraSqlCacheBuilder<T>.GetSqlString(LibraSqlCacheBuilderType.Query);
            ExpressionToSql visit = new ExpressionToSql();
            visit.Visit(expression);
            string wheres = visit.GetSqlWhere();
            if (!string.IsNullOrWhiteSpace(wheres)) sql += $" where {wheres}";
            return LibraDbExecute.Execute<IEnumerable<T>>(sql, LibraDbBehaviorEnum.Read, command =>
            {
                return command.ExecuteReader(CommandBehavior.CloseConnection).ReaderToList<T>();
            }, true).ValueFormat();
        }

        /// <summary>
        /// 执行单模型查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">目录树表达式</param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(Expression<Func<T, bool>> expression) where T : LibraBaseModel, new()
        {
            string sql = LibraSqlCacheBuilder<T>.GetSqlString(LibraSqlCacheBuilderType.Query);
            ExpressionToSql visit = new ExpressionToSql();
            visit.Visit(expression);
            string wheres = visit.GetSqlWhere();
            if (!string.IsNullOrWhiteSpace(wheres)) sql += $" where {wheres}";
            yield return (T)LibraDbExecute.Execute<IEnumerable<T>>(sql, LibraDbBehaviorEnum.Read, command =>
            {
                return command.ExecuteReader(CommandBehavior.CloseConnection).ReaderToList<T>();
            }, false).ValueFormat();
        }

        /// <summary>
        /// 执行复杂的SQL查询 
        /// 非实时结果. 如果未配置读写分离，该查询结果依旧是实时的.
        /// 返回值为：List<dynamic> dynamic 为匿名对象，根据查询语句返回结果集中的属性 可以根据 item.属性 访问
        /// 遍历请使用 KeyValuePair<string,object>
        /// </summary>
        /// <param name="sql">查询的语句</param>
        /// <param name="expressions">参数表达式 参数为字段,表达式后为值 例如: 属性 => 值</param>
        /// <returns></returns>
        public IEnumerable<dynamic> Query(string sql, params Expression<Func<string, object>>[] expressions)
        {
            return LibraDbExecute.Execute(sql, LibraDbBehaviorEnum.Read, command =>
            {
                return command.ExecuteReader(CommandBehavior.CloseConnection).ReaderToList();
            }, false, expressions.ToDbParameterExtend());
        }

        /// <summary>
        /// 执行复杂的SQL查询 
        /// 实时查询数据结果.
        /// 返回值为：List<dynamic> dynamic 为匿名对象，根据查询语句返回结果集中的属性 可以根据 item.属性 访问
        /// 遍历请使用 KeyValuePair<string,object>
        /// </summary>
        /// <param name="sql">查询的语句</param>
        /// <param name="parameters">参数表达式 参数为字段,表达式后为值 例如: 属性 => 值</param>
        /// <returns></returns>
        public IEnumerable<dynamic> QueryRealtime(string sql, params Expression<Func<string, object>>[] expressions)
        {
            return LibraDbExecute.Execute(sql, LibraDbBehaviorEnum.Read, command =>
            {
                return command.ExecuteReader(CommandBehavior.CloseConnection).ReaderToList();
            }, true, expressions.ToDbParameterExtend());
        }

        /// <summary>
        /// 新增模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">需要增加的实体对象</param>
        /// <returns></returns>
        public bool Insert<T>(T t) where T : LibraBaseModel, new()
        {
            if (!t.PropertyVailRequired()) throw new Exception("数据限制校验失败.");
            t.ValueFormat(); // 属性值格式化
            Type type = typeof(T);
            string sql = LibraSqlCacheBuilder<T>.GetSqlString(LibraSqlCacheBuilderType.Insert);
            SqlParameter[] parameters = type.FilterPropertyWithNoKey()
                .FilterPropertyWithNoMapped()
                .Select(prop => new SqlParameter($"@{prop.GetMappingName()}", prop.GetValue(t) ?? DBNull.Value)).ToArray();
            return LibraDbExecute.Execute(sql, LibraDbBehaviorEnum.Write, command =>
            {
                return command.ExecuteNonQuery() != 0;
            }, parameters);
        }

        /// <summary>
        /// 删除符合条件的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">目录树表达式</param>
        /// <returns></returns>
        public bool Delete<T>(Expression<Func<T, bool>> expression) where T : LibraBaseModel, new()
        {
            Type type = typeof(T);
            string sql = LibraSqlCacheBuilder<T>.GetSqlString(LibraSqlCacheBuilderType.Delete);
            ExpressionToSql visit = new ExpressionToSql();
            visit.Visit(expression);
            string wheres = visit.GetSqlWhere();
            if (!string.IsNullOrWhiteSpace(wheres))
                sql += $" where {wheres}";
            return LibraDbExecute.Execute(sql, LibraDbBehaviorEnum.Write, command =>
            {
                return command.ExecuteNonQuery() != 0;
            });
        }

        /// <summary>
        /// 数据更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">新修改的实体对象</param>
        /// <param name="expression">目录树表达式</param>
        /// <returns></returns>
        public bool Update<T>(T t, Expression<Func<T, bool>> expression) where T : LibraBaseModel, new()
        {
            if (!t.PropertyVailRequired()) throw new Exception("数据限制校验失败.");
            t.ValueFormat(); // 属性值格式化
            Type type = typeof(T);
            string sql = LibraSqlCacheBuilder<T>.GetSqlString(LibraSqlCacheBuilderType.Update);
            ExpressionToSql visit = new ExpressionToSql();
            visit.Visit(expression);
            string wheres = visit.GetSqlWhere();
            if (!string.IsNullOrWhiteSpace(wheres))
                sql += $" where {wheres}";
            SqlParameter[] parameters = type.FilterPropertyWithNoKey().FilterPropertyWithNoMapped().Select(prop => new SqlParameter($"@{prop.GetMappingName()}", prop.GetValue(t) ?? DBNull.Value)).ToArray();
            return LibraDbExecute.Execute(sql, LibraDbBehaviorEnum.Write, command =>
            {
                return command.ExecuteNonQuery() != 0;
            }, parameters);
        }
    }
}