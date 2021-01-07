using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Task6.Models
{
    public class StripeCharge
    {
        [Required]
        public string Token { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string CardHolderName { get; set; }
        [Required]
        public string AddressLine1 { get; set; }
        [Required]
        public string AddressLine2 { get; set; }
        [Required]
        public string AddressCity { get; set; }
        [Required]
        [RegularExpression(@"^(\d{6})$", ErrorMessage = "Enter a valid 6 digit postalcode")]
        public string AddressPostalcode { get; set; }
        [Required]
        public string AddressCountry { get; set; }
    }
}
