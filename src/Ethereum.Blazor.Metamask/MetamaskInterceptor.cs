using Nethereum.JsonRpc.Client;
using System;
using System.Threading.Tasks;
using Ethereum.Blazor.Metamask.Abstractions;
using Ethereum.Blazor.Metamask.Abstractions.Models;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.JsonRpc.Client.RpcMessages;

namespace Ethereum.Blazor.Metamask
{
    public class MetamaskInterceptor : RequestInterceptor
    {
        private readonly IMetamaskInterop _metamaskInterop;
        public MetamaskInterceptor(IMetamaskInterop metamaskInterop)
        {
            _metamaskInterop = metamaskInterop;
        }

        public override async Task<object> InterceptSendRequestAsync<T>(
            Func<RpcRequest, string, Task<T>> interceptedSendRequestAsync, RpcRequest request,
            string route = null)
        {
            if (request.Method == "eth_sendTransaction")
            {
                var transaction = (TransactionInput)request.RawParameters[0];
                transaction.From = await _metamaskInterop.GetSelectedAddress();
                request.RawParameters[0] = transaction;
                var response = await _metamaskInterop.SendAsync(new MetamaskRpcRequestMessage(request.Id,
                    request.Method, await GetSelectedAccount(),
                    request.RawParameters));
                return ConvertResponse<T>(response);
            }
            else
            {
                var response = await _metamaskInterop.SendAsync(new RpcRequestMessage(request.Id,
                    request.Method,
                    request.RawParameters));
                return ConvertResponse<T>(response);
            }
        }

        public override async Task<object> InterceptSendRequestAsync<T>(
            Func<string, string, object[], Task<T>> interceptedSendRequestAsync, string method,
            string route = null, params object[] paramList)
        {
            var selectedAddress = await GetSelectedAccount();

            if (method == "eth_sendTransaction")
            {
                var transaction = (TransactionInput)paramList[0];
                transaction.From = selectedAddress;
                paramList[0] = transaction;
                var response = await _metamaskInterop.SendAsync(new MetamaskRpcRequestMessage(route, method,
                    selectedAddress,
                    paramList));
                return ConvertResponse<T>(response);
            }
            else
            {
                var response = await _metamaskInterop.SendAsync(new RpcRequestMessage(route, selectedAddress,
                    method,
                    paramList));
                return ConvertResponse<T>(response);
            }
        }

        private void HandleRpcError(RpcResponseMessage response)
        {
            if (response.HasError)
                throw new RpcResponseException(new Nethereum.JsonRpc.Client.RpcError(response.Error.Code, response.Error.Message,
                    response.Error.Data));
        }

        private T ConvertResponse<T>(RpcResponseMessage response,
            string route = null)
        {
            HandleRpcError(response);
            try
            {
                return response.GetResult<T>();
            }
            catch (FormatException formatException)
            {
                throw new RpcResponseFormatException("Invalid format found in RPC response", formatException);
            }
        }

        private ValueTask<string> GetSelectedAccount() => _metamaskInterop.GetSelectedAddress();
    }
}