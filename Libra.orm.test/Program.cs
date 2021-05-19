using Libra.orm.DbModel;
using Libra.orm.LibraBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Libra.orm.test
{
    class Program
    {
        static void Main(string[] args)
        {
            // 常规创建
            LibraContent content = new LibraContent(new LibraConfigure
            {
                WriteConnection = new LibraConnectionStringModel { ConnectionString = "Data Source=.;Initial Catalog=DIP_BASE;User ID=sa;Password=123456;" }
            });
            // 依赖注入
            /*services.AddLibraDistributed(new LibraConfigure
            {
                WriteConnection = new LibraConnectionStringModel { ConnectionString = "Data Source=.;Initial Catalog=ZQ_SYSTEM_LOG;User ID=sa;Password=zq@418" }
            });*/
            // 执行查询

            Stopwatch watch = new Stopwatch();
            watch.Restart();
            var result = content.Query("select * from SYS_USER_INFO (nolock) where Username like @username ", username => "%demo%");
            watch.Stop();
            Console.WriteLine($"dynamic耗时：{watch.Elapsed.TotalMilliseconds} ms");

            foreach (var item in result)
            {
                Console.WriteLine(item.jwh);
            }

            /*watch.Restart();
            // 结果处理
            foreach (var item in result)
            {
                // 整行访问
                foreach (KeyValuePair<string, object> kv in item)
                {
                    Console.Write($"  {kv.Key}:{kv.Value}  ");
                }
                Console.WriteLine();
            }
            watch.Stop();*/

            watch.Restart();
            DbHandler_Function dbContent = new DbHandler_Function("Data Source=.;Initial Catalog=DIP_BASE;User ID=sa;Password=123456");
            DataTable table = dbContent.AdtExecuteDataTable("select * from SYS_USER_INFO (nolock) where Username like @username ",
                new System.Data.SqlClient.SqlParameter("@username", "%demo%"));
            watch.Stop();
            Console.WriteLine($"table耗时：{watch.Elapsed.TotalMilliseconds} ms");
            Console.ReadKey();
        }
    }
}
