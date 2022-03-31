- [Explore Azure Key Vault](#explore-azure-key-vault)
  - [Key benefits of using Azure Key Vault](#key-benefits-of-using-azure-key-vault)
- [Discover Azure Key Vault best practices](#discover-azure-key-vault-best-practices)
  - [Authentication](#authentication)
  - [Encryption of data in transit](#encryption-of-data-in-transit)
  - [Azure Key Vault best practices](#azure-key-vault-best-practices)
- [Authenticate to Azure Key Vault](#authenticate-to-azure-key-vault)
  - [Authentication to Key Vault in application code](#authentication-to-key-vault-in-application-code)
  - [Authentication to Key Vault with REST](#authentication-to-key-vault-with-rest)
- [Exercise: Set and retrieve a secret from Azure Key Vault by using Azure CLI](#exercise-set-and-retrieve-a-secret-from-azure-key-vault-by-using-azure-cli)
  - [Create a Key Vault](#create-a-key-vault)
  - [Add and retrieve a secret](#add-and-retrieve-a-secret)
  - [Clean up resources](#clean-up-resources)

# Explore Azure Key Vault
Azure Key Vault service supports two types of containers: *vaults* and *managed hardware security module* (HSM) pools. Vaults support storing software and HSM-backend keys, secrets, and certificates. Managed HSM pools only support HSM-backed keys.

Azure Key Vault helps solve the following problems:

- **Secrets Management**: Azure Key Vault can be used to Securely store and tightly control access to tokens, passwords, certificates, API keys, and other secrets

- **Key Management**: Azure Key Vault can also be used as a Key Management solution. Azure Key Vault makes it easy to create and control the encryption keys used to encrypt your data.

- **Certificate Management**: Azure Key Vault is also a service that lets you easily provision, manage, and deploy public and private Secure Sockets Layer/Transport Layer Security (SSL/TLS) certificates for use with Azure and your internal connected resources.

## Key benefits of using Azure Key Vault
- **Centralized application secrets**: Centralizing storage of application secrets in Azure Key Vault allows you to control their distribution. For example, instead of storing the connection string in the app's code you can store it securely in Key Vault. 
- **Securely store secrets and keys**: Access to a key vault requires proper authentication and authorization before a caller (user or application) can get access. Authentication is done via Azure Active Directory. Authorization may be done via Azure role-based access control (Azure RBAC) or Key Vault access policy. 
- **Monitor access and use**: You can monitor activity by enabling logging for your vaults. 
- **Simplified administration of application secrets**: Security information must be secured, it must follow a life cycle, and it must be highly available.

# Discover Azure Key Vault best practices
## Authentication
To do any operations with Key Vault, you first need to authenticate to it. There are three ways to authenticate to Key Vault:

- **Managed identities for Azure resources**: When you deploy an app on a virtual machine in Azure, you can assign an identity to your virtual machine that has access to Key Vault. We recommend this approach as a best practice.
- **Service principal and certificate**: You can use a service principal and an associated certificate that has access to Key Vault.
- **Service principal and secret**: Although you can use a service principal and a secret to authenticate to Key Vault, we don't recommend it. It's hard to automatically rotate the bootstrap secret that's used to authenticate to Key Vault.

## Encryption of data in transit
Azure Key Vault enforces Transport Layer Security (TLS) protocol to protect data when it’s traveling between Azure Key Vault and clients. 

## Azure Key Vault best practices
- Use separate key vaults: Recommended to use a vault per application per environment (Development, Pre-Production and Production).
- Control access to your vault: Key Vault data is sensitive and business critical, you need to secure access to your key vaults by allowing only authorized applications and users.
- Backup: Create regular back ups of your vault on update/delete/create of objects within a Vault.
- Logging: Be sure to turn on logging and alerts.
- Recovery options: Turn on soft-delete and purge protection if you want to guard against force deletion of the secret.

# Authenticate to Azure Key Vault
Authentication with Key Vault works in conjunction with Azure Active Directory, which is responsible for authenticating the identity of any given security principal.
> It is recommended to use a system-assigned managed identity.

Below is information on authenticating to Key Vault without using a managed identity.

## Authentication to Key Vault in application code
Key Vault SDK is using Azure Identity client library, which allows seamless authentication to Key Vault across environments with same code.

## Authentication to Key Vault with REST
Access tokens must be sent to the service using the HTTP Authorization header:
```
PUT /keys/MYKEY?api-version=<api_version>  HTTP/1.1  
Authorization: Bearer <access_token>
```

When an access token is not supplied, or when a token is not accepted by the service, an HTTP 401 error will be returned to the client and will include the WWW-Authenticate header

# Exercise: Set and retrieve a secret from Azure Key Vault by using Azure CLI
1. Login to Azure Portal
2. Open cloud shell and select **Bash** environment

## Create a Key Vault
1. Let's set some variables for the CLI commands to use to reduce the amount of retyping.

```
myKeyVault=az204vault-$RANDOM
myLocation=<myLocation>
```

2. Create a resource group

```
az group create --name az204-vault-rg --location $myLocation
```

3. Create a Key Vault by using the `az keyvault create` command

```
az keyvault create --name $myKeyVault --resource-group az204-vault-rg --location $myLocation
```

## Add and retrieve a secret
To add a secret to the vault you need to take a couple of additional steps.

1. Create a secret. Let's add a password that could be used by an app.

```
az keyvault secret set --vault-name $myKeyVault --name "ExamplePassword" --value "hVFkk965BuUv"
```

2. Use the `az keyvault secret show` command to retrieve the secret

this command will return some JSON. THe last line will contain the password in plain text.

```
{
  "attributes": {
    "created": "2022-03-31T14:50:58+00:00",
    "enabled": true,
    "expires": null,
    "notBefore": null,
    "recoveryLevel": "Recoverable+Purgeable",
    "updated": "2022-03-31T14:50:58+00:00"
  },
  "contentType": null,
  "id": "https://az204-10151.vault.azure.net/secrets/ExamplePassword/e4a3912110b845c68db90502c00f8f92",
  "kid": null,
  "managed": null,
  "name": "ExamplePassword",
  "tags": {
    "file-encoding": "utf-8"
  },
  "value": "hVFkk965BuUv"
}
```

## Clean up resources
```
az group delete --name az204-vault-rg --no-wait
```