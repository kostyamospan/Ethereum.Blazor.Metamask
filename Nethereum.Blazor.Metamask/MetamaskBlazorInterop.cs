using System;
using Microsoft.JSInterop;
using Nethereum.JsonRpc.Client.RpcMessages;
using Nethereum.Metamask;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Nethereum.Blazor.Metamask.Abstractions;
using Nethereum.Hex.HexTypes;

namespace Nethereum.Blazor.Metamask
{
    public class MetamaskBlazorInterop : IMetamaskInterop
    {
        private readonly IJSRuntime _jsRuntime;

        private static MetamaskHostProvider _hostProvider;
        
        public MetamaskBlazorInterop(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public static void InitBlazorInterop(MetamaskHostProvider hostProvider) =>
            _hostProvider = hostProvider;

        public async ValueTask<string> EnableEthereumAsync()
        {
            return await _jsRuntime.InvokeAsync<string>("NethereumMetamaskInterop.EnableEthereum");
        }

        public async ValueTask<bool> CheckMetamaskAvailability()
        {
            return await _jsRuntime.InvokeAsync<bool>("NethereumMetamaskInterop.IsMetamaskAvailable");
        }

        public async ValueTask<bool> CheckAccountConnected()
        {
            return await _jsRuntime.InvokeAsync<bool>("NethereumMetamaskInterop.IsMetamaskConnected");
        }
        
        

        public async ValueTask<RpcResponseMessage> SendAsync(RpcRequestMessage rpcRequestMessage)
        {
            var response = await _jsRuntime.InvokeAsync<string>("NethereumMetamaskInterop.Request", JsonConvert.SerializeObject(rpcRequestMessage));
            return JsonConvert.DeserializeObject<RpcResponseMessage>(response);
        }

        public async ValueTask<RpcResponseMessage> SendTransactionAsync(MetamaskRpcRequestMessage rpcRequestMessage)
        {
            var response = await _jsRuntime.InvokeAsync<string>("NethereumMetamaskInterop.Request", JsonConvert.SerializeObject(rpcRequestMessage));
            return JsonConvert.DeserializeObject<RpcResponseMessage>(response);
        }

        public async ValueTask<string> SignAsync(string utf8Hex)
        {
            var result = await _jsRuntime.InvokeAsync<string>("NethereumMetamaskInterop.Sign", utf8Hex);
            return result.Trim('"');
        }

        public async Task SwitchChainAsync(int newChainId) =>
            await _jsRuntime.InvokeVoidAsync("NethereumMetamaskInterop.SwitchChain", newChainId);

        public async ValueTask<string> GetSelectedAddress()
        {
            return await _jsRuntime.InvokeAsync<string>("NethereumMetamaskInterop.GetSelectedAddress");
        }
        
        public async ValueTask<int> GetSelectedNetwork()
        {
            return await _jsRuntime.InvokeAsync<int>("NethereumMetamaskInterop.GetSelectedNetwork");
        }

        [JSInvokable()]
        public static async Task MetamaskAvailableChanged(bool available)
        {
            await _hostProvider.ChangeMetamaskAvailableAsync(available);
        }

        [JSInvokable()]
        public static async Task SelectedAccountChanged(string selectedAccount)
        {
            await _hostProvider.ChangeSelectedAccountAsync(selectedAccount);
        }
        
        [JSInvokable()]
        public static async Task SelectedNetworkChanged(string selectedNetwork)

        {
            await _hostProvider.ChangeSelectedNetworkAsync((int)(new HexBigInteger(selectedNetwork).Value));
        }
    }
}