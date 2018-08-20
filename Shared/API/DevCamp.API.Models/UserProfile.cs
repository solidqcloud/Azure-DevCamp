using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DevCamp.API.Models
{
    public class UserProfile
    {
        public string City { get; set; }
        public string CompanyName { get; set; }
        public string Country { get; set; }
        public string Department { get; set; }
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public string Id { get; set; }
        public string Mail { get; set; }
        public string MySite { get; set; }
        public string OfficeLocation { get; set; }
        public string PostalCode { get; set; }
        public string PreferredName { get; set; }
        public string State { get; set; }
        public string StreetAddress { get; set; }
        public string Surname { get; set; }
        public string UserPrincipalName { get; set; }
        public string UserType { get; set; }
    }
}