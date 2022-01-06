# Discover Azure message queues 
## Introduction
Azure supports two types of mechanisms: **Service Bus queues** and **Storage queues**.

Service Bus queues are designed to integrate applications or application components that may span multiple communication protocols, data contracts, trust domains, or network environments.

Storage queues allow you to store large numbers of messages.

# Choose a message queue solution
When determining which queuing technology fits the purpose of a given solution, solution architects and developers should consider these recommendations.

## Consider using Service Bus queues
- Your solution needs to receive messages without having to poll the queue
- Require guaranteed FIFO ordered delivery
- Automatic duplicate detection
- Process messages as parallel long-running streams. Each node in this model is consuming streams opposed to messages.
- Transactional behavior and atomicity when sending/receiving messages
- Messages can exceed 64 KB byt won't likely approach the 256 KB limit

## Consider using Storage queues
- Must store over 80 gigabytes of messages in a queue
- Track progress of processing in the queue
- Server side logs of all of the transactions executed against your queues

# Explore Azure Service Bus
 A message is a container decorated with metadata, and contains data. The data can be any kind of information, including structured data encoded with the common formats such as the following ones: JSON, XML, Apache Avro, Plain Text.

## Service Bus tiers
The premium tier of Service Bus Messaging addresses common customer requests around scale, performance, and availability for mission-critical applications.

|Premium|	Standard|
|-------|-----------|
High throughput |	Variable throughput
Predictable performance |	Variable latency
Fixed pricing |	Pay as you go variable pricing
Ability to scale workload up and down |	N/A
Message size up to 1 MB. Support for message payloads up to 100 MB currently exists in preview. |	Message size up to 256 KB

## Compliance with standards and protocols
The primary wire protocol for Service Bus is Advanced Messaging Queueing Protocol (AMQP) 1.0, an open ISO/IEC standard. It allows customers to write applications that work against Service Bus and on-premises brokers such as ActiveMQ or RabbitMQ. 

# Discover Service Bus queues, topics and subscriptions
## Queues
Queues offer First In, First Out (FIFO) message delivery to one or more competing consumers. Only one message consumer receives and processes each message.

A related benefit is **load-leveling**, which enables producers and consumers to send and receive messages at different rates. 

Using queues to intermediate between message producers and consumers provides an inherent loose coupling between the components. Because producers and consumers are not aware of each other.

## Receive modes
### Receive and delete
In this mode, when Service Bus receives the request from the consumer, it marks the message as being consumed and returns it to the consumer application. This mode is the simplest model. It works best for scenarios in which the application can tolerate not processing a message if a failure occurs. For example, consider a scenario in which the consumer issues the receive request and then crashes before processing it. As Service Bus marks the message as being consumed, the application begins consuming messages upon restart. It will miss the message that it consumed before the crash.

### Peek lock
This makes it possible to support applications that can't tolerate missing messages.

1. Finds the next message to be consumed, locks it to prevent other consumers from receiving it, and then, return the message to the application.
2. After the application finishes processing the message, it requests the Service Bus service to complete the second stage of the receive process. Then, the service marks the message as being consumed.

If the application is unable to process the message for some reason, it can request the Service Bus service to abandon the message. If the application fails to process the message before the lock timeout expires, Service Bus unlocks the message and makes it available to be received again.

## Topics and subscriptions
A queue allows processing of a message by a single consumer. In contrast to queues, topics and subscriptions provide a one-to-many form of communication in a publish and subscribe pattern. It's useful for scaling to large numbers of recipients. Each published message is made available to each subscription registered with the topic. Publisher sends a message to a topic and one or more subscribers receive a copy of the message, depending on filter rules set on these subscriptions. 

## Rules and actions
While Service Bus subscriptions see all messages sent to the topic, you can only copy a subset of those messages to the virtual subscription queue. This filtering is accomplished using subscription filters.

Each rule consists of a **filter condition** that selects particular messages, and optionally contain an action that annotates the selected message.

Each rule with an action produces a copy of the message.
Consider the following scenario:

- Subscription has five rules.
- Two rules contain actions.
- Three rules don't contain actions.\

In this example, if you send one message that matches all five rules, you get three messages on the subscription. That's two messages for two rules with actions and one message for three rules without actions.

### Filters
- **SQL Filters** - A SqlFilter holds a SQL-like conditional expression that is evaluated in the broker against the arriving messages' user-defined properties and system properties. 
- **Boolean filters** - The TrueFilter and FalseFilter either cause all arriving messages (true) or none of the arriving messages (false) to be selected for the subscription. These two filters derive from the SQL filter.
- **Correlation Filters** - set of conditions that are matched against one or more of an arriving message's user and system properties. A common use is to match against the CorrelationId property, but the application can also choose to match against the following properties:

  - ContentType
  - Label
  - MessageId
  - ReplyTo
  - ReplyToSessionId
  - SessionId
  - To
  - any user-defined properties.

  A match exists when an arriving message's value for a property is equal to the value specified in the correlation filter

# Explore Service Bus message payloads and serialization
Messages carry a payload and metadata. The metadata is in the form of key-value pair properties, and describes the payload, and gives handling instructions to Service Bus and applications. Occasionally, that metadata alone is sufficient to carry the information that the sender wants to communicate to receivers, and the payload remains empty.

## Message routing and correlation
A subset of the broker properties described previously, specifically `To`, `ReplyTo`, `ReplyToSessionId`, `MessageId`, `CorrelationId`, and `SessionId`, are used to help applications route messages to particular destinations. To illustrate this, consider a few patterns:
- **Simple request/reply**: A publisher sends a message into a queue and expects a reply from the message consumer. When the consumer responds, it copies the `MessageId` of the handled message into the `CorrelationId` property of the reply message and delivers the message to the destination indicated by the `ReplyTo` property. 
- **Multicast request/reply**: As a variation of the prior pattern, a publisher sends the message into a topic and multiple subscribers become eligible to consume the message.
- **Multiplexing**: Messages identified by matching SessionId values, are routed to a specific receiver while the receiver holds the session under lock.
- **Multiplexed request/reply**: his session feature enables multiplexed replies, allowing several publishers to share a reply queue. By setting `ReplyToSessionId`, the publisher can instruct the consumer(s) to copy that value into the `SessionId` property of the reply message.

## Payload serialization
 The receiver can retrieve those objects with the `GetBody<T>()` method, supplying the expected type. With AMQP, the objects are serialized into an AMQP graph of `ArrayList` and `IDictionary<string,object>` objects, and any AMQP client can decode them.



