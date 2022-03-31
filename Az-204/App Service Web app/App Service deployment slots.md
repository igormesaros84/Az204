# Table of content
- [Table of content](#table-of-content)
- [Explore staging environments](#explore-staging-environments)
- [Slot swapping](#slot-swapping)
- [Route traffic in App Service](#route-traffic-in-app-service)
- [Routing manually](#routing-manually)
# Explore staging environments
When you deploy your we app to Azure App Service, you can use a separate deployment slot instead of the default production slot when you're running in the Standard, Premium, or Isolated App Service plan tier.
Each App Service plan tier supports a different number of deployment slots. There's no additional charge for using deployment slots. 
# Slot swapping
1. Apply settings from target slot(ie. production slot) to all instances of the source slot:
    - Slot-specific app settings and connection strings, if applicable
    - Continuos deployment settings, if enabled.
    - App Service authentication settings, if enabled.

    Any of these cases trigger all instances in the source slot to restart. During swap with preview, this marks the end of the first phase. The swap operation is paused and you can validate that the source slot works correctly with the target slot's settings.

2. Wait for every instance in the source slot to complete its restart. If any fail to restart the swap operation reverts all changes and stops operation.
3. If local cache is enabled, trigger local cache initialization by making an HTTP request to the application root ("/") on each instance of the source slot. Local cache initialization causes another restart.
4. If auto swap is enabled with custom warm-up, trigger Application Initiation by making an HTTP request to the application root ("/") on each instance of the source slot.
    - If `applicationInitialization` inst specified, trigger an HTTP request to the application root of the source slot on eah instance.
    - If an instance returns any HTTP response, it's considered to be warmed up.
5. If all instances on the source slot are warmed up successfully, swap the two slots by switching the routing rules for the two slots.
6. Now that the source slot has the pre-swap app previously in the target slot, perform the same operation by applying all settings and restarting the instances.

Some configuration elements follow the content across a swap (not slot specific), whereas other configuration elements stay in the same slot after a swap (slot specific).
| Settings that are swapped | Settings that aren't swapped |
|---------------------------| ---------------------------- |
|General settings, such as framework version, 32/64 bit, web sockets | Publishing endpoints |
|App settings (can be configured to stick to slot) | Custom domain names |
| Connection strings (can be configured to stick to a slot) | Non-public certificates and TLS/SSL settings |
| Handler mappings | Scale settings |
|Public certificates | WebJobs schedulers |
| WebJobs content | IP restrictions |
|Hybrid connections * | Always On |
| Virtual network integration* | Diagnostic log settings |
|Service endpoints* | Cross-origin resource sharing (CORS)|
|Azure Content Delivery Networks* |

Features marked with an asterisk (*) are planed to be un-swapped.

To make a slot swappable, add the app setting `WEBSITE_OVERRIDE_PRESERVE_DEFAULT_STICKY_SLOT_SETTINGS` in every slot of the app and set its value to `0` or `false`.
# Route traffic in App Service
By default all client requests to the app's production URL (`http://<app_name>.azurewebsites.net`)are routed to the production slot. You can rout a portion of the traffic to another slot.
On the client browser, you can see which slot your session is pinned to by looking ath the `x-ms-routing-name` cookine in your HTTP headers. A request that's routed to the "staging" slot has the cookie `x-ms-routing-name=staging`. A request that's routed to the production slot has the cookie `x-ms-routing-name=self`.
# Routing manually
To let users opt out of your beta app, for example, you can put this link on your webpage:
```
<a href="<webappname>.azurewebsites.net/?x-ms-routing-name=self">Go back to production app</a>
```


