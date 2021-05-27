using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libra.orm.LibraAttributes.DbFilter
{
    /// <summary>
    /// 标注属性为主键
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LibraKeyAttribute:Attribute
    { }
}
