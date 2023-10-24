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