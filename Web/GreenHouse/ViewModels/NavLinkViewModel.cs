using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenHouse.ViewModels
{
    public class NavLinkViewModel
    {
        public string Name { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string RequestUrlPath { get; set; }
    }
}