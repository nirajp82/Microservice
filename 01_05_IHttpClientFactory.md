### IHttpClientFactory 

- https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory

**Introduction to HttpClient**: HttpClient is a class in .NET used for making HTTP requests and handling responses. It has been available since .NET Framework 4.5.

**Introduction to IHttpClientFactory**: `IHttpClientFactory` was introduced in .NET Core 2.1. It acts as a factory abstraction for creating HttpClient instances with custom configurations.

**Benefits of IHttpClientFactory:**

* **Dependency Injection:** Makes HttpClient a DI-ready type.
* **Centralized Configuration:** Offers a central location for naming and configuring HttpClient instances.
* **Delegating Handlers:** Supports adding outgoing middleware via delegating handlers.
* **Polly Integration:** Provides extension methods for Polly-based middleware for resilient HTTP requests.
* **HttpClientHandler Management:** Manages the caching and lifetime of underlying HttpClientHandler instances.
* **Logging:** Offers a configurable logging experience for all requests.
* **Caching:** It manages caching and lifetime of underlying HttpClientHandler instances, avoiding common DNS problems.


**Consumption Patterns:**

* **Basic Usage:** Involves registering IHttpClientFactory with `AddHttpClient()` and creating clients using CreateClient.
* **Named Clients:** Registering multiple HttpClient instances with distinct configurations. Useful when the app requires many distinct HttpClient instances with different configurations.
* **Typed Clients:** Provide IntelliSense and compiler help, and offer a single location to configure and interact with a specific endpoint.
* **Generated Clients:** IHttpClientFactory can be used with third-party libraries like Refit for generating REST API clients. 


**HttpClient Lifetime Management:**

* IHttpClientFactory creates a new HttpClient instance each time CreateClient is called and it is intended to be short-lived.
* It creates one HttpClientHandler instance per client name and manages their lifetimes.
* Caching is used to reduce resource consumption by reusing HttpClientHandler instances if their lifetime hasn't expired.
* The default handler lifetime is two minutes, but it can be overridden using SetHandlerLifetime.
* IHttpClientFactory creates short-lived HttpClient instances. Disposing them won't lead to socket exhaustion.

**Important Considerations:**

**Avoiding Typed Clients in Singleton Services**:
   - Using typed clients in singleton services can prevent proper DNS change handling (Can not react to DNS changes). It's recommended to use named clients or SocketsHttpHandler in such scenarios.  
   - When using IHttpClientFactory with SocketsHttpHandler, configure PooledConnectionLifetime to manage connection pooling and recycling.
**Message Handler Scopes in IHttpClientFactory**:
   - IHttpClientFactory creates a separate DI scope per HttpMessageHandler instance.
   - Be cautious about using scoped dependencies within message handlers due to separate DI scopes.
   - It's advised not to cache scope-related information inside HttpMessageHandler instances to avoid leaking sensitive information.
