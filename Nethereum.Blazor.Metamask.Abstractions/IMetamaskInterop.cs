﻿using Nethereum.JsonRpc.Client.RpcMessages;
using System.Threading.Tasks;
using Nethereum.Metamask;

namespace Nethereum.Blazor.Metamask.Abstractions
{
    public interface IMetamaskInterop
    {
        ValueTask<string> EnableEthereumAsync();
        ValueTask<bool> CheckMetamaskAvailability();
        ValueTask<bool> CheckAccountConnected();
        ValueTask<string> GetSelectedAddress();
        ValueTask<int> GetSelectedNetwork();
        
        ValueTask<RpcResponseMessage> SendAsync(RpcRequestMessage rpcRequestMessage);
        ValueTask<RpcResponseMessage> SendTransactionAsync(MetamaskRpcRequestMessage rpcRequestMessage);
        ValueTask<string> SignAsync(string utf8Hex);
        Task SwitchChainAsync(int newChainId);
    }
}