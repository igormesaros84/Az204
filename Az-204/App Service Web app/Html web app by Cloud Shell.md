# Create a static HTML web app by using Azure Cloud Shell

After gathering information about App Service you've decided to create and update a simple web app to try it out. In this exercise you'll deploy a basic HTML+CSS site to Azure App Service by using the Azure CLI az webapp up command. You will then update the code and redeploy it by using the same command.

The az webapp up command makes it easy to create and update web apps. When executed it performs the following actions:

Create a default resource group.
Create a default app service plan.
Create an app with the specified name.
Zip deploy files from the current working directory to the web app.

---

## Login to Azure and download the sample app

### Step 1
Login to azure portal and open *Cloud Shell* and make sure to select `Bash` environment

### Create working directory and navigate to it
`mkdir htmlapp cd $_`

### Clone the sample app repo to the *htmlapp* directory
`git clone https://github.com/Azure-Samples/html-docs-hello-world.git`

---

## Create the web app

### Change to the directory that contains the sample code and run the az webapp up command.

```
cd html-docs-hello-world

az webapp up --location uksouth --name imWebAppTest --html
```
This command may take a few minutes to run. While running, it displays information similar to the example below. Make a note of the resourceGroup value. You need it for the Clean up resources section later.
```
{
  "URL": "http://imwebapptest.azurewebsites.net",
  "appserviceplan": "igorm84_asp_3746",
  "location": "uksouth",
  "name": "imWebAppTest",
  "os": "Windows",
  "resourcegroup": "igorm84_rg_4622",
  "runtime_version": "-",
  "runtime_version_detected": "-",
  "sku": "FREE",
  "src_path": "//home//igor//hrmlapp//html-docs-hello-world"
}
```
---
## Update and redeploy the app
1. In the *Cloud Shell*, type `code index.html` to open the editor. In the `<h1>` heading tag, change the *Azure App Service Sample Static HTML Site* to *Azure App Service Updated*
2. Use the commands **ctrl-s** to save and **ctrl-q** to exit.
3. Redeploy the app with the same az webapp up command.
```
az webapp up --location uksouth --name imWebAppTest --html
```
4. Once complete refresh the page opened before or navigate to the *URL* returned from the JSON that was displayed at the end of the depoloyment
---
## Clean up 
```
az group delete --name <resource_group> --no-wait
```