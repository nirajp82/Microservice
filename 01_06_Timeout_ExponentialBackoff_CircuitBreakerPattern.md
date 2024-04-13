1. **Timeout Pattern**:
   - **Why we need it**: In a distributed system, services might experience delays or failures. A timeout ensures that if a service call takes longer than expected, the calling application doesn't hang indefinitely waiting for a response, preventing resource exhaustion and improving system responsiveness.
   - **What problem it solves**: Prevents applications from hanging indefinitely when waiting for responses from slow or unresponsive services.
   - **How to implement it**: Using Polly, you can easily add a timeout policy to an HttpClient instance.

		```csharp
		using System;
		using System.Net.Http;
		using Polly;

		public class TimeoutExample
		{
			private readonly HttpClient _httpClient;

			public TimeoutExample(HttpClient httpClient)
			{
				_httpClient = httpClient;
			}

			public async Task<string> GetExternalDataWithTimeout()
			{
				var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(30));

				return await timeoutPolicy.ExecuteAsync(async () =>
				{
					var response = await _httpClient.GetAsync("https://www.example.com/data");
					response.EnsureSuccessStatusCode();
					return await response.Content.ReadAsStringAsync();
				});
			}
		}
    ```	
   In this example, the timeout policy ensures that the HTTP request to "https://example.com" completes within 30 seconds. If it takes longer, a TimeoutRejectedException will be thrown.

   - **When to use**: Use the timeout pattern when interacting with external services or making network calls to ensure that your application remains responsive and doesn't get stuck indefinitely waiting for a response.

2. **Exponential Backoff Pattern**:
   - **Why we need it**: In scenarios where a service is temporarily overloaded or experiencing transient failures, repeatedly retrying immediately can exacerbate the problem. Exponential backoff introduces a delay between retry attempts, reducing the load on the service and increasing the chances of successful retries.
   - **What problem it solves**: Helps manage transient failures by introducing a delay between retry attempts, reducing the load on the service (preventing overwhelming it with repeated requests) and increasing the likelihood of successful retries.
   - **How to implement it**: Using Polly, you can configure a retry policy with exponential backoff.

   ```csharp
   var httpClient = new HttpClient();
   var retryPolicy = Policy
       .Handle<HttpRequestException>()
       .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

   var response = await retryPolicy.ExecuteAsync(() => httpClient.GetAsync("https://example.com"));
   ```

   In this example, the retry policy retries the HTTP request up to 3 times with exponentially increasing wait times between retries. The purpose of .Handle<HttpRequestException>() is to specify the type of exception that the retry policy should handle. In this case, it indicates that the retry policy should only be triggered if the operation being executed throws an HttpRequestException.


   - **When to use**: Use the exponential backoff pattern when dealing with transient failures in network calls, database connections, or other remote service interactions.

3. **Circuit Breaker Pattern**:
   - **Why we need it**: In situations where a service is consistently failing or unavailable, continuously retrying can overwhelm the system and exacerbate the problem. The circuit breaker pattern monitors for these failures and temporarily stops making requests to the failing service, allowing it time to recover. After a certain period, the circuit can be "half-opened" to allow a limited number of requests to test if the service has recovered.
   - **What problem it solves**: Prevents applications from repeatedly calling failing services, which can lead to cascading failures and system overload.
   - **How to implement it**: Using Polly, you can configure a circuit breaker policy.

   ```csharp
   var httpClient = new HttpClient();
   var circuitBreakerPolicy = Policy
       .Handle<HttpRequestException>()
	    //HandleResult<HttpResponseMessage>(msg => msg.IsSuccessStatusCode == false)
       .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));

   var response = await circuitBreakerPolicy.ExecuteAsync(async () =>
        {
			var response = await httpClient.GetAsync("https://example.com"))
			response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
   ```

   In this example, the circuit breaker policy monitors the HTTP requests and opens the circuit if there are 3 consecutive failures within 30 seconds. Once the circuit is open, subsequent requests will fail immediately without attempting to call the service until the specified timeout period elapses, at which point the circuit will enter a half-open state and allow a single trial call to the service.

   - **When to use**: Use the circuit breaker pattern when interacting with external services or making network calls to prevent cascading failures and overload in case of repeated failures. In other words when dealing with potentially unreliable services where continuous failures can significantly impact your application's performance or stability.

