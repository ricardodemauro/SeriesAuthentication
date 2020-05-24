using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppIdentityMvc.Identity.Email
{
    public class SendGridEmailOptions
    {
        public string SendGridUser { get; set; }

        public string SendGridKey { get; set; }

        public string SendGridSender { get; set; }
    }
}
