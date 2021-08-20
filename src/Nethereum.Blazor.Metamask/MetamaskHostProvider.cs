using Nethereum.Hex.HexConvertors.Extensions;
using System;
using System.Threading.Tasks;
using Nethereum.Blazor.Metamask.Abstractions;
using Nethereum.Metamask;
using Microsoft.JSInterop;

namespace Nethereum.Blazor.Metamask
{
    public  class MetamaskHostProvider : IEthereumHostProvider
    {
        private readonly IMetamaskInterop _metamaskInterop;
        private MetamaskInterceptor _metamaskInterceptor;

        public event Func<string, Task> SelectedAccountChanged;
        public event Func<int, Task> NetworkChanged;
        public event Func<int, Task> ProviderConnected;
        public event Func<Task> ProviderDisconnected;

        public MetamaskHostProvider(IJSRuntime jsRuntime, IMetamaskInterop metamaskInterop, MetamaskInterceptor metamaskInterceptor)
        {
            _metamaskInterop = metamaskInterop;
            _metamaskInterceptor = metamaskInterceptor;
        }

        public Task SwitchChainAsync(int newChainId)
            => _metamaskInterop.SwitchChainAsync(newChainId);

        public ValueTask<bool> IsMetamaskAvailableAsync()
             => _metamaskInterop.CheckMetamaskAvailability();

        public ValueTask<bool> IsMetamaskConnectedAsync()
            => _metamaskInterop.CheckAccountConnected();

        public Web3.Web3 GetWeb3()
            => new Web3.Web3 {Client = {OverridingRequestInterceptor = _metamaskInterceptor}};

        public async ValueTask<string> EnableProviderAsync()
        {
            var selectedAccount = await _metamaskInterop.EnableEthereumAsync();
            var enabled = !string.IsNullOrEmpty(selectedAccount);

            if (enabled)
            {
                if (SelectedAccountChanged != null) await SelectedAccountChanged.Invoke(selectedAccount);
                return selectedAccount;
            }

            return null;
        }

        public ValueTask<string> GetSelectedAccountAsync()
             => _metamaskInterop.GetSelectedAddress(); 

        public ValueTask<int> GetSelectedNetworkAsync()
            => _metamaskInterop.GetSelectedNetwork();

        public async Task ChangeSelectedAccountAsync(string selectedAccount)
        {
            if (SelectedAccountChanged is not null)
                await SelectedAccountChanged.Invoke(selectedAccount);
        }

        public async Task ChangeSelectedNetworkAsync(int selectedNetwork)
        {
            if (NetworkChanged is not null)
                await NetworkChanged.Invoke(selectedNetwork);
        }
        
        public async Task OnMetamaskConnected(int chainId)
        {
            if (ProviderConnected is not null)
                await ProviderConnected.Invoke(chainId);
        }
        
        public async Task OnMetamaskDisconnected()
        {
            if (ProviderDisconnected is not null)
                await ProviderDisconnected.Invoke();
        }

        public ValueTask<string> SignMessageAsync(string message)
             => _metamaskInterop.SignAsync(message.ToHexUTF8());
    }
}