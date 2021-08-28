# Ethereum.Blazor.Metamask

This project is based on https://github.com/Nethereum/Nethereum.Metamask.Blazor

# TODO
- Add custom tokens (``ERC20``, ``ERC721``, etc.)
- Make this package a little bit easier to install, by removing js script import statement from index.html (``load script in .net runtime``) 


# How to install

1.Download the package from ``NuGet``
2.Place this script import into header of index.html file
```<script src="_content/Ethereum.Blazor.Metamask/EthereumMetamaskIterop.js"></script>```
3.Register the necessary service in Progra.cs ``builder.Services.AddBlazorMetamask();``
4.Wrap all components with ``<Metamask>...</Metamask>`` in App.razor file

# How to use

****The regular flow of using this package is looks like this****

1.Check for metamask availability
```bool isAvailable = await _ethereumHostProvider.IsMetamaskAvailableAsync();```
If its not avalaible, ask user to install the Metamask

2.Check, if user attached Metamask to your Dapp:
``bool isConnected = await _ethereumHostProvider.IsMetamaskConnectedAsync();``

3.Enable provider. Allways call this method to initially load users selected account:
``string selectedAccount await _ethereumHostProvider.EnableProviderAsync();``
If user previosly didnt attached his Metamask account, calling this method will triger Metamask account selection

4.***Optional***. Subscribe on on ``SelectedAccountChanged`` and ``NetworkChanged`` events to handle user iteraction with Metamask

***AT THAT TIME TESTED ONLY ON BLAZOR WASM***

Check SampleApp to get more of it 😉
