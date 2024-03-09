## Microservice

- **What is microservice?**
  - It is also known as the microservice architecture. is an architectural style that structures an application as a collection of Independently deployable and Loosely coupled services that are modeled around a business domain and are usually owned by small team.

 ### Benefits

1. **Agility**: Microservices allow for independent deployment, making it easier to manage bug fixes and feature releases. Updates can be made to a service without redeploying the entire application, and rollback options are available if issues arise. This agility is especially beneficial in traditional applications where a bug in one part can halt the entire release process.

2. **Small, Focused Teams**: Microservices encourage small team sizes, enabling greater agility. Each microservice can be built, tested, and deployed by a single feature team, reducing communication overhead and promoting productivity.

3. **Small Code Base**: Microservices minimize code dependencies, making it easier to add new features without affecting other parts of the application. Unlike monolithic architectures, microservices don't share code or data stores, simplifying the addition of new functionalities.

4. **Mix of Technologies**: Teams can select the best technology for each service, fostering flexibility and innovation. Microservices allow for the use of different technology stacks as needed for individual services.

5. **Fault Isolation**: Individual microservices can fail without disrupting the entire application, provided upstream services handle faults appropriately. Techniques like the Circuit Breaker pattern or asynchronous messaging facilitate fault isolation.

6. **Scalability**: Microservices enable independent scaling of services, allowing subsystems requiring more resources to scale out without affecting the entire application. Tools like Kubernetes enhance resource utilization by packing more services onto a single host.

7. **Data Isolation**: Schema updates are simplified in microservices since changes only affect a single service. Monolithic applications face challenges with schema updates due to shared data across different parts of the application.

### Challenges

1. **Complexity**: Microservices introduce more moving parts compared to monolithic architectures, increasing system complexity despite simpler individual services.

2. **Development and Testing**: Writing and testing services with dependencies requires a different approach, challenging existing tools and requiring careful refactoring across service boundaries. Testing service dependencies can be particularly difficult in rapidly evolving applications.

3. **Lack of Governance**: Decentralized development in microservices can lead to an abundance of languages and frameworks, making maintenance challenging. Establishing project-wide standards without overly restricting flexibility is crucial, especially for cross-cutting functionalities like logging.

4. **Network Congestion and Latency**: Interservice communication increases with granular services, potentially causing network congestion and latency issues. Careful API design and asynchronous communication patterns can mitigate these challenges.

5. **Data Integrity**: Each microservice manages its own data persistence, posing challenges for data consistency. Embracing eventual consistency can help address this issue.

6. **Management**: Successful microservices implementation requires a mature DevOps culture, with challenges including correlated logging across services and managing multiple service calls for a single user operation.

7. **Versioning**: Updates to services must maintain backward and forward compatibility to avoid breaking dependent services. Careful design and versioning strategies are necessary to manage updates effectively.

8. **Skill Set**: Microservices architecture requires expertise in distributed systems. Assessing the team's skills and experience is essential for successful implementation.

References: 
- https://learn.microsoft.com/en-us/azure/architecture/guide/architecture-styles/microservices
- 
