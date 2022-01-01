# Table of content
- [Table of content](#table-of-content)
- [Explore Microsoft .Net SDK v3 for Azure Cosmos DB](#explore-microsoft-net-sdk-v3-for-azure-cosmos-db)
  - [Cosmos client](#cosmos-client)
  - [Database examples](#database-examples)
    - [Create a database](#create-a-database)
    - [Read a database by ID](#read-a-database-by-id)
    - [Delete a database](#delete-a-database)
  - [Container Examples](#container-examples)
    - [Create container](#create-container)
    - [Read item](#read-item)
    - [Query an item](#query-an-item)
  - [Example](#example)
- [Create stored procedures](#create-stored-procedures)
  - [Writing stored procedures](#writing-stored-procedures)
  - [Create an item using stored procedure](#create-an-item-using-stored-procedure)
  - [Arrays as input parameters for stored procedures](#arrays-as-input-parameters-for-stored-procedures)
  - [Bounded execution](#bounded-execution)
  - [Transactions with stored procedures](#transactions-with-stored-procedures)
- [Create triggers and user-defined functions](#create-triggers-and-user-defined-functions)
  - [Pre-triggers](#pre-triggers)
  - [Post-triggers](#post-triggers)
  - [User-defined functions](#user-defined-functions)
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

## Example
Example on how to create resources using .Net can be found [here](https://github.com/igormesaros84/Az204/blob/master/Az-204/Develop%20solutions%20that%20use%20Blob%20storage/Exercise%20-%20Create%20Blob%20storage%20resources%20by%20using%20the%20NET%20client%20library.md).

# Create stored procedures
Azure Cosmos DB provides language-integrated, transactional execution of JavaScript that lets you write stored procedures, triggers, and user-defined functions (UDFs). 
## Writing stored procedures
Stored procedures can create, update, read, query, and delete items inside an Azure Cosmos container. Stored procedures are registered per collection, and can operate on any document or an attachment present in that collection.
Here is a simple stored procedure that returns a "Hello World" response.
```
var helloWorldStoredProc = {
    id: "helloWorld",
    serverScript: function () {
        var context = getContext();
        var response = context.getResponse();

        response.setBody("Hello, World");
    }
}
```
The context object provides access to all operations that can be performed in Azure Cosmos DB, as well as access to the request and response objects.

## Create an item using stored procedure
When you create an item by using stored procedure it is inserted into the Azure Cosmos container and an ID for the newly created item is returned. Creating an item is an asynchronous operation and depends on the JavaScript callback functions. The callback function has two parameters:

- The error object in case the operation fails
- A return value

The following example stored procedure takes an input parameter named documentToCreate and the parameter’s value is the body of a document to be created in the current collection. The callback throws an error if the operation fails. Otherwise, it sets the id of the created document as the body of the response to the client.
```
function createSampleDocument(documentToCreate) {
    var context = getContext();
    var collection = context.getCollection();
    var accepted = collection.createDocument(
        collection.getSelfLink(),
        documentToCreate,
        function (error, documentCreated) {                 
            context.getResponse().setBody(documentCreated.id)
        }
    );
    if (!accepted) return;
}
```

## Arrays as input parameters for stored procedures
You can define a function within your stored procedure to parse the string as an array. The following code shows how to parse a string input parameter as an array:
```
function sample(arr) {
    if (typeof arr === "string") arr = JSON.parse(arr);

    arr.forEach(function(a) {
        // do something here
        console.log(a);
    });
}
```
## Bounded execution
All Azure Cosmos DB operations must complete within a limited amount of time. Stored procedures have a limited amount of time to run on the server. All collection functions return a Boolean value that represents whether that operation will complete or not

## Transactions with stored procedures
You can implement transactions on items within a container by using a stored procedure. JavaScript functions can implement a continuation-based model to batch or resume execution. The continuation value can be any value of your choice and your applications can then use this value to resume a transaction from a new starting point. The diagram below depicts how the transaction continuation model can be used to repeat a server-side function until the function finishes its entire processing workload.
![transaction](Resources/transaction-continuation-model.png)

# Create triggers and user-defined functions
Azure Cosmos DB supports pre-triggers and post-triggers. Pre-triggers are executed before modifying a database item and post-triggers are executed after modifying a database item.
For examples of how to register and call a trigger, see [pre-triggers](https://docs.microsoft.com/en-us/azure/cosmos-db/sql/how-to-use-stored-procedures-triggers-udfs#pre-triggers) and [post-triggers](https://docs.microsoft.com/en-us/azure/cosmos-db/sql/how-to-use-stored-procedures-triggers-udfs#post-triggers).

## Pre-triggers
The following example shows how a pre-trigger is used to validate the properties of an Azure Cosmos item that is being created, it adds a timestamp property to a newly added item if it doesn't contain one.

```
function validateToDoItemTimestamp() {
    var context = getContext();
    var request = context.getRequest();

    // item to be created in the current operation
    var itemToCreate = request.getBody();

    // validate properties
    if (!("timestamp" in itemToCreate)) {
        var ts = new Date();
        itemToCreate["timestamp"] = ts.getTime();
    }

    // update the item that will be created
    request.setBody(itemToCreate);
}
```
When triggers are registered, you can specify the operations that it can run with. This trigger should be created with a `TriggerOperation` value of `TriggerOperation.Create`, which means using the trigger in a replace operation as shown in the following code is not permitted.

## Post-triggers
The following example shows a post-trigger. This trigger queries for the metadata item and updates it with details about the newly create item.

```
function updateMetadata() {
var context = getContext();
var container = context.getCollection();
var response = context.getResponse();

// item that was created
var createdItem = response.getBody();

// query for metadata document
var filterQuery = 'SELECT * FROM root r WHERE r.id = "_metadata"';
var accept = container.queryDocuments(container.getSelfLink(), filterQuery,
    updateMetadataCallback);
if(!accept) throw "Unable to update metadata, abort";

function updateMetadataCallback(err, items, responseOptions) {
    if(err) throw new Error("Error" + err.message);
        if(items.length != 1) throw 'Unable to find metadata document';

        var metadataItem = items[0];

        // update metadata
        metadataItem.createdItems += 1;
        metadataItem.createdNames += " " + createdItem.id;
        var accept = container.replaceDocument(metadataItem._self,
            metadataItem, function(err, itemReplaced) {
                    if(err) throw "Unable to update metadata, abort";
            });
        if(!accept) throw "Unable to update metadata, abort";
        return;
    }
}
```

One thing that is important to note is the transactional execution of triggers in Azure Cosmos DB. The post-trigger runs as part of the same transaction for the underlying item itself. An exception during the post-trigger execution will fail the whole transaction. Anything committed will be rolled back and an exception returned.

## User-defined functions
The following sample creates a UDF to calculate income tax for various income brackets. This user-defined function would then be used inside a query. For the purposes of this example assume there is a container called "Incomes" with properties as follows:
```
{
   "name": "User One",
   "country": "USA",
   "income": 70000
}
```
The following is a function definition to calculate income tax for various income brackets:
```
function tax(income) {

        if(income == undefined)
            throw 'no input';

        if (income < 1000)
            return income * 0.1;
        else if (income < 10000)
            return income * 0.2;
        else
            return income * 0.4;
    }
```

