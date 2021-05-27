using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Libra.orm.LibraAttributes.DbValiformat
{
    public static class LibraFormatExtend
    {
        /// <summary>
        /// 属性值格式化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public static void ValueFormat<T>(this T t)
        {
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (prop.IsDefined(typeof(LibraBaseFormatAttribute), true))
                {
                    object oValue = prop.GetValue(t);
                    var attributes = prop.GetCustomAttributes<LibraBaseFormatAttribute>();
                    foreach (var attribute in attributes)
                    {
                        prop.SetValue(t, attribute.ValueFormat(oValue));
                    }
                }
            }
        }

        /// <summary>
        /// 结果集格式化数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerable<T> ValueFormat<T>(this IEnumerable<T> data)
        {
            foreach (var item in data)
            {
                foreach (var prop in typeof(T).GetProperties())
                {
                    var attributes = prop.GetCustomAttributes<LibraBaseFormatAttribute>();
                    foreach (var attribute in attributes)
                    {
                        prop.SetValue(item, attribute.ValueFormat(prop.GetValue(item)));
                    }
                }
            }
            return data;
        }
    }
}
