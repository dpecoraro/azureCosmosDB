using AzureCosmoDB.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureCosmoDB.Services
{
    public interface ICosmosDBService
    {
        Task<bool> AddAsync(Book book);
        Task DeleteAsync(string id);
        Task<IEnumerable<Book>> GetAllAsync(string queryString);
        Task<Book> GetAsync(string id);
        Task<Book> UpdateAsync(Book book);
    }
}