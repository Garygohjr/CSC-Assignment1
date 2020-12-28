using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocalAccounts.Models
{
    public class CaptchaRequest
    {
        public string secret { get; set; } = "6LdjqRcaAAAAAJtpKAuEGZkoz6lUpANUkPKfJT-F";
        public string response { get; set; }
    }
}