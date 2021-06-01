using Libra.orm.DbModel;
using Libra.orm.ExpressionExtend;
using Libra.orm.LibraBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Libra.orm.test
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=**;Initial Catalog=**;User ID=**;Password=**;";

            //LibraContent content = new LibraContent(new LibraConfigure
            //{
            //    WriteConnection = new LibraConnectionStringModel { ConnectionString = connectionString },
            //    AssemblyDirectory = AppDomain.CurrentDomain.BaseDirectory
            //});
            //Stopwatch watch = new Stopwatch();
            //double dynamicSumTime = 0;
            //double tableSumTime = 0;
            //for (int i = 1; i <= 10; i++)
            //{
            //    watch.Restart();
            //    var result = content.Query("select * from dbo.hwsp (nolock) where hwshl > 0 ");
            //    watch.Stop();
            //    Console.WriteLine($"第{i}次 dynamic耗时：{watch.Elapsed.TotalMilliseconds} ms");

            //    dynamicSumTime += watch.Elapsed.TotalMilliseconds;

            //    watch.Restart();
            //    DbHandler_Function dbContent = new DbHandler_Function(connectionString);
            //    DataTable table = dbContent.AdtExecuteDataTable("select * from dbo.hwsp (nolock) where hwshl > 0 ");
            //    watch.Stop();
            //    Console.WriteLine($"第{i}次 table耗时：{watch.Elapsed.TotalMilliseconds} ms");

            //    tableSumTime += watch.Elapsed.TotalMilliseconds;
            //}
            //Console.WriteLine($"dynamic总耗时:{dynamicSumTime},平均 {dynamicSumTime / 10}");
            //Console.WriteLine($"table总耗时:{tableSumTime},平均 {tableSumTime / 10}");
            //Console.WriteLine(dynamicSumTime > tableSumTime ? "框架性能低":"");

            LibraContent content = new LibraContent(new LibraConfigure
            {
                WriteConnection = new LibraConnectionStringModel { ConnectionString = connectionString },
                AssemblyDirectory = new string[] { AppDomain.CurrentDomain.BaseDirectory + "Libra.orm.test.dll" }
            });
            UserInfoModel uInfo = new UserInfoModel
            {
                Account = "shenjiageng",
                DeptGuid = Guid.Parse("62203331-558F-4069-8601-28623851FB5B"),
                FullDepartment = "........",
                IsAdministrator = true,
                IsEnable = true,
                Password = "123456",
                Position = "...",
                Username = "...",
                UGuid = Guid.NewGuid(),
                RegisterTime = DateTime.Now
            };
            content.Insert(uInfo);
            uInfo = content.Query<UserInfoModel>(uinfo => uinfo.Account == uInfo.Account).First();
            uInfo.Account = "11111";
            content.Update(uInfo, u => u.ID == 1);
            content.Delete<UserInfoModel>(uinfo => uinfo.Account == "shenjiageng");
            Console.ReadKey();
        }
    }
}
