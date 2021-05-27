using System;
using System.Collections.Generic;
using System.Text;

namespace Libra.orm.LibraAttributes.DbValiformat
{
    /// <summary>
    /// 数据格式化基类
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class LibraBaseFormatAttribute : Attribute
    {
        public abstract object ValueFormat(object oValue);

        public abstract string GetSqlType();
    }
}
