using System.ComponentModel;
using System.Data;
using System.Security.Cryptography;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Npgsql.NameTranslation;
using TechDemo.Core;
using TechDemo.Data.Entities;
using TechDemo.Data.Mappers;

namespace TechDemo.Data.Source
{
    public interface IDapperContext
    {
        Task ExecuteNonQueryAsync(string procedure, params NpgsqlParameter[] parameters);
        Task<List<T>> ExecuteReaderAsync<T>(string procedure, params NpgsqlParameter<T>[] parameters) where T : class;
        Task<T?> ExecuteReadSingleAsync<T>(string procedure, params NpgsqlParameter<T>[] parameters) where T : class;
        Task<T?> ExecuteReadSingleAsync<T>(string procedure, params NpgsqlParameter[] parameters) where T : class;
    }
    
    public class PostgresContext : IDapperContext
    {
        private readonly NpgsqlDataSource _source;
        
        public PostgresContext(IConfiguration configuration)
        {
            var builder = new NpgsqlDataSourceBuilder((configuration["Postgres:ConnectionString"]
#if DEBUG                
                += ";Include Error Detail=true"
#endif            
            ));
             
#if DEBUG
            
                builder.EnableParameterLogging();
#endif
            //builder.MapComposite<Document>("document");
            builder.DefaultNameTranslator = new NpgsqlSnakeCaseNameTranslator();
            builder.MapEnum<DocumentCategory>("category");
            builder.MapComposite<Group>("c_group");
            builder.MapComposite<User>("c_user");
            builder.MapComposite<GroupUser>("c_group_user");
            builder.MapComposite<DocumentAccess>("c_document_access");
            builder.MapComposite<Document>("c_document");
            _source = builder.Build();
        }

        public async Task ExecuteNonQueryAsync(string procedure, params NpgsqlParameter[] parameters)
        {
            using (var cmd = new NpgsqlCommand(procedure, await _source.OpenConnectionAsync()))
            {
                parameters.ToList().ForEach(x =>
                {
                    cmd.Parameters.Add(x);
                });
                await cmd.ExecuteNonQueryAsync();
            }
        }
        
        public async Task<List<T>> ExecuteReaderAsync<T>(string procedure, params NpgsqlParameter<T>[] parameters) where T : class
        {
            using (var cmd = new NpgsqlCommand(procedure, await _source.OpenConnectionAsync()))
            {
                parameters.ToList().ForEach(x =>
                {
                    cmd.Parameters.Add(x);
                });
                
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    List<T> results = new();
                    while (await reader.NextResultAsync())
                    {
                        results.Add(await reader.GetFieldValueAsync<T>(0));
                    }

                    return results;
                }
            }
        }
        
                
        public async Task<T?> ExecuteReadSingleAsync<T>(string procedure, params NpgsqlParameter<T>[] parameters) where T : class
        {
            using (var cmd = new NpgsqlCommand(procedure, await _source.OpenConnectionAsync()))
            {
                parameters.ToList().ForEach(x =>
                {
                    cmd.Parameters.Add(x);
                });
                
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        return await reader.GetFieldValueAsync<T>(0);     
                    }
                    else
                    {
                        return null;                        
                    }
                }
            }
        }
        
        public async Task<T?> ExecuteReadSingleAsync<T>(string procedure, params NpgsqlParameter[] parameters) where T : class
        {
            using (var cmd = new NpgsqlCommand(procedure, await _source.OpenConnectionAsync()))
            {
                parameters.ToList().ForEach(x =>
                {
                    cmd.Parameters.Add(x);
                });
                
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        return await reader.GetFieldValueAsync<T>(0);     
                    }
                    else
                    {
                        return null;                        
                    }
                }
            }
        }
    }   
}