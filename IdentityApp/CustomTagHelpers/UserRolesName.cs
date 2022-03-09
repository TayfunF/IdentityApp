using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp.CustomTagHelpers
{
    [HtmlTargetElement("td", Attributes = "user-roles")]
    public class UserRolesName : TagHelper
    {
        public UserManager<AppUser> userManager { get; set; }

        public UserRolesName(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        [HtmlAttributeName("user-roles")] //@item.Id yi UserId ye bind etme islemi
        public string UserId { get; set; }

        //Outputa vermis oldugum Html ya da herhangi bir string ifadeyi <td>...buraya basiyor</td>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            AppUser appUser = await userManager.FindByIdAsync(UserId);
            IList<string> roles = await userManager.GetRolesAsync(appUser);

            string html = string.Empty;
            roles.ToList().ForEach(x =>
            {
                html += $"<span class='badge badge-info'>{x}</span>";
            });

            output.Content.SetHtmlContent(html);
        }

    }
}
