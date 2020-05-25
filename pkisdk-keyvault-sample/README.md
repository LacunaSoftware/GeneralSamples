# PKI SDK Sample with Azure Key Vault
This is a ASP.NET Core 3.1 project that performs a signature using PKI SDK
using a imported certificate from Azure Key Vault.

## Configuration
Open [appsettings.Development.json](/KeyVaultSample/appsettings.Development.json#L2-L6)
file and add Azure Key Vault parameters for authentication. 

```json
"AzureKeyVault": {
    "Endpoint": "KEY VAULT ENDPOINT",
    "AppId": "YOUR APPLICATION ID",
    "AppSecret": "YOUR APPLICATION SECRET"
},
```

In the section [Configure Key Vault](#configure-key-vault),
you can follow the steps to create these required parameters.

## Execution

Steps to execute the sample:
1. Open the solution file (.sln) on Visual Studio
1. Run the solution. Make sure your system allows automatic Nuget package restore (if it doesn't, manually restore the packages)

This project is served at https://localhost:44337/ and has two API endpoints:

`POST /api/certificates/import`
This action will import a PKCS#12 file to Azure Key Vault.
It will return a identifier for that certificate, which is necessary to retrieve that certificate in the signature action.
This value should be stored on a database instead of return on the API.

Parameters:
| Name | Type | Mandatory | Description |
| pkcs12 | string | yes | The content of the PKCS#12 file |
| pkcs12Password | string | yes | The password of the PKCS#12 file. If it doesn't have one, pass an empty string |

Response:
```
{
	"certId": "00000000-0000-0000-0000-000000000000"
}
```

`POST /api/signatures`
This action will retrieve the PKCS#12 file from Azure Key Vault and perform a signature.
It will store the signature file in the server and provided it's identifier as response.

Parameters:
| Name | Type | Mandatory | Description |
| certId | Guid | yes | The identifier of the certificate store on Azure Key Vault |
| pkcs12Password | string | yes | The password of the PKCS#12 file. If it doesn't have one, pass an empty string |

Response:
```
{
	"fileId": "00000000-0000-0000-0000-000000000000"
}
```


## Configure Key Vault
To configure the Azure Key Vault store, that is used in this project, log in
[Azure](https://portal.azure.com/) and use the following settings:

### Create Application And Authentication Secreat
Create an application corresponding to your application instance:
1. In the option **Azure Active Directory**, go to **App registrations** and click in **New registration**
1. Enter a name for the application
1. Leave the rest of the fields in the default options
1. Click on **Register**

Once the application is created, take note of the **Application (client) ID**.
This value will be used in `AppId` parameter on [Configuration](#configuration).

Generate a secret for applicatio identification:
1. In the application settings, click in **Certificate & secrets**
1. In **Client secrets**, click in **New client secret**
1. Fill in a description and choose validity **Never** (does not expire)
1. Click on **Add**
1. **Copy the displayed value** in column **Value** (it will not be possible to recover this value later!)

### Creating the Key Vault
Create a Key Vault (skip this part if you want to use an existing Key Vault):
1. In the option **Key Vaults**, click in **Add**
1. Fill in the data according to your infrastructure
1. Click on **Review + create**
1. Click on **Create**

Once the creation of the Key Vault is complete, click **Go to resource**. Then
take note of the Key Vault's **DNS Name** (ex: `https://my-key-vault.vault.azure.net`).
This value will be used in `Endpoint` parameter on [Configuration](#configuration).

Grant permissions to the application:
1. In the Key Vault settings, click in **Access policies**, after in **Add Access Policy**
1. In *Configure from template*, do not fill anything
1. In *Secret permission*, select **Get** and **Set**.
1. In *Select principal*, select the created application and click **Select**
1. Leave the *Key permissions*, *Certificate permissions* and *Authorized application* fields unchanged
1. Click on **Add**
1. Back in the *policies* screen, click **Save**

> **Warning**
> This last step (click on **Save** button after clicking **Add** is necessary,
> otherwise permissions are **not** saved

