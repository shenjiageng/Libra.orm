using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libra.orm.DbMapping
{
    public class LibraBaseMappingAttribute : Attribute
    {
        private readonly string _MappingName = string.Empty;

        public LibraBaseMappingAttribute(string mappingName)
        {
            this._MappingName = mappingName;
        }

        /// <summary>
        /// 获取标注表的实例名称
        /// </summary>
        /// <returns></returns>
        public string GetMappingName() => this._MappingName;
    }
}
