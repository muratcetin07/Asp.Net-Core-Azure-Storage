using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreAzureStorage.Helpers
{
    public class AzureTableHelper
    {
        private CloudTable table;
        string _accessKey = string.Empty;
        string _storageName = string.Empty;

        // Constructor   
        public AzureTableHelper(string cloudTableName, string storageName, string accessKey)
        {
            if (string.IsNullOrEmpty(cloudTableName))
            {
                throw new ArgumentNullException("Table", "Table Name can't be empty");
            }
            try
            {
                _storageName = storageName;
                _accessKey = accessKey;

                CloudStorageAccount storageAccount = new CloudStorageAccount(new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(_storageName, _accessKey), true);

                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                table = tableClient.GetTableReference(cloudTableName);

                table.CreateIfNotExistsAsync();
            }
            catch (StorageException StorageExceptionObj)
            {
                throw StorageExceptionObj;
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }

        public async Task InsertOrMergeEntityAsync<T>(T entity) where T : TableEntity, new()
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                // Create the TableOperation that inserts the entity.
                TableOperation insertOperation = TableOperation.Insert(entity);

                // Execute the insert operation.
                await table.ExecuteAsync(insertOperation);
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public async Task<T> RetrieveEntityUsingPointQueryAsync<T>(string partitionKey, string rowKey) where T : TableEntity, new()
        {
            try
            {
                // Create a retrieve operation that takes an entity.
                TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);

                // Execute the retrieve operation.
                TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

                return (T)retrievedResult.Result;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public async Task DeleteEntityAsync<T>(string partitionKey, string rowKey) where T : TableEntity, new()
        {
            try
            {
                // Create a retrieve operation that takes a entity.
                TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);

                // Execute the retrieve operation.
                TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

                // Assign the result to an object.
                T deleteEntity = (T)retrievedResult.Result;

                // Create the Delete TableOperation and then execute it.
                if (deleteEntity != null)
                {
                    TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                    // Execute the operation.
                    await table.ExecuteAsync(deleteOperation);

                    Console.WriteLine("Entity deleted.");
                }
                else
                    Console.WriteLine("Couldn't delete the entity.");
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }
    }
}
