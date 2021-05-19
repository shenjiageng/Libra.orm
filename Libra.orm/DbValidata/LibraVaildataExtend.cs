using Libra.orm.DbModel;
using Libra.orm.LibraBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Libra.orm.DbValidata
{
    public static class LibraVaildataExtend
    {
        /// <summary>
        /// 模型中属性标记校验
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool PropertyVailRequired<T>(this T t) where T : LibraBaseModel, new()
        {
            Type type = typeof(T);
            foreach (var prop in type.GetProperties())
            {
                if (prop.IsDefined(typeof(LibraBaseAttribute), true))
                {
                    object oValue = prop.GetValue(t);
                    var attributes = prop.GetCustomAttributes<LibraBaseAttribute>();
                    foreach (var attribute in attributes)
                    {
                        if (!attribute.VailValue(oValue)) return false;
                    }
                }
            }
            return true;
        }
    }
}
