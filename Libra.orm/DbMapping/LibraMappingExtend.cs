using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Libra.orm.DbMapping
{
    public static class LibraMappingExtend
    {
        /// <summary>
        /// 获取标注 [LibraBaseMapping] 特性的名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetMappingName<T>(this T t) where T : MemberInfo
        {
            if (t.IsDefined(typeof(LibraBaseMappingAttribute), true))
            {
                var attribute = t.GetCustomAttribute<LibraBaseMappingAttribute>();
                return attribute.GetMappingName();
            }
            return t.Name;
        }
    }
}