Absolutely! Besides Timeout, Exponential Backoff, and Circuit Breaker, there are other important resilience patterns that work well with Polly in .NET for building robust and fault-tolerant applications. Here are two key ones:

**4. Retry Pattern (Basic Retry):**

* **What it is:** A simple retry mechanism that attempts a failed operation a specified number of times before giving up. It's a more basic version compared to Exponential Backoff.
* **Problem it solves:** Similar to Backoff, it helps handle transient failures that might resolve on subsequent attempts.
* **Implementation with Polly:**

```csharp
using System;
using System.Net.Http;
using Polly;

public class RetryExample
{
    private readonly HttpClient _httpClient;

    public RetryExample(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetExternalDataWithRetry()
    {
        var retryPolicy = Policy.HandleResult<HttpResponseMessage>(msg => msg.IsSuccessStatusCode == false)
            .RetryAsync(3); // Retry up to 3 times

        return await retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _httpClient.GetAsync("https://www.example.com/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
    }
}
```

**Explanation:**

- The `RetryAsync` method defines the retry policy with a maximum of 3 attempts.
- This pattern is useful when you know the number of retries is sufficient and exponential backoff isn't necessary.

**5. Fallback Pattern:**

* **What it is:** Provides an alternative course of action when the primary operation fails. The fallback might return a default value, use cached data, or perform a simpler operation.
* **Problem it solves:** Prevents complete application failure due to primary operation issues by offering a secondary option.
* **Implementation with Polly:**

```csharp
using System;
using System.Net.Http;
using Polly;

public class FallbackExample
{
    private readonly HttpClient _httpClient;

    public FallbackExample(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetExternalDataWithFallback()
    {
        var fallbackData = "Fallback value";

        var policy = Policy.HandleResult<HttpResponseMessage>(msg => msg.IsSuccessStatusCode == false)
            .FallbackAsync(fallbackData);

        return await policy.ExecuteAsync(async () =>
        {
            var response = await _httpClient.GetAsync("https://www.example.com/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
    }
}
```

**Explanation:**

- The `FallbackAsync` method defines the fallback behavior. Here, it returns a predefined string (`fallbackData`).
- This pattern is useful when some data is better than no data at all, even if it's not the ideal result.

**6. Bulkhead Pattern:**

The Bulkhead pattern is a design principle used in distributed systems to prevent failures in one part of the system from causing failures in other parts, thereby improving overall system resilience. The name "bulkhead" is borrowed from shipbuilding, where bulkheads are used to partition different compartments of a ship to prevent flooding in case of a breach. Similarly, in software architecture, the Bulkhead pattern involves segregating resources or components to limit the impact of failures.

**Why do we need it?**
In distributed systems, failures in one part of the system can lead to cascading failures that affect other parts of the system. For example, if a service experiences a sudden surge in traffic or a failure due to resource exhaustion, it could lead to resource contention or complete unavailability, affecting other services sharing the same resources or dependencies. The Bulkhead pattern helps mitigate this risk by isolating components or resources, limiting the spread of failures, and containing the impact within defined boundaries.

**What problem does it solve?**
The Bulkhead pattern addresses the following problems:
- Preventing cascading failures: By partitioning resources or components, failures in one bulkhead do not propagate to other bulkheads, limiting the impact to a localized area.
- Resource management: It helps manage resources more effectively by ensuring that each bulkhead has its own allocation, preventing resource contention and exhaustion.
- Improved system stability: By containing failures within individual bulkheads, the system as a whole becomes more stable and resilient, with failures localized to specific areas.

