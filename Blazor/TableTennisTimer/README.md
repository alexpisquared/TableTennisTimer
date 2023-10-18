# 2023-10-18:
From https://learn.microsoft.com/en-us/aspnet/core/blazor/progressive-web-app?view=aspnetcore-7.0&tabs=visual-studio:
dotnet new blazorwasm -o TableTennisTimer --pwa             :used to create the app
dotnet new blazorwasm -o TableTennisTimer --pwa --hosted    :GPT suggested to use this one

Cost analysis: https://portal.azure.com/?feature.quickstart=true#view/Microsoft_Azure_CostManagement/CostAnalysis/scope/%2Fproviders%2FMicrosoft.Billing%2FbillingAccounts%2F179a44a9-a5fa-5ab8-699f-a3851e2c2f86%3Ace93213f-2825-43a7-8e54-2d38f7314a5e_2019-05-31/isAcmContext~/true/viewId/%2Fproviders%2FMicrosoft.Billing%2FbillingAccounts%2F179a44a9-a5fa-5ab8-699f-a3851e2c2f86%3Ace93213f-2825-43a7-8e54-2d38f7314a5e_2019-05-31%2Fproviders%2FMicrosoft.CostManagement%2Fviews%2Fms%3AAccumulatedCosts/openByNewTab~/true

Publish successful: https://tabletennistimer.azurewebsites.net/
- followed/copied AlexPi.API 
- //todo: no PWA icon yet on 