# OAuth2: Overview and Usage for Securing Microservices

## What is OAuth2?

OAuth 2.0, short for "Open Authorization," is a widely recognized industry-standard protocol for authorization. It enables a Client App (such as a website or application) to access user resources hosted by other web applications on behalf of the user, all without exposing user passwords. OAuth2 is widely used for authorizing access to resources in a secure and controlled manner.

## The OAuth 2.0 flow typically involves the following parties:

* **Resource Owner:** The user or entity that owns the data.
* **Client:** The application requesting access to the resource owner's data.
* **Authorization Server:** Issues access tokens to the client.
* **Resource Server:** Protects the resource and validates access tokens.

## How OAuth2 Works

OAuth2 operates through a series of interactions between a user, an authorization server, and a resource server. Here's a high-level overview of the OAuth2 flow:

1. **Authorization Request**: The client application requests authorization from the user to access their resources on the resource server.

2. **Authorization Grant**: If the user consents, the authorization server provides an authorization grant to the client application. This grant is a temporary credential representing the user's consent.

3. **Access Token Request**: The client application exchanges the authorization grant for an access token by making a request to the authorization server.

4. **Access Token Response**: The authorization server issues an access token to the client application. This token represents the authorization granted by the user.

5. **Resource Access**: The client application uses the access token to access the protected resources on the resource server.

6. **Token Validation**: The resource server validates the access token before granting access to the requested resources.

OAuth2 supports several grant types, including:
- **Authorization Code**: Suitable for web applications with server-side components.
- **Implicit**: Suitable for single-page applications (SPAs) running in a user's browser.
- **Resource Owner Password Credentials**: Suitable for trusted applications where the user’s credentials are directly used.
- **Client Credentials**: Suitable for machine-to-machine communication.

## Why Use OAuth2 to Secure Microservices?

Securing microservices is crucial to protecting sensitive data and ensuring that only authorized clients can access specific services. OAuth2 provides several benefits for securing microservices:

1. **Granular Access Control**: OAuth2 allows you to define scopes and permissions, enabling fine-grained access control to your microservices.

2. **Token-Based Authentication**: By using access tokens, OAuth2 provides a stateless and scalable approach to authenticate and authorize service requests.

3. **Delegated Access**: OAuth2 supports delegation, allowing users to grant third-party applications limited access to their resources without sharing their credentials.

4. **Enhanced Security**: OAuth2 promotes secure practices such as token expiration and refresh tokens, reducing the risk of long-term credential exposure.

5. **Interoperability**: OAuth2 is a widely adopted standard, supported by many frameworks and services, facilitating integration with various authentication providers and systems.

6. **Separation of Concerns**: OAuth2 enables the separation of authentication (identity verification) from authorization (access control), allowing for more flexible and manageable security policies.

By leveraging OAuth2, organizations can build robust security frameworks for their microservices, ensuring that resources are accessed only by authenticated and authorized entities.

# Understanding OAuth2 Scope: A Real-World Example

## What is Scope?

In OAuth2, a "scope" is a parameter used to define the level of access that a client application is requesting from the authorization server. Scopes specify which resources and actions the application can access, providing fine-grained control over permissions.

## Real-World Example: **PhotoShareApp**

### Key Parties Involved

- **Resource Owner**: You, the user of the social media platform.
- **Client**: **PhotoShareApp**, a third-party application that wants to access your photos.
- **Authorization Server**: The server operated by the social media platform that manages authorization requests and issues tokens.
- **Resource Server**: The server where your photos are stored and managed by the social media platform.

### Scenario

You use a social media platform called **SocialSnap** where you upload and share photos. You come across **PhotoShareApp**, which helps you create photo albums by importing pictures from your SocialSnap account.

### OAuth2 Flow with Scopes

1. **Authorization Request**: **PhotoShareApp** wants to access your photos on SocialSnap. It sends an authorization request to SocialSnap’s **Authorization Server**, specifying the scopes it needs:
   - `read_photos`: Allows PhotoShareApp to view and retrieve your photos.
   - `upload_photos`: Allows PhotoShareApp to add new photos to your account.

2. **User Consent**: The **Authorization Server** presents you with a consent screen showing what permissions **PhotoShareApp** is requesting:
   - "PhotoShareApp wants to access your photos (read only)."
   - "PhotoShareApp also wants to upload new photos to your account."

3. **Authorization Grant**: If you approve the request, the **Authorization Server** provides **PhotoShareApp** with an authorization grant.

