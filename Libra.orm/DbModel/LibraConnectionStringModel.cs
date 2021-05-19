using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libra.orm.DbModel
{
    /// <summary>
    /// 每个数据库链接模型
    /// </summary>
    public class LibraConnectionStringModel
    {
        /// <summary>
        /// 当前连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 权重数值 Weighing模式可用
        /// </summary>
        public int WeighingNumber { get; set; }

        /// <summary>
        /// 请求次数
        /// </summary>
        internal int AskCount { get; set; } = 0;

        /// <summary>
        /// 最近10次耗时
        /// </summary>
        internal double[] TimeConsume { get; set; } = new double[10];
    }
}
