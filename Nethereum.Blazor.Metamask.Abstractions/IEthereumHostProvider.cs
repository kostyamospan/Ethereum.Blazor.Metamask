using System;
using System.Threading.Tasks;

namespace Nethereum.Blazor.Metamask.Abstractions
{
    public interface IEthereumHostProvider
    {
        string Name { get; }

        bool Available { get; }
        bool Connected { get; }
        string SelectedAccount { get; }
        int SelectedNetwork { get; }
        bool Enabled { get; }

        event Func<string, Task> SelectedAccountChanged;
        event Func<int, Task> NetworkChanged;
        event Func<bool, Task> AvailabilityChanged;
        event Func<bool, Task> EnabledChanged;


        Task SwitchChainAsync(int newChainId);
        Task<bool> CheckProviderAvailabilityAsync();
        Task<bool> CheckProviderConnectedAsync();
        Task<Web3.Web3> GetWeb3Async();
        Task<string> EnableProviderAsync();
        Task<string> GetProviderSelectedAccountAsync();
        Task<int> GetProviderSelectedNetworkAsync();
        Task<string> SignMessageAsync(string message);
    }
}