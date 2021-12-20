# Exercise: Create Blob storage resources by using the .NET client library

- Create a container
- Upload blobs to a container
- List the blobs in a container
- Download blobs
- Delete a container

1. Login to Azure
``` 
az login --tenant igorcomp.onmicrosoft.com
```

2. Create a resource group for the resources needed for this exercise.
```
az group create --location uksouth --name az204-blob-rg
```

3. Create a storage account. We need a storage account created to sue in the application.
```
az storage account create --resource-group az204-blob-rg --name imaz204blobsa --location uksouth --sku Standard_LRS
```
4. Get credentials for the storage account.

- Navigate to the Azure portal.
- Locate the storage account created.
- Select **Access keys** in the **Security + networking** section of the navigation pane. Here, you can view your account access keys and the complete connection string for each key.
- Find the **Connection** string value under key1, and select the **Copy** button to copy the connection string. You will add the connection string value to the code in the next section.
- In the **Blob** section of the storage account overview, select Containers. Leave the windows open so you can view changes made to the storage as you progress through the exercise.

## Prepare the .NET project
1. In vs Code terminal navigate to directory where you want to store your project
2. In the terminal, use the dotnet new command to create a new console app. This command creates a simple "Hello World" C# project with a single source file: Program.cs.
```
dotnet new console -n az204-blob
```
3. Use the following commands to switch to the newly created az204-blob folder and build the app to verify that all is well.
```
cd az204-blob
dotnet build
```
4. Inside the *az204-blob* folder, create another folder named *data*. This is where the blob data files will be created and stored.
```
mkdir data
```
5. While still in the application directory, install the Azure Blob Storage client library for .NET package by using the dotnet add package command.
```
dotnet add package Azure.Storage.Blobs
```
6. Open the *Program.c*s file in your editor, and replace the contents with the following code.
```
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace az204_blob
{
    class Program
    {
        public static void Main()
        {
            Console.WriteLine("Azure Blob Storage exercise\n");

            // Run the examples asynchronously, wait for the results before proceeding
            ProcessAsync().GetAwaiter().GetResult();

            Console.WriteLine("Press enter to exit the sample application.");
            Console.ReadLine();

        }

        private static async Task ProcessAsync()
        {
            // Copy the connection string from the portal in the variable below.
            string storageConnectionString = "CONNECTION STRING";

            // Create a client that can authenticate with a connection string
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);

            // EXAMPLE CODE STARTS BELOW HERE


        }
    }
}
```

## Create a container
To create the container first create an instance of the `BlobServiceClient` class, then call the `CreateBlobContainerAsync` method to create the container in your storage account. A GUID value is appended to the container name to ensure that it is unique. The `CreateBlobContainerAsync` method will fail if the container already exists.

```
//Create a unique name for the container
string containerName = "wtblob" + Guid.NewGuid().ToString();

// Create the container and return a container client object
BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
Console.WriteLine("A container named '" + containerName + "' has been created. " +
    "\nTake a minute and verify in the portal." + 
    "\nNext a file will be created and uploaded to the container.");
Console.WriteLine("Press 'Enter' to continue.");
Console.ReadLine();
```
## Upload blobs to a container
The following code snippet gets a reference to a `BlobClient` object by calling the `GetBlobClient` method on the container created in the previous section. It then uploads the selected local file to the blob by calling the `UploadAsync` method. This method created the blob if it doesn't already exist, and overwrites it if it does.
```
// Create a local file in the ./data/ directory for uploading and downloading
string localPath = "./data/";
string fileName = "wtfile" + Guid.NewGuid().ToString() + ".txt";
string localFilePath = Path.Combine(localPath, fileName);

// Write text to the file
await File.WriteAllTextAsync(localFilePath, "Hello, World!");

// Get a reference to the blob
BlobClient blobClient = containerClient.GetBlobClient(fileName);

Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

// Open the file and upload its data
using FileStream uploadFileStream = File.OpenRead(localFilePath);
await blobClient.UploadAsync(uploadFileStream, true);
uploadFileStream.Close();

Console.WriteLine("\nThe file was uploaded. We'll verify by listing" + 
        " the blobs next.");
Console.WriteLine("Press 'Enter' to continue.");
Console.ReadLine();
```

## List the blobs in a container
List the blobs in the container by using the `GetBlobsAsync` method. In this case, only one blob has been added to the container, so the listing operation returns just that one blob.

```
// List blobs in the container
Console.WriteLine("Listing blobs...");
await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
{
    Console.WriteLine("\t" + blobItem.Name);
}

Console.WriteLine("\nYou can also verify by looking inside the " + 
        "container in the portal." +
        "\nNext the blob will be downloaded with an altered file name.");
Console.WriteLine("Press 'Enter' to continue.");
Console.ReadLine();
```

## Download blobs
Download the blob created previously to your local file system by using the DownloadAsync method. The example code adds a suffix of "DOWNLOADED" to the blob name so that you can see both files in local file system.
```
// Download the blob to a local file
// Append the string "DOWNLOADED" before the .txt extension 
string downloadFilePath = localFilePath.Replace(".txt", "DOWNLOADED.txt");

Console.WriteLine("\nDownloading blob to\n\t{0}\n", downloadFilePath);

// Download the blob's contents and save it to a file
BlobDownloadInfo download = await blobClient.DownloadAsync();

using (FileStream downloadFileStream = File.OpenWrite(downloadFilePath))
{
    await download.Content.CopyToAsync(downloadFileStream);
    downloadFileStream.Close();
}
Console.WriteLine("\nLocate the local file to verify it was downloaded.");
Console.WriteLine("The next step is to delete the container and local files.");
Console.WriteLine("Press 'Enter' to continue.");
Console.ReadLine();
```

## Delete a container
The following code cleans up the resources the app created by deleting the entire container using `DeleteAsync`. It also deletes the local files created by the app.
```
// Delete the container and clean up local files created
Console.WriteLine("\n\nDeleting blob container...");
await containerClient.DeleteAsync();

Console.WriteLine("Deleting the local source and downloaded files...");
File.Delete(localFilePath);
File.Delete(downloadFilePath);

Console.WriteLine("Finished cleaning up.");
```

## Run the code

```
dotnet build
dotnet run
```

## Clean up other resources
```
az group delete --name az204-blob-rg --no-wait
```
