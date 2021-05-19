using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Libra.orm.DbFilter
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

    }
}
