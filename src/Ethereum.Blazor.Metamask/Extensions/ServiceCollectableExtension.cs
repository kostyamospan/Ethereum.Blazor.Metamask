using Ethereum.Blazor.Metamask.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Ethereum.Blazor.Metamask.Extensions
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