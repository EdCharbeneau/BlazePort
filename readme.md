# BlazePort

Are you ready to count down to liftoff? In this session, we test the limits of the .NET stack in an incredible mission to build BlazePort, a futuristic ride share app for space travel. We'll use a mashup of the latest in .NET technologies: leveraging CosmosDB for data persistence, model travel prices with ML.NET (via an Azure Function), and go full-stack with ASP.NET Core and Blazor for an end-to-end C# development experience. Strap yourself in for fast paced demos and hot bits on this journey through the .NET ecosystem.

## Requirements

This app requires a Trial license to Telerik UI for Blazor. Obtain a free trial from [Telerik.com](https://www.telerik.com/blazor-ui)

Once you obtain a free trial, please follow the NuGet Source instructions to [add the Telerik packages.](https://docs.telerik.com/blazor-ui/installation/nuget)

### CosmosDB

This application uses CosmosDB. To initialize, seed, and read/write you must create a CosmosDB account, then apply the settings from your account to [user secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.2&tabs=windows). The application uses the following user secrets settings.

```
{
  "CosmosSettings": {
    "AccountEndpoint": "https://(yourinfo).documents.azure.com:443/",
    "AccountKey": "(yourinfo)",
    "DatabaseName": "BlazePort"
  }
}
```