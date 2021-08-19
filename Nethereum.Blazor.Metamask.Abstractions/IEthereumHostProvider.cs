using System;
using System.Threading.Tasks;

namespace Nethereum.Blazor.Metamask.Abstractions
{
    public interface IEthereumHostProvider
    {
        event Func<string, Task> SelectedAccountChanged;
        event Func<int, Task> NetworkChanged;
        event Func<bool, Task> AvailabilityChanged;
        event Func<bool, Task> EnabledChanged;


        Task SwitchChainAsync(int newChainId);
        ValueTask<bool> IsMetamaskAvailableAsync();
        ValueTask<bool> IsMetamaskConnectedAsync();
        Web3.Web3 GetWeb3();
        ValueTask<string> EnableProviderAsync();
        ValueTask<string> GetSelectedAccountAsync();
        ValueTask<int> GetSelectedNetworkAsync();
        ValueTask<string> SignMessageAsync(string message);
    }
}