using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw_itkpi.Models
{
    public class AdminClass
    {
        public string Password { get; set; }
        public string actionToPerform { get; set; }

        public AdminClass()
        {
            Password = "";
            actionToPerform = "";
        }
    }
}
