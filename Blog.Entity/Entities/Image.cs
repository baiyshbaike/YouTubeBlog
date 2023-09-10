using Blog.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Entity.Enums;

namespace Blog.Entity.Entities
{
    public class Image : EntityBase
    {
        public Image()
        {
            
        }

        public Image(string fileName,string fileType, string createdBy)
        {
            
            CreatedBy = createdBy;
            FileType = fileType;
            FileName = fileName;
        }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public ICollection<Article> Articles { get; set; }
        public ICollection<AppUser> Users { get;set; }
    }
}
