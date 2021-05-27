using Libra.orm.LibraBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libra.orm.LibraAttributes.DbValidata
{
    /// <summary>
    /// 属性校验是否为空
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LibraRequiredAttribute : LibraBaseAttribute
    {
        /// <summary>
        /// 校验属性数据是否为空
        /// </summary>
        /// <param name="oValue"></param>
        /// <returns></returns>
        public override bool VailValue(object oValue)
        {
            return oValue != null && !string.IsNullOrWhiteSpace(oValue.ToString());
        }
    }
}
