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
    public class ArticleMap : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            //throw new NotImplementedException();
            //builder.Property(x=> x.Title).HasMaxLength(150);
            //builder.Property(x => x.Title).IsRequired();
            builder.HasData(new Article
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Content = "content",
                ViewCount = 1,
                CategoryId = Guid.Parse("913EB8E0-D5F2-4F1E-A4D9-753EC4693C92"),
                ImageId = Guid.Parse("0552F288-BC7F-4F98-A8EF-7641BDD53A90"),
                CreatedBy = "Admin Test",
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false,
                UserId = Guid.Parse("1CC65D1E-6154-450F-83F2-76609D2AC6ED"),
            },
            new Article
            {
                Id = Guid.NewGuid(),
                Title = "Title2",
                Content = "content2",
                ViewCount = 1,
                CategoryId = Guid.Parse("A2E79D28-A812-4BE4-A6DA-E4347634860F"),
                ImageId = Guid.Parse("3AFB1433-1BDF-4E18-8EFE-A8E99506A3DD"),
                CreatedBy = "Admin Test2",
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false,
                UserId = Guid.Parse("D415697D-005C-4F06-866D-F2CF19C096F9"),
            }
            );
        }
    }
}
