# Explore Microsoft .Net SDK v3 for Azure Cosmos DB
## Cosmos client
Creates a new `CosmosClient` with a connection string. `CosmosClient` is thread-safe. Its recommended to maintain a single instance of `CosmosClient` per lifetime of the application which enables efficient connection management and performance.
```
CosmosClient client = new CosmosClient(endpoint, key);
```

## Database examples
### Create a database
The `CosmosClient.CreateDatabaseIfNotExistsAsync` checks if a database exists, and if it doesn't, creates it. Only the database `id` is used to verify if there is an existing database.
```
// An object containing relevant information about the response
DatabaseResponse databaseResponse = await client.CreateDatabaseIfNotExistsAsync(databaseId, 10000);

// A client side reference object that allows additional operations like ReadAsync
Database database = databaseResponse;
```

### Read a database by ID
Reads a database from the Azure Cosmos service as an asynchronous operation.
```
DatabaseResponse readResponse = await database.ReadAsync();
```

### Delete a database
Delete a Database as an asynchronous operation.
```
await database.DeleteAsync();
```

## Container Examples
### Create container
Use the `Container.CreateItemAsync` method to create an item. The method requires a JSON serializable object that must contain an `id` property, and a `partitionKey`.
```
ItemResponse<SalesOrder> response = await container.CreateItemAsync(salesOrder, new PartitionKey(salesOrder.AccountNumber));
```
### Read item
Use the `Container.ReadItemAsync` method to read an item. The method requires type to serialize the item to along with an `id` property, and a `partitionKey`.
```
string id = "[id]";
string accountNumber = "[partition-key]";
ItemResponse<SalesOrder> response = await container.ReadItemAsync(id, new PartitionKey(accountNumber));
```
### Query an item
The `Container.GetItemQueryIterator` method creates a query for items under a container in an Azure Cosmos database using a SQL statement with parameterized values. It returns a `FeedIterator`.
```
QueryDefinition query = new QueryDefinition(
    "select * from sales s where s.AccountNumber = @AccountInput ")
    .WithParameter("@AccountInput", "Account1");

FeedIterator<SalesOrder> resultSet = container.GetItemQueryIterator<SalesOrder>(
    query,
    requestOptions: new QueryRequestOptions()
    {
        PartitionKey = new PartitionKey("Account1"),
        MaxItemCount = 1
    });
```

