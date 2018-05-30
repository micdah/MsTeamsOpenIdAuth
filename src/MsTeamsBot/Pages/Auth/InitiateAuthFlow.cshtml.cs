using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace MsTeamsBot.Pages.Auth
{
    public class InitiateAuthFlowModel : PageModel
    {
        [BindProperty]
        public string BotClientId
        {
            get { return Configuration["BotClientId"]; }
        }

        [BindProperty]
        public string BotAuthority
        {
            get { return Configuration["BotAuthority"]; }
        }

        private IConfiguration Configuration
        {
            get;
        }

        public InitiateAuthFlowModel(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void OnGet()
        {
        }
    }
}
