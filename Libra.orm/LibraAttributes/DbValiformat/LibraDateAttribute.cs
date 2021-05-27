using System;
using System.Collections.Generic;
using System.Text;

namespace Libra.orm.LibraAttributes.DbValiformat
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LibraDateAttribute : LibraBaseFormatAttribute
    {
        public override object ValueFormat(object oValue)
        {
            return DateTime.Parse(oValue.ToString()).ToString("yyyy-MM-dd");
        }

        public override string GetSqlType()
        {
            return "date";
        }
    }
}
