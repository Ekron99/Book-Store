using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
    public class PasswordModel
    {
        public string OldPassword { get; set; }
        public string CurrentPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}