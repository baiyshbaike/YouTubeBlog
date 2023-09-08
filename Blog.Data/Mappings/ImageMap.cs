using Blog.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Mappings
{
    public class ImageMap : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasData(new Image
            {
                Id = Guid.Parse("0552F288-BC7F-4F98-A8EF-7641BDD53A90"),
                FileName = "images/filename",
                FileType = "jpg",
                CreatedBy = "Admin Test",
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false,
            },
            new Image
            {
                Id = Guid.Parse("3AFB1433-1BDF-4E18-8EFE-A8E99506A3DD"),
                FileName = "images/filename2",
                FileType = "jpg",
                CreatedBy = "Admin Test2",
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false,
            }
            );
        }
    }
}
