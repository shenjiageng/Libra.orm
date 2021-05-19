using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libra.orm.LibraBase;
using Microsoft.Extensions.DependencyInjection;

namespace Libra.orm
{
    public static class LibraServiceCollectionExtensions
    {
        /// <summary>
        /// 注入Libra ORM组件服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction">配置信息</param>
        /// <returns></returns>
        public static IServiceCollection AddLibraDistributed(this IServiceCollection services, Action<LibraConfigure> setupAction)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));
            services.AddOptions();
            services.Configure(setupAction);
            services.Add(ServiceDescriptor.Singleton<ILibraContent, LibraContent>());
            return services;
        }
    }
}
