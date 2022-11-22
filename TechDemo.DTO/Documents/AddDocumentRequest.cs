using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using TechDemo.Core;


namespace TechDemo.DTO.Documents
{
    public class AddDocumentRequest
    {
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Name { get; set; } = null!;
        
        
        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string Description { get; set; } = null!;
        
        [Required]
        public DocumentCategory Category { get; set; } 
        
        public IFormFile File { get; set; } = null!;
        
        public string Tags { get; set; } = "{}";

    }   
}