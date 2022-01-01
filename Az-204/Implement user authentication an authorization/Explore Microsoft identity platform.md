# Table of content
- [Table of content](#table-of-content)
- [Explore the Microsoft identity platform](#explore-the-microsoft-identity-platform)
- [Explore service principals](#explore-service-principals)
  - [Application object](#application-object)
  - [Service principal object](#service-principal-object)
  - [Relationship between application objects and service principals](#relationship-between-application-objects-and-service-principals)
- [Discover permission and consent](#discover-permission-and-consent)
  - [Permission types](#permission-types)
  - [Consent types](#consent-types)
    - [Static user consent](#static-user-consent)
    - [Incremental and dynamic user consent](#incremental-and-dynamic-user-consent)
    - [Admin consent](#admin-consent)
    - [Requesting individual user consent](#requesting-individual-user-consent)
- [Discover conditional access](#discover-conditional-access)
  - [How does Conditional Access impact an app?](#how-does-conditional-access-impact-an-app)
# Explore the Microsoft identity platform
There are a several components that make up the Microsoft identity platform:
- **OAuth 2.0 and OpenId Connect standard-compliant authentication** service enabling developers to authenticate several identity types, including:
    - Work or school accounts, provisioned through Azure Active Directory
    - Personal Microsoft account, like Skype, Xbox and outlook.com
    - Social or local accounts, by using Azyre Active Directory B2C
- Open source libraries: Microsoft Authentication Libraries (MSAL)
- Application management portal
- Application configuration API and PowerShell: Microsoft Graph API and PowerShell so you can automate you DevOps tasks.

# Explore service principals
To delegate Identity an application must be registered with an Azure Active Directory tenant. When you register an app in the Azure portal, you choose whether it is:
- Single tenant
- Multi-tenant

## Application object
An Azure AD application is defined by its one and only application object, which resides in the Azure AD tenant where the application was registered (known as home tenant).
A service principal is created in every tenant where the application is used. 

## Service principal object
The security principal defines the access policy and permissions for the user/application.

There are three types of service principal:
- **Application** - The type of service principal is the local representation, it defines what the app can actually do in the specific tenant, who can access the app, and what resources the app has access to.
- **Managed identity** - Managed identities eliminate the need for developers to manage credentials. Managed identities provide an identity for applications to use when connecting to resources that support Azure Active Directory (Azure AD) authentication.
- **Legacy** - this represents a legacy app, which is an app created before app registrations were introduced.

## Relationship between application objects and service principals
The application object is the *global* representation of your application for use across all tenants, and the service principal is the *local* representation for use in a specific tenant. The application object serves as the template from which common and default properties are *derived* for use in creating corresponding service principal objects.

# Discover permission and consent
The MS identity platform implements the OAuth 2.0 authorization protocol, through which a third party app can access web-hosted resources on behalf of a user.
When a resource's functionality is chunked into small permission sets, third-party apps can be built to request only the permissions that they need to perform their function.\
In OAuth 2.0, these types of permission sets are called scopes. They're also often referred to as *permissions*. In the Micosoft identity platform a permission is represented as a string value.For example, the permission string `https://graph.microsoft.com/Calendars.Read` is used to request permission to read users calendars in Microsoft Graph.

## Permission types
- **Delegated permissions** - The app is delegated with the permission to act as a signed-in user when it makes call to the target resource
- **Application permissions** - are used by apps that run without a signed-in user present, for example apps that run as background services or daemons.

## Consent types
There are four consent types: *static user consent, incremental user consent, dynamic user consent*, and *admin consent*.

### Static user consent
In the static user consent scenario, you must specify all the permission it needs in the app's configuration in the Azure portal. If the user(or administrator, as appropriate) has not granted consent for this app, then Microsoft identity platform will prompt the user to provide consent at this time. It also enables administrators to consent on behalf of all users in the organization.
It presents some possible issues for developer:
- The app needs to request all the permissions it would ever need upon the user's first sing-in. This can lead to a long list of permission that discourages end users from approving the app's access on initial sing-in.
- The app needs to know all the resources it would ever access ahead of time. It is difficult to create apps that could access an arbitrary number of resources.

### Incremental and dynamic user consent
With the Microsoft identity platform endpoint, you can ignore the static permissions defined in the app registration information in the Azure portal and request permissions incrementally instead. You can ask for a minimum set of permissions upfront and request more over time as the customer uses additional app features.\
If the user hasn't yet consented to new scopes added to the request, they'll be prompted to consent only to the new permissions. Incremental, or dynamic consent, only applies to delegated permissions and not to application permissions.

### Admin consent
Admin consent is required when your app needs access to certain high-privilege permissions. Admin consent ensures that administrators have some additional controls before authorizing apps or users to access highly privileged data from the organization.\
Admin consent done on behalf of an organization still requires the static permissions registered for the app.

### Requesting individual user consent
In an OpenID Connect or OAuth 2.0 authorization request, an app can request the permissions it needs by using the scope query parameter. For example, when a user signs in to an app, the app sends a request like the following example. Line breaks are added for legibility.
```
GET https://login.microsoftonline.com/common/oauth2/v2.0/authorize?
client_id=6731de76-14a6-49ae-97bc-6eba6914391e
&response_type=code
&redirect_uri=http%3A%2F%2Flocalhost%2Fmyapp%2F
&response_mode=query
&scope=
https%3A%2F%2Fgraph.microsoft.com%2Fcalendars.read%20
https%3A%2F%2Fgraph.microsoft.com%2Fmail.send
&state=12345
```
The `scope` parameter is a space-separated list of delegated permissions that the app is requesting. Each permission is indicated by appending the permission value to the resource's identifier (the application ID URI). In the request example, the app needs permission to read the user's calendar and send mail as the user.

# Discover conditional access
Conditional Access enables developers and enterprise customers to protect services in a multitude of ways including:

- Multifactor authentication
- Allowing only Intune enrolled devices to access specific services
- Restricting user locations and IP ranges

## How does Conditional Access impact an app?
Specifically, the following scenarios required code to handle Conditional Access challenges:
- Apps performing the on-behalf-of flow
- Apps accessing multiple service/resources
- Single-page apps using MSAL.js
- Web apps calling a resource
