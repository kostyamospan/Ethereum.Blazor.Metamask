using Microsoft.Extensions.DependencyInjection;
using Nethereum.Blazor.Metamask.Abstractions;
using Nethereum.Metamask;

namespace Nethereum.Blazor.Metamask.Extensions
{
    public static class ServiceCollectableExtension
    {
        public static void AddBlazorMetamask(this IServiceCollection services)
        {
            services.AddSingleton<IMetamaskInterop, MetamaskBlazorInterop>();
            services.AddSingleton<MetamaskInterceptor>();
            services.AddSingleton<MetamaskHostProvider>();
            services.AddSingleton<IEthereumHostProvider>(serviceProvider =>
                serviceProvider.GetService<MetamaskHostProvider>()
            );
        }
    }
}