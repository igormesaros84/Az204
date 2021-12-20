# Explore Azure Blob storage
Optimized for storing massive amounts of unstructured data.

Used for:
- Serving images or documents directly to a browser.
- Storing files for distributed access.
- Streaming video and audio.
- Writing to log files.
- Storing data for backup and restore, disaster recovery, and archiving.
- Storing data for analysis by an on-premises or Azure-hosted service.

## Types of storage accounts
- Standard: General purpose v2 account and is recommended for most scenarios using Azure Storage.
- Premium: Offer higher performance by using solid-state drives. If you create a premium account you can choose between three account types, block blobs, page blobs, or file shares.

## Access tiers
Available access tiers:
- **Hot**: optimized for frequent access of objects in the storage account. New storage accounts are created in the hot tier by default. Storage is more expensive but access is cheap.
- **Cool**: optimized for storing large amounts of data that is infrequently accessed and stored for at least 30 days. Accessing data is more expensing, but storage is cheap.
- **Archive**: only available for individual block blobs. Optimised for data that can tolerate several hours of retrieval latency and will remain in the archive tier for at least 180 days. Very cost effective for storing data, but accessing data is the most expensive of the three.

## Blob types
- **Block blobs** store binary data up to about 4.7 TB.
- **Append blobs** are made up of blocks like block blobs but are optimized for append operations. Ideal for logging.
- **Page blobs** up to 8 TB size. Store virtual hard drive files and serve as disks.

# Security features
- All data is automatically encrypted using Storage Service Encryption (SSE)
- Azure Active Directory and Role-Based Access Control are both supported.
- Data can be secured in transit between an application and Azure by sing Client-Side Encryption, HTTPS, or SMB 3.0.
- OS and data disks used by Azure virtual machined can be encrypted using Azure Disk Encryption.
- Delegated access to the data objects in Azure Storage can be granted using a shared access signature.

# Evaluate Azure Storage redundancy
## Redundancy in the primary region
Data in az Azure Storage account is always replicated three times in the primary region. Azure Storage offers two options for how your data is replicated in the primary region.
- **Locally redundant storage (LRS)**:Copies data synchronously three times within a single physical location in the primary region. This is the least expensive replication option.
- **Zone-redundant storage (ZRS)**: Copies data synchronously across three Azure availability zones in the primary region. For applications requiring high availability it's recommended to use ZRS and also replicating to a second region.

## Redundancy in a secondary region
The paired secondary region is determined based on the primary region and can't be changed.\
There are two options:
- **Geo-redundant storage (GRS)**:Copies data synchronously three times within a single physical location in the primary region using LRS. It then copies your data asynchronously to a single physical location in the secondary region. Within the secondary region, your data is copied synchronously three times using LRS.
- **Geo-zone-redundant storage(GZRS)**: copies your data synchronously across three Azure availability zones in the primary region using ZRS. It then copies your data asynchronously to a single physical location in the secondary region. Within the secondary region, your data is copied synchronously three times using LRS.

# Exercise: Create a block blob storage account
## Create account by using Azure Cloud Shell
1. Create a new resource group
```
az group create --name az204-blob-rg --location uksouth
```
2. Create the block blob storage account. The Storage account name must adhere to the following guidelines:
- The name must be unique across Azure.
- The name must be between three and 24 characters long.
- The name can include only numbers and lowercase letters.
```
az storage account create --resource-group az204-blob-rg 
--name mystorageaccountname --location uksouth 
--kind BlockBLobStorage --sku Premium_LRS
```

