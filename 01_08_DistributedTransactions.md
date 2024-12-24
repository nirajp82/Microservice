1. [What Are Distributed Transactions?](#what-are-distributed-transactions)
2. [Importance of Distributed Transactions](#importance-of-distributed-transactions)
3. [Key Concepts](#key-concepts)
4. [Challenges of Distributed Transactions](#challenges-of-distributed-transactions)
5. [Approaches to Handling Distributed Transactions](#approaches-to-handling-distributed-transactions)
6. [Practical Considerations](#practical-considerations)
7. [Conclusion](#conclusion)

### Distributed Transactions in Microservices

#### What Are Distributed Transactions?

In a microservices architecture, applications are built as a collection of loosely coupled services, each managing its own data and state. When a business process requires coordination among multiple services (e.g., updating inventory, processing payments, sending notifications), a distributed transaction ensures that either all operations complete successfully, or none do. This is crucial for maintaining data integrity and consistency across the system.

### Importance of Distributed Transactions

1. **Data Integrity**: Ensuring that all changes across services are consistent and valid.
2. **User Experience**: Users expect a seamless interaction; partial updates can lead to confusion and frustration.
3. **Business Logic**: Many business processes span multiple services, making it necessary to coordinate actions and maintain consistency.

### Key Concepts

1. **ACID Properties**:
   - **Atomicity**: Ensures that all parts of the transaction either complete or none do.
   - **Consistency**: Ensures that the database moves from one valid state to another, maintaining all rules and constraints.
   - **Isolation**: Ensures that the execution of a transaction is isolated from others, preventing interference.
   - **Durability**: Once a transaction is committed, it remains committed even in the case of failures.

2. **Eventual Consistency**: In some microservices architectures, strict ACID compliance is relaxed in favor of eventual consistency, where data will become consistent over time but may not be immediately consistent after transactions.

### Challenges of Distributed Transactions

1. **Network Latency**: Each service call incurs network latency, which can delay the transaction process and affect performance.
2. **Partial Failures**: If one service in a transaction fails, others may have already committed their changes, leading to an inconsistent state.
3. **Complexity**: Implementing distributed transactions requires additional coordination logic, which can complicate system design and debugging.
4. **Blocking Resources**: Some approaches, like Two-Phase Commit, can hold resources for extended periods, leading to potential bottlenecks.
5. **Error Handling**: Managing errors across multiple services requires robust strategies to ensure system reliability.

### Approaches to Handling Distributed Transactions

1. **Two-Phase Commit (2PC)**:
   - **How It Works**:
     - **Phase 1: Prepare**: The coordinator (a designated service) asks all participants (services) to prepare for commit. Each service locks resources and responds with a vote (commit or abort).
     - **Phase 2: Commit/Rollback**: If all participants vote to commit, the coordinator sends a commit command; otherwise, it sends a rollback command.
   - **Pros**:
     - Guarantees strong consistency and atomicity.
   - **Cons**:
     - Blocking: If the coordinator crashes, participants may be left in a locked state.
     - Not fault-tolerant: A failure in one participant can lead to the entire transaction failing.
To handle concurrent transactions in a Two-Phase Commit (2PC) scenario, you need to ensure that the system can manage isolation between transactions and prevent conflicts. This typically involves incorporating mechanisms like locking, versioning, or using transaction IDs.


2. **Sagas**:
The **Saga Pattern** is a design pattern used to manage distributed transactions in microservice-based systems, especially when each service has its own database (as is the case with the **Database per Service** pattern). This pattern helps in maintaining data consistency across multiple services without relying on traditional ACID (Atomicity, Consistency, Isolation, Durability) transactions, which are difficult to achieve in a distributed system due to limitations like network failures and service unavailability.

### Context and Problem:
In a microservices architecture, each service typically manages its own database. This separation can lead to complex transactions that span across multiple services. For example, when processing an e-commerce order, the order service and the customer service are separate, but the customer’s credit limit needs to be checked before the order is approved. A simple ACID transaction won’t work here because each service has its own database, and using distributed transactions (like 2PC) is not viable due to issues of complexity and reliability in distributed systems.

### Forces:
- **2PC (Two-Phase Commit)** is not a good option because it’s hard to implement and prone to failure in distributed systems.
- There is a need for a mechanism to ensure business rules (like checking credit limits) are enforced, even when operations span multiple services.

### Solution: The Saga Pattern
A **saga** is a sequence of **local transactions** that are executed in a specific order to complete a business process that spans multiple services. Each local transaction updates the database of one service and then sends a message/event to trigger the next service in the saga. If a transaction fails, compensating transactions (to undo the previous steps) are triggered to maintain consistency.

![image](https://github.com/user-attachments/assets/b7c397da-5da5-4b34-a5b9-a0c59badbf3f)

There are two common ways to implement a saga:

#### 1. **Choreography-based Saga:**
In this approach, each service involved in the saga publishes domain events that trigger local transactions in other services. There’s no central controller, and each service knows what to do next by listening for events.

**Example** (E-commerce Application):
![image](https://github.com/user-attachments/assets/c2bc215c-961f-4187-bb37-67289b013f38)

1. **Order Service** receives a request to create an order (`POST /orders`) and creates an order in a **PENDING** state.
2. The **Order Service** emits an **OrderCreated** event.
3. The **Customer Service** listens for the `OrderCreated` event and tries to **reserve credit** for the customer.
4. The **Customer Service** emits a message indicating whether the credit reservation was successful or failed.
5. Based on the result, the **Order Service** either **approves** or **rejects** the order.

**Benefits of Choreography**:
- Decentralized, with services acting autonomously.
- More scalable and loosely coupled since there’s no central orchestrator.

**Challenges of Choreography**:
- Coordination between services can be complex, especially as the number of services grows.
- Lack of a clear control flow can make debugging and tracing the flow of transactions difficult.

#### 2. **Orchestration-based Saga:**
In this approach, an **orchestrator** service controls the saga's flow by telling each service what to do next. The orchestrator is responsible for sending commands and managing the saga's state.
![image](https://github.com/user-attachments/assets/b47dcb74-ac72-4289-a80c-991b64252678)

**Example** (E-commerce Application):
1. **Order Service** receives the request to create an order (`POST /orders`) and starts the saga by creating an **Order** in the **PENDING** state.
2. The **Order Service** sends a **Reserve Credit** command to the **Customer Service**.
3. The **Customer Service** tries to reserve credit and replies with the result.
4. The **Order Service** decides to either approve or reject the order based on the outcome of the credit reservation.

**Benefits of Orchestration**:
- Centralized control over the saga, making it easier to manage and monitor.
- The orchestrator has full knowledge of the process and can make decisions based on the overall state.

**Challenges of Orchestration**:
- The orchestrator becomes a central point of failure.
- The orchestrator needs to be aware of all services involved, leading to tighter coupling.

### Compensating Transactions:
When a saga fails (e.g., the credit reservation fails after the order is created), **compensating transactions** are used to undo the changes made by the previous transactions. For example, if the credit reservation fails after the order was created, the system would need to cancel the order (change its state back to `CANCELLED`) or take other compensatory actions.

These compensating actions are crucial because, unlike traditional ACID transactions, a saga does not automatically roll back changes across services. The developer must design these compensating transactions to ensure consistency in the system.

### Benefits of the Saga Pattern:
- **Data Consistency Across Services**: The saga pattern enables distributed transactions to maintain consistency without needing distributed transactions, which are difficult to implement in microservices.
- **Flexibility**: Both choreography and orchestration offer flexibility depending on the application’s needs.
- **Failure Recovery**: With compensating transactions, the system can recover from failures and ensure that business rules are respected.

### Drawbacks of the Saga Pattern:
- **No Automatic Rollback**: Unlike traditional ACID transactions, sagas require developers to manually design compensating transactions. This adds complexity and responsibility to ensure data consistency.
- **Risk of Data Anomalies**: Since sagas are not fully isolated like ACID transactions, concurrent executions can result in inconsistent data unless countermeasures (such as eventual consistency, locks, or careful transaction design) are implemented.
- **Reliability in Messaging**: Since sagas rely on message/event communication, ensuring reliable message delivery (e.g., handling failures, retries, and duplicates) becomes important.
- **Complexity in Monitoring and Debugging**: Especially in choreography-based sagas, it can be hard to trace the flow of events and identify where failures occurred.

### Client-Server Interaction in Sagas:
When a saga is initiated via an HTTP request (e.g., creating an order), the client needs to know the final outcome of the saga. There are several approaches to notify the client about the outcome of the saga:
1. **Immediate Response**: The service sends a response once the saga completes (e.g., an `OrderApproved` or `OrderRejected` event).
2. **Polling**: After initiating the saga, the service responds with a `orderID` and the client periodically polls (`GET /orders/{orderID}`) to check the status.
3. **Asynchronous Notification**: The service responds with the `orderID`, and once the saga completes, it sends an asynchronous notification (e.g., via WebSocket or webhook) to the client.

### Conclusion:
The Saga pattern is essential in microservice architectures where transactions span multiple services and databases. By using a sequence of local transactions and compensating actions, sagas provide a way to maintain consistency and business rules across services, despite not having traditional ACID transactions. However, it introduces complexity, requiring careful design and handling of failure cases, message reliability, and compensating actions.
### Sample Code: Implementing a Simple Saga Pattern in .NET

