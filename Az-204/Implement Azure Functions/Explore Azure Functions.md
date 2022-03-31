- [Compare Azure Functions and Azure Logic Apps](#compare-azure-functions-and-azure-logic-apps)
- [Compare Functions and WebJobs](#compare-functions-and-webjobs)
- [Compare Azure Functions hosting options](#compare-azure-functions-hosting-options)
- [Scale Azure Functions](#scale-azure-functions)
- [Scaling behaviors](#scaling-behaviors)
# Compare Azure Functions and Azure Logic Apps

Both enable serverless workloads. **Functions** is a serverless compute service, whereas **Azure Logic Apps** provides serverless workflows. Both support orchestrations, which is a collection of functions or steps, called actions in Logic apps.

For **Azure Functions**, you develop orchestrations by writing code and using the *Durable Functions* extension. For **Logic Apps**, you create orchestrations by using a GUI or editing configuration files.

Some key differences:
||Durable Functions | Logic Apps |
|-|-----------------|------------|
|**Development** | Code-first (imperative) | Designer-first (declarative) |
| **Connectivity**| About a dozen built-in binding types, write code for custom bindings | Large collection of connectors, Enterprise Integration Pack for B2B scenarios, build custom connectors |
| **Actions** | Each activity is an Azure function; write code for activity functions | Large collection of ready-made actions |
|**Monitoring** | Azure Application Insights | Azure portal, Azure Monitor logs |
| **Execution context** | Can run locally or in the cloud | Runs only in the cloud |

# Compare Functions and WebJobs
Azure Functions is built on the WebJobs SDK, so it shares many of the same event triggers and connections to other Azure services. Here are some factors to consider when you're choosing between Azure Functions and WebJobs with the WebJobs SDK:

||	Functions |	WebJobs with WebJobs SDK|
|-|-----------|-------------------------|
Serverless app model with automatic scaling	|Yes	|No
Develop and test in browser	|Yes|	No
Pay-per-use pricing	|Yes	|No
Integration with Logic Apps	|Yes	|No
Trigger events | Timer, Azure Storage queues and blobs, Azure Service Bus queues and topics, Azure Cosmos DB, Azure Event Hubs, HTTP/WebHook (GitHub Slack), Azure Event Grid, | Timer , Azure Storage queues and blobs , Azure Service Bus queues and topics, Azure Cosmos DB, Azure Event Hubs, File system

Azure Functions offers more developer productivity than Azure App Service WebJobs does.
# Compare Azure Functions hosting options
|Plan | Benefits |
|-----|---------|
Consumption plan | Default hosting plan. Scales automatically and you only pay for compute resources when your functions are running. 
Functions Premium plan |Automatically scales based on demand using pre-warmed workers which run applications with no delay after being idle, runs on more powerful instances and connector to virtual networks.
App service plan | Run at regular App Service plan rates. Best for long-running scenarios where Durable Functions can't be used.

There are two other hosting options which provide the highest amount of control and isolation in which to run your function apps.

|Hosting option | Details |
|---------------|---------|
**ASE** | [App Service Environment ASE](https://docs.microsoft.com/en-us/azure/app-service/environment/intro) is a fully isolated and dedicate environment for securely running App Service apps at high scale.
**Kubernetes**|Fully isolated and dedicated environment running on top of Kubernetes platform. For more info [Azure Functions on Kubernetes with KEDA](https://docs.microsoft.com/en-us/azure/azure-functions/functions-kubernetes-keda)

# Scale Azure Functions
Each instance of the Functions host in the Consumption plan is limited to 1.5 GB of memory and one CPU. All functions within a function app share resource with an instance and scale at the same time. In the Premium plan, the plan size determines the available memory and CPU for all apps and that plan on that service.

# Scaling behaviors
- **Maximum instances**: A single function app only scales out to maximum of 200 instances.
- **New instance rate**: For HTTP triggers, new instances are allocated, at most once per second. For non-Http triggers, new instances are allocated, at most once every 30 seconds. Scaling is faster when running in a Premium plan.

You can specify a lower maximum for a specific app by modifying the `functionAppScaleLimit` value, it can be set to `0` or `null` for unrestricted.