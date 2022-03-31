- [Examine Azure App Service](#examine-azure-app-service)
  - [Limitations](#limitations)
- [Examine Azure App Service plans](#examine-azure-app-service-plans)
  - [How does my app run and scale?](#how-does-my-app-run-and-scale)
- [Deploy to App Service](#deploy-to-app-service)
  - [Automated deployment](#automated-deployment)
  - [Manual deployment](#manual-deployment)
- [Explore authentication and authorization in App Service](#explore-authentication-and-authorization-in-app-service)
- [Discover App Service Networking features](#discover-app-service-networking-features)
  - [Find outbound IPs](#find-outbound-ips)


# Examine Azure App Service
Azure App Service is an HTTP-based service for hosting web applications, REST APIs, and mobile back ends. It can be developed in .NET, .NET Core, Java, Ruby, Node.js, PHP, or Python. Applications run and scale with ease on both Windows and Linux-based environments.

- Built-in auto scale support
- Continuous integration/deployment support
- Deployment slots

## Limitations
App Service on Linux does have some limitations:
- App Service on Linux is not supported on Shared pricing tier.
- You can't mix Windows and Linux apps in the same App Service plan.
- Historically, you could not mix Windows and Linux apps in the same resource group. However, all resource groups created on or after january 21, 2021 do support this scenario
- Azure portal shows only features that currently work for Linux apps

# Examine Azure App Service plans
In App Service, an app (Web Apps, API Apps, or Mobile Apps) always runs in an *App Service plan*. An App Service plan defines a set of compute resources for a web app to run. One or more apps can be configured to run on the same computing resources (or in the same App Service plan). In addition, Azure Functions also has the option of running in an App Service plan.

*The pricing tier* of an App Service plan determines what App Service features you get and how much you pay for the plan. There are a few categories of pricing tiers:
- **Shared compute**: Both **Free** and **Shared** share the resource pools of your apps with the apps of other customers. These tiers allocate CPU quotas to each app that runs on the shared resources, and the resources can't scale out.
- **Dedicated compute**: The **Basic**, **Standard**, **Premium**, **Premium V2**, and **Premium V3**, tiers run apps on dedicated Azure VMs. The higher the tier, the more VM instances are available to scale out.
- **Isolated**: this tier runs dedicated Azure VMs on dedicated Azure Virtual Networks. It provides the maximum scale0out capabilities.
- **Consumption**:This tier is only available to *function apps*. It scales the functions dynamically depending on workload.

> App Service Free and Shared (preview) hosting plans are base tiers that run on the same Azure virtual machines as other App Service apps. Some apps might belong to other customers. These tiers are intended to be used only for development and testing purposes.

## How does my app run and scale?
In the Free and Shared tiers, an app receives CPU minutes on a shared VM instance and can't scale out. In other tiers, an app runs and scales as follows:

An app runs on all the VM instances configured in the App Service plan.
If multiple apps are in the same App Service plan, they all share the same VM instances.
If you have multiple deployment slots for an app, all deployment slots also run on the same VM instances.
If you enable diagnostic logs, perform backups, or run WebJobs, they also use CPU cycles and memory on these VM instances.

Isolate your app into a new App Service plan when:

The app is resource-intensive.
You want to scale the app independently from the other apps in the existing plan.
The app needs resource in a different geographical region.

# Deploy to App Service
App Service supports both automated and manual deployment.

## Automated deployment
Azure supports automated deployment directly from several sources.
- Azure Devops
- Github
- Bitbucket

## Manual deployment
There are a few options that you can use to manually push your code to Azure
- Git
- CLI
- Zip deploy
- FTP

# Explore authentication and authorization in App Service
Azure App Service provides built-in authentication and authorization support, so you can sign in users and access data by writing minimal or no code in your web app.

# Discover App Service Networking features
By default, apps hosted in App Service are accessible directly through the internet and can reach only internet-hosted endpoints. But for many applications, you need to control the inbound and outbound network traffic.

## Find outbound IPs
You can find the information by running the following command in the Cloud Shell. They are listed in the Additional Outbound IP Addresses field.

```
az webapp show \
    --resource-group <group_name> \
    --name <app_name> \ 
    --query outboundIpAddresses \
    --output tsv
```

To find all possible outbound IP addresses for your app, regardless of pricing tiers, run the following command in the Cloud Shell.

```
az webapp show \
    --resource-group <group_name> \ 
    --name <app_name> \ 
    --query possibleOutboundIpAddresses \
    --output tsv
```
