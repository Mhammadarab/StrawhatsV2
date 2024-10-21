using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cargohub.models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Address_Extra { get; set; }
        public string City { get; set; }
        public string Zip_Code { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string Contact_Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Reference { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
    }
}