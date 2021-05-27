using System;
using System.Collections.Generic;
using System.Text;

namespace Libra.orm.LibraAttributes.DbFilter
{
    /// <summary>
    /// 标记属性不映射数据库字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LibraNotMappedAttribute : Attribute
    { }
}
