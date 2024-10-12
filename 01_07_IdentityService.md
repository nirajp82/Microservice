# OAuth2: Overview and Usage for Securing Microservices

## Table of Contents
1. [What is OAuth2?](#what-is-oauth2)
2. [OAuth 2.0 Flow and Key Parties](#the-oauth-20-flow-and-key-parties)
3. [How OAuth2 Works](#how-oauth2-works)
4. [OAuth2 Grant Types](#oauth2-grant-types)
5. [Why Use OAuth2 to Secure Microservices?](#why-use-oauth2-to-secure-microservices)
6. [Understanding OAuth2 Scope: A Real-World Example](#understanding-oauth2-scope-a-real-world-example)
7. [OAuth 2.0 Authorization Code Flow with PKCE for PhotoShareApp](#oauth-20-authorization-code-flow-with-pkce-for-photoshareapp)
8. **************************
9. [What is OpenID Connect (OIDC)?](#what-is-openid-connect-oidc)
10. [The Problem OIDC Solves Over OAuth 2.0](#the-problem-oidc-solves-over-oauth-20)
11. [The OIDC Flow](#the-oidc-flow)
12. [**Verification of ID Token by the Client**](#verification-of-id-token-by-the-client)
13. [**Role of the Authorization Server in OIDC**](#role-of-the-authorization-server-in-oidc)
14. [**Resource Server's Role in OIDC**](#resource-servers-role-in-oidc)
15. [What is the difference between Bearer and ID Token](#what-is-the-difference-between-bearer-and-id-token)
16. *************************
17. [**Introduction to IdentityServer**](#introduction-to-identityserver)
18. [**IdentityServer Middleware Endpoints**](#identityserver-middleware-endpoints)
19. [**What is Duende IdentityServer?**](#What-is-Duende-identityserver)
20. [Summary](#summary)
21. *************************
22. [Explain What a JSON Web Token (JWT) Is and How It Works](#explain-what-a-json-web-token-jwt-is-and-how-it-works)

---
## What is OAuth2?

OAuth 2.0, short for "Open Authorization," is a widely recognized industry-standard protocol for authorization. It enables a Client App (such as a website or application) to access user resources hosted by other web applications on behalf of the user, all without exposing user passwords. OAuth2 is widely used for authorizing access to resources in a secure and controlled manner.

## The OAuth 2.0 Flow and Key Parties

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

## OAuth2 Grant Types

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
### OAuth2 Flow with Scopes: Detailed Steps Including User Credentials

#### 1. **User Initiates Login**
You want to use **PhotoShareApp** to access your photos on **SocialSnap**. You start by clicking the **"Login with SocialSnap"** button on the PhotoShareApp website.

#### 2. **Authorization Request**
This action triggers PhotoShareApp to send an authorization request to SocialSnap’s Authorization Server:
- The request specifies the required scopes:
  - **read_photos**: Allows PhotoShareApp to view and retrieve your photos.
  - **upload_photos**: Allows PhotoShareApp to add new photos to your SocialSnap account.

#### 3. **Redirection to Login Page**
After sending the authorization request, you are redirected to the SocialSnap Authorization Server, which presents you with the login page. Here, you need to enter your username and password to authenticate yourself.

#### 4. **User Consent**
Once you successfully log in, the Authorization Server presents you with a consent screen. It details the permissions that PhotoShareApp is requesting:
- **"PhotoShareApp wants to access your photos (read only)."**
- **"PhotoShareApp also wants to upload new photos to your account."**

You review these permissions, and if you agree, you click the **"Allow"** button.

#### 5. **Authorization Grant**
After granting permission, the Authorization Server provides PhotoShareApp with an **authorization grant**. This grant is a temporary code that allows PhotoShareApp to request an access token.

#### 6. **Access Token Request**
With the authorization grant in hand, PhotoShareApp makes a request to the Authorization Server to exchange it for an **access token**. This request includes:
- The **authorization grant** received in the previous step.
- The **client ID** and **client secret** to verify PhotoShareApp’s identity.
- The **redirect URI** to ensure it matches what was registered.

#### 7. **Access Token Response**
If the Authorization Server verifies the request and the **code verifier**, it issues an access token to PhotoShareApp. 

Here's how the verification process works:
- The Authorization Server hashes the **code verifier** received from PhotoShareApp to generate a **code challenge**.
- It compares this generated code challenge with the original code challenge sent during the authorization request.
- If they match, the request is confirmed as legitimate, since only PhotoShareApp knows the original code verifier.

Upon successful verification, the Authorization Server responds with an access token that includes the granted scopes:
- **read_photos**
- **upload_photos**
  
#### 8. **Resource Access**
Now equipped with the access token, PhotoShareApp can interact with SocialSnap’s Resource Server. When making requests, it includes the access token. The Resource Server checks the token’s scopes to determine what actions PhotoShareApp is allowed to perform:
- **Fetching Photos**: With the **read_photos** scope, PhotoShareApp retrieves your photos from SocialSnap.
- **Uploading New Photos**: If you choose to add photos through PhotoShareApp, it uses the **upload_photos** scope to add them to your SocialSnap account.

#### 9. **User Experience**
As a result, you can easily create and organize photo albums in PhotoShareApp using your existing SocialSnap photos. The process is seamless, ensuring you maintain control over your data.

#### 10. **Scope Management and Revocation**
If you later decide to limit PhotoShareApp's access, you can revoke the permissions through your SocialSnap account settings. This action will prevent PhotoShareApp from accessing your photos or uploading new ones.

### Example Scenarios

- **Scope: `read_photos`**: If you grant only this scope, **PhotoShareApp** can fetch and display your photos but cannot modify or upload new ones. It will only have read access.

- **Scope: `upload_photos`**: If **PhotoShareApp** needs to add new photos to your SocialSnap account, it will request this scope in addition to `read_photos`. With `upload_photos`, **PhotoShareApp** can both read and upload photos.

### Key Points

- **Granular Permissions**: Scopes provide specific, controlled permissions rather than broad access, enhancing security.
- **User Awareness**: Scopes make it clear what actions the third-party application will be able to perform, helping users make informed decisions.
- **Minimized Risk**: By requesting only the necessary scopes, applications limit their access to what is required, reducing security risks.

In summary, OAuth2 scopes allow precise control over what a third-party application can do, ensuring secure and transparent access to resources.

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

### OAuth 2.0 Authorization Code Flow with PKCE for PhotoShareApp

To enhance security, PhotoShareApp implements Proof Key for Code Exchange (PKCE). Before sending the authorization request, PhotoShareApp generates a code verifier and a code challenge.

This prevents attacks by ensuring that even if someone intercepts the authorization code, they cannot exchange it for an access token without the original code verifier. Only PhotoShareApp knows this code, adding an extra layer of security.

if the attacker (Man in the middle or Phishing) obtains the authorization code, they cannot exchange it for an access token without the original code verifier. The code verifier is generated by PhotoShareApp and is not sent over the network. This means only PhotoShareApp knows the verifier, effectively blocking the attacker from completing the token exchange, even with the stolen code.

#### 1. **User Initiates Login**
You want to use **PhotoShareApp** to access your photos on **SocialSnap**. You begin by clicking the **"Login with SocialSnap"** button on the PhotoShareApp website.

#### 2. **PKCE Setup**
To enhance security, PhotoShareApp implements **Proof Key for Code Exchange (PKCE)**. Before sending the authorization request, PhotoShareApp generates:
- A **code verifier**: A random string used later in the token exchange.
- A **code challenge**: A hashed version of the code verifier that is sent to the Authorization Server.

#### 3. **Authorization Request**
When you click the login button:
- PhotoShareApp sends an authorization request to SocialSnap’s Authorization Server, which includes:
  - The **client ID**: Identifying PhotoShareApp.
  - The **redirect URI**: Where the user will be sent after authorization.
  - The **code challenge**: The hashed version of the code verifier.
  - The required scopes:
    - **read_photos**: Allows PhotoShareApp to view and retrieve your photos.
    - **upload_photos**: Allows PhotoShareApp to add new photos to your SocialSnap account.

#### 4. **Redirection to Login Page**
After sending the authorization request, you are redirected to the **SocialSnap Authorization Server**, which presents you with the login page. Here, you need to enter your **username** and **password** to authenticate yourself.

#### 5. **User Consent**
Once you successfully log in, the Authorization Server shows you a consent screen. It details the permissions that PhotoShareApp is requesting:
- **"PhotoShareApp wants to access your photos (read only)."**
- **"PhotoShareApp also wants to upload new photos to your account."**

You review these permissions, and if you agree, you click the **"Allow"** button.

#### 6. **Authorization Grant**
After granting permission, the Authorization Server provides PhotoShareApp with an **authorization code**. This code is temporary and allows PhotoShareApp to request an access token.

#### 7. **Access Token Request**
PhotoShareApp now exchanges the authorization code for an access token. This request is sent to the Authorization Server and includes:

- The **authorization code** received in the previous step.
- The **client ID**.
- The **redirect URI** to ensure it matches what was registered.
- The **code verifier**: This is the original string generated during the PKCE setup. It is sent to confirm that the request is legitimate; only PhotoShareApp knows this code. If an attacker intercepted the authorization code, they couldn't use it to get an access token without the code verifier, adding a layer of security.t.

#### 8. **Access Token Response**
If the Authorization Server verifies the request and the code verifier, it responds by issuing an **access token** to PhotoShareApp. This token includes the granted scopes:
- **read_photos**
- **upload_photos**

#### 9. **Resource Access**
Now equipped with the access token, PhotoShareApp can interact with SocialSnap’s Resource Server. When making requests, it includes the access token in the request header.

Here’s how the Resource Server validates the access token to ensure it’s legitimate:

9.1. **Token Verification**: The Resource Server checks the access token against the Authorization Server. This process involves:
   - **Signature Generation by Authorization Server**:
     - The access token is signed by the Authorization Server using its **private key**, which is kept confidential and never shared.
     - When the token is created, the Authorization Server generates a unique signature by applying a cryptographic algorithm to the token’s content (such as the payload and metadata) along with the private key.
     - This signature ensures that the token can be verified later and guarantees its integrity.
     - **Signature Verification by Resource Server**:
     - The Resource Server uses the corresponding **public key** to verify this signature. The public key can be freely distributed and is available to anyone who needs to verify tokens.
     - **How it Works**:
       - The Resource Server extracts the signature from the received access token.
       - It uses the public key to validate the signature by performing a cryptographic operation (e.g., using algorithms like RSA or ECDSA).
       - If the signature is valid, it confirms that the token was issued by the trusted Authorization Server and has not been altered.

9.2. **Token Introspection**: In addition to signature verification, the Resource Server may call the Authorization Server’s token introspection endpoint to check:
   - The token's validity (e.g., not revoked or expired).
   - Expiration time.
   - Associated scopes, ensuring the token has the necessary permissions for the requested action.

9.3. **Scope Check**: After validating the token, the Resource Server examines its scopes to determine what actions PhotoShareApp is allowed to perform:
   - **Fetching Photos**: With the **read_photos** scope, PhotoShareApp can retrieve your photos from SocialSnap.
   - **Uploading New Photos**: If you choose to add photos through PhotoShareApp, it uses the **upload_photos** scope to add them to your SocialSnap account.

This comprehensive process ensures that only valid access tokens are accepted, preventing unauthorized access and maintaining security throughout the interaction. The use of public/private key pairs facilitates a secure method of verifying the authenticity of the access token, ensuring that only the legitimate Authorization Server can issue valid tokens, while allowing anyone with the public key to verify their integrity.

#### 10. **User Experience**
As a result, you can easily create and organize photo albums in PhotoShareApp using your existing SocialSnap photos. The process is seamless, ensuring you maintain control over your data.

#### 11. **Scope Management and Revocation**
If you later decide to limit PhotoShareApp's access, you can revoke the permissions through your SocialSnap account settings. This action will prevent PhotoShareApp from accessing your photos or uploading new ones.

### Diagram
![image](https://github.com/user-attachments/assets/bed8b70f-e013-41b6-81e9-b8469051a684)
Reference: https://xebia.com/blog/get-rid-of-client-secrets-with-oauth-authorization-code-pkce-flow/ 

This diagram and explanation illustrate the OAuth 2.0 Authorization Code Flow with PKCE, detailing how **PhotoShareApp** securely obtains access to your photos on **SocialSnap** without exposing sensitive information.


# OIDC
### What is OpenID Connect (OIDC)?

OIDC stands for OpenID Connect. It is an authentication/identity layer built on top of the OAuth 2.0 protocol. OIDC allows clients (like web applications or mobile apps) to verify the identity of users based on the authentication performed by an authorization server. It provides a standardized way to obtain user identity information, allowing for single sign-on (SSO) and simplifying the authentication process across different services. OIDC extends OAuth 2.0 by adding an additional token called the **ID Token**, which contains information about the user’s identity.


### The Problem OIDC Solves Over OAuth 2.0

OAuth 2.0 is designed primarily for authorization, allowing applications to request access to a user’s resources (like photos) without needing to know the user’s credentials. However, OAuth 2.0 does not inherently authenticate the user. This means that while an app can obtain permission to access a user's resources, it doesn’t necessarily know *who* the user is.

#### Example: JohnDoe and JaneSmith

Imagine JohnDoe is using PhotoShareApp, which connects to SocialSnap (a social media platform) via OAuth 2.0. After JohnDoe logs in to SocialSnap, PhotoShareApp receives an **Access Token** that allows it to access JohnDoe’s photos.

Now, if JohnDoe shares this Access Token with JaneSmith, JaneSmith can use it to access JohnDoe’s photos through PhotoShareApp, even though she isn’t JohnDoe. Since OAuth 2.0 alone doesn’t require the app to know who the token belongs to, PhotoShareApp wouldn’t recognize that JaneSmith is using JohnDoe’s token—it just sees a valid token.

### How OpenID Connect (OIDC) Addresses This Problem

OIDC solves this issue by introducing an **ID Token** along with the Access Token. The ID Token contains information about the authenticated user, such as their unique identifier (user ID), and how they were authenticated.

### The OIDC Flow

The OIDC flow is very similar to the OAuth 2.0 flow, with an additional step for user authentication. Here’s how it works:

1. **Authorization Request**: PhotoShareApp redirects JohnDoe to SocialSnap’s Authorization Server, requesting permission to access JohnDoe’s photos. PhotoShareApp also requests the `openid` scope, which indicates that it wants to authenticate the user and receive an ID Token.

2. **User Authentication**: SocialSnap’s Authorization Server authenticates JohnDoe (e.g., via username and password).

3. **Authorization Response**: If authentication is successful, SocialSnap’s Authorization Server sends back an Authorization Code to PhotoShareApp.

4. **Token Exchange**: PhotoShareApp exchanges the Authorization Code for an Access Token and an ID Token by making a request to SocialSnap’s Token Endpoint.

5. **ID Token and Access Token Received**: SocialSnap’s Token Endpoint returns both an Access Token (for accessing resources like photos) and an ID Token (which contains information about JohnDoe’s identity).

6. **Access Resources and Verify Identity**: PhotoShareApp can now use the Access Token to retrieve JohnDoe’s photos from SocialSnap. Additionally, PhotoShareApp can verify JohnDoe’s identity using the ID Token to ensure that the token is being used by the correct person.

#### Continuing the Example with OIDC

In the earlier scenario, if PhotoShareApp was using OIDC, it would receive both an Access Token and an ID Token after JohnDoe logged in. The ID Token would contain information specific to JohnDoe, like his user ID.

Now, if JaneSmith tried to use JohnDoe’s Access Token, PhotoShareApp could check the ID Token and see that the identity associated with the token is JohnDoe, not JaneSmith. This would allow PhotoShareApp to detect the misuse and prevent JaneSmith from accessing JohnDoe’s photos.

#### **Verification of ID Token by the Client**
   - **Signature Verification:** The ID Token is signed by the Authorization Server (e.g., SocialSnap). The PhotoSharing app, acting as the Client, uses the public key provided by SocialSnap to verify the signature, ensuring the token hasn’t been altered.
   - **Issuer Validation:** The app checks that the `iss` (issuer) claim in the ID Token matches SocialSnap’s URL.
   - **Audience Validation:** The app verifies that the `aud` (audience) claim in the ID Token matches its own client ID, ensuring the token was intended for the app.
   - **Expiration Check:** The app checks the `exp` (expiration time) claim to ensure the token is still valid.
   - **Nonce Check:** If the app included a nonce during the authorization request, it verifies that the nonce in the ID Token matches, protecting against replay attacks.

#### **Role of the Authorization Server in OIDC**
   - **Issuing ID Tokens:** The Authorization Server (SocialSnap) issues and signs the ID Token, including claims that represent the authenticated user’s identity.
   - **Providing Public Keys:** The Authorization Server publishes its public keys in its metadata, allowing Clients like the PhotoSharing app to verify the ID Token’s signature.

#### **Resource Server's Role in OIDC**
   - **Verifying Access Tokens:** The Resource Server (SocialSnap’s photo storage server) is primarily concerned with verifying Access Tokens to determine if the Client can access specific resources. 
   - **ID Token Usage:** While the Resource Server typically doesn’t verify the ID Token, it relies on the Access Token, which was obtained based on the ID Token, ensuring that resource access is granted only to authorized entities.

#### What is the difference between Bearer and ID Token

The generation of bearer tokens directly in the same API that later will use authorize requests based on those tokens. This approach is fine for simple setups, like a single API with a frontend on the same domain. However, in a more complex environment like a microservices architecture, where you have multiple services and clients (sometimes ones you don’t own), using OIDC (OpenID Connect) tokens is preferred. Here’s why:


1. **Standardization:** OIDC is built on OAuth 2.0 and follows industry standards, making it compatible with many services and tools.

2. **User Authentication**: OIDC ensures that the app knows who the user is, providing identity verification in addition to resource access.

3. **Preventing Token Misuse**: The ID Token helps ensure that only the legitimate user can use the token, reducing the risk of token misuse.

4. **Enhanced Security:** OIDC offers features like token introspection and revocation, plus public/private key validation, which enhance security and help prevent token forgery.  By adding an identity layer on top of OAuth 2.0, OIDC improves security, ensuring that resources are accessed only by the correct user.

5. **Single Sign-On (SSO):** OIDC supports SSO, allowing users to log in once and access multiple services without re-authenticating, improving user experience and reducing credential management.

6. **Separation of Concerns:** OIDC handles authentication through an identity provider (IdP), so your API doesn’t need to manage sensitive credentials, lowering security risks and compliance issues.

7. **Scalability and Flexibility:** OIDC works well in distributed environments and integrates easily with various identity providers, making it scalable and adaptable.

8. **Rich User Information:** OIDC tokens can include detailed user information (claims) for personalization and authorization without extra database queries.

For a microservices setup, OIDC tokens are the better choice. For simpler scenarios, basic bearer tokens might suffice.

### Summary

OpenID Connect extends OAuth 2.0 by adding user authentication through the ID Token, which allows applications to verify the identity of the user in addition to gaining access to their resources. The flow is similar to OAuth 2.0 but includes the `openid` scope and the additional step of receiving and verifying the ID Token. This helps prevent scenarios where tokens could be misused by unauthorized users, as shown in the example of JohnDoe and JaneSmith.

*******************************

## Introduction to IdentityServer
#### What is Identity Server
IdentityServer is an authentication server that implements OpenID Connect (OIDC) and OAuth 2.0 standards for ASP.NET Core. It's designed to provide a common way to authenticate requests to all of your applications, whether they're web, native, mobile, or API endpoints. IdentityServer can be used to implement Single Sign-On (SSO) for multiple applications and application types. 
IdentityServer is used for building OpenID Connect and OAuth 2.0 solutions. It provides a comprehensive implementation of these protocols, which are widely used for managing authentication and authorization in web applications and APIs.

## IdentityServer Middleware Endpoints

IdentityServer exposes several key endpoints to support standard functionality. Each endpoint plays a specific role in handling authentication, token management, and user information. Below is a summary of these endpoints:

1. **Authorize**
   - **Purpose**: Authenticate the end user.
   - **Description**: Used to initiate the authentication process. The user is redirected to this endpoint to log in and grant consent to the client application.
   - **Request**: GET https://localhost:5003/connect/authorize?response_type=code&client_id=postman&scope=openid%20profile%20catalog.fullaccess&redirect_uri=urn%3Aietf%3Awg%3Aoauth%3A2.0%3Aoob&code_challenge=bK_JI8FLLclnGONSAWCsSyEXlS_FoD7LSCvSU_kAO8k&code_challenge_method=S256
   
2. **Token**
   - **Purpose**: Request a token programmatically.
   - **Description**: This endpoint allows clients to request access tokens, refresh tokens, and ID tokens using authorization grants (e.g., authorization code, client credentials).
   - **Request**: **Request**: POST https://localhost:5003/connect/token

3. **Discovery**
   - **Purpose**: Provide metadata about the server.
   - **Description**: Exposes metadata about the IdentityServer instance, including information on available endpoints, supported scopes, and more. This is usually available at the `.well-known/openid-configuration` URL. For ex: https://localhost:5003/.well-known/openid-configuration

4. **User Info**
   - **Purpose**: Retrieve user information with a valid access token.
   - **Description**: Allows clients to obtain user profile information using a valid access token. This endpoint provides details such as user name, email, and other claims.

5. **Device Authorization**
   - **Purpose**: Start device flow authorization.
   - **Description**: Supports the device authorization flow, allowing devices with limited input capabilities (e.g., smart TVs) to obtain tokens.

6. **Introspection**
   - **Purpose**: Validate tokens.
   - **Description**: Used to check the validity of access tokens or refresh tokens. Clients can use this endpoint to verify the status and metadata of tokens.

7. **Revocation**
   - **Purpose**: Revoke tokens.
   - **Description**: Allows clients to invalidate access or refresh tokens, effectively ending the session for the associated user.

8. **End Session**
   - **Purpose**: Trigger single sign-out across all applications.
   - **Description**: Initiates the logout process, which can include terminating sessions in all applications where the user is currently signed in.

These endpoints are integral to managing authentication and authorization processes effectively in applications using IdentityServer.

#### What is Duende IdentityServer?
Duende IdentityServer is an enterprise-grade identity and access management (IAM) solution for .NET applications. It is built on the same foundation as the open-source IdentityServer4, but with additional features, support, and licensing options for commercial use. Duende IdentityServer is designed to offer robust, secure, and scalable solutions for handling authentication and authorization.


#### Explain `IdentityServerApi` Scope.
This SPECIAL scope. It is used to allow access to APIs that are part of the same application or service that is running IdentityServer itself. It essentially represents a special scope for local APIs that don't require external authorization but need to be protected internally.
      
Internal Authorization: When you configure an API within Duende IdentityServer, you can use this scope to restrict access to that API. It ensures that only clients or services with the appropriate tokens containing this scope can access the API.  
*******************************

#### Explain what a JSON Web Token (JWT) is and how it works?

JSON Web Token, or JWT, is a standardized format used for securely transmitting information between parties as a JSON object. It's widely used for authentication and authorization purposes. 

A JWT is composed of three parts:

1. **Header**: This part usually consists of two elements: the type of token (which is JWT) and the signing algorithm being used, such as HMAC SHA256 or RSA.

2. **Payload**: This contains the claims, which are statements about an entity (typically the user) and additional data. Claims can be of three types:
   - **Registered Claims**: These are predefined claims like `sub` (subject), `exp` (expiration), and `iat` (issued at).
   - **Public Claims**: These are custom claims that can be defined by the user and should be registered to avoid collisions.
   - **Private Claims**: Custom claims agreed upon by the parties using the JWT.

3. **Signature**: To create the signature part, you take the encoded header and payload, combine them with a secret key (or private key in the case of asymmetric algorithms), and apply the specified algorithm. This signature ensures that the token hasn’t been altered and verifies its authenticity.

**How it works in practice**:
- **Authentication**: When a user logs in, the server generates a JWT that includes information about the user and their permissions. This token is sent to the client, which stores it (often in local storage or cookies).
- **Authorization**: For subsequent requests, the client includes the JWT in the `Authorization` header of the HTTP request, typically using the `Bearer` schema. The server then validates the JWT's signature and extracts the user’s information and claims from the payload to authorize access to resources.

**Benefits**:
- **Stateless**: The server does not need to store session state, as all the necessary information is contained within the token.
- **Decentralized**: JWTs can be easily passed between different systems and services, which is useful in microservices architectures.

**Considerations**:
- **Security**: Since JWTs contain sensitive information, they should be transmitted over secure channels (like HTTPS), and the signing key must be kept confidential to prevent unauthorized access.

In summary, JWTs are a powerful tool for managing user authentication and authorization in a stateless manner, enhancing both security and scalability in distributed systems.
