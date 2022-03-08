using Microsoft.AspNetCore.Identity;
using System;

namespace IdentityApp.Models
{
    public class AppUser : IdentityUser
    {
        public string City { get; set; }
        public string Picture { get; set; }
        public DateTime? BirthDay { get; set; }
        public int Gender { get; set; }
    }
}
