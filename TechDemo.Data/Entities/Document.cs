using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;
using TechDemo.Core;
using TechDemo.Data.Mappers;

namespace TechDemo.Data.Entities
{
    [PgName("public.c_document")]
    public class Document
    {
        [Key]
        [PgName("id")]
        public Guid Id { get; set; }
        
        [PgName("name")]
        public string Name { get; set; } = null!;
        
        [PgName("description")]
        public string Description { get; set; } = null!;
        
        [PgName("category")]
        public DocumentCategory Category { get; set; }
        
        [PgName("location")]
        public string Location { get; set; } = null!;
        
        [PgName("added")]
        public DateTime Added { get; set; }
        
        [PgName("hash")]
        public string? Hash { get; set; } = null!;

        [PgName("tags")]
        [Column(TypeName = "jsonb")]
        public string Tags { get; set; } = "{]";

    }
}