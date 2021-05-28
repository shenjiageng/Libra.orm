using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Libra.orm.LibraAttributes.DbMapping;
using System.Linq;
using Libra.orm.LibraBase;
using Libra.orm.LibraAttributes.DbFilter;
using System.IO;
using System.Data;
using Libra.orm.LibraAttributes.DbValiformat;
using Libra.orm.LibraAttributes.DbValidata;
using System.Data.SqlClient;

namespace Libra.orm
{
    internal static class LibraInitialization
    {
        /// <summary>
        /// 模型model初始化成表
        /// </summary>
        internal static void InitDatabaseTable()
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

        private static Type[] SearchTable()
        {
            List<Type> types = new List<Type>();
            string directory = LibraConnectionStringPool.Configure.AssemblyDirectory;
            if (string.IsNullOrWhiteSpace(directory)) throw new Exception("请配置model程序集目录.");
            string[] files = Directory.GetFiles(directory, "*.dll");
            foreach (var filePath in files)
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

        private static bool IsTableAttribute(Attribute[] o)
        {
            foreach (Attribute a in o)
            {
                if (a is LibraTableAttribute)
                    return true;
            }
            return false;
        }

        private static void InitExecute(List<string> waitExecuteSql)
        {
            // 获取所有的数据库链接
            List<string> connections = new List<string>();
            connections.Add(LibraConnectionStringPool.Configure.WriteConnection.ConnectionString);
            if (LibraConnectionStringPool.Configure.ReadConnections != null)
                connections.AddRange(LibraConnectionStringPool.Configure.ReadConnections.Select(rc => rc.ConnectionString).ToArray());
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

        private static SqlDbType TypeToDbType(this Type t)
        {
            return Type.GetTypeCode(t) switch
            {
                TypeCode.Boolean => SqlDbType.Bit,
                TypeCode.Byte => SqlDbType.TinyInt,
                TypeCode.DateTime => SqlDbType.DateTime,
                TypeCode.Decimal => SqlDbType.Decimal,
                TypeCode.Double => SqlDbType.Float,
                TypeCode.Int16 => SqlDbType.SmallInt,
                TypeCode.Int32 => SqlDbType.Int,
                TypeCode.Int64 => SqlDbType.BigInt,
                TypeCode.SByte => SqlDbType.TinyInt,
                TypeCode.Single => SqlDbType.Real,
                TypeCode.String => SqlDbType.NVarChar,
                TypeCode.UInt16 => SqlDbType.SmallInt,
                TypeCode.UInt32 => SqlDbType.Int,
                TypeCode.UInt64 => SqlDbType.BigInt,
                TypeCode.Char => SqlDbType.Char,
                _ => t == typeof(byte[]) ? SqlDbType.Binary : SqlDbType.Variant,
            };
        }

        private static string ToTSqlText(this SqlDbType type)
        {
            return type switch
            {
                SqlDbType.BigInt => "long",
                SqlDbType.Binary => "binary",
                SqlDbType.Bit => "bit",
                SqlDbType.Char => "char",
                SqlDbType.DateTime => "datetime",
                SqlDbType.Decimal => "numeric",
                SqlDbType.Float => "float",
                SqlDbType.Image => "image",
                SqlDbType.Int => "int",
                SqlDbType.Money => "money",
                SqlDbType.NChar => "nchar(max)",
                SqlDbType.NText => "ntext",
                SqlDbType.NVarChar => "nvarchar(max)",
                SqlDbType.Real => "real",
                SqlDbType.UniqueIdentifier => "uniqueIdentifier",
                SqlDbType.SmallDateTime => "smalldatetime",
                SqlDbType.SmallInt => "smallint",
                SqlDbType.SmallMoney => "smallmoney",
                SqlDbType.Text => "text",
                SqlDbType.Timestamp => "timestamp",
                SqlDbType.TinyInt => "tinyint",
                SqlDbType.VarBinary => "varbinary",
                SqlDbType.VarChar => "varchar",
                SqlDbType.Variant => "sql_variant",
                SqlDbType.Xml => "xml",
                SqlDbType.Date => "date",
                SqlDbType.Time => "time",
                SqlDbType.DateTime2 => "datetime2",
                SqlDbType.DateTimeOffset => "datetimeoffset",
                _ => "",
            };
        }
    }
}
