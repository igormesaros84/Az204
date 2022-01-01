# Table of content
- [Table of content](#table-of-content)
- [Introduction](#introduction)
- [Explore partitions](#explore-partitions)
  - [Logical partitions](#logical-partitions)
  - [Physical partitions](#physical-partitions)
- [Choose a partition key](#choose-a-partition-key)
  - [Partition key for read heavy containers](#partition-key-for-read-heavy-containers)
  - [Using item ID as the partition key](#using-item-id-as-the-partition-key)
- [Create a synthetic partition key](#create-a-synthetic-partition-key)
  - [Concatenate multiple properties of an item](#concatenate-multiple-properties-of-an-item)
  - [Use a partition key with a random suffix](#use-a-partition-key-with-a-random-suffix)
  - [Use a partition key with pre-calculated suffixes](#use-a-partition-key-with-pre-calculated-suffixes)
# Introduction
Azure Cosmos DB uses partitioning to scale individual containers in a database to meet the performance needs of your application.

# Explore partitions
In partitioning the items in a container are divided into distinct sunsets called *logical partitions*. Logical partitions are formed based on the value of a *partition key* that is associated with each item in a container.

For example, a container holds items. Each item has a unique value for the `UserID` property. If `UserID` serves as the partition key for the items in the container and there are 1,000 unique `UserID` values, 1,000 logical partitions are created for the container.

In addition to a partition key that determines the item's logical partition, each item in a container has an item ID which is unique within a logical partition. Combining the partition key and the item ID creates the item's index, which uniquely identifies the item. Choosing a partition key is an important decision that will affect your application's performance.

## Logical partitions
A logical partition consists of a set of items that have the same partition key. For example, in a container that contains data about food nutrition, all items contain a `foodGroup` property. You can use `foodGroup` as the partition key for the container.

## Physical partitions
Unlike logical partitions, physical partitions are an internal implementation of the system and they are entirely managed by Azure Cosmos DB.
- The number of throughput provisioned (each individual physical partition can provide a throughput of up to 10,000 request units per second). The 10,000 RU/s limit for physical partitions implies that logical partitions also have a 10,000 RU/s limit, as each logical partition is only mapped to one physical partition.

- The total data storage (each individual physical partition can store up to 50GB data).

# Choose a partition key
For example, consider an item `{ "userId" : "Andrew", "worksFor": "Microsoft" }` if you choose "userId" as the partition key, the following are the two partition key components:

- The partition key path (for example: "/userId"). The partition key path accepts alphanumeric and underscore(_) characters. You can also use nested objects by using the standard path notation(/).
- The partition key value (for example: "Andrew"). The partition key value can be of string or numeric types.

For all containers, your partition key should:

- Be a property that has a value which does not change. If a property is your partition key, you can't update that property's value.
- Have a high cardinality. In other words, the property should have a wide range of possible values.
- Spread request unit (RU) consumption and data storage evenly across all logical partitions. This ensures even RU consumption and storage distribution across your physical partitions.

## Partition key for read heavy containers
For large read-heavy containers you might want to choose a partition key that appears frequently as a filter in your queries. Queries can be efficiently routed to only the relevant physical partitions by including the partition key in the filter predicate.
If most of your workload's requests are queries and most of your queries have an equality filter on the same property, this property can be a good partition key choice.

## Using item ID as the partition key
If your container has a property that has a wide range of possible values, it is likely a great partition key choice.

# Create a synthetic partition key
It's the best practice to have a partition key with many distinct values, such as hundreds or thousands. If such a property doesn’t exist in your data, you can construct a *synthetic partition* key. 

## Concatenate multiple properties of an item
You can form a partition key by concatenating multiple property values into a single artificial `partitionKey` property.\
For example, consider the following example document:
```
{
"deviceId": "abc-123",
"date": 2018
}
```
You can concatenate these two values into a synthetic `partitionKey` property that's used as the partition key.
```
{
"deviceId": "abc-123",
"date": 2018,
"partitionKey": "abc-123-2018"
}
```

## Use a partition key with a random suffix
Another possible strategy to distribute the workload more evenly is to append a random number at the end of the partition key value. When you distribute items in this way, you can perform parallel write operations across partitions.

## Use a partition key with pre-calculated suffixes
The random suffix strategy can greatly improve write throughput, but it's difficult to read a specific item. You don't know the suffix value that was used when you wrote the item. To make it easier to read individual items, use the pre-calculated suffixes strategy. Instead of using a random number to distribute the items among the partitions, use a number that is calculated based on something that you want to query.