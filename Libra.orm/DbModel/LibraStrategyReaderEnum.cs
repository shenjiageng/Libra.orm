using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libra.orm.DbModel
{
    public enum LibraStrategyReaderEnum
    {
        /*
            读取负载均衡实现方式：
            1、轮询策略
            2、随机策略
            3、权重策略
            4、压力策略
                a) 可根据近百次响应时间的平均来分配
                b) 根据服务器硬件实时监控来分配
                c) 根据当前每台服务器的数据库链接数来分配
            5 ...
        */
        /// <summary>
        /// 轮询平均策略
        /// </summary>
        RoundRobin,
        /// <summary>
        /// 随机平均策略
        /// </summary>
        Random,
        /// <summary>
        /// 服务器权重策略
        /// </summary>
        Weighing,
        /// <summary>
        /// 数据库压力策略
        /// 使用响应平均分配策略
        /// </summary>
        Pressure
    }
}
