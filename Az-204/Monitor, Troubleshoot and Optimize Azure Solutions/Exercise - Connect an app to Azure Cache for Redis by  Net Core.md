# Exercise: Connect an app to Azure Cache for Redis by using .NET Core
## Create Azure resources
1. Sign in to the portal: https://portal.azure.com and open the Cloud Shell, and select Bash as the shell.

2. Create a resource group for Azure resources. 
```
az group create --name az204-redis-rg --location uksouth
```

3. Create an Azure Cache for Redis instance by using the az redis create command. 
```
redisName=az204redis$RANDOM
az redis create --location uksouth \
    --resource-group az204-redis-rg \
    --name $redisName \
    --sku Basic --vm-size c0
```

4. In the Azure portal navigate to the new Redis Cache you created.

5. Select Access keys in the Settings section of the Navigation Pane and leave the portal open. We'll copy the Primary connection string (StackExchange.Redis) value to use in the app later.

## Create the console application
1. Create a console app by running the command below in the Visual Studio Code terminal.
```
dotnet new console -o Rediscache
```
2. Open the app in Visual Studio Code by selecting File > Open Folder and choosing the folder for the app.

3. Add the StackExchange.Redis package to the project.
```
dotnet add package StackExchange.Redis
```
> NOTE: At the time of writing this I had to use version 2.1.58 for the example bellow to work, as suggested [here](https://github.com/StackExchange/StackExchange.Redis/issues/1120)

4. In the *Program.cs* file paste the following code
```
using StackExchange.Redis;
using System.Threading.Tasks;

namespace RedisCache
{
    public class Program
    {
        // connection string to your Redis Cache
        static string connectionString = "REDIS_CONNECTION_STRING";

        static async Task Main(string[] args)
        {
            // The connection to the Azure Cache for Redis is managed by the ConnectionMultiplexer class.
            using (var cache = ConnectionMultiplexer.Connect(connectionString))
            {
                IDatabase db = cache.GetDatabase();

                // Snippet below executes a PING to test the server connection
                var result = await db.ExecuteAsync("ping");
                Console.WriteLine($"PING = {result.Type} : {result}");

                // Call StringSetAsync on the IDatabase object to set the key "test:key" to the value "100"
                bool setValue = await db.StringSetAsync("test:key", "100");
                Console.WriteLine($"SET: {setValue}");

                // StringGetAsync takes the key to retrieve and return the value
                string getValue = await db.StringGetAsync("test:key");
                Console.WriteLine($"GET: {getValue}");

            }
        }
    }
}
```

5. Cleanup
```
az group delete -n az204-redis-rg --no-wait
```