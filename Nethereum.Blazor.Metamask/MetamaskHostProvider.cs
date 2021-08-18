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

        public static MetamaskHostProvider Current { get; private set; }
        
        public string Name { get; } = "Metamask";
        public bool Available { get; private set; }
        public bool Connected { get; private set; }
        public string SelectedAccount { get; private set; }
        public int SelectedNetwork { get; private set; }
        public bool Enabled { get; private set; }

        public event Func<bool, Task> AvailabilityChanged;
        public event Func<string, Task> SelectedAccountChanged;
        public event Func<int, Task> NetworkChanged;
        public event Func<bool, Task> EnabledChanged;

        public MetamaskHostProvider(IJSRuntime jsRuntime, IMetamaskInterop metamaskInterop)
        {
            _metamaskInterop = metamaskInterop;
            _metamaskInterceptor = new MetamaskInterceptor(_metamaskInterop, this);
            Current = this;
        }

        public async Task SwitchChainAsync(int newChainId)
        {
            await _metamaskInterop.SwitchChainAsync(newChainId);
        }

        public async Task<bool> CheckProviderAvailabilityAsync()
        {
            var result = await _metamaskInterop.CheckMetamaskAvailability();
            await ChangeMetamaskAvailableAsync(result);
            return result;
        }

        public async Task<bool> CheckProviderConnectedAsync()
        {
            Connected = await _metamaskInterop.CheckAccountConnected();
            if (Connected)
                SelectedAccount = await _metamaskInterop.GetSelectedAddress();
            return Connected;
        }

        public Task<Web3.Web3> GetWeb3Async()
        {
            var web3 = new Web3.Web3 {Client = {OverridingRequestInterceptor = _metamaskInterceptor}};
            return Task.FromResult(web3);
        }

        public async Task<string> EnableProviderAsync()
        {
            var selectedAccount = await _metamaskInterop.EnableEthereumAsync();
            Enabled = !string.IsNullOrEmpty(selectedAccount);

            if (Enabled)
            {
                SelectedAccount = selectedAccount;
                if (SelectedAccountChanged != null)
                {
                    await SelectedAccountChanged.Invoke(selectedAccount);
                }

                return selectedAccount;
            }

            return null;
        }

        public async Task<string> GetProviderSelectedAccountAsync()
        {
            var result = await _metamaskInterop.GetSelectedAddress();
            await ChangeSelectedAccountAsync(result);
            return result;
        }

        public async Task<int> GetProviderSelectedNetworkAsync()
        {
            var result = await _metamaskInterop.GetSelectedNetwork();
            await ChangeSelectedNetworkAsync(result);
            return result;
        }

        

        public async Task ChangeSelectedAccountAsync(string selectedAccount)
        {
            if (SelectedAccount != selectedAccount)
            {
                SelectedAccount = selectedAccount;
                if (SelectedAccountChanged != null)
                {
                    await SelectedAccountChanged.Invoke(selectedAccount);
                }
            }
        }

        public async Task ChangeSelectedNetworkAsync(int selectedNetwork)
        {
            if (SelectedNetwork == selectedNetwork) return;

            SelectedNetwork = selectedNetwork;

            if (NetworkChanged is not null)
                await NetworkChanged.Invoke(selectedNetwork);
        }

        public async Task ChangeMetamaskAvailableAsync(bool available)
        {
            Available = available;
            if (AvailabilityChanged != null)
            {
                await AvailabilityChanged.Invoke(available);
            }
        }

        public async Task<string> SignMessageAsync(string message)
        {
            return await _metamaskInterop.SignAsync(message.ToHexUTF8());
        }
    }
}