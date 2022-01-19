# Monitor, troubleshoot, and optimize Azure solutions

### **Question**: Ensure that the cost of Application insights does not exceed a pre-set budget
- Implement adaptive sampling using the Application Insights SDK
## Redis cache
### **Question**: Establish connection to the cache
```
IDatabase db = redisConnection.GetDatabase();
```
### **Question**: Invalidate the cache
```
cache.KeyDelete(p_Customer);
```

### **Question**: Store value in cache
```
bool wasSet = db.StringSet("favorite:flavor", "i-love-rocky-road");
```
### **Question**: Get value for cache
```
string value = db.StringGet("favorite:flavor");
```

# Connect to and consume Azure services and third-party services

### **Question**: Create an Azure Service Bus Namespace with PS
- New-AzServiceBusNamespace -ResourceGroupName "whizlabs-rg" -Location "EastUs" -Name "whizlabsnamespace" -SkuName "Standard"

# Develop Azure compute solutions
### **Question**: You have to move blobs from one container to another across storage accounts. You decided to use Azure CLI tool to implement this requirement. Would this fulfil the requirement?
- Yes

### **Question**: How to log only errors for an Azure Web App service?
- `--only-show-errors`

### **Question**: Your app needs to be running all the time. Whats the most effective plan?
- Basic App Service plan
![basic-app-service-plan](Resources/ckeditor_36_54_56.png)
# Develop fo Azure storage
## Cosmos DB
### Consistency levels
- **Strong** - The reads are guaranteed to return the most recent committed version of an item. 
![stron-consistency](Resources/strong-consistency.gif)
- **Bounded staleness** - The reads might lag behind writes by at most "K" versions (that is, "updates") of an item or by "T" time interval, whichever is reached first.
![bounded-staleness](resources/bounded-staleness-consistency.gif)
- **Session** - In session consistency, within a single client session reads are guaranteed to honor the consistent-prefix, monotonic reads, monotonic writes, read-your-writes, and write-follows-reads guarantees. The following graphic illustrates the session consistency with musical notes. The "West US 2 writer" and the "West US 2 reader" are using the same session (Session A) so they both read the same data at the same time. Whereas the "Australia East" region is using "Session B" so, it receives data later but in the same order as the writes.
![session-consistency](Resources/session-consistency.gif)
- **Consistent prefix** - In consistent prefix option, updates that are returned contain some prefix of all the updates, with no gaps. Consistent prefix consistency level guarantees that reads never see out-of-order writes.
![consistent-prefix](Resources/strong-consistency.gif)
- **Eventual** - In eventual consistency, there's no ordering guarantee for reads. 
![eventual](resources/eventual-consistency.gif)

## Blob storage
### **Question** How would you call for implementing a blob lease?
- `comp=lease`