using Nethereum.Web3;
using System;
using System.Threading.Tasks;

namespace Ethereum.Blazor.Metamask.Abstractions
{
    public interface IEthereumHostProvider
    {
        event Func<string, Task> SelectedAccountChanged;
        event Func<int, Task> NetworkChanged;
        Task SwitchChainAsync(int newChainId);
        ValueTask<bool> IsMetamaskAvailableAsync();
        ValueTask<bool> IsMetamaskConnectedAsync();
        Web3 GetWeb3();
        ValueTask<string> EnableProviderAsync();
        ValueTask<string> GetSelectedAccountAsync();
        ValueTask<int> GetSelectedNetworkAsync();
        ValueTask<string> SignMessageAsync(string message);
    }
}