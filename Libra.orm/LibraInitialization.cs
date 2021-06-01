using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Libra.orm.LibraAttributes.DbMapping;
using System.Linq;
using Libra.orm.LibraBase;
using Libra.orm.LibraAttributes.DbFilter;
using System.Data;
using System.IO;
using Libra.orm.LibraAttributes.DbValiformat;
using Libra.orm.LibraAttributes.DbValidata;
using System.Data.SqlClient;

namespace Libra.orm
{
    internal class LibraInitialization
    {
        readonly LibraConfigure _Configure;

        public LibraInitialization(LibraConfigure configure)
        {
            this._Configure = configure;
        }

        /// <summary>
        /// 模型model初始化成表
        /// </summary>
        internal void InitDatabaseTable()
        {
            // 获取所有dll中标记 LibraTableAttribute 的类
            Type[] types = SearchTable();
            // 存储即将执行的sql创建语句
            List<string> waitExecute = new List<string>();
            foreach (var item in types)
            {
                string tableName = item.GetMappingName();
                if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentException();
                StringBuilder tSqlBuilder = new StringBuilder();
                tSqlBuilder.Append($"if OBJECT_ID(N'{tableName}',N'U') is null begin ");
                tSqlBuilder.Append($"create table {tableName} (");
                var key = item.FilterPropertyWithKey();
                tSqlBuilder.Append($"[{key.GetMappingName()}] { key.PropertyType.TypeToDbType().ToTSqlText() } identity(1,1) primary key");
                foreach (var prop in item.FilterPropertyWithNoKey().FilterPropertyWithNoMapped())
                {
                    tSqlBuilder.Append($",[{prop.GetMappingName()}] ");
                    if (prop.IsDefined(typeof(LibraBaseFormatAttribute), true))
                        tSqlBuilder.Append(prop.GetCustomAttribute<LibraBaseFormatAttribute>().GetSqlType());
                    else
                        tSqlBuilder.Append(prop.PropertyType.TypeToDbType().ToTSqlText());
                    if (prop.IsDefined(typeof(LibraRequiredAttribute), true))
                        tSqlBuilder.Append(" not null");
                    else
                        tSqlBuilder.Append(" null");
                }
                tSqlBuilder.Append(") end ");
                waitExecute.Add(tSqlBuilder.ToString());
            }
            // 执行表初始化
            InitExecute(waitExecute);
        }

        private Type[] SearchTable()
        {
            List<Type> types = new List<Type>();
            string[] directory = this._Configure.AssemblyDirectory;
            if (directory.Length == 0) throw new Exception("请配置数据库模型model所在程序集.");
            foreach (var filePath in directory)
            {
                // 通过反射加载所有标记 LibraTableAttribute 的类
                Assembly asm = Assembly.LoadFrom(filePath);
                Type[] cosType = asm.GetExportedTypes().Where(o =>
                {
                    return IsTableAttribute(Attribute.GetCustomAttributes(o, true));
                }).ToArray();
                if (cosType.Length > 0)
                    types.AddRange(cosType);
            }
            return types.ToArray();
        }

        private bool IsTableAttribute(Attribute[] o)
        {
            foreach (Attribute a in o)
            {
                if (a is LibraTableAttribute)
                    return true;
            }
            return false;
        }

        private void InitExecute(List<string> waitExecuteSql)
        {
            // 获取所有的数据库链接
            List<string> connections = new List<string>();
            connections.Add(this._Configure.WriteConnection.ConnectionString);
            if (this._Configure.ReadConnections != null)
                connections.AddRange(this._Configure.ReadConnections.Select(rc => rc.ConnectionString).ToArray());
            foreach (var connString in connections)
            {
                foreach (var sql in waitExecuteSql)
                {
                    using SqlConnection conn = new SqlConnection(connString);
                    SqlCommand command = new SqlCommand(sql, conn);
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
    }
}
