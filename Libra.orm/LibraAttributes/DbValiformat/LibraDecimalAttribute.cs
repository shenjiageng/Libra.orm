using System;
using System.Collections.Generic;
using System.Text;

namespace Libra.orm.LibraAttributes.DbValiformat
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LibraDecimalAttribute : LibraBaseFormatAttribute
    {
        private byte _precision = 12;
        private byte _scale = 2;

        /// <summary>
        /// 设置属性的精确度
        /// </summary>
        /// <param name="precision">精确度</param>
        /// <param name="scale">保留小数位数</param>
        public LibraDecimalAttribute(byte precision = 12, byte scale = 2)
        {
            _precision = precision;
            _scale = scale;
        }

        public override object ValueFormat(object oValue)
        {
            return Math.Round(Convert.ToDecimal(oValue), this._scale);
        }

        public override string GetSqlType()
        {
            return $"decimal({this._precision},{this._scale})";
        }
    }
}
