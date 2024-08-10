# OAuth2: Overview and Usage for Securing Microservices

## Table of Contents
1. [What is OAuth2?](#what-is-oauth2)
2. [OAuth 2.0 Flow and Key Parties](#the-oauth-20-flow-and-key-parties)
3. [How OAuth2 Works](#how-oauth2-works)
4. [OAuth2 Grant Types](#oauth2-grant-types)
5. [Why Use OAuth2 to Secure Microservices?](#why-use-oauth2-to-secure-microservices)
6. [Understanding OAuth2 Scope: A Real-World Example](#understanding-oauth2-scope-a-real-world-example)
7. [OAuth 2.0 Authorization Code Flow with PKCE for PhotoShareApp](#oauth-20-authorization-code-flow-with-pkce-for-photoshareapp)
8. [What is OpenID Connect (OIDC)?](#what-is-openid-connect-oidc)
9. [The Problem OIDC Solves Over OAuth 2.0](#the-problem-oidc-solves-over-oauth-20)
10. [The OIDC Flow](#the-oidc-flow)
11. [Advantages of OpenID Connect](#advantages-of-openid-connect)
12. [Summary](#summary)

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


# OIDC
### What is OpenID Connect (OIDC)?

OpenID Connect (OIDC) is an identity layer built on top of the OAuth 2.0 protocol. It provides a way for applications to verify a user’s identity while still allowing them to gain access to the user’s resources. OIDC extends OAuth 2.0 by adding an additional token called the **ID Token**, which contains information about the user’s identity.

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

### Advantages of OpenID Connect

1. **User Authentication**: OIDC ensures that the app knows who the user is, providing identity verification in addition to resource access.

2. **Preventing Token Misuse**: The ID Token helps ensure that only the legitimate user can use the token, reducing the risk of token misuse.

3. **Single Sign-On (SSO)**: OIDC supports Single Sign-On, enabling users to log in once and access multiple applications without needing to authenticate again.

4. **Enhanced Security**: By adding an identity layer on top of OAuth 2.0, OIDC improves security, ensuring that resources are accessed only by the correct user.

### Summary

OpenID Connect extends OAuth 2.0 by adding user authentication through the ID Token, which allows applications to verify the identity of the user in addition to gaining access to their resources. The flow is similar to OAuth 2.0 but includes the `openid` scope and the additional step of receiving and verifying the ID Token. This helps prevent scenarios where tokens could be misused by unauthorized users, as shown in the example of JohnDoe and JaneSmith.
