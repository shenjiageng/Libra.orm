using Libra.orm.DbModel;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libra.orm.LibraBase
{
    public class LibraConfigure : IOptions<LibraConfigure>
    {
        /// <summary>
        /// 系统配置的主写连接模型
        /// </summary>
        public LibraConnectionStringModel WriteConnection { get; set; }

        /// <summary>
        /// 读取的连接模型
        /// </summary>
        public LibraConnectionStringModel[] ReadConnections { get; set; }

        /// <summary>
        /// 数据库读取策略 
        /// 如果未配置读取连接字符串, 该策略无效
        /// </summary>
        public LibraStrategyReaderEnum Strategy { get; set; }

        public LibraConfigure Value => this;

        /// <summary>
        /// 当前执行程序的目录
        /// </summary>
        public string[] AssemblyDirectory { get; set; }
    }
}
