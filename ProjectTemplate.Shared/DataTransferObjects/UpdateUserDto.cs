using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTemplate.Shared.DataTransferObjects
{
    public class UpdateUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Department { get; set; }
        public string Manager { get; set; }
        public List<string> Roles { get; set; }
    }
}