**How to implement it?**
The Bulkhead pattern can be implemented using libraries such as Polly in .NET. Polly provides support for bulkhead isolation, allowing you to define policies for managing concurrency and limiting the number of concurrent executions within each bulkhead.

Here's a simplified example of implementing the Bulkhead pattern using Polly in .NET:

```csharp
using System;
using Polly;
using System.Net.Http;

public class BulkheadExample
{
    private readonly HttpClient _httpClient;
    private readonly Policy _bulkheadPolicy;

    public BulkheadExample(HttpClient httpClient)
    {
        _httpClient = httpClient;

        // Define bulkhead policy with max parallel executions
       _bulkheadPolicy = Policy.BulkheadAsync(maxParallelization: 10, maxQueuingActions: Int32.MaxValue);
	}

    public async Task<string> ExecuteRequestWithinBulkhead()
    {
        // Execute request within bulkhead
        return await _bulkheadPolicy.ExecuteAsync(async () =>
        {
            var response = await _httpClient.GetAsync("https://www.example.com");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
    }
}
```

In this example:
- We define a `BulkheadPolicy` using `Policy.BulkheadAsync`, specifying the maximum number of parallel executions (`10`) and the maximum number of queued executions (`Int32.MaxValue`).
- We execute the HTTP request within the bulkhead using `bulkheadPolicy.ExecuteAsync`.

**When to use:**
- Use the Bulkhead pattern when designing or refactoring distributed systems to improve resilience and prevent cascading failures.
- It is particularly useful in scenarios where resources are shared among multiple components or services, and failures in one component could affect others.
- Use it when you want to limit the impact of failures by isolating components or resources, improving overall system stability and reliability.

In summary, the Bulkhead pattern provides a mechanism for partitioning resources or components to contain failures and prevent them from propagating throughout the system. By implementing bulkheads, you can improve system resilience, manage resources more effectively, and maintain stability in the face of failures.


## Real world example of setting up policy
```csharp
//Create Typed clients, It will add the IHttpClientFactory and related services to the IServiceCollection.
//https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory
builder.Services.AddHttpClient<CatalogClient>(cc =>
{
    cc.BaseAddress = new Uri("https://localhost:5001");
})
//Handle transient HTTP errors and timeouts gracefully. 
//It sets up a retry policy to automatically retry failed requests up to 5 times with 
//Exponentially increasing wait times between retries and adds a timeout policy 
//to cancel requests that exceed 1 second without receiving a response.
.AddPolicyHandler((serviceProvider, request) =>
    HttpPolicyExtensions.HandleTransientHttpError()
        .Or<TimeoutRejectedException>()
        .WaitAndRetryAsync
        (
            retryCount: 5,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                    + TimeSpan.FromMilliseconds(randomJitter.Next(0, 1000)),
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                var msg = $"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}";
                serviceProvider.GetService<ILogger<CatalogClient>>()?
                    .LogWarning("{msg}", msg);
            }
        ))
.AddPolicyHandler((serviceProvider, request) =>
    HttpPolicyExtensions.HandleTransientHttpError()
        .Or<TimeoutRejectedException>()
        .CircuitBreakerAsync
        (
            handledEventsAllowedBeforeBreaking: 3,
            durationOfBreak: TimeSpan.FromSeconds(15),
            onBreak: (outcome, timespan) =>
            {
                var msg = $"Opening the circuit for {timespan.TotalSeconds} seconds...";
                serviceProvider.GetService<ILogger<CatalogClient>>()?
                    .LogWarning("{msg}", msg);
            },
            onReset: () =>
            {
                var msg = $"Closing the circuit...";
                serviceProvider.GetService<ILogger<CatalogClient>>()?
                    .LogWarning("{msg}", msg);
            }
        ))
//Timeout the request if not completed with in 1 seconds
.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
```
The provided code snippet demonstrates how to configure resilience for HTTP requests made by a `CatalogClient` in a .NET application using the Polly library. It leverages dependency injection and the `IHttpClientFactory` to create a resilient client.

