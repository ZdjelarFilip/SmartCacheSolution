using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using SmartCacheAPI.Grains;
using SmartCacheAPI.Models;
using SmartCacheAPI.Services;
using SmartCacheAPI.Settings;
using System.Text.Json;

namespace SmartCacheAPI.Repositories
{
    public class AzureBreachedEmailStorage : IBreachedEmailStorage
    {
        private readonly BlobContainerClient _container;
        private readonly IClusterClient _orleansClient;

        public AzureBreachedEmailStorage(IOptions<AzureSettings> settings, IClusterClient orleansClient)
        {
            var blobServiceClient = new BlobServiceClient(settings.Value!.ConnectionString);
            _container = blobServiceClient.GetBlobContainerClient(settings.Value!.ContainerName);
            _container.CreateIfNotExists();

            _orleansClient = orleansClient;
        }

        public async Task AddAsync(BreachedEmail breach)
        {
            var blob = _container.GetBlobClient($"{breach.Email}.json");
            var data = JsonSerializer.Serialize(breach);
            await blob.UploadAsync(new BinaryData(data));
        }

        public async Task<bool> ExistsAsync(string email)
        {
            // Check Orleans in-memory cache first
            var grain = _orleansClient.GetGrain<IBreachedEmailGrain>(email);
            bool existsInCache = await grain.IsBreachedAsync();

            if (existsInCache)
            {
                return true; // Found in cache, avoid slow Azure calls
            }

            // If not found in cache, check Azure Blob Storage
            var blob = _container.GetBlobClient($"{email}.json");
            return await blob.ExistsAsync();
        }
    }
}