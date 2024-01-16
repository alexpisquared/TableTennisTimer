# 2023-10-18:
New .Net 8.0 Blazor template with all default settings during project creation + PWA.

# 2023-10-18:
From https://learn.microsoft.com/en-us/aspnet/core/blazor/progressive-web-app?view=aspnetcore-7.0&tabs=visual-studio:
dotnet new blazorwasm -o TableTennisTimer --pwa             :used to create the app
dotnet new blazorwasm -o TableTennisTimer --pwa --hosted    :GPT suggested to use this one

Cost analysis: https://portal.azure.com/?feature.quickstart=true#view/Microsoft_Azure_CostManagement/CostAnalysis/scope/%2Fproviders%2FMicrosoft.Billing%2FbillingAccounts%2F179a44a9-a5fa-5ab8-699f-a3851e2c2f86%3Ace93213f-2825-43a7-8e54-2d38f7314a5e_2019-05-31/isAcmContext~/true/viewId/%2Fproviders%2FMicrosoft.Billing%2FbillingAccounts%2F179a44a9-a5fa-5ab8-699f-a3851e2c2f86%3Ace93213f-2825-43a7-8e54-2d38f7314a5e_2019-05-31%2Fproviders%2FMicrosoft.CostManagement%2Fviews%2Fms%3AAccumulatedCosts/openByNewTab~/true

Publish successful: https://tabletennistimer.azurewebsites.net/
- followed/copied AlexPi.API 
- //todo: no PWA icon yet on 

## manifest.webmanifest:1  Manifest: Line: 1, column: 1, Syntax error:
Reason: Azure takes manifest.webmanifest for an HTML file, not a JSON file.
https://developer.mozilla.org/en-US/docs/Web/Manifest
https://stackoverflow.com/questions/53527972/cant-get-rid-of-missing-manifest-json-error
?? works in incognito mode ... must be an extension that is blocking it.

??? Is it make it a PWA install:
Chrome: You can install any site in chrome using Add a shortcut to a website as an app.
Navigate to the website you want to add as an app.
At the top right, click More.
Click More Tools.
Click Create shortcut.
Enter a name for the shortcut and click Create.
+++
Edge: How to create a desktop shortcut to a website
2) Open the website that you want a short cut to.
3) Open the Edge main Menu, (three dots on far top right)
4) Hover on the "Apps" menu option.
5) Click on the pop-up option to "install this site as a web app".
6) Select "Manage Apps" option.
7) The Web page should now be listed as an app.

 
I had the exact same problem (on an Azure Windows Web Service). I just created a new web.config in the root folder with the following content (or edit the existing, if there is one):
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
  <system.webServer>
    <staticContent>
      <mimeMap fileExtension=".json" mimeType="application/json" />
    </staticContent>
    <modules runAllManagedModulesForAllRequests="true"/>
  </system.webServer>
</configuration>
This adds the mime configuration for json-files.

# 2023-10-24:
Perm denied on iPhone
Test audio button
Silent mode
https://learn.microsoft.com/en-us/aspnet/core/blazor/forms-and-input-components?view=aspnetcore-7.0
https://learn.microsoft.com/en-us/aspnet/core/blazor/forms-and-input-components?view=aspnetcore-8.0#built-in-input-components
https://blazor-university.com/forms/editcontext-fieldidentifiers-and-fieldstate/

# 2023-10-26:
https://whuysentruit.medium.com/blazor-wasm-pwa-adding-a-new-update-available-notification-d9f65c4ad13

# 2023-10-27:
Blazor's JSInterop doesn't have the ability to wait until audio finishes playing. This is because it doesn't have a mechanism for the JavaScript function to locally "await" something like the `ended` event of the HTML Audio API and then return to the C# code. 

However, you could implement a workaround by encapsulating the audio file in a Promise and asynchronously waiting for the "ended" event in JavaScript. Also, you would need to adjust your JS function to return a Promise that is resolved when the audio finishes playing.

Create `wwwroot/js/audio.js` file with the following JavaScript code:

```js
window.audioPlayer = {
    playAudio: function (filePath) {
        return new Promise((resolve, reject) => {
            let audio = new Audio(filePath);
            audio.onended = () => {
                resolve();
            };
            audio.onerror = (e) => {
                reject(e);
            };
            audio.play();
        });
    }
};
```

In `_Host.cshtml` (or `_Host.razor` if you are using a hosted solution), make sure this JavaScript file is referenced after `_framework/blazor.webassembly.js`:

```html
<script src="_framework/blazor.webassembly.js"></script>
<script src="js/audio.js"></script>
```

Then, in your Razor file, you can call this function:

```csharp
await JSRuntime.InvokeVoidAsync("audioPlayer.playAudio", filePath);
```

Now, the `JSRuntime.InvokeVoidAsync` method in your Razor code should wait until the audio finishes playing before proceeding.

# 2023-11-02  WASM + KeyVault:
Direct calls from a Blazor WebAssembly (WASM) application to Azure Key Vault are not recommended due to security concerns1. The browser’s CORS policy would block such requests1.
This is because anyone can access the browser’s debug tools and potentially retrieve sensitive information, such as access keys1. For this reason, CORS is not supported for client-side calls to Azure Key Vault1.
Instead, it’s recommended to create a server-side API that securely communicates with Azure Key Vault. Your Blazor WASM app can then communicate with this API to retrieve the necessary data1. This way, sensitive information like your Client Secret is not exposed in the client-side code2.

It makes no sense for a client browser app to access the key vault. Anyone can get the access keys from the browsers debug tools and access all key vault data. For this reason CORS is not supported.

# 2023-11-03  WASM + Web API:
https://learn.microsoft.com/en-us/aspnet/core/blazor/call-web-api?view=aspnetcore-5.0&pivots=webassembly
https://docs.microsoft.com/en-us/aspnet/core/blazor/call-web-api?view=aspnetcore-5.0&pivots=webassembly
apparently, it works in C:\temp\x\Azure2023\BlazorApp_WASM_Net8_CallWebApi\BlazorApp_WASM_Net8_CallWebApi.Client\Pages\Counter.razor but very flaky (shows the correct response ..followed by 404).
# SOLUTION!!!
WithOrigins at C:\Users\alexp\source\repos\alex-pi\AlexPiApi\Startup.cs is the culprit: must be updated with ORIGINS, which are the callers (alexpi.ca, ttt, etc.)

# 2023-11-16: downloadable ICONs: https://icon-icons.com/download/12566/ICO/512/

# 2024-01-08: Cannot run/debug :cert problem => use publish to develop.  2024-01-16: HEALED!?!?!?!

# 2024-01-15: //todo: flexbox for the timer layout: https://www.youtube.com/watch?v=fYq5PXgSsbE

