- [Azure Resource Manager Fundamentals](#azure-resource-manager-fundamentals)
  - [Resource relationships](#resource-relationships)
- [Using ARM templates](#using-arm-templates)
  - [Where to get templates](#where-to-get-templates)
  - [Deploying an ARM template](#deploying-an-arm-template)
- [Best Practices](#best-practices)

# Azure Resource Manager Fundamentals
ARM Supports massive parallel operations via declarative templates with granular RBAC. Resource providers provide resource types

View all locations
```
Get-AzLocation | Format-Table Location, DisplayName -AutoSize
```

List the resource providers that are registered
```
Get-AzResourceProvider -Location "uksouth"
```

Get all the resource providers
```
Get-AzResourceProvider -Location "uksouth" -ListAvailable
```

Same information can be seen in the Azure portal as well for a given subscription in the `Resource providers` blade.
![resource-provider](Resources/resource-providers.png)

If you wish to create a template for a certain resource the appropriate resource provider needs to be registered.

To look into what's actually in a resource provider
```
Get-AzResourceProvider -ProviderNamespace Microsoft.Compute
```

## Resource relationships
For example a VM depends on a disk which are both separate resources. When they are provisioned azure knows that it needs to create the disc first as the VM depends on it.
![resource-relationships](Resources/resource-relationships.png)

# Using ARM templates
The ARM template has 3 key elements
- **Parameters** - These are values that are provided when the template is being executed.
- **Variables** - In this section you can define logic to define some values. This is so that these sometimes complex variables can be defined here and then just simply reused in the *Resources*.
- **Resources** - These are the resource types that come from the *Resource providers*. 

> **Important!** The template should not have any instance specific values hard coded. The goal is to be able to deploy a template unchanged between environments.

## Where to get templates
1. When creating a resource you always get offered the option to Download the resource as a template.
2. Every resource has an `Export template` blade under the automation section.
3. You can search for deploy custom template and there create a template with the help of the Azure Portal
![deploy-custom-template](resrouces/../Resources/deoploy-custom-template.png)
4. You can find a bunch of templates on [github](https://github.com/Azure/azure-quickstart-templates)
5. VS Code has **Azure Resource Manager Tool** extension that can be used to edit the template JSON

## Deploying an ARM template
```
New-AzResourceGroupDeployment -ResourceGroupName RG-IaCSample `
    -TemplateFile "<somefilePath>\template.json" `
    -TemplateParameterFile "<somefilePath>\template.parameters.json" `
```

You can override some of the parameters by extending the command above:
```
New-AzResourceGroupDeployment -ResourceGroupName RG-IaCSample `
    -TemplateFile "<somefilePath>\template.json" `
    -TemplateParameterFile "<somefilePath>\template.parameters.json" `
    -OverrideParameterName 'New_Value'
```

When specifying `-Mode Complete` will clear out everything in the resource group that is not specified in the template. Resources that exist in the target and the template will get updated by the template.\
By default this mode is `Incremental` that will update values that already exist in the template. Values that exist in the target will get updated.

# Best Practices
- There are limits for a template, if the template is too big, use linked templates
- Use parameters only as required
- Use variables if the value is needed more than once or needs to be constructed by an expression
- Use "comments" for resources
- Don't use DependsOn unless required as will reduce parallelism
- Try and create a library of template components