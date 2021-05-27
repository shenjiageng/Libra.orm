using System;
using System.Collections.Generic;
using System.Text;

namespace Libra.orm.LibraAttributes.DbValiformat
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LibraVarcharAttribute : LibraBaseFormatAttribute
    {
        private int _length;

        public LibraVarcharAttribute(int length)
        {
            if (length > 2000) throw new ArgumentException("仅支持2000以内的数值.");
            this._length = length;
        }

        public override object ValueFormat(object oValue)
        {
            if (oValue.ToString().Length > _length) throw new ArgumentException("值超过设定的最大值.");
            return oValue;
        }

        public override string GetSqlType()
        {
            return $"varchar({this._length})";
        }
    }
}
