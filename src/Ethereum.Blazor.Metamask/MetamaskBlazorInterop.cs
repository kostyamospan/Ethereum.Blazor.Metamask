using Microsoft.JSInterop;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Ethereum.Blazor.Metamask.Abstractions;
using Ethereum.Blazor.Metamask.Abstractions.Models;

namespace Ethereum.Blazor.Metamask
{
    public class MetamaskBlazorInterop : IMetamaskInterop
    {
        private readonly IJSRuntime _jsRuntime;

        public MetamaskBlazorInterop(IJSRuntime jsRuntime) =>
            _jsRuntime = jsRuntime;

        public ValueTask<string> EnableEthereumAsync()
            => InvokeJsAsync<string>("NethereumMetamaskInterop.EnableEthereum");

        public ValueTask<bool> CheckMetamaskAvailability()
            => InvokeJsAsync<bool>("NethereumMetamaskInterop.IsMetamaskAvailable");

        public ValueTask<bool> CheckAccountConnected()
            => InvokeJsAsync<bool>("NethereumMetamaskInterop.IsMetamaskConnected");

        public async ValueTask<RpcResponseMessage> SendAsync(RpcRequestMessage rpcRequestMessage)
        {
            var response = await InvokeJsAsync<string>("NethereumMetamaskInterop.Request", JsonConvert.SerializeObject(rpcRequestMessage));
            return JsonConvert.DeserializeObject<RpcResponseMessage>(response);
        }

        public async ValueTask<RpcResponseMessage> SendTransactionAsync(MetamaskRpcRequestMessage rpcRequestMessage)
        {
            var response = await InvokeJsAsync<string>("NethereumMetamaskInterop.Request", JsonConvert.SerializeObject(rpcRequestMessage));
            return JsonConvert.DeserializeObject<RpcResponseMessage>(response);
        }

        public async ValueTask<string> SignAsync(string label, string name)
        {
            var result = await InvokeJsAsync<string>("NethereumMetamaskInterop.Sign", label, name);
            return result.Trim('"');
        }

        public Task SwitchChainAsync(int newChainId) =>
            InvokeJsAsync("NethereumMetamaskInterop.SwitchChain", newChainId);

        public ValueTask<string> GetSelectedAddress()
            => InvokeJsAsync<string>("NethereumMetamaskInterop.GetSelectedAddress");

        public ValueTask<int> GetSelectedNetwork()
             => InvokeJsAsync<int>("NethereumMetamaskInterop.GetSelectedNetwork");

        private async ValueTask<TResult> InvokeJsAsync<TResult>(string identifier, params object[] args)
         => await _jsRuntime.InvokeAsync<TResult>(identifier, args);

        private async Task InvokeJsAsync(string identifier, params object[] args)
            => await _jsRuntime.InvokeVoidAsync(identifier, args);
    }
}