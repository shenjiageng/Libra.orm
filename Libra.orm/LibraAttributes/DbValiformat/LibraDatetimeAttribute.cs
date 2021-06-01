using System;
using System.Collections.Generic;
using System.Text;

namespace Libra.orm.LibraAttributes.DbValiformat
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LibraDatetimeAttribute : LibraBaseFormatAttribute
    {
        public override object ValueFormat(object oValue)
        {
            if (oValue == null) return null;
            return DateTime.Parse(DateTime.Parse(oValue.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public override string GetSqlType()
        {
            return "datetime";
        }
    }
}
