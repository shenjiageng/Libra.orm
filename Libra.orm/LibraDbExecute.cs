using Libra.orm.LibraAttributes.DbMapping;
using Libra.orm.DbModel;
using Libra.orm.LibraBase;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.Threading.Tasks;

namespace Libra.orm
{
    internal static class LibraDbExecute
    {
        private static readonly Stopwatch _watch = new Stopwatch();

        public static T Execute<T>(string sql, LibraDbBehaviorEnum behavior, Func<SqlCommand, T> func, params SqlParameter[] parameter)
        {
            return Execute<T>(sql, behavior, func, false, parameter);
        }

        /// <summary>
        /// 执行SQL语句的操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="func"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static T Execute<T>(string sql, LibraDbBehaviorEnum behavior, Func<SqlCommand, T> func, bool realtime, params SqlParameter[] parameter)
        {

            var connectionModel = LibraConnectionStringPool.GetConnection(behavior, realtime);
            using SqlConnection connection = new SqlConnection(connectionModel.ConnectionString);
            SqlCommand command = new SqlCommand(sql, connection);
            if (parameter != null && parameter.Length > 0)
                command.Parameters.AddRange(parameter);
            connection.Open();
            _watch.Restart();
            var result = func.Invoke(command);
            _watch.Stop();
            connectionModel.TimeConsume[connectionModel.AskCount++ % connectionModel.TimeConsume.Length] = _watch.Elapsed.TotalMilliseconds;
            connection.Close();
            return result;
        }

        /// <summary>
        /// reader对象转运行时结果集
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<dynamic> ReaderToList(this SqlDataReader reader)
        {
            List<dynamic> list = new List<dynamic>();
            IReadOnlyCollection<DbColumn> columns = null;
            while (reader.Read())
            {
                if (columns == null)
                    columns = reader.GetColumnSchema();
                var eo = new ExpandoObject();
                foreach (var item in columns)
                {
                    ((IDictionary<string, object>)eo).Add(item.ColumnName, reader[item.ColumnName]);
                }
                list.Add(eo);
            }
            reader.Close();
            return list;
        }

        /// <summary>
        /// reader对象转实体模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<T> ReaderToList<T>(this SqlDataReader reader) where T : LibraBaseModel, new()
        {
            List<T> listModels = new List<T>();
            PropertyInfo[] propertys = null;
            while (reader.Read())
            {
                if (propertys == null) propertys = typeof(T).GetProperties();
                T t = new T();
                foreach (var prop in propertys)
                {
                    prop.SetValue(t, reader[prop.GetMappingName()]);
                }
                listModels.Add(t);
            }
            return listModels;
        }

    }
}