**Breakdown:**

1. **Adding `HttpClient` (Typed Client):**
   - `builder.Services.AddHttpClient<CatalogClient>(cc => ... )`:
     - This line registers a typed HTTP client for `CatalogClient` in the application's dependency injection container.
     - `cc` (configuration action): Used to configure the client's behavior.
   - `cc.BaseAddress = new Uri("https://localhost:5001")`:
     - Sets the base address for the client to "https://localhost:5001". This is the endpoint the client will communicate with.

2. **Handling Transient Errors and Timeouts (Retry Policy):**
   - `.AddPolicyHandler((serviceProvider, request) => ... )`:
     - Adds a policy handler to the client pipeline. This handler intercepts outgoing HTTP requests and applies the configured policy.
   - `HttpPolicyExtensions.HandleTransientHttpError()`:
     - Instructs the policy to handle transient HTTP errors, such as network timeouts, connection resets, or server errors that might be resolved on retry.
   - `.Or<TimeoutRejectedException>()`:
     - Extends the policy to also handle `TimeoutRejectedException`, which could occur if a request exceeds a predefined timeout limit.
   - `.WaitAndRetryAsync(...)`:
     - Configures a retry strategy:
       - `retryCount: 5`: Retries the request up to 5 times.
       - `sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(randomJitter.Next(0, 1000))`:
         - Defines an exponential backoff strategy for delays between retries.
           - The delay increases with each retry attempt (2 raised to the power of the attempt number).
           - A random jitter (between 0 and 1000 milliseconds) is added to prevent synchronization issues with multiple clients retrying simultaneously.
       - `onRetry: (outcome, timespan, retryAttempt, context) => ...`:
         - A callback function invoked when a retry occurs. It logs a warning message using the `ILogger<CatalogClient>` service (if available) stating the delay and retry attempt number.

3. **Circuit Breaker Pattern:**
   - `.AddPolicyHandler((serviceProvider, request) => ... )`: (Another instance of the policy handler)
     - Adds another policy handler to the client pipeline for the circuit breaker pattern.
   - `HttpPolicyExtensions.HandleTransientHttpError()`:
     - Instructs the policy to handle transient HTTP errors (same as before).
   - `.Or<TimeoutRejectedException>()`:
     - Extends the policy to also handle `TimeoutRejectedException` (same as before).
   - `.CircuitBreakerAsync(...)`:
     - Configures a circuit breaker pattern:
       - `handledEventsAllowedBeforeBreaking: 3`: The number of handled errors (transient errors or timeouts) allowed within a timeframe before the circuit trips (3 in this case).
       - `durationOfBreak: TimeSpan.FromSeconds(15)`: The duration for which the circuit remains open (tripped) after it breaks, preventing further requests for 15 seconds.
       - `onBreak: (outcome, timespan) => ...`:
         - A callback function invoked when the circuit breaks. It logs a warning message using the `ILogger<CatalogClient>` service (if available) stating that the circuit is opening for 15 seconds.
       - `onReset: () => ...`:
         - A callback function invoked when the circuit resets (closes) after the break duration elapses. It logs a warning message indicating that the circuit is closing.

4. **Timeout Policy:**
   - `.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1))`:
     - Adds another policy handler to the client pipeline for a simple timeout policy.
   - `Policy.TimeoutAsync<HttpResponseMessage>(1)`:
     - Instructs the policy to timeout requests that don't receive a response within 1 second.

**In summary:**

This code configures the `CatalogClient` to be resilient against failures by:

- Retrying transient errors and timeouts with an exponential backoff strategy.
- Implementing a circuit breaker pattern that prevents cascading failures by halting requests for a short period after a certain number of errors occur.
- Enforcing a strict timeout limit for requests.

These resilience strategies help ensure that the client continues to function gracefully even in the face of temporary network issues or server problems.
