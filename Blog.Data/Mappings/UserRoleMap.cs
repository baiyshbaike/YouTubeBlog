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
    public class UserRoleMap : IEntityTypeConfiguration<AppUserRole>
    {
        public void Configure(EntityTypeBuilder<AppUserRole> builder)
        {
            // Primary key
            builder.HasKey(r => new { r.UserId, r.RoleId });

            // Maps to the AspNetUserRoles table
            builder.ToTable("AspNetUserRoles");
            builder.HasData(new AppUserRole
            {
                UserId = Guid.Parse("1CC65D1E-6154-450F-83F2-76609D2AC6ED"),
                RoleId = Guid.Parse("0101AC60-FE97-4698-A86A-81E0C737F2FB"),
            },
            new AppUserRole
            {
                UserId = Guid.Parse("D415697D-005C-4F06-866D-F2CF19C096F9"),
                RoleId = Guid.Parse("DC8A1816-0CE5-4E9D-AC71-1AFD20E439B5"),
            });
        }
    }
}
