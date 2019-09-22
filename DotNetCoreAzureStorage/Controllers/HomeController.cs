using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DotNetCoreAzureStorage.Models;
using DotNetCoreAzureStorage.Helpers;
using Model.CloudTableEntities;

namespace DotNetCoreAzureStorage.Controllers
{
    public class HomeController : Controller
    {
        string azureCloudTableName = string.Empty;
        string azureStorageName = string.Empty;
        string azureAccesskey = string.Empty;

        public HomeController()
        {
            azureCloudTableName = "your-azure-cloud-table-name";
            azureStorageName = "your-azure-storage-name";
            azureAccesskey = "your-azure-access-key";

        }
        public async Task<IActionResult> Index()
        {
            var userEntity = new UserEntity
            {
                Username = "testUsername",
                Email = "test@mail.com"
            };

            await SaveUserToAzureTable(userEntity);

            return View();
        }


        private async Task<UserEntity> GetUserFromAzureTableData(int userId, string userName = "")
        {
            UserEntity userEntity = new UserEntity();

            AzureTableHelper storage = new AzureTableHelper(azureCloudTableName, azureStorageName, azureAccesskey);

            userEntity = await storage.RetrieveEntityUsingPointQueryAsync<UserEntity>(userId.ToString(), userName);
            if (userEntity != null)
                return userEntity;

            return userEntity;
        }


        private async Task SaveUserToAzureTable(UserEntity userEntity)
        {
            AzureTableHelper storage = new AzureTableHelper(azureCloudTableName, azureStorageName, azureAccesskey);

            await storage.InsertOrMergeEntityAsync<UserEntity>(userEntity);
        }

        private async Task DeleteUserFromAzureTable(UserEntity userEntity)
        {
            AzureTableHelper storage = new AzureTableHelper(azureCloudTableName, azureStorageName, azureAccesskey);

            await storage.DeleteEntityAsync<UserEntity>(userEntity.Id.ToString(),userEntity.Username);
        }

    }
}