4. **Access Token Request**: **PhotoShareApp** exchanges this authorization grant for an access token by making a request to the **Authorization Server**.

5. **Access Token Response**: The **Authorization Server** issues an access token to **PhotoShareApp** that includes the granted scopes, such as `read_photos` and `upload_photos`.

6. **Resource Access**: **PhotoShareApp** uses the access token to interact with SocialSnap’s **Resource Server**. The **Resource Server** checks the token’s scopes to determine what actions **PhotoShareApp** is allowed to perform.

### Example Scenarios

- **Scope: `read_photos`**: If you grant only this scope, **PhotoShareApp** can fetch and display your photos but cannot modify or upload new ones. It will only have read access.

- **Scope: `upload_photos`**: If **PhotoShareApp** needs to add new photos to your SocialSnap account, it will request this scope in addition to `read_photos`. With `upload_photos`, **PhotoShareApp** can both read and upload photos.

### Key Points

- **Granular Permissions**: Scopes provide specific, controlled permissions rather than broad access, enhancing security.
- **User Awareness**: Scopes make it clear what actions the third-party application will be able to perform, helping users make informed decisions.
- **Minimized Risk**: By requesting only the necessary scopes, applications limit their access to what is required, reducing security risks.

In summary, OAuth2 scopes allow precise control over what a third-party application can do, ensuring secure and transparent access to resources.

Certainly! Here’s a complete explanation of the OAuth 2.0 Authorization Code Flow with PKCE for **PhotoShareApp**, including a revised diagram for easy copying.

---

## OAuth 2.0 Authorization Code Flow with PKCE for **PhotoShareApp**

### Key Parties Involved

1. **Resource Owner**: You, the user of the social media platform **SocialSnap**.
2. **Client**: **PhotoShareApp**, a third-party application requesting access to your photos on SocialSnap.
3. **Authorization Server**: The server operated by SocialSnap that manages the authorization process and issues tokens.
4. **Resource Server**: The server hosted by SocialSnap where your photos are stored.

### Key Concepts

- **Code Verifier**: A random, cryptographically secure string generated by **PhotoShareApp** to prove the legitimacy of the authorization request.
- **Code Challenge**: A hashed version of the Code Verifier sent to SocialSnap’s Authorization Server during the initial authorization request. A hashing method agreed between the client and the server.

### Authorization Flow Steps

1. **Generate Code Verifier and Code Challenge**:
   - **PhotoShareApp** generates a **Code Verifier** (a random string).
   - The **Code Verifier** is hashed to create a **Code Challenge** (usually using SHA-256).

2. **Authorization Request**:
   - **PhotoShareApp** sends an authorization request to SocialSnap’s **Authorization Server**, including the **Code Challenge** and the `code_challenge_method` (e.g., `S256` for SHA-256). The request also includes the scope (e.g., `read_photos` and `upload_photos`), redirect URI, and other required parameters.

3. **User Consent**:
   - The **Authorization Server** presents a consent screen to you, the Resource Owner, detailing the permissions **PhotoShareApp** is requesting. You decide whether to approve or deny these permissions.

4. **Authorization Grant**:
   - If you grant permission, SocialSnap’s **Authorization Server** sends an **Authorization Code** to **PhotoShareApp** via the redirect URI. This **Authorization Code** is valid for very short duration of the time like 10 minute or so and good for only one use.

5. **Access Token Request**:
   - **PhotoShareApp** exchanges the **Authorization Code** for an **Access Token** by sending a request to the **Authorization Server**. This request includes the original **Code Verifier** along with **Authorization Code**.

6. **Access Token Response**:
   - The **Authorization Server** validates the **Code Verifier** against the **Code Challenge** used in the initial authorization request. If they match, the **Authorization Server** issues an **Access Token** and sends it back to **PhotoShareApp**.

7. **Resource Access**:
   - **PhotoShareApp** uses the **Access Token** to make requests to SocialSnap’s **Resource Server**. The **Resource Server** checks the token’s validity and grants or denies access to your photos based on the token's permissions.

### Diagram
![image](https://github.com/user-attachments/assets/bed8b70f-e013-41b6-81e9-b8469051a684)
Reference: https://xebia.com/blog/get-rid-of-client-secrets-with-oauth-authorization-code-pkce-flow/ 

This diagram and explanation illustrate the OAuth 2.0 Authorization Code Flow with PKCE, detailing how **PhotoShareApp** securely obtains access to your photos on **SocialSnap** without exposing sensitive information.
