window.DotNetReference = null;

window.SetDotnetReference = function (pDotNetReference) {
    window.DotNetReference = pDotNetReference;
};

window.NethereumMetamaskInterop = {
    EnableEthereum: async () => {
        try {
            const accounts = await ethereum.request({method: 'eth_requestAccounts'});
            ethereum.autoRefreshOnNetworkChange = false;
            ethereum.on("accountsChanged",
                function (accounts) {
                    console.debug("account changed JS");
                    window.DotNetReference.invokeMethodAsync('SelectedAccountChanged', accounts[0]);
                });
            ethereum.on("chainChanged",
                function (chainId) {
                    console.debug("chain changed JS");
                    window.DotNetReference.invokeMethodAsync('SelectedNetworkChanged', parseInt(chainId, 16));
                });
            return accounts[0];
        } catch (error) {
            return null;
        }
    },
    IsMetamaskAvailable: () => {
        return Boolean(window.ethereum);
    },
    IsMetamaskConnected: () => {
        return (Boolean(window.ethereum) && Boolean(window.ethereum.selectedAddress) && window.ethereum.selectedAddress != null);
    },
    GetSelectedAddress: () => {
        return ethereum.selectedAddress;
    },
    GetSelectedNetwork: () => {
        return parseInt(ethereum.chainId, 16);
    },
    SwitchChain: async (newChainId) => {
        try {
            await window.ethereum.request({
                method: 'wallet_switchEthereumChain',
                params: [{chainId: '0x' + (+newChainId).toString(16)}],
            });
        } catch (switchError) {
            console.error(switchError);
        }
    },
    Request: async (message) => {
        let parsedMessage = {};
        try {
            parsedMessage = JSON.parse(message);
            const response = await window.ethereum.request(parsedMessage);
            let rpcResonse = {
                jsonrpc: "2.0",
                result: response,
                id: parsedMessage.id,
                error: null
            }
            return JSON.stringify(rpcResonse);
        } catch (e) {
            let rpcResonseError = {
                jsonrpc: "2.0",
                id: parsedMessage.id,
                error: {
                    message: e,
                }
            }
            return JSON.stringify(rpcResonseError);
        }
    },

    Send: async (message) => {
        return new Promise(function (resolve, reject) {
            window.ethereum.send(JSON.parse(message), function (error, result) {
                if (error) console.log(error);
                resolve(JSON.stringify(result));
            });
        });
    },

    Sign: async (label, value) => {
        const from = window.ethereum.selectedAddress;
        const params = [
            {
                type: 'string',
                name: label,
                value: value
            }
        ];
        return await window.ethereum.request({
            method: 'eth_signTypedData',
            params: [params, from]
        });
    }
}