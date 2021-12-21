# Exercise: Create resources by using the Microsoft .NET SDK v3
## Prerequisites
- An Azure account with an active subscription. If you don't already have one, you can sign up for a free trial at https://azure.com/free.
- Visual Studio Code: You can install Visual Studio Code from https://code.visualstudio.com.
- Azure CLI: You can install the Azure CLI from https://docs.microsoft.com/cli/azure/install-azure-cli.
- The .NET Core 3.1 SDK, or .NET 5.0 SDK. You can install from https://dotnet.microsoft.com/download.

## Connecting to Azure
1. Login to azure
```
az login --tenant igorcomp.onmicrosoft.com
```
## Create resources in Azure
1. Create resource group
```
az group create --name az204-cosmos-rg --location uksouth
```
2. Create the Azure Comsos DB account.
```
az cosmosdb create --name im-az204-cosmos --resource-group az204-cosmos-rg 
```
Record the `documentEndpoint` shown in the response.

3. Retrieve the primary key for the account by using the command below. Record the `primaryMasterKey` from the result.
```
az cosmosdb keys list --name im-az204-cosmos --resource-group az204-cosmos-rg
```

## Set up the console application
Now that the needed resources are deployed to Azure the next step is to set up the console application using the same terminal window in Visual Studio Code.
1. Create a folder for the project and change it to the folder.
```
md az204-cosmos
cd az204-cosmos
```
2. Create the .NET console app
```
dotnet new console
```

# Build the console app
## Add packages and using statements
1. Add `Microsoft.Azure.Cosmos` package to the project.
```
dotnet add package Microsoft.Azure.Cosmos
```
2. Add using statements to include `Microsoft.Azure.Cosmos` and to enable async operations.
```
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
```
## Add code to connect to an Azure Cosmos DB account
1. Replace the entire `class Program` with the code snippet below.Be sure to replace the placeholder values for `EndpointUri` and `PrimaryKey` following the directions in the code comments.
```
public class Program
{
    // Replace <documentEndpoint> with the information created earlier
    private static readonly string EndpointUri = "<documentEndpoint>";

    // Set variable to the Primary Key from earlier.
    private static readonly string PrimaryKey = "<your primary key>";

    // The Cosmos client instance
    private CosmosClient cosmosClient;

    // The database we will create
    private Database database;

    // The container we will create.
    private Container container;

    // The names of the database and container we will create
    private string databaseId = "az204Database";
    private string containerId = "az204Container";

    public static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("Beginning operations...\n");
            Program p = new Program();
            await p.CosmosAsync();

        }
        catch (CosmosException de)
        {
            Exception baseException = de.GetBaseException();
            Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e);
        }
        finally
        {
            Console.WriteLine("End of program, press any key to exit.");
            Console.ReadKey();
        }
    }
}
```
2. Below the `Main` method, add a new asynchronous task called `CosmosAsync`, which instantiates our new `CosmosClient`.
```
public async Task CosmosAsync()
{
    // Create a new instance of the Cosmos Client
    this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
}
```

## Create a database
1. Copy and paste the `CreateDatabaseAsync` method after the `CosmosAsync` method.
```
private async Task CreateDatabaseAsync()
{
    // Create a new database using the cosmosClient
    this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
    Console.WriteLine("Created Database: {0}\n", this.database.Id);
}
```
2. Add the code belo at the end of the `CosmosAsync` method, it calls the `CreateDatabaseAsync` method you just added.

## Create a container
1. Copy and paste the `CreateContainerAsync` method below the `CreateDatabaseAsync` method
```
private async Task CreateContainerAsync()
{
    // Create a new container
    this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/LastName");
    Console.WriteLine("Created Container: {0}\n", this.container.Id);
}
```
2. Copy and paste the code below where you instantiated the `CosmosClient` to call the CreateContainer method you just added.
```
// Run the CreateContainerAsync method
await this.CreateContainerAsync();
```

## Run the application
Save your work and, in a terminal in VS Code, run the dotnet run command. The console will display the following messages.

##Clean up Azure resources
You can now safely delete the az204-cosmos-rg resource group from your account by running the command below.
```
az group delete --name az204-cosmos-rg --no-wait
```