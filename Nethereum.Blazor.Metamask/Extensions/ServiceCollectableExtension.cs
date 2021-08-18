using Microsoft.Extensions.DependencyInjection;

namespace Nethereum.Blazor.Metamask.Extensions
{
    public static class ServiceCollectableExtension
    {
        public static void AddBlazorMetamask(this IServiceCollection services)
        {
            services.AddSingleton<MetamaskHostProvider>();
        }
    }
}