window.NethereumMetamaskInterop = {
    EnableEthereum: async () => {
        try {
            const accounts = await ethereum.request({ method: 'eth_requestAccounts' });
            ethereum.autoRefreshOnNetworkChange = false;
            ethereum.on("accountsChanged",
                function (accounts) {
                    console.log("JS ACCOUNT CHANGED");
                    DotNet.invokeMethodAsync('Client', 'SelectedAccountChanged', accounts[0]);
                });
            ethereum.on("chainChanged",
                function (networkId) {
                    console.log(`JS NETWORK CHANGED: ${networkId}`);
                    DotNet.invokeMethodAsync('Client', 'SelectedNetworkChanged', networkId.toString());
                });
            return accounts[0];
        } catch (error) {
            console.log("JS EXCEPTION ACCURED");
            window.DotNetExceptionHandler.throw(error)
            return null;
        }
    },
    IsMetamaskAvailable: () => {
        return Boolean(window.ethereum);
    },
    IsMetamaskConnected: () => {
        return (Boolean(window.ethereum) && Boolean(window.selectedAddress));
    },
    GetSelectedAddress: () => {
        return ethereum.selectedAddress;
    },
    GetSelectedNetwork: () => {
        return parseInt(ethereum.chainId, 16);
    },
    SwitchChain: async (newChainId) => {
        console.log("Switching chain");

        try {
            await window.ethereum.request({
                method: 'wallet_switchEthereumChain',
                params: [{ chainId: '0x' + decimalToHexZeroPadded(newChainId) }],
            });
        } catch (switchError) {
            console.log("cannot switch chain")
            console.error(switchError);
        }
    },
    Request: async (message) => {
        let parsedMessage = {};
        try {
            parsedMessage = JSON.parse(message);
            console.log(parsedMessage);
            const response = await ethereum.request(parsedMessage);
            let rpcResonse = {
                jsonrpc: "2.0",
                result: response,
                id: parsedMessage.id,
                error: null
            }
            console.log(rpcResonse);

            return JSON.stringify(rpcResonse);
        } catch (e) {
            let rpcResonseError = {
                jsonrpc: "2.0",
                id: parsedMessage.id,
                error: {
                    message: e,
                }
            }
            window.DotNetExceptionHandler.throw(e);
            return JSON.stringify(rpcResonseError);
        }
    },

    Send: async (message) => {
        return new Promise(function (resolve, reject) {
            console.log(JSON.parse(message));
            ethereum.send(JSON.parse(message), function (error, result) {
                console.log(result);
                console.log(error);
                resolve(JSON.stringify(result));
            });
        });
    },

    Sign: async (utf8HexMsg) => {
        return new Promise(function (resolve, reject) {
            const from = ethereum.selectedAddress;
            const params = [utf8HexMsg, from];
            const method = 'personal_sign';
            ethereum.send({
                method,
                params,
                from,
            }, function (error, result) {
                if (error) {
                    window.DotNetExceptionHandler.throw(error)
                    reject(error);
                } else {
                    console.log(result.result);
                    resolve(JSON.stringify(result.result));
                }
            });
        });
    }

}

function decimalToHexZeroPadded(d) {
    let s = (+d).toString(16);
    if(s.length < 2) {
        s = '0' + s;
    }
    return s;
}