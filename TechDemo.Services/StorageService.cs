using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace TechDemo.Services
{
    public interface IStorageService
    {
        Task Upload(string location, Stream data);
        Task<string> Download(string location);
        Task Delete(string location);
    }
    
    //TODO Add CloudStorageService implementation.
    public class LocalStorageService : IStorageService
    {
        private readonly ILogger<IStorageService> _log; 
        
        public LocalStorageService(ILogger<IStorageService> log)
        {
            _log = log;
        }

        public async Task Upload(string location, Stream data)
        {
            using (var handle = System.IO.File.Open(location, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter writer = new StreamWriter(handle))
                {
                    using (StreamReader reader = new StreamReader(data))
                    {
                        await writer.WriteAsync(await reader.ReadToEndAsync());
                    }
                }
            }
        }

        public async Task<string> Download(string location)
        {
            using (var handle = System.IO.File.Open(location, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                using (StreamReader reader = new StreamReader(handle))
                {
                    //TODO Can optimize by looping over a smaller buffer or otherwise not globbing the entire file.
                    return await reader.ReadToEndAsync();
                }
            }        
        }

        public async Task Delete(string location)
        {
            File.Delete(location);
            await Task.CompletedTask;
        }
    }   
}