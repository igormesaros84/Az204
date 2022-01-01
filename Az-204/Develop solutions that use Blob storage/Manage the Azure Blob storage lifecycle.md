# Table of content
- [Table of content](#table-of-content)
- [Explore the Azure Blob storage lifecycle](#explore-the-azure-blob-storage-lifecycle)
  - [Access tiers](#access-tiers)
  - [Manage the data lifecycle](#manage-the-data-lifecycle)
- [Discover Blob storage lifecycle policies](#discover-blob-storage-lifecycle-policies)
- [Implement Blob storage lifecycle policies](#implement-blob-storage-lifecycle-policies)
  - [Azure portal Code View](#azure-portal-code-view)
  - [Azure CLI](#azure-cli)
- [Rehydrate blob data from the archive tier](#rehydrate-blob-data-from-the-archive-tier)
  - [Rehydration priority](#rehydration-priority)
  - [Copy an archived blob to an online tier](#copy-an-archived-blob-to-an-online-tier)
  - [Change a blobs access tier to an online tier](#change-a-blobs-access-tier-to-an-online-tier)
# Explore the Azure Blob storage lifecycle
## Access tiers
Azure storage offers different access tiers, allowing you to store blob object data in the most cost-effective manner. Available access tiers include:

- **Hot**: Optimized for storing data that is accessed frequently.
- **Cool**: Optimized for storing data that is infrequently accessed and stored for at least 30 days.
- **Archive**: Optimized for storing data that is rarely accessed and stored for at least 180 days with flexible latency requirements, on the order of hours.

## Manage the data lifecycle
The lifecycle management policy let you:
- Transition blobs to a cooler storage tier (hot to cool, ot to archive, or cool to archive) to optimize for performance and cost.
- Delete blobs at the end of their lifecycle
- Define rules to be run once per day at the storage account level
- Apply rules to containers or a subset of blobs (using prefixes as filters)

 By adjusting storage tiers in respect to the age of data, you can design the least expensive storage options for your needs

 # Discover Blob storage lifecycle policies
 A lifecycle management policy is a collection of rules in a JSON document.
 ```
 {
  "rules": [
    {
      "name": "rule1",
      "enabled": true,
      "type": "Lifecycle",
      "definition": {...}
    },
    {
      "name": "rule2",
      "type": "Lifecycle",
      "definition": {...}
    }
  ]
}
```

The following sample rule filters the account to run the actions on objects that exist inside container1 and start with foo.

- Tier blob to cool tier 30 days after last modification
- Tier blob to archive tier 90 days after last modification
- Delete blob 2,555 days (seven years) after last modification
- Delete blob snapshots 90 days after snapshot creation

```
{
  "rules": [
    {
      "name": "ruleFoo",
      "enabled": true,
      "type": "Lifecycle",
      "definition": {
        "filters": {
          "blobTypes": [ "blockBlob" ],
          "prefixMatch": [ "container1/foo" ]
        },
        "actions": {
          "baseBlob": {
            "tierToCool": { "daysAfterModificationGreaterThan": 30 },
            "tierToArchive": { "daysAfterModificationGreaterThan": 90 },
            "delete": { "daysAfterModificationGreaterThan": 2555 }
          },
          "snapshot": {
            "delete": { "daysAfterCreationGreaterThan": 90 }
          }
        }
      }
    }
  ]
}
```

> If you define more than one action on the same blob, lifecycle management applies the least expensive action to the blob. For example, action delete is cheaper than action tierToArchive. Action tierToArchive is cheaper than action tierToCool.

# Implement Blob storage lifecycle policies

## Azure portal Code View
1. Sign in to the Azure portal.
2. Select All resources and then select your storage account.
3. Under Blob Service, select Lifecycle management to view or change your rules.
4. Select the **Code view** tab. The following JSON is an example of a policy that can be pasted into the **Code view** tab.

```
{
"rules": [
    {
    "name": "ruleFoo",
    "enabled": true,
    "type": "Lifecycle",
    "definition": {
        "filters": {
        "blobTypes": [ "blockBlob" ],
        "prefixMatch": [ "container1/foo" ]
        },
        "actions": {
        "baseBlob": {
            "tierToCool": { "daysAfterModificationGreaterThan": 30 },
            "tierToArchive": { "daysAfterModificationGreaterThan": 90 },
            "delete": { "daysAfterModificationGreaterThan": 2555 }
        },
        "snapshot": {
            "delete": { "daysAfterCreationGreaterThan": 90 }
        }
        }
    }
    }
]
}
```
## Azure CLI
To add a lifecycle management policy with Azure CLI, write the policy to a JSON file, then call the az storage account management-policy create command to create the policy.

```
az storage account management-policy create \
    --account-name <storage-account> \
    --policy @policy.json \
    --resource-group <resource-group>
```

# Rehydrate blob data from the archive tier
While a blob is in the archive access tier, it's considered to be offline and can't be read or modified. In order to read or modify data in an archived blob, you must first rehydrate the blob to an online tier, either the hot or cool tier.\
There are two options:
- **Copy an archived blob to an online tier**: Copy the blob to a new blob in the hot or cool tier with the [Copy Blob](https://docs.microsoft.com/en-us/rest/api/storageservices/copy-blob) or [Copy Blob from URL](https://docs.microsoft.com/en-us/rest/api/storageservices/copy-blob-from-url) operation. MS recommends this for most scenarios
- **Change a blobs's access tier to an online tier**: You can change the tier to hot or cool using the [Set Blob Tier](https://docs.microsoft.com/en-us/rest/api/storageservices/set-blob-tier)operation.

## Rehydration priority
Can be set with the optional `x-ms-rehydrate-priority` header.
- **Standard priority**: may take up to 15 hours 
- **High priority**: may complete in under 1 hour for objects under 10GB in size.

## Copy an archived blob to an online tier
When you copy an archived blob to a new blob an online tier, the source blob remains unmodified in the archive tier. You must copy to a new blob with a different name or to a different container (within the same storage account).

||Hot tier source|Cool tier source|Archive tier source|
|-|--------------|----------------|-------------------|
**Hot tier destination**|Supported|Supported|Supported within the same account. Required blob rehydration
**Cool tier destination**|Supported|Supported|Supported within the same account. Requires blob rehydration.
**Archive tier destination**|Supported|Supported|Unsupported

## Change a blobs access tier to an online tier
With this operation, you can change the tier of the archived blob to either hot or cool.

> Changing a blob's tier doesn't affect its last modified time. If there is a lifecycle management policy in effect for the storage account, then rehydrating a blob with Set Blob Tier can result in a scenario where the lifecycle policy moves the blob back to the archive tier after rehydration because the last modified time is beyond the threshold set for the policy.
