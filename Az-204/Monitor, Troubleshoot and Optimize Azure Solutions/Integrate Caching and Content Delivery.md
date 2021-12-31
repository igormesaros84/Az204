# Table of contents
- [Develop for Azure Cache for Redis](#Develop-for-Azure-Cache-for-Redis)
    - [Key scenarios](#Key-scenarios)
    - [Service tiers](#Service-tiers)
    - [Virtual Network support](#Virtual-Network-support)
    - [Clustering support](#Clustering-support)
    - [Accessing the Redis instance](#Accessing-the-Redis-instance)
    - [Adding an expiration time to values](#Adding-an-expiration-time-to-values)
- [Interact with Azure Cache for Redis by using .NET](#Interact-with-Azure-Cache-for-Redis-by-using-.NET)
-[Executing other commands](#Executing-other-commands)

# Develop for Azure Cache for Redis 
Caching is done by temporarily copying frequently accessed data to fast storage that's located close to the application. If this fast data storage is located closer to the application than the original source, then caching can significantly improve response times for client applications by service data more quickly. Redis improves the performance and scalability of an application that uses backend data stored heavily. It keeps frequently accessed data in the server memory.

## Key scenarios
| Pattern | Description |
|---------|-------------|
Data cache | It's common to use the [cache-aside](https://docs.microsoft.com/en-us/azure/architecture/patterns/cache-aside) pattern to load data into the cache only as needed.
Content cache | Static items such as headers, footers and banners can be stored in an in-memory cache for quick access.
Session store |This pattern is commonly used with shopping carts and other user history data that a web application might associate with user cookies.
Job and message queueing | Longer running operations are queued to be processed in sequence, often by another server. This method of deferring work is called task queuing.
Distributed transactions | Azure Cache for Redis supports executing a batch of commands as a single transaction.

## Service tiers
| Tier | Description |
|------|-------------|
Basic | An OSS Redis cache running on a single VM. This tier has no service-level agreement (SLA)
Standard | An OSS Redis cache running on two VMs in a replicated configuration.
Premium | High-performance OSS Redis caches. This tier offers higher throughput, lower latency, better availability, and more features. Premium caches are deployed on more powerful VMs compared to the VMs for Basic or Standard caches.
Enterprise | High-performance caches powered by Redis Labs' Redis Enterprise software. This tier supports Redis modules including RediSearch, RedisBloom, and RedisTimeSeries. Also, it offers even higher availability than the Premium tier.
Enterprise Flash | Cost-effective large caches powered by Redis Labs' Redis Enterprise software. This tier extends Redis data storage to non-volatile memory, which is cheaper than DRAM, on a VM. It reduces the overall per-GB memory cost.

## Virtual Network support
If you create a premium tier Redis cache, you can deploy it to a virtual network in the cloud. Your cache will be available to only other virtual machines and applications in the same virtual network. This provides a higher level of security when your service and cache are both hosted in Azure, or are connected through an Azure virtual network VPN.

## Clustering support
With a premium tier Redis cache, you can implement clustering to automatically split your dataset among multiple nodes. To implement clustering, you specify the number of shards to a maximum of 10. The cost incurred is the cost of the original node, multiplied by the number of shards.

## Accessing the Redis instance
Redis has a command-line tool for interacting with an Azure Cache for Redis as a client. 
| Command | Description |
|---------|-------------|
`ping` | Ping the server. Returns "PONG"
`set [key] [value]` | Sets a key/value in teh cache. Returns "OK"
`get [key]` | Gets a value from the cache
`exists [key]` | Returns '1' if the key exists in the cache, '0' if it doesn't
`type [key]` | Returns type associated with the value given
`incr [key]` | Increment the given value by 1. Returns new value
`incrby [key] [amount]` | Increment the given value by a specific amount
`del [key]` | Deletes the value associated with the key
`flushdb` | Delete all keys and values in the database

Example:
```
> set somekey somevalue
OK
> get somekey
"somevalue"
> exists somekey
(string) 1
> del somekey
(string) 1
> exists somekey
(string) 0
```

## Adding an expiration time to values
Caching is important because it allows us to store commonly used values in memory. However, we also need a way to expire values when they are stale. In Redis this is done by applying a time to live (TTL) to a key.

Here is an example of an expiration:
```
> set counter 100
OK
> expire counter 5
(integer) 1
> get counter
100
... wait ...
> get counter
(nil)
```

# Interact with Azure Cache for Redis by using .NET
## Executing commands on the Redis cache
A popular high-performance Redis client for the .NET language is StackExchange.Redis. The package is available through NuGet and can be added to your .NET code using the command line or IDE. Below are examples of how to use the client.

## Creating a connection
The main connection object in StackExchange.Redis is the StackExchange.Redis.ConnectionMultiplexer class. This object abstracts the process of connecting to a Redis server (or group of servers). \
You create a `ConnectionMultiplexer` instance using the static `ConnectionMultiplexer.Connect` or `ConnectionMultiplexer.ConnectAsync` method, passing in either a connection string or a ConfigurationOptions object.

Here's a simple example:
```
using StackExchange.Redis;
...
var connectionString = "[cache-name].redis.cache.windows.net:6380,password=[password-here],ssl=True,abortConnect=False";
var redisConnection = ConnectionMultiplexer.Connect(connectionString);
```

## Accessing a Redis database
The Redis database is represented by the IDatabase type. You can retrieve one using the GetDatabase() method:
```
IDatabase db = redisConnection.GetDatabase();
```
Once you have a IDatabase object, you can execute methods to interact with the cache.

Here is an example of storing a key/value in the cache:
```
bool wasSet = db.StringSet("favorite:flavor", "i-love-rocky-road");
```
The `StringSet` method returns a `bool` indicating whether the value was set (`true`) or not (`false`). We can then retrieve the value with the `StringGet` method:
```
string value = db.StringGet("favorite:flavor");
Console.WriteLine(value); // displays: ""i-love-rocky-road""
```

## Getting and Setting binary values
Recall that Redis keys and values are binary safe. These same methods can be used to store binary data. There are implicit conversion operators to work with byte[] types so you can work with the data naturally:

```
byte[] key = ...;
byte[] value = ...;

db.StringSet(key, value);
```
```
byte[] key = ...;
byte[] value = db.StringGet(key);
```

## Other common operations

| Method | Description |
| -------|-------------|
`CreateBatch` | Creates a group of operations that will be sent to the server as a single unit, but not necessarily processed as a unit
`CreateTransaction` | Creates a group of operations that will be sent to the server as a single unit and processed on the server as a single unit.
`KeyDelete` | Delete the key/value
`KeyExists` | Returns whether the given key exists in cache
`KeyExpire` | Sets a TTL expiration on a key.
`KeyRename` | Renames a key
`KeyTimeToLive` | Returns the TTL for a key
`KeyType` | Returns the string representations of the type of the value stored at key

# Executing other commands
The IDatabase object has an Execute and ExecuteAsync method which can be used to pass textual commands to the Redis server. For example:
```
var result = db.Execute("ping");
Console.WriteLine(result.ToString()); // displays: "PONG"
```

You can use Execute to perform any supported commands - for example, we can get all the clients connected to the cache ("CLIENT LIST"):
```
var result = await db.ExecuteAsync("client", "list");
Console.WriteLine($"Type = {result.Type}\r\nResult = {result}");
```
