# Introduction
A shared access signature (SAS) is a URI that grants restricted access rights to Azure Storage resources. You can provide a shared access signature to clients that you want grant delegate access to certain storage accounts resources.

# Discover shared access signatures
A shared access signature (SAS) is a signed URI that points to one or more storage resources and includes a token that contains a special set of query parameters.

## Types o shared access signatures
Azure Storage supports three types of shared access signatures:
- **User delegation SAS**: Applies to Blob storage only
- **Service SAS**: A service SAS delegates access to a resource in the following Azure Storage services: Blob storage, Queue storage, Table storage, or Azure Files.
- **Account SAS**: An account SAS delegates access to resources in one or more of the storage services.

## How shared access signatures work
In a single URI, such as `https://medicalrecords.blob.core.windows.net/patient-images/patient-116139-nq8z7f.jpg?sp=r&st=2020-01-20T11:42:32Z&se=2020-01-20T19:42:32Z&spr=https&sv=2019-02-02&sr=b&sig=SrW1HZ5Nb6MbRzTbXCaPm%2BJiSEn15tC91Y4umMPwVZs%3D`, you can separate the URI from the SAS token as follows:
|URI|	SAS token|
|---|------------|
`https://medicalrecords.blob.core.windows.net/patient-images/patient-116139-nq8z7f.jpg?` |	`sp=r&st=2020-01-20T11:42:32Z&se=2020-01-20T19:42:32Z&spr=https&sv=2019-02-02&sr=b&sig=SrW1HZ5Nb6MbRzTbXCaPm%2BJiSEn15tC91Y4umMPwVZs%3D`

The SAS token itself is made up of several components.

|Component	|Description|
|-----------|-----------|
`sp=r` |	Controls the access rights. The values can be `a` for add, `c` for create, `d` for delete, `l` for list, `r` for read, or `w` for write. This example is read only. The example `sp=acdlrw` grants all the available rights.
`st=2020-01-20T11:42:32Z`|	The date and time when access starts.
`se=2020-01-20T19:42:32Z`|	The date and time when access ends. This example grants eight hours of access.
`sv=2019-02-02`|	The version of the storage API to use.
`sr=b`|	The kind of storage being accessed. In this example, `b` is for blob.
`sig=SrW1HZ5Nb6MbRzTbXCaPm%2BJiSEn15tC91Y4umMPwVZs%3D`|	The cryptographic signature.

# Choose when to use shared access signatures
Use a SAS when you want to provide secure access to resources in your storage account to any client who does not otherwise have permissions to those resources.

# Explore stored access policies
A stored access policy provides an additional level of control over service-level shared access signatures (SAS) on the server side.

The following storage resources support stored access policies:
- Blob containers
- File shares
- Queues
- Tables

## Creating a stored access policy
The access policy for a SAS consists of the start time, expiry time, and permissions for the signature. You can specify all of these parameters on the signature URI and none within the stored access policy; all on the stored access policy and none on the URI; or some combination of the two. However, you cannot specify a given parameter on both the SAS token and the stored access policy.

Below are examples of creating a stored access policy by using C# .NET and the Azure CLI.
```
BlobSignedIdentifier identifier = new BlobSignedIdentifier
{
    Id = "stored access policy identifier",
    AccessPolicy = new BlobAccessPolicy
    {
        ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
        Permissions = "rw"
    }
};

blobContainer.SetAccessPolicy(permissions: new BlobSignedIdentifier[] { identifier });
```

CLI:
```
az storage container policy create \
    --name <stored access policy identifier> \
    --container-name <container name> \
    --start <start time UTC datetime> \
    --expiry <expiry time UTC datetime> \
    --permissions <(a)dd, (c)reate, (d)elete, (l)ist, (r)ead, or (w)rite> \
    --account-key <storage account key> \
    --account-name <storage account name> \
```