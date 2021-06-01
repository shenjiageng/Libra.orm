using Libra.orm.DbModel;
using Libra.orm.LibraBase;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libra.orm
{
    internal class LibraConnectionStringPool
    {
        internal static LibraConnectionStringModel WriteConnection;
        internal static List<LibraConnectionStringModel> ReadConnections;
        internal static LibraStrategyReaderEnum Strategy;

        /// <summary>
        /// 初始化连接字符串
        /// </summary>
        internal static void PoolInitialization(LibraConnectionStringModel writeConnection, LibraStrategyReaderEnum strategy, LibraConnectionStringModel[] readConnections)
        {
            WriteConnection = writeConnection ?? throw new ArgumentNullException(nameof(writeConnection));
            ReadConnections = new List<LibraConnectionStringModel>();
            if (readConnections != null)
            {
                Strategy = strategy;
                if (Strategy == LibraStrategyReaderEnum.Weighing)
                {
                    foreach (var item in readConnections)
                    {
                        for (int i = 0; i < item.WeighingNumber; i++)
                        {
                            ReadConnections.Add(item);
                        }
                    }
                }
                else
                {
                    ReadConnections = readConnections.ToList();
                }
            }
        }


        internal static LibraConnectionStringModel GetConnection(LibraDbBehaviorEnum behavior, bool realtime = false)
        {
            return behavior switch
            {
                LibraDbBehaviorEnum.Read => Dispatcher(realtime),
                LibraDbBehaviorEnum.Write => WriteConnection,
                _ => throw new Exception("错误的数据操作目的."),
            };
        }

        private static int _nIndex = 0;

        internal static LibraConnectionStringModel Dispatcher(bool realtime)
        {
            // 未配置读写分离，则自动返回主库.
            // 或者需要实时查询，则返回主库
            if (ReadConnections == null || ReadConnections.Count == 0 || realtime) return WriteConnection;
            switch (Strategy)
            {
                case LibraStrategyReaderEnum.RoundRobin:
                    {
                        var readModel = ReadConnections[_nIndex++ % ReadConnections.Count];
                        if (_nIndex == ReadConnections.Count) _nIndex = 0;
                        return readModel;
                    }
                case LibraStrategyReaderEnum.Random:
                case LibraStrategyReaderEnum.Weighing:
                    {
                        var readModel = ReadConnections[new Random(_nIndex++).Next(0, ReadConnections.Count)];
                        if (_nIndex == ReadConnections.Count) _nIndex = 0;
                        return readModel;
                    }
                case LibraStrategyReaderEnum.Pressure:
                    {
                        // 压力策略下，自动获取最小耗时的连接
                        var sumMaxTimeConsume = ReadConnections.Select(r => r.TimeConsume.Sum()).Min();
                        return ReadConnections.Where(r => r.TimeConsume.Sum() == sumMaxTimeConsume).First();
                    }
                // 未配置策略则自动返回主库
                default: return WriteConnection;
            }
        }
    }
}
