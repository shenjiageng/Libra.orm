using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libra.orm.LibraAttributes.DbMapping
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LibraTableAttribute : LibraBaseMappingAttribute
    {
        public LibraTableAttribute(string tableName) : base(tableName) { }
    }
}
