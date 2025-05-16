using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Abstractions;
using YingCaiAiService.IService;
using YingCaiAiService.Service;
using YingCaiAiWin.ViewModels;

namespace YingCaiAiWin.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNavigationViewPageProvider(this IServiceCollection services)
        {
            _ = services.AddSingleton<
                INavigationViewPageProvider,
                DependencyInjectionNavigationViewPageProvider
            >();

            return services;
        }
        public static IServiceCollection AddTransientFromNamespace(
                this IServiceCollection services,
                string namespaceName,
                params Assembly[] assemblies
)
        {
            foreach (Assembly assembly in assemblies)
            {
                IEnumerable<Type> types = assembly
                    .GetTypes()
                    .Where(x =>
                        !x.IsAbstract &&
                        !x.IsGenericType &&
                        !x.IsNestedPrivate &&
                        !x.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false) &&
                        x.Namespace != null 
                        && x.Namespace!.StartsWith(namespaceName, StringComparison.InvariantCultureIgnoreCase)
                    );

                foreach (Type? type in types)
                {
                    if (services.All(x => x.ServiceType != type))
                    {
                        if (type == typeof(ViewModel))
                        {
                            continue;
                        }

                        _ = services.AddTransient(type);
                    }
                }
            }

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            var assembly = new[]
            {
                Assembly.GetExecutingAssembly(), // 主程序集
                typeof(IBaseService).Assembly, // 服务接口所在程序集
            };
            // 注册单例服务
            RegisterServices<ISingletonService>(services, assembly, ServiceLifetime.Singleton);

            // 注册作用域服务
            RegisterServices<IScopedService>(services, assembly, ServiceLifetime.Scoped);

            // 注册瞬时服务
            RegisterServices<ITransientService>(services, assembly, ServiceLifetime.Transient);

            return services;
        }

        private static void RegisterServices<TServiceInterface>(
            IServiceCollection services,
            Assembly[] assembly,
            ServiceLifetime lifetime)
        {
            var serviceTypes = assembly.SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass &&
                          !t.IsAbstract &&
                          typeof(TServiceInterface).IsAssignableFrom(t));

            foreach (var serviceType in serviceTypes)
            {
                // 获取服务实现的所有接口（排除生命周期标记接口）
                var interfaces = serviceType.GetInterfaces()
                    .Where(i => i != typeof(IBaseService) &&
                              i != typeof(ISingletonService) &&
                              i != typeof(IScopedService) &&
                              i != typeof(ITransientService));

                foreach (var interfaceType in interfaces)
                {
                    switch (lifetime)
                    {
                        case ServiceLifetime.Singleton:
                            services.AddSingleton(interfaceType, serviceType);
                            break;
                        case ServiceLifetime.Scoped:
                            services.AddScoped(interfaceType, serviceType);
                            break;
                        case ServiceLifetime.Transient:
                            services.AddTransient(interfaceType, serviceType);
                            break;
                    }
                }

                // 如果没有实现特定接口，但继承了生命周期基类，注册自身
                if (!interfaces.Any() && serviceType.IsAssignableTo(typeof(TServiceInterface)))
                {
                    switch (lifetime)
                    {
                        case ServiceLifetime.Singleton:
                            services.AddSingleton(serviceType);
                            break;
                        case ServiceLifetime.Scoped:
                            services.AddScoped(serviceType);
                            break;
                        case ServiceLifetime.Transient:
                            services.AddTransient(serviceType);
                            break;
                    }
                }
            }
        }
    }
}
