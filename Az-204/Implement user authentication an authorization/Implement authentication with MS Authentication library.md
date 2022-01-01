# Table of content
- [Table of content](#table-of-content)
- [Introduction](#introduction)
- [Explore the Microsoft Authentication Library (MSAL)](#explore-the-microsoft-authentication-library-msal)
- [Initialize client applications](#initialize-client-applications)
  - [Initializing public and confidential client applications from code](#initializing-public-and-confidential-client-applications-from-code)
  - [Builder modifiers](#builder-modifiers)
  - [Modifiers common to public and confidential client applications](#modifiers-common-to-public-and-confidential-client-applications)
  - [Modifiers specific to confidential client applications](#modifiers-specific-to-confidential-client-applications)
# Introduction
The Microsoft Authentication Library (MSAL) enables developer to acquire tokens from the Microsoft identity platform in order to authenticate users and access secured web api's

# Explore the Microsoft Authentication Library (MSAL)
The Microsoft Authentication Library (MSAL) can be used to provide secure access to Microsoft Graph, other Microsoft APIs, third-party web APIs, or your own web API.

# Initialize client applications
With MSAL.NET 3.x, the recommended way to instantiate an application is by using the application builders: `PublicClientApplicationBuilder` and `ConfidentialClientApplicationBuilder`. They offer a powerful mechanism to configure the application either from the code, or from a configuration file, or even by mixing both approaches.\
Before initializing an application, you first need to register it so that your app can be integrated with the Microsoft identity platform. After registration, you may need the following information (which can be found in the Azure portal):

- The client ID
- The identity provider URL and the sing-in audience for your application. Known as authority.
- The tenant ID if you writing a line of business application solely for your organization
-The application secret or certificate if it's a confidential client app

## Initializing public and confidential client applications from code
The following code instantiates a public client application, signing-in users in the MS Azure public cloud, with their work or school accounts, or their personal MS accounts.
```
IPublicClientApplication app = PublicClientApplicationBuilder.Create(clientId).Build();
```
The application is identified with the identity provider by sharing a client secret:
```
string redirectUri = "https://myapp.azurewebsites.net";
IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(clientId)
    .WithClientSecret(clientSecret)
    .WithRedirectUri(redirectUri )
    .Build();
```

## Builder modifiers
In the code snippets using application builders, a number of `.With` methods can be applied as modifiers (for example, `.WithAuthority` and `.WithRedirectUri`).
- `.WithAuthority` modifier - sets the application default authority to an AZ AD authority, with the possibility of choosing the AZ Cloud, the audience, the tenant (tenant ID or domain name), or providing directly the authority URI.
    ```
    var clientApp = PublicClientApplicationBuilder.Create(client_id)
        .WithAuthority(AzureCloudInstance.AzurePublic, tenant_id)
        .Build();
    ```
- `.WithRedirectUri` modifier - overrides the default redirect URI.In the case of public client applications, this will be useful for scenarios which require a broker.
    ```
    var clientApp = PublicClientApplicationBuilder.Create(client_id)
    .WithAuthority(AzureCloudInstance.AzurePublic, tenant_id)
    .WithRedirectUri("http://localhost")
    .Build();
    ```

## Modifiers common to public and confidential client applications
The table below lists some of the modifiers you can set on a public, or client confidential client.

|Modifier|	Description|
|-------|-------------|
`.WithAuthority()` 7 overrides|	Sets the application default authority to an Azure Active Directory authority, with the possibility of choosing the Azure Cloud, the audience, the tenant (tenant ID or domain name), or providing directly the authority URI.
`.WithTenantId(string tenantId)`|	Overrides the tenant ID, or the tenant description.
`.WithClientId(string)`|	Overrides the client ID.
`.WithRedirectUri(string redirectUri)`	|Overrides the default redirect URI. In the case of public client applications, this will be useful for scenarios requiring a broker.
`.WithComponent(string)`	|Sets the name of the library using MSAL.NET (for telemetry reasons).
`.WithDebugLoggingCallback()`|	If called, the application will call Debug.Write simply enabling debugging traces.
`.WithLogging()` |	If called, the application will call a callback with debugging traces.
`.WithTelemetry(TelemetryCallback telemetryCallback)`|	Sets the delegate used to send telemetry.

## Modifiers specific to confidential client applications
The modifiers you can set on a confidential client application builder are:


|Modifier|	Description|
|---------|------------|
`.WithCertificate(X509Certificate2 certificate)`|	Sets the certificate identifying the application with Azure Active Directory.
`.WithClientSecret(string clientSecret)`|	Sets the client secret (app password) identifying the application with Azure Active Directory.