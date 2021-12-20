# Explore Azure Blob storage client library

Below are the classes in the Azure.Storage.Blobs namespace and their purpose:

|Class	| Description|
|-------|------------|
`BlobClient` |	The `BlobClient` allows you to manipulate Azure Storage blobs.
`BlobClientOptions` |	Provides the client configuration options for connecting to Azure Blob Storage.
`BlobContainerClient` |	The `BlobContainerClient` allows you to manipulate Azure Storage containers and their blobs.
`BlobServiceClient` |	The `BlobServiceClient` allows you to manipulate Azure Storage service resources and blob containers. The storage account provides the top-level namespace for the Blob service.
`BlobUriBuilder` |	The `BlobUriBuilder` class provides a convenient way to modify the contents of a Uri instance to point to different Azure Storage resources like an account, container, or blob.

# Manage container properties and metadata by using .NET
## Retrieve container properties

To retrieve container properties, call one of the following methods of the `BlobConainerClient` class:

- `GetProperties`
- `GetPropertiesAsync`

The following code example fetches a container's system properties and writes some property values to a console window:

```
private static async Task ReadContainerPropertiesAsync(BlobContainerClient container)
{
    try
    {
        // Fetch some container properties and write out their values.
        var properties = await container.GetPropertiesAsync();
        Console.WriteLine($"Properties for container {container.Uri}");
        Console.WriteLine($"Public access level: {properties.Value.PublicAccess}");
        Console.WriteLine($"Last modified time in UTC: {properties.Value.LastModified}");
    }
    catch (RequestFailedException e)
    {
        Console.WriteLine($"HTTP error code {e.Status}: {e.ErrorCode}");
        Console.WriteLine(e.Message);
        Console.ReadLine();
    }
}
```

## Set and retrieve metadata
o set metadata, add name-value pairs to an `IDictionary` object, and then call one of the following methods of the `BlobContainerClient `class to write the values:

- `SetMetadata`
- `SetMetadataAsync`

The following code example sets metadata on a container.

```
public static async Task AddContainerMetadataAsync(BlobContainerClient container)
{
    try
    {
        IDictionary<string, string> metadata =
           new Dictionary<string, string>();

        // Add some metadata to the container.
        metadata.Add("docType", "textDocuments");
        metadata.Add("category", "guidance");

        // Set the container's metadata.
        await container.SetMetadataAsync(metadata);
    }
    catch (RequestFailedException e)
    {
        Console.WriteLine($"HTTP error code {e.Status}: {e.ErrorCode}");
        Console.WriteLine(e.Message);
        Console.ReadLine();
    }
}
```

The `GetProperties` and `GetPropertiesAsync` methods are used to retrieve metadata in addition to properties as shown earlier.

The following code example retrieves metadata from a container.
```
public static async Task ReadContainerMetadataAsync(BlobContainerClient container)
{
    try
    {
        var properties = await container.GetPropertiesAsync();

        // Enumerate the container's metadata.
        Console.WriteLine("Container metadata:");
        foreach (var metadataItem in properties.Value.Metadata)
        {
            Console.WriteLine($"\tKey: {metadataItem.Key}");
            Console.WriteLine($"\tValue: {metadataItem.Value}");
        }
    }
    catch (RequestFailedException e)
    {
        Console.WriteLine($"HTTP error code {e.Status}: {e.ErrorCode}");
        Console.WriteLine(e.Message);
        Console.ReadLine();
    }
}
```

# Set and retriev properties and metadata from blob resources by using REST
## Metadata header formate
```
x-ms-meta-name:string-value
```
Names are case-insensitive. Note that metadata names preserve the case with which they were created, but are case-insensitive when set or read. 

## Standard HTTP properties for containers and blobs
The standard HTTP headers supported on containers include:

- `ETag`
- `Last-Modified`

The standard HTTP headers supported on blobs include:

- `ETag`
- `Last-Modified`
- `Content-Length`
- `Content-Type`
- `Content-MD5`
- `Content-Encoding`
- `Content-Language`
- `Cache-Control`
- `Origin`
- `Range`