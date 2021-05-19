using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libra.orm.DbMapping
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LibraColumnAttribute : LibraBaseMappingAttribute
    {
        public LibraColumnAttribute(string columnName) : base(columnName) { }
    }
}
