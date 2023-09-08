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
    public class CategoryMap : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasData(new Category
            {
                Id = Guid.Parse("913EB8E0-D5F2-4F1E-A4D9-753EC4693C92"),
                Name = "Name",
                CreatedBy = "Admin Test",
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false,
            },
            new Category
            {
                Id = Guid.Parse("A2E79D28-A812-4BE4-A6DA-E4347634860F"),
                Name = "Name",
                CreatedBy = "Admin Test2",
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false,
            }
             );
        }
    }
}
