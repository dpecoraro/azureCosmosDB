using AzureCosmoDB.BusinessLogic;
using AzureCosmoDB.Model;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureCosmoDB.Services
{
    public class CosmosDBService: ICosmosDBService
    {
        private static Container _container;

        public CosmosDBService(CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<bool> AddAsync(Book book)
        {
            try
            {
                var req = await _container.CreateItemAsync<Book>(book);

                if(req.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }

                return false;

            }
            catch (CosmosException ex)
            {

                return false;
            }            
        }

        public async Task DeleteAsync(string id)
        {
            await _container.DeleteItemAsync<Book>(id, new PartitionKey(id));
        }

        public async Task<Book> GetAsync(string id)
        {
            try
            {
                ItemResponse<Book> response = await _container.ReadItemAsync<Book>(id, new PartitionKey(id));

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {

                throw;
            }
        }

        public async Task<IEnumerable<Book>> GetAllAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<Book>(new QueryDefinition(queryString));

            List<Book> results = new List<Book>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<Book> UpdateAsync(Book entity)
        {
            try
            {
                var book = await GetAsync(entity.Id);
                if(book != null)
                {
                    return await _container.UpsertItemAsync<Book>(entity, new PartitionKey(entity.Id));
                }

                return null;
            }
            catch (CosmosException ex)
            {

                throw;
            }             
        }
    }
}
