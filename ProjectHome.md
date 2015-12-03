Nick's OAuth Server is a Visual Studio 2013 Express project I used to learn how to implement the Resource Owner Password Credential grant using Microsoft's WebAPI 2 and the OAuth Middleware. It also implements refresh tokens and supports access token revocation. This is an ongoing research project of mine, and it is not guaranteed to be secure, though I put security controls in place to try to address threats I could think of. So far, I have only had time to get the OAuth grants mentioned above working. I have not reviewed the software for security vulnerabilities.

# Updates #
July 3, 2014: The project now supports the Client Credentials Grant.

# Features #
  * WebAPI VS 2013 Express Project
  * Implements an Authorization Server for the OAuth Resource Owner Password Credentials Grant and Client Credentials Grant
  * Implements revocation of access tokens
  * Implements refresh tokens
  * Designed to be multi-tenant. Has "Organizations"
## Grant Types ##
When implementing an OAuth server, there are four main grant types identified by RFC 6749:
  * Authorization Code
  * Implicit
  * Resource Owner Password Credentials
  * Client Credentials
Each of these have specific use cases and security concerns. Please refer to the RFC to identify which grant is appropriate for your OAuth Server and client application.
## Blog Articles ##
  * [Initial Blog Post](http://blog.securityps.com/2014/06/oauth-resource-owner-password.html)
  * More to come...
# Built In Accounts #
## Organizations ##
Three grocery stores are used as seed data for my organizations. I have no relationship to these grocery stores other than shopping at them. Cooking is a wonderful past time.
  * Hy-Vee
  * Hen House
  * Price Chopper

## Clients (as in a mobile phone application or a Single Page Application) ##
Each of these has a "Client ID" and "Client Secret" (which is a hashed password). Open the dbo.OAuthClients table and look up the ClientId column for each client to get the proper client id (a Guid).
### Mobile API for Hy-Vee ###
  * Client Id: <Obtain GUID from database, Table name is OAuthClients>
  * Client Secret: Start123!
### Mobile API for Hen House ###
  * Client Id: <Obtain GUID from database, Table name is OAuthClients>
  * Client Secret: Start123!
### Mobile API for Price Chopper ###
  * Client Id: <Obtain GUID from database, Table name is OAuthClients>
  * Client Secret: Start123!

## Users ##
### firsthenhouseuser ###
  * Username: firsthenhouseuser
  * Password: Start123!
  * Organization: Hen House
### firsthyveeuser ###
  * Username: firsthyveeuser
  * Password: Start123!
  * Organization: Hy-vee
### firstpricechopperuser ###
  * Username: firstpricechopperuser
  * Password: Start123!
  * Organization: Price Chopper

# URL Locations and Sample Requests #
## Obtaining an Access Token Using the Resource Owner Password Credentials Grant ##
Important! You need to submit the Client ID and Client Secret in a Basic Authentication header and submit the user's username and password. Their organization must match. The Basic Authentication header value can be created by:
```
Base64Encode(client id + ":" + client secret)
```
For Example:
```
Base64Encode("013a391f-4945-443a-913f-4527ca1b2ec9:Start123!")
```
Request:
```
POST /Token HTTP/1.1
Host: localhost:44306
Accept: */*
Accept-Language: en
User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Win64; x64; Trident/5.0)
Connection: close
Authorization: Basic MDEzYTM5MWYtNDk0NS00NDNhLTkxM2YtNDUyN2NhMWIyZWM5OlN0YXJ0MTIzIQ==
Content-Type: application/x-www-form-urlencoded
Content-Length: 65

grant_type=password&username=firsthenhouseuser&password=Start123!
```

You will receive a JSON response with an authorization and refresh token. Grab each to be used in future requests.

```
HTTP/1.1 200 OK
Cache-Control: no-cache
Pragma: no-cache
Content-Length: 733
Content-Type: application/json;charset=UTF-8
Expires: -1
Date: Wed, 18 Jun 2014 21:13:16 GMT
Connection: close

{"access_token":"mLCbWig_jRsLPw2BZ57d5QIEpZwkjjRSDIN2Q_c2TnFAHH4qrX8GwyBAwMAdCQjO7db0zejYr7QSICDkEs34JxBEHtzOr2SHwHLfkjv45TU5FdaJomhfxJHAdgo3HQoDcnXUOphCjU1o1THbzkXD31_G-oK_BzVhvNP8x0oizpN4r7YQjDjE4JS8bntCHmRak1JepoMmtye4xFee7UI82u1m0cNXFJqnaP0bW9jB-GybZibufnFIrf0w0JD1ekfhK0UFmzQWbLTMWn-jaCEB7--Nr7pMKGnp0KCmQW4ETnAqwAjTYY5mIlYC9-eQna8ak1fDm95WA3I0uU0de32H14-opBtT2vW4CoS1d_Fu3nqoQa-Jr2hpSFaY9J9YBcit2KV659ZyNEAkiBNzfMhuZqU1F8jkggbzDO0yKmQWWXpoTU5ZmQyiySMbm6jGEhkNbuGvXryqZ9hBKz8FZYppRGH2yL7CxXuyFfeF79cD1WpJeXPeXeC0iITTRc0_6xVrzwqsvTQgiNJFg8KyFmYwxw","token_type":"bearer","expires_in":3599,"refresh_token":"73bf7349-76a4-4690-973f-f038fae30f93",".issued":"Wed, 18 Jun 2014 21:13:16 GMT",".expires":"Wed, 18 Jun 2014 22:13:16 GMT"}
```

## Accessing a Resource using the Access Token ##
Use the "Authorization" HTTP header with the value "Bearer" + access token. You don't need the client id or client secret. Like this:
```
GET /api/Values/5 HTTP/1.1
Host: localhost:44306
Accept: */*
Accept-Language: en
User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Win64; x64; Trident/5.0)
Connection: close
Content-Type: application/x-www-form-urlencoded
Content-Length: 540
Authorization: Bearer aurfY7lKT7onSZZhqBDSOujWGHUyxFWYvLm61jVxG2q455yFjfW6dpOEoW9g41UP5oynkZ69EwfowL4yrYELhBT31DGZVJ1Z3FEo8mhPrVwqAkDeZZfMik-1FPJ-DcHadfuwFEcVIqUXFTdhAhBXVrm7HOQD_uJsHsh9yijTtc7CRMHhwtMNa56QjM4m1MRtoz-I3pMdb1KZuOcLXjwItCpPFfCPUzKwp_EGykPuI9CP2ZSie5I4-n8pcEKUt9fxbnNHqnS9G8Sd5SWtfivDoo4VMu1Fw81FOGo_CUh_BazJPeF6BYo0k9eGAzBEH7-l6tqOZzFC1x6OGjqjNx0sIDw9mDQDlVRfP8EidFQ1HC6T54U8ciyWyRL_4TRhymdJyD_P_y_fpV7jllheHtoHF1d0vefBu_uJHGWdseknmBt7KvGI4uDro9hA3ZExWfLRTNkcuSYKsWRe2Kz-KFepT-fXg72Avh7266n8U7AGCbPdHtKJ48kLRK1GgPgeODLnc9cNg6l7gWcKAKdXYSrX7A
```

## Getting a New Access Token using the Refresh Token ##
Client Id and Client Secret in Basic Authentication header required!

```
POST /Token HTTP/1.1
Host: localhost:44306
Accept: */*
Accept-Language: en
User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Win64; x64; Trident/5.0)
Connection: close
Authorization: Basic MDEzYTM5MWYtNDk0NS00NDNhLTkxM2YtNDUyN2NhMWIyZWM5OlN0YXJ0MTIzIQ==
Content-Type: application/x-www-form-urlencoded
Content-Length: 75

grant_type=refresh_token&refresh_token=73bf7349-76a4-4690-973f-f038fae30f93
```
Response:
```
HTTP/1.1 200 OK
Cache-Control: no-cache
Pragma: no-cache
Content-Length: 721
Content-Type: application/json;charset=UTF-8
Expires: -1
Date: Wed, 18 Jun 2014 21:13:58 GMT
Connection: close

{"access_token":"rz--x7zuijIvJKm1TUYLwPIGkfElGrsYZse3YSH1VY9kkA5WHjTqg58dNsOtZaYlCi5XyXI5fgmW2Ox3rkOC8XNDx0IdfzLYKaQexOsiGtvjZ3eglQ_ya-rHMXDF0P5UUOchr0-_o0deaGT6-rHKKyT2dyeictS65F4vR3eUlf6GHqYsM4Z0YYrPGS9fojRpZSyQ4DVsOodqhAVjnfbgSSpN1QILJaB0cWfzeSgcIdYW1-f8b2vvZqjJZOLiOUq3CZDnu97EF4r-FJeV95cwX__gXHH0VvJVBQZPokVL86mB8VOz4NsVmBcWYuyqUFNYiHnLcGa6VkBpnfsrzSEB65z5ZtMV9om9OVlUTaKbJhCHhim4cmt5EpCHnHnAchz8z_HuwEVbmO1u33mRZFL7PF-3QLpzbwp6GYmai4cjxFNbeoHicQQ-9_ZBE9_WrvBEVCn9TRCR4VmwXSvyRd9VfYLlxh92Eo61pNbhFQXRcC8","token_type":"bearer","expires_in":3599,"refresh_token":"2d4630d1-735c-4235-a33f-6c373cda3f92","userName":"firsthenhouseuser",".issued":"Wed, 18 Jun 2014 21:13:58 GMT",".expires":"Wed, 18 Jun 2014 22:13:58 GMT"}
```

## Revoking All Tokens for a Particular User ##
Client Id and Client Secret in Basic Authentication header required!
```
POST /api/Account/Revoke HTTP/1.1
Host: localhost:44306
Accept: */*
Accept-Language: en
User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Win64; x64; Trident/5.0)
Connection: close
Authorization: Basic NjdlNmVkM2UtMGE3Yy00NjQ3LWFlOGItODQ4YThmMGRhYTE3OlN0YXJ0MTIzIQ==
Content-Type: application/x-www-form-urlencoded
Content-Length: 540

token=2kLwS6EWXc_LlgCj0QIJ2NeFBtZK9iFVXbCP_j8WbdyDuVj4kdDAErFl1fZsdbJZyPCHXqoIhq4Chf8NivhAKmCDjSqbrILGRtFfJ3L2lPYKYXLn5CdSizxcEoD7mtoVeB3CO-yuw0zN77o_Jsk4BvGvtRse2kaiq1yl7kdExOjCzdPFuv3_bpDcMyZyklU8C_s3PiEJic71pqXMxyROmBuA2GhqhCzlKUU5HcNx9BrFde27s1Bq8wcGUie9i2d10K5eHbu7w4AzVSPpUAsLTOiWncZwum9QLMWByKWHNr6tzYRAq1008uA1mwMskjIGxExTkmCuYH2e3ZfvjhK8aNZUzmPyjShQeDxqknQhXqsLL_eplDPzXWRgS70t0tjrRZbZN17jr2z1xxUCDMygyLyNSOgVmcenc-S0KZRYGqmN-a8d0ckawqh37pGIl7nrTEnjlsh-RqO0dXST_8NweEmcHkk7N_Rvo7cX2k5OiAekV4ZmLLkPdAB5mVSO4Mi3KzGiOxdIQ_XeFYn8UzQOyg
```

## Obtaining an Access Token Using the Client Credentials Grant ##
The Clients for the Resource Owner Password Credentials Grant and the Client Credentials Grant are not the same. The grants do not share clients. The database includes a Hy-Vee client specifically for client credentials. Use that Client ID and Client Secret and submit it as a Basic Authentication header.

```
POST /token HTTP/1.1
Host: localhost:44306
Accept: */*
Accept-Language: en
User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Win64; x64; Trident/5.0)
Connection: close
Content-Type: application/x-www-form-urlencoded
Authorization: Basic ZDUwMDExYzQtZTBhMC00YzNjLWEyYTQtMjJiY2JkOTA2MjFhOlN0YXJ0MTIzIQ==
Content-Length: 29

grant_type=client_credentials
```

When "ZDUwMDExYzQtZTBhMC00YzNjLWEyYTQtMjJiY2JkOTA2MjFhOlN0YXJ0MTIzIQ==" is base64 decoded, it looks like this:
```
d50011c4-e0a0-4c3c-a2a4-22bcbd90621a:Start123!
```

The response from the server will look like this:
```
HTTP/1.1 200 OK
Cache-Control: no-cache
Pragma: no-cache
Content-Length: 626
Content-Type: application/json;charset=UTF-8
Expires: -1
Date: Thu, 03 Jul 2014 15:19:41 GMT
Connection: close

{"access_token":"saWOZ5NSs4RuujmU4DuMBTZUTjQq2sl30qzx-ZxNxYry1IHYbkdVUYDUFc367j6P6aHoTxFHSAehy8WXsAZIlceKW4X3H5SGGMRsKLhRFl-F5oI-D_xxHpJu-8dKJw-dCw-ELCTADVaGE_j4oi3hTHdTLPMiMGpppd1qYAdGn7PNYLF8d6NLZnXitbGWjP5gkCnJGLHbd5EBFfNQXkv0qFdbe29ToFtKwcfrcqHk7nRo_Wy09FghB05KOa2fSRskh3KK2NVSvJ0ZKDirjWwnmLZkPHGu8xvYZT-FnXPOQJl8cVcWWYMFo-ubhRbYbSziL3DCXc3-BNu6JvIqqGKCiJKwU8zBnLRz-QbfU9VoeEzzVVYL7Efd71EGxXi9co0S2mSBN-Z5qCM9135xZcxxRg","token_type":"bearer","expires_in":299,"userName":"d50011c4-e0a0-4c3c-a2a4-22bcbd90621a","scope":"values-available",".issued":"Thu, 03 Jul 2014 15:19:40 GMT",".expires":"Thu, 03 Jul 2014 15:24:40 GMT"}
```

This access token is permitted to access only one method currently. Here's an example request and response.
```
GET /api/values/ping HTTP/1.1
Host: localhost:44306
Accept: */*
Accept-Language: en
User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Win64; x64; Trident/5.0)
Connection: close
Authorization: Bearer saWOZ5NSs4RuujmU4DuMBTZUTjQq2sl30qzx-ZxNxYry1IHYbkdVUYDUFc367j6P6aHoTxFHSAehy8WXsAZIlceKW4X3H5SGGMRsKLhRFl-F5oI-D_xxHpJu-8dKJw-dCw-ELCTADVaGE_j4oi3hTHdTLPMiMGpppd1qYAdGn7PNYLF8d6NLZnXitbGWjP5gkCnJGLHbd5EBFfNQXkv0qFdbe29ToFtKwcfrcqHk7nRo_Wy09FghB05KOa2fSRskh3KK2NVSvJ0ZKDirjWwnmLZkPHGu8xvYZT-FnXPOQJl8cVcWWYMFo-ubhRbYbSziL3DCXc3-BNu6JvIqqGKCiJKwU8zBnLRz-QbfU9VoeEzzVVYL7Efd71EGxXi9co0S2mSBN-Z5qCM9135xZcxxRg
```

```
HTTP/1.1 200 OK
Cache-Control: no-cache
Pragma: no-cache
Expires: -1
Date: Thu, 03 Jul 2014 15:19:55 GMT
Connection: close
Content-Length: 0
```
# Resources #
I drew upon the following websites and authors in order to write this software:
  * [OAuth 2.0 RFC](http://tools.ietf.org/html/rfc6749)
  * [OAuth 2.0 Revocation RFC](http://tools.ietf.org/html/rfc7009)
  * [OAuth 2.0 Bearer Token Usage](http://tools.ietf.org/html/rfc6750)
  * [OAuth 2.0 Threat Model and Security Considerations](http://tools.ietf.org/html/rfc6819)
  * [NDC2014 Owin and Katana: What the Func? by Brock Allen](http://vimeo.com/97329189)
  * Most helpful and a great starting point: http://www.tugberkugurlu.com/archive/simple-oauth-server-implementing-a-simple-oauth-server-with-katana-oauth-authorization-server-components-part-1
  * Helped to know which methods are called when on the middleware. Otherwise, this tutorial was hard to follow: http://www.asp.net/aspnet/overview/owin-and-katana/owin-oauth-20-authorization-server
  * http://leastprivilege.com/2014/03/24/the-web-api-v2-oauth2-authorization-server-middlewareis-it-worth-it/
  * http://leastprivilege.com/2013/11/13/embedding-a-simple-usernamepassword-authorization-server-in-web-api-v2/
  * http://leastprivilege.com/2013/11/15/adding-refresh-tokens-to-a-web-api-v2-authorization-server/
  * http://leastprivilege.com/2013/11/25/dissecting-the-web-api-individual-accounts-templatepart-1-overview/
  * http://leastprivilege.com/2013/11/26/dissecting-the-web-api-individual-accounts-templatepart-2-local-accounts/
  * http://leastprivilege.com/2013/11/26/dissecting-the-web-api-individual-accounts-templatepart-3-external-accounts/

# Sequence Diagrams #
  * [PDF Format](http://nicksoauthserver.googlecode.com/svn/trunk/Resource%20Owner%20Password%20Grant%20-%20Web%20API%202.pdf)
  * [PNG Format](http://nicksoauthserver.googlecode.com/svn/trunk/Resource%20Owner%20Password%20Grant%20-%20Web%20API%202.png)
  * [SVG Format](http://nicksoauthserver.googlecode.com/svn/trunk/Resource%20Owner%20Password%20Grant%20-%20Web%20API%202.svg)
  * [Original UMLet File](http://nicksoauthserver.googlecode.com/svn/trunk/Resource%20Owner%20Password%20Grant%20-%20Web%20API%202.uxf)
![http://nicksoauthserver.googlecode.com/svn/trunk/Resource%20Owner%20Password%20Grant%20-%20Web%20API%202.png](http://nicksoauthserver.googlecode.com/svn/trunk/Resource%20Owner%20Password%20Grant%20-%20Web%20API%202.png)

# Modified Code Locations #
I started with the default Visual Studio Express 2013 WebAPI 2 project with the "Individual Accounts" option selected. From there, Here's a list of files I remember modifying:
|App\_Start|
|:---------|
|---->Startup.Auth.cs|Modified, Configure OAuth Providers and options, add applicationdbcontext to owin, |
|---->IdentityConfig.cs|
|-------->FindWithLockoutAsync method|new, supports lockout|
|-------->Create Method|modified, enable options for lockout|
|Controllers|
|---->AccountController.cs|
|-------->PostRevoke method|new, revoke OAuth tokens|
|Migrations|
|---->Configuration.cs|
|-------->Seed method|new, set up users, orgs, clients, etc.|
|Models    |
|---->AccountBindingModels.cs|
|-------->OAuthTokenRevocationBindingModel class|new, support post parameters for token revocation action|
|---->IdentityModel.cs|
|-------->ApplicationUser class|modified, added some fields|
|-------->ApplicationDbContext class|modified, added some dbsets|
|-------->OAuthClients.cs|new, clients in DB|
|-------->OAuthSessions.cs|new, sessions in DB|
|-------->Organizations.cs|new, organizations in DB|
|Providers |
|---->NicksApplicationOAuthProvider.cs|modified, almost everything changed. Responsible for handling /Token requests|
|---->NicksRefreshTokenProvider.cs|new, generates and receives refresh tokens|
|---->OAuthBearerAuthenticationWithRevocationProvider.cs|Like an authorization filter, but used to authenticate the user via bearer tokens. checks to see if token has been revoked|
|---->OAuthGrant.cs|new, defines grant types to allow clients to be associated with a specific one|