using Nethereum.Hex.HexConvertors.Extensions;
using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Ethereum.Blazor.Metamask.Abstractions;
using Nethereum.Web3;

namespace Ethereum.Blazor.Metamask
{
    public class MetamaskHostProvider : IEthereumHostProvider
    {
        private readonly IMetamaskInterop _metamaskInterop;
        private readonly MetamaskInterceptor _metamaskInterceptor;

        public event Func<string, Task> SelectedAccountChanged;
        public event Func<int, Task> NetworkChanged;
        public MetamaskHostProvider(IJSRuntime jsRuntime, IMetamaskInterop metamaskInterop,
            MetamaskInterceptor metamaskInterceptor)
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

        public Web3 GetWeb3()
            => new Web3 { Client = { OverridingRequestInterceptor = _metamaskInterceptor } };

        public async ValueTask<string> EnableProviderAsync()
        {
            var selectedAccount = await _metamaskInterop.EnableEthereumAsync();
            
            if (string.IsNullOrEmpty(selectedAccount)) return null;
            
            if (SelectedAccountChanged != null) 
                await SelectedAccountChanged.Invoke(selectedAccount);
            
            return selectedAccount;
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
        
        public ValueTask<string> SignMessageAsync(string label, string message)
            => _metamaskInterop.SignAsync(label, message);
    }
}