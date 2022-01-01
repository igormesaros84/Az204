# Table of content
- [Table of content](#table-of-content)
- [Explore Azure Functions development](#explore-azure-functions-development)
- [Create triggers and bindings](#create-triggers-and-bindings)
  - [Trigger and binding definitions](#trigger-and-binding-definitions)
  - [Azure Functions trigger and binding example](#azure-functions-trigger-and-binding-example)
  - [C# script example](#c-script-example)
  - [JavaScript example](#javascript-example)
  - [Class library example](#class-library-example)
# Explore Azure Functions development
The function.json file defines the function's trigger, bindings, and other configuration settings. 

```
{
    "disabled":false,
    "bindings":[
        // ... bindings here
        {
            "type": "bindingType",
            "direction": "in",
            "name": "myParamName",
            // ... more depending on binding
        }
    ]
}
```

The `bindings` property is where you configure both triggers and bindings. Each binding shares a few common settings and some settings which are specific to a particular type of binding. Every binding requires the following settings:

|Property|	Types|	Comments|
|---------|-------|---------|
`type`	|string	|Type of the binding. For example, queueTrigger.
`direction`	|string	|Indicates whether the binding is for receiving data into the function or sending data from the function. For example, in or out.
`name`|	string|	The name that is used for the bound data in the function. For example, myQueue.

# Create triggers and bindings
Triggers are what cause a function to run. A trigger defines how a function is invoked and a function must have exactly one trigger.
Binding to a function is a way of declaratively connecting another resource to the function; 

## Trigger and binding definitions
Triggers and bindings are defined differently depending on the development language.

|Language	|Triggers and bindings are configured by...|
|----------|----------------------------------|
C# class library|	decorating methods and parameters with C# attributes
Java	|decorating methods and parameters with Java annotations
JavaScript/PowerShell/Python/TypeScript	|updating function.json schema

## Azure Functions trigger and binding example
Suppose you want to write a new row to Azure Table storage whenever a new message appears in Azure Queue storage. This scenario can be implemented using an Azure Queue storage trigger and an Azure Table storage output binding.

Here's a function.json file for this scenario.
```
{
  "bindings": [
    {
      "type": "queueTrigger",
      "direction": "in",
      "name": "order",
      "queueName": "myqueue-items",
      "connection": "MY_STORAGE_ACCT_APP_SETTING"
    },
    {
      "type": "table",
      "direction": "out",
      "name": "$return",
      "tableName": "outTable",
      "connection": "MY_TABLE_STORAGE_ACCT_APP_SETTING"
    }
  ]
}
```

The first element in the `bindings` array is the Queue storage trigger. The `type` and `direction` properties identify the trigger. The `name` property identifies the function parameter that receives the queue message content. The name of the queue to monitor is in `queueName`, and the connection string is in the app setting identified by `connection`.

The second element in the `bindings` array is the Azure Table Storage output binding. The `type` and `direction` properties identify the binding. The `name` property specifies how the function provides the new table row, in this case by using the function return value. The name of the table is in `tableName`, and the connection string is in the app setting identified by `connection`.

## C# script example
Here's C# script code that works with this trigger and binding. Notice that the name of the parameter that provides the queue message content is order; this name is required because the name property value in function.json is order.

```
#r "Newtonsoft.Json"

using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

// From an incoming queue message that is a JSON object, add fields and write to Table storage
// The method return value creates a new row in Table Storage
public static Person Run(JObject order, ILogger log)
{
    return new Person() { 
            PartitionKey = "Orders", 
            RowKey = Guid.NewGuid().ToString(),  
            Name = order["Name"].ToString(),
            MobileNumber = order["MobileNumber"].ToString() };  
}

public class Person
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public string Name { get; set; }
    public string MobileNumber { get; set; }
}
```

## JavaScript example
The same function.json file can be used with a JavaScript function:

```
// From an incoming queue message that is a JSON object, add fields and write to Table Storage
// The second parameter to context.done is used as the value for the new row
module.exports = function (context, order) {
    order.PartitionKey = "Orders";
    order.RowKey = generateRandomId(); 

    context.done(null, order);
};

function generateRandomId() {
    return Math.random().toString(36).substring(2, 15) +
        Math.random().toString(36).substring(2, 15);
}
```

## Class library example
In a class library, the same trigger and binding information — queue and table names, storage accounts, function parameters for input and output — is provided by attributes instead of a function.json file. Here's an example:

```
public static class QueueTriggerTableOutput
{
    [FunctionName("QueueTriggerTableOutput")]
    [return: Table("outTable", Connection = "MY_TABLE_STORAGE_ACCT_APP_SETTING")]
    public static Person Run(
        [QueueTrigger("myqueue-items", Connection = "MY_STORAGE_ACCT_APP_SETTING")]JObject order,
        ILogger log)
    {
        return new Person() {
                PartitionKey = "Orders",
                RowKey = Guid.NewGuid().ToString(),
                Name = order["Name"].ToString(),
                MobileNumber = order["MobileNumber"].ToString() };
    }
}

public class Person
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public string Name { get; set; }
    public string MobileNumber { get; set; }
}
```