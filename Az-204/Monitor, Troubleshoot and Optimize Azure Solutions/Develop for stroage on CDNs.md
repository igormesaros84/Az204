# Table of content
- [Table of content](#table-of-content)
- [Develop for storage on CDNs](#develop-for-storage-on-cdns)
- [Explore Azure Content Delivery Networks](#explore-azure-content-delivery-networks)
  - [How it works](#how-it-works)
  - [Requirements](#requirements)
  - [Limitations](#limitations)
- [Control cache behavior on Azure Content Delivery Networks](#control-cache-behavior-on-azure-content-delivery-networks)
  - [Controlling caching behavior](#controlling-caching-behavior)
  - [Caching and time to live (TTL)](#caching-and-time-to-live-ttl)
  - [Content updating](#content-updating)
  - [Geo-filtering](#geo-filtering)
- [Interact with Azure Content Delivery Networks by using .NET](#interact-with-azure-content-delivery-networks-by-using-net)
  - [Create a CDN client](#create-a-cdn-client)
  - [List CDN profiles and endpoints](#list-cdn-profiles-and-endpoints)
  - [Create CDN profiles and endpoints](#create-cdn-profiles-and-endpoints)
  - [Purge an endpoint](#purge-an-endpoint)
# Develop for storage on CDNs
A content delivery network (CDN) is a distributed network of servers that can efficiently deliver web content to users. CDNs' store cached content on edge servers in point-of-presence (POP) locations that are close to end users, to minimize latency.

# Explore Azure Content Delivery Networks
Azure Content Delivery Network (CDN) offers developers a global solution for rapidly delivering high-bandwidth content to users by caching their content at strategically placed physical nodes across the world. 
Benefits:
- Better performance and improved experience for end users.
- Large scaling to better handle instantaneous high loads
- Distribution of user requests and servicing of content directly from edge servers so that less traffic is sent to the origin server

## How it works
![how-it-works](Resources/azure-content-delivery-network.png)

## Requirements
To use Azure CDN you need to create at least one CDN profile, which is a collection of CDN endpoints. To organize your CDN endpoints by internet domain, web application, or some other criteria, you can use multiple profiles. Because Azure CDN pricing is applied at the CDN profile level, you must create multiple CDN profiles if you want to use a mix of pricing tiers.

## Limitations
- The number of CDN profiles that can be created
- The number of endpoints that can be created in a CDN profile
- The number of custom domains that can be mapped to an endpoint

# Control cache behavior on Azure Content Delivery Networks
To save time and bandwidth consumption, a cached resource is not compared to the version on the origin server every time it is accessed. Instead, as long as a cached resource is considered to be fresh, it is assumed to be the most current version and is sent directly to the client. A cached resource is considered to be fresh when its age is less than the age or period defined by a cache setting.

## Controlling caching behavior
Caching rules in Azure CDN Standard for Microsoft are set at the endpoint level and provide three configuration options. Other tiers provide additional configuration options, which include:
- **Caching rules** - can be either global or custom
- **Query string caching** - Query string caching enables you to configure how Azure CDN responds to a query string.

With the Azure CDN Standard for Microsoft Tier, caching rules are as simple as following three options
- Ignore query strings. This option is the default mode.
- Bypass caching for query strings. Each query request from the client is passed directly to the origin server with no caching.
- Cache every unique URL. Every time a requesting client generates a unique URL, that URL is passed back to the origin server and the response cached with its own TTL. 

## Caching and time to live (TTL)
If you publish a website through Azure CDN, the files on that site are cached until their TTL expires. The Cache-Control header contained in the HTTP response from origin server determines the TTL duration.

Default values:
- Generalized web delivery optimizations: seven days
- Large file optimizations: one day
- Media streaming optimizations: one year

## Content updating
In normal operation, an Azure CDN edge node will serve an asset until its TTL expires.
To ensure that users always receive the latest version of an asset, consider including a version string in the asset URL. This approach causes the CDN to retrieve the new asset immediately.\
Alternatively, you can purge cached content from the edge nodes, which refreshes the content on the next client request. You might purge cached content when publishing a new version of a web app or to replace any out-of-date assets.

You can purge content in several ways.

- On an endpoint by endpoint basis, or all endpoints simultaneously should you want to update everything on your CDN at once.
- Specify a file, by including the path to that file or all assets on the selected endpoint by checking the Purge All checkbox in the Azure portal.
- Based on wildcards (*) or using the root (/).

Purge with CLI:
```
az cdn endpoint purge \
    --content-paths '/css/*' '/js/app.js' \
    --name ContosoEndpoint \
    --profile-name DemoProfile \
    --resource-group ExampleGroup
```

You can also preload assets into an endpoint:
```
az cdn endpoint load \
    --content-paths '/img/*' '/js/module.js' \
    --name ContosoEndpoint \
    --profile-name DemoProfile \
    --resource-group ExampleGroup
```

## Geo-filtering
Geo-filtering enables you to allow or block content in specific countries, based on the country code.

# Interact with Azure Content Delivery Networks by using .NET
You can use the Azure CDN Library for .NET to automate creation and management of CDN profiles and endpoints. Install the [Microsoft.Azure.Management.Cdn.Fluent](https://www.nuget.org/packages/Microsoft.Azure.Management.Cdn.Fluent) directly from the Visual Studio Package Manager console or with the .NET Core CLI.

## Create a CDN client
The example below shows creating a client by using the `CdnManagementClient `class.
```
static void Main(string[] args)
{
    // Create CDN client
    CdnManagementClient cdn = new CdnManagementClient(new TokenCredentials(authResult.AccessToken))
        { SubscriptionId = subscriptionId };
}
```

## List CDN profiles and endpoints

```
private static void ListProfilesAndEndpoints(CdnManagementClient cdn)
{
    // List all the CDN profiles in this resource group
    var profileList = cdn.Profiles.ListByResourceGroup(resourceGroupName);
    foreach (Profile p in profileList)
    {
        Console.WriteLine("CDN profile {0}", p.Name);
        if (p.Name.Equals(profileName, StringComparison.OrdinalIgnoreCase))
        {
            // Hey, that's the name of the CDN profile we want to create!
            profileAlreadyExists = true;
        }

        //List all the CDN endpoints on this CDN profile
        Console.WriteLine("Endpoints:");
        var endpointList = cdn.Endpoints.ListByProfile(p.Name, resourceGroupName);
        foreach (Endpoint e in endpointList)
        {
            Console.WriteLine("-{0} ({1})", e.Name, e.HostName);
            if (e.Name.Equals(endpointName, StringComparison.OrdinalIgnoreCase))
            {
                // The unique endpoint name already exists.
                endpointAlreadyExists = true;
            }
        }
        Console.WriteLine();
    }
}
```

## Create CDN profiles and endpoints
```
private static void CreateCdnProfile(CdnManagementClient cdn)
{
    if (profileAlreadyExists)
    {
        //Check to see if the profile already exists
    }
    else
    {
        //Create the new profile
        ProfileCreateParameters profileParms =
            new ProfileCreateParameters() { Location = resourceLocation, Sku = new Sku(SkuName.StandardVerizon) };
        cdn.Profiles.Create(profileName, profileParms, resourceGroupName);
    }
}
```
Once the profile is created, we'll create an endpoint.
```
private static void CreateCdnEndpoint(CdnManagementClient cdn)
{
    if (endpointAlreadyExists)
    {
        //Check to see if the endpoint already exists
    }
    else
    {
        //Create the new endpoint
        EndpointCreateParameters endpointParms =
            new EndpointCreateParameters()
            {
                Origins = new List<DeepCreatedOrigin>() { new DeepCreatedOrigin("Contoso", "www.contoso.com") },
                IsHttpAllowed = true,
                IsHttpsAllowed = true,
                Location = resourceLocation
            };
        cdn.Endpoints.Create(endpointName, endpointParms, profileName, resourceGroupName);
    }
}
```

## Purge an endpoint
```
private static void PromptPurgeCdnEndpoint(CdnManagementClient cdn)
{
    if (PromptUser(String.Format("Purge CDN endpoint {0}?", endpointName)))
    {
        Console.WriteLine("Purging endpoint. Please wait...");
        cdn.Endpoints.PurgeContent(resourceGroupName, profileName, endpointName, new List<string>() { "/*" });
        Console.WriteLine("Done.");
        Console.WriteLine();
    }
}
```
