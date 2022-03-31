# Table of content
- [Table of content](#table-of-content)
- [Explore Durable Function app patterns](#explore-durable-function-app-patterns)
  - [Application Patterns](#application-patterns)
  - [Function chaining](#function-chaining)
  - [Fan out/fan in](#fan-outfan-in)
  - [Async HTTP APIs](#async-http-apis)
  - [Monitor](#monitor)
  - [Human interaction](#human-interaction)
- [Discover the four function types](#discover-the-four-function-types)
  - [Orchestrator](#orchestrator)
  - [Activity functions](#activity-functions)
  - [Entity functions](#entity-functions)
  - [Client functions](#client-functions)
- [Explore task hubs](#explore-task-hubs)
  - [Task hub Names](#task-hub-names)
- [Explore durable orchestrations](#explore-durable-orchestrations)
  - [Reliability](#reliability)
- [Control timing in Durable Functions](#control-timing-in-durable-functions)
  - [Usage for delay](#usage-for-delay)
  - [Usage for timeout](#usage-for-timeout)
- [Send and wait for events](#send-and-wait-for-events)
# Explore Durable Function app patterns
The *durable functions* extension lets you define stateful workflows by writing *orchestrator functions* and stateful entities by writing entity functions using the Azure Functions programming model. 

## Application Patterns
The primary use case for Durable Functions is simplifying complex, stateful coordination requirements in serverless applications.
- Function chaining
- Fan-out/fan-in
- Async HTTP APIs
- Monitor
- Human interaction

## Function chaining
In the function chaining pattern, a sequence of functions executes in a specific order. In this pattern, the output of one function is applied to the input of another function.
![chaining](Resources/function-chaining.png)

```
[FunctionName("Chaining")]
public static async Task<object> Run(
    [OrchestrationTrigger] IDurableOrchestrationContext context)
{
    try
    {
        var x = await context.CallActivityAsync<object>("F1", null);
        var y = await context.CallActivityAsync<object>("F2", x);
        var z = await context.CallActivityAsync<object>("F3", y);
        return  await context.CallActivityAsync<object>("F4", z);
    }
    catch (Exception)
    {
        // Error handling or compensation goes here.
    }
}
```

## Fan out/fan in
In the fan out/fan in pattern, you execute multiple functions in parallel and then wait for all functions to finish. Often, some aggregation work is done on the results that are returned from the functions.\
![Fan in out](Resources/fan-out-fan-in.png)

In the code example below, the fan-out work is distributed to multiple instances of the `F2` function. The work is tracked by using a dynamic list of tasks. The .NET `Task.WhenAll` API or JavaScript `context.df.Task.all` API is called, to wait for all the called functions to finish. Then, the `F2` function outputs are aggregated from the dynamic task list and passed to the `F3` function.

```
[FunctionName("FanOutFanIn")]
public static async Task Run(
    [OrchestrationTrigger] IDurableOrchestrationContext context)
{
    var parallelTasks = new List<Task<int>>();

    // Get a list of N work items to process in parallel.
    object[] workBatch = await context.CallActivityAsync<object[]>("F1", null);
    for (int i = 0; i < workBatch.Length; i++)
    {
        Task<int> task = context.CallActivityAsync<int>("F2", workBatch[i]);
        parallelTasks.Add(task);
    }

    await Task.WhenAll(parallelTasks);

    // Aggregate all N outputs and send the result to F3.
    int sum = parallelTasks.Sum(t => t.Result);
    await context.CallActivityAsync("F3", sum);
}
```

## Async HTTP APIs
The async HTTP API pattern addresses the problem of coordinating the state of long-running operations with external clients. A common way to implement this pattern is by having an HTTP endpoint trigger the long-running action. Then, redirect the client to a status endpoint that the client polls to learn when the operation is finished.\
![Async](Resources/async-http-api.png)

## Monitor
The monitor pattern refers to a flexible, recurring process in a workflow. An example is polling until specific conditions are met.\
![Monitor](Resources/monitor.png)

## Human interaction
Many automated processes involve some kind of human interaction. Involving humans in an automated process is tricky because people aren't as highly available and as responsive as cloud services. An automated process might allow for this interaction by using timeouts and compensation logic.\
![Human interaction](Resources/human-interaction-pattern.png)

# Discover the four function types
- Orchestrator
- Activity
- Entity
- Client

## Orchestrator
Describe how actions are executed and the order in which actions are executed.

## Activity functions
Activity functions are the basic unit of work in a durable function orchestration. The task involve checking the inventory, charging the customer, and creating a shipment. Each task would be a separate activity function.

## Entity functions
They define operations for reading and updating small pieces of state. We often refer to these stateful entities as durable entities.They are functions with special trigger type, *entity trigger*.

## Client functions
Any non-orchestrator function can be a client function. What makes a function a client function is its use of the *durable client output binding*.

Orchestrator and entity functions cannot be triggered directly using the buttons in the Azure portal. If you want to test an orchestrator or entity function in the Azure portal, you must instead run a client function that starts an orchestrator or entity function as part of its implementation.

# Explore task hubs
A task hub in Durable Functions is a logical container for durable storage resources that are used for orchestrations and entities. Orchestrator, activity and entity functions can only directly interact with each other when they belong to the same task hub. 

If multiple function apps share a storage account, each function app must be configured with a separate tak hub name. A storage account can containe multiple task hubs.

## Task hub Names
The task hub name is declared in the host.json file, as shown in the following example:
```
{
  "version": "2.0",
  "extensions": {
    "durableTask": {
      "hubName": "MyTaskHub"
    }
  }
}
```

# Explore durable orchestrations
You can use an *orchestrator function* to orchestrate the execution of other Durable functions within a function app.

## Reliability
Orchestrator functions reliably maintain their execution state by using the event sourcing design pattern. Instead of directly storing the current state of an orchestration, the Durable Task Framework uses an append-only store to record the full series of actions the function orchestration takes.

When an orchestration function is given more work to do, the orchestrator wakes up and re-executes the entire function from the start to rebuild the local state. During the replay, if the code tries to call a function (or do any other async work), the Durable Task Framework consults the execution history of the current orchestration. If it finds that the activity function has already executed and yielded a result, it replays that function's result and the orchestrator code continues to run. Replay continues until the function code is finished or until it has scheduled new async work.

# Control timing in Durable Functions
Durable Functions provides durable timers for use in orchestrator functions to implement delays or to set up timeouts on async actions. Durable timers should be used in orchestrator functions instead of Thread.Sleep and Task.Delay (C#), or setTimeout() and setInterval() (JavaScript), or time.sleep() (Python).

You create a durable timer by calling the CreateTimer (.NET) method or the createTimer (JavaScript) method of the orchestration trigger binding. 


## Usage for delay
```
[FunctionName("BillingIssuer")]
public static async Task Run(
    [OrchestrationTrigger] IDurableOrchestrationContext context)
{
    for (int i = 0; i < 10; i++)
    {
        DateTime deadline = context.CurrentUtcDateTime.Add(TimeSpan.FromDays(1));
        await context.CreateTimer(deadline, CancellationToken.None);
        await context.CallActivityAsync("SendBillingEvent");
    }
}
```

## Usage for timeout
```
[FunctionName("TryGetQuote")]
public static async Task<bool> Run(
    [OrchestrationTrigger] IDurableOrchestrationContext context)
{
    TimeSpan timeout = TimeSpan.FromSeconds(30);
    DateTime deadline = context.CurrentUtcDateTime.Add(timeout);

    using (var cts = new CancellationTokenSource())
    {
        Task activityTask = context.CallActivityAsync("GetQuote");
        Task timeoutTask = context.CreateTimer(deadline, cts.Token);

        Task winner = await Task.WhenAny(activityTask, timeoutTask);
        if (winner == activityTask)
        {
            // success case
            cts.Cancel();
            return true;
        }
        else
        {
            // timeout case
            return false;
        }
    }
}
```

# Send and wait for events
The following example listens for a specific single event and takes action when it's received.

```
[FunctionName("BudgetApproval")]
public static async Task Run(
    [OrchestrationTrigger] IDurableOrchestrationContext context)
{
    bool approved = await context.WaitForExternalEvent<bool>("Approval");
    if (approved)
    {
        // approval granted - do the approved action
    }
    else
    {
        // approval denied - send a notification
    }
}
```

Below is an example queue-triggered function that sends an "Approval" event to an orchestrator function instance.

```
[FunctionName("ApprovalQueueProcessor")]
public static async Task Run(
    [QueueTrigger("approval-queue")] string instanceId,
    [DurableClient] IDurableOrchestrationClient client)
{
    await client.RaiseEventAsync(instanceId, "Approval", true);
}
```
