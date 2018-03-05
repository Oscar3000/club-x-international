using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Club_X_International.Models.ViewModels
{
    public class ViewModels
    {
        public LoginViewModel LoginModel { get; set; }
        public RegisterViewModel RegisterModel { get; set; }
        public RoleViewModel RoleViewModel { get; set; }
        public Blog Blog { get; set; }
        public Writer Writer { get; set; }
        public Event Event { get; set; }
    }
}