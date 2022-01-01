# Azure Notification Hubs Fundamentals

## Features
- Cross platform: Front-end and back-end
- Multiple delivery formats: Pus to user, Push to device, localisation, Silent Push
- Telemetry
- Scalable

## Components
- Platform Notification service (PNS) (vendor-specific)
- Notification hub: you code communicates with this and then this communicates with PNS
- Namespace: Regional collection hubs

# Namespaces and registering devices
## Notification Hubs and Namespaces
- Namespace is a collection of Notification Hubs
- One namespace per application
- Within that namespace create one hub per application environment (ie: production, development, testing etc.)
- You can access all the Notification Hubs within a namespace with the credentials supplied on the namespace level
- Billing at namespace level

## Sending notifications workflow
- Setup PNS: Vendor specific implementation for each platform
- Setup ANH: Create namespace and hub through portal
- Map PNS to ANH
- Register devices: use .NET SDK Web api backend
- Send pushes: .NET SDK via web api


