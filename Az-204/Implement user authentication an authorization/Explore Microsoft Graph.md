- [Discover Microsoft Graph](#discover-microsoft-graph)
- [Query Microsoft Graph by using REST](#query-microsoft-graph-by-using-rest)
  - [Call a REST API method](#call-a-rest-api-method)
  - [Resource](#resource)
  - [Query parameters](#query-parameters)
- [Query Microsoft Graph by using SDKs](#query-microsoft-graph-by-using-sdks)
  - [Install the Microsoft Graph .NET SDK](#install-the-microsoft-graph-net-sdk)
  - [Create a Microsoft Graph client](#create-a-microsoft-graph-client)
  - [Read information from Microsoft Graph](#read-information-from-microsoft-graph)
  - [Retrieve a list of entities](#retrieve-a-list-of-entities)
  - [Delete an entity](#delete-an-entity)
  - [Create a new entity](#create-a-new-entity)
- [Apply best practices to Microsoft Graph](#apply-best-practices-to-microsoft-graph)
  - [Authentication](#authentication)
  - [Consent and authorization](#consent-and-authorization)
  - [Handle responses effectively](#handle-responses-effectively)
  - [Storing data locally](#storing-data-locally)

# Discover Microsoft Graph
Microsoft Graph is the gateway to data and intelligence in Microsoft 365. 

![microsoft-graph](Resources/microsoft-graph-data-connectors.png)

**The Microsoft Graph API** offers a single endpoint, https://graph.microsoft.com. You can use REST APIs or SDKs to access the endpoint. Microsoft Graph also includes a powerful set of services that manage user and device identity, access, compliance, security, and help protect organizations from data leakage or loss.

**Microsoft Graph connectors** deliver data external to the Microsoft cloud into Microsoft Graph services and applications.

**Microsoft Graph Data Connect** provides tools for scalable deliver of MS Graph.

# Query Microsoft Graph by using REST
After registration you can get authentication tokens for a user or service, this token can be used to make requests to the Graph API.

## Call a REST API method

```
{HTTP method} https://graph.microsoft.com/{version}/{resource}?{query-parameters}
```

The components of a request include:

- `{HTTP method}` - The HTTP method used on the request to Microsoft Graph (GET, POST, PATCH, PUT, DELETE).
- `{version}` - The version of the Microsoft Graph API your application is using.
- `{resource}` - The resource in Microsoft Graph that you're referencing.
- `{query-parameters}` - Optional OData query options or REST method parameters that customize the response.

The response will contain the usual HTTP Status code and Response Message, and `nextLink` which if the request returns a lot of data, you will need to page through it by using the url returned in `@odata.nextLink`

## Resource 
A resource can be an entity or complex type, commonly defined with properties. \
Your URL will include the resource you are interacting with in the request, such as me, user, group, drive, and site. You can also interact with resources using methods; for example, to send an email, use `me/sendMail`.

## Query parameters
For example, adding the following `filter` parameter restricts the messages returned to only those with the `emailAddress` property of `jon@contoso.com.`

```
GET https://graph.microsoft.com/v1.0/me/messages?filter=emailAddress eq 'jon@contoso.com'
```

# Query Microsoft Graph by using SDKs
The core library provides a set of features that enhance working with all the Microsoft Graph services. Embedded support for retry handling, secure redirects, transparent authentication, and payload compression, improve the quality of your application's interactions with Microsoft Graph, with no added complexity, while leaving you completely in control. \
In this unit you will learn about the available SDKs and see some code examples of some of the most common operations.

## Install the Microsoft Graph .NET SDK
- [Microsoft.Graph](https://github.com/microsoftgraph/msgraph-sdk-dotnet) - Contains the models and request builders for accessing the `v1.0` endpoint with the fluent API. Microsoft.Graph has a dependency on Microsoft.Graph.Core.
- [Microsoft.Graph.Beta](https://github.com/microsoftgraph/msgraph-beta-sdk-dotnet) - Contains the models and request builders for accessing the beta endpoint with the fluent API.
- [Microsoft.Graph.Core](https://github.com/microsoftgraph/msgraph-sdk-dotnet) - The core library for making calls to Microsoft Graph.
- [Microsoft.Graph.Auth](https://github.com/microsoftgraph/msgraph-sdk-dotnet-auth) Provides an authentication scenario-based wrapper of the Microsoft Authentication Library (MSAL) for use with the Microsoft Graph SDK. 

## Create a Microsoft Graph client
You can use a single client instance for the lifetime of the application. 

```
// Build a client application.
IPublicClientApplication publicClientApplication = PublicClientApplicationBuilder
            .Create("INSERT-CLIENT-APP-ID")
            .Build();
// Create an authentication provider by passing in a client application and graph scopes.
DeviceCodeProvider authProvider = new DeviceCodeProvider(publicClientApplication, graphScopes);
// Create a new instance of GraphServiceClient with the authentication provider.
GraphServiceClient graphClient = new GraphServiceClient(authProvider);
```

## Read information from Microsoft Graph
```
// GET https://graph.microsoft.com/v1.0/me

var user = await graphClient.Me
    .Request()
    .GetAsync();
```

## Retrieve a list of entities
```
// GET https://graph.microsoft.com/v1.0/me/messages?$select=subject,sender&$filter=<some condition>&orderBy=receivedDateTime

var messages = await graphClient.Me.Messages
    .Request()
    .Select(m => new {
        m.Subject,
        m.Sender
    })
    .Filter("<filter condition>")
    .OrderBy("receivedDateTime")
    .GetAsync();
```

## Delete an entity
```
// DELETE https://graph.microsoft.com/v1.0/me/messages/{message-id}

string messageId = "AQMkAGUy...";
var message = await graphClient.Me.Messages[messageId]
    .Request()
    .DeleteAsync();
```

## Create a new entity
```
// POST https://graph.microsoft.com/v1.0/me/calendars

var calendar = new Calendar
{
    Name = "Volunteer"
};

var newCalendar = await graphClient.Me.Calendars
    .Request()
    .AddAsync(calendar);
```

# Apply best practices to Microsoft Graph
## Authentication
To access the data in Microsoft Graph, your application will need to acquire an OAuth 2.0 access token, and present it to Microsoft Graph in either of the following:

- The HTTP *Authorization* request header, as a *Bearer* token
- The graph client constructor, when using a Microsoft Graph client library

Use the Microsoft Authentication Library API, MSAL to acquire the access token to Microsoft Graph.

## Consent and authorization
- **Use least privilege**. Only request permissions that are absolutely necessary, and only when you need them. 
- **Use the correct permission type based on scenarios**. If you're building an interactive application where a signed in user is present, your application should use delegated permissions. If, however, your application runs without a signed-in user, such as a background service or daemon, your application should use application permissions.
- **Consider the end user and admin experience**. This will directly affect end user and admin experiences. 
- **Consider multi-tenant applications**. Expect customers to have various application and consent controls in different states. 
  - Tenant administrators can disable the ability for end users to consent to applications. In this case, an administrator would need to consent on behalf of their users.
  - Tenant administrators can set custom authorization policies such as blocking users from reading other user's profiles, or limiting self-service group creation to a limited set of users.

## Handle responses effectively
Your applications should be prepared to handle different types of responses. 

- **Pagination** - Your application should always handle the possibility that the responses are paged in nature, and use the `@odata.nextLink`. The final page will not contain an `@odata.nextLink` property.
- **Evolvable enumerations** - By default, a GET operation returns only known members for properties of evolvable enum types and your application needs to handle only the known members. If you design your application to handle unknown members as well, you can opt-in to receive those members by using an HTTP `Prefer` request header.

## Storing data locally
You should only cache or store data locally if necessary for a specific scenario, and if that use case is covered by your terms of use and privacy policy, and does not violate the [Microsoft APIs Terms of Use](https://docs.microsoft.com/en-us/legal/microsoft-apis/terms-of-use?context=/graph/context). 