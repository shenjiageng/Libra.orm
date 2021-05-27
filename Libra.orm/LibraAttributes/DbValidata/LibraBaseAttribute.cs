using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libra.orm.LibraAttributes.DbValidata
{
    /// <summary>
    /// 校验的基类
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class LibraBaseAttribute : Attribute
    {
        public abstract bool VailValue(object oValue);
    }
}
