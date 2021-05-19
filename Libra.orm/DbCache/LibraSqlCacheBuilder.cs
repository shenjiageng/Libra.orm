using Libra.orm.DbFilter;
using Libra.orm.DbMapping;
using Libra.orm.DbModel;
using Libra.orm.LibraBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libra.orm.DbCache
{
    /// <summary>
    /// 负责生成SQL语句，泛型类缓存重用
    /// 针对每个对象的缓存机制
    /// </summary>
    public class LibraSqlCacheBuilder<T> where T : LibraBaseModel, new()
    {
        private static readonly Dictionary<LibraSqlCacheBuilderType, string> SqlCache = new Dictionary<LibraSqlCacheBuilderType, string>();

        static LibraSqlCacheBuilder()
        {
            Type type = typeof(T);
            // 反射初始化SQL语句
            {
                string columnsString = string.Join(',', type.FilterPropertyWithNoKey().Select(prop => $"[{prop.GetMappingName()}]"));
                string valuesString = string.Join(',', type.FilterPropertyWithNoKey().Select(prop => $"@{prop.GetMappingName()}"));
                string insertSql = $"insert into {type.GetMappingName()} ({columnsString}) values ({valuesString})";
                SqlCache.Add(LibraSqlCacheBuilderType.Insert, insertSql);
            }
            {
                string deleteSql = $"delete from {type.GetMappingName()}";
                SqlCache.Add(LibraSqlCacheBuilderType.Delete, deleteSql);
            }
            {
                string expressionString = string.Join(',', type.FilterPropertyWithNoKey().Select(prop => $"[{prop.GetMappingName()}] = @{prop.GetMappingName()}"));
                string updateSql = $"update {type.GetMappingName()} set {expressionString}";
                SqlCache.Add(LibraSqlCacheBuilderType.Update, updateSql);
            }
            {
                string columnsString = string.Join(',', type.GetProperties().Select(prop => $"[{prop.GetMappingName()}]"));
                string querySql = $"select {columnsString} from {type.GetMappingName()}";
                SqlCache.Add(LibraSqlCacheBuilderType.Query, querySql);
                SqlCache.Add(LibraSqlCacheBuilderType.NoLockQuery, $"{querySql} (nolock)");
            }
        }

        /// <summary>
        /// 根据操作获取操作字符串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetSqlString(LibraSqlCacheBuilderType type)
        {
            if (!SqlCache.ContainsKey(type)) throw new Exception("未找到对应类型的SQL语句.");
            return SqlCache[type];
        }
    }
}
