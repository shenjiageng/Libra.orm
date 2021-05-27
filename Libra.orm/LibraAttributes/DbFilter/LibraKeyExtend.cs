using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Libra.orm.LibraAttributes.DbFilter
{
    public static class LibraKeyExtend
    {
        /// <summary>
        /// 过滤标记LibraKey的属性 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> FilterPropertyWithNoKey(this Type type)
        {
            return type.GetProperties().Where(p => !p.IsDefined(typeof(LibraKeyAttribute), true));
        }

        /// <summary>
        /// 过滤标记LibraKey的属性 
        /// </summary>
        /// <param name="propertys"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> FilterPropertyWithNoKey(this IEnumerable<PropertyInfo> propertys)
        {
            return propertys.Where(p => !p.IsDefined(typeof(LibraKeyAttribute), true));
        }

        /// <summary>
        /// 过滤未映射的属性字段
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> FilterPropertyWithNoMapped(this Type type) 
        {
            return type.GetProperties().Where(p => !p.IsDefined(typeof(LibraNotMappedAttribute), true));
        }

        /// <summary>
        /// 过滤未映射的属性字段
        /// </summary>
        /// <param name="propertys"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> FilterPropertyWithNoMapped(this IEnumerable<PropertyInfo> propertys)
        {
            return propertys.Where(p => !p.IsDefined(typeof(LibraNotMappedAttribute), true));
        }
    }
}
