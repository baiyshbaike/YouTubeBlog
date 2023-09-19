using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Entity.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid ImageId { get; set; } = Guid.Parse("0552F288-BC7F-4F98-A8EF-7641BDD53A90");
        public Image Image { get; set; }
        public ICollection<Article> Articles { get; set; }
    }
}
