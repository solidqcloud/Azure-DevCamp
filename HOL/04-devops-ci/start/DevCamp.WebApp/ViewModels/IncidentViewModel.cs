using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DevCamp.WebApp.ViewModels
{
    public class IncidentViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage="Please enter a description")]
        [Display(Prompt = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter a street")]
        [Display(Prompt = "123 Main st")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Please enter a city")]
        [Display(Prompt = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please select a state")]
        public string State { get; set; }
    
        [Required(ErrorMessage = "Please enter a zip code")]
        [Display(Prompt = "555555")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Please enter a first name")]
        [Display(Prompt ="Jane")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter a last name")]
        [Display(Prompt = "Doe")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter a phone number")]
        [RegularExpression("^(\\+?1?( ?.?-?\\(?\\d{3}\\)?) ?.?-?)?(\\d{3})( ?.?-? ?\\d{4})$", ErrorMessage = "Please enter a properly formatted Phone number.")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone number", Prompt = "(555) 555-5555")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Outage type")]
        public string OutageType { get; set; }

        public bool IsEmergency { get; set; }

        [Display(Name= "Image of the outage")]
        public Image ImageFile { get; set; }

        public string ImageUri { get; set; }

        public string ThumbnailUri { get; set; }

		public string Tags { get; set; }

		public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public bool Resolved { get; set; }

        public IncidentViewModel()
        {
        }


        public static SelectList GetOutageTypes()
        {
            SelectList outageTypes = new SelectList(new List<SelectListItem>
            {
                new SelectListItem() { Text = "WiFi down/slow", Value = "WiFi down-slow" },
                new SelectListItem() { Text = "Downed power line", Value= "Downed power line"},
                new SelectListItem() { Text = "Sewer", Value = "Sewer" },
                new SelectListItem() { Text = "Pot hole", Value = "Pot hole" },
                new SelectListItem() { Text = "Stray dog/cat", Value = "Stray dog-cat" },
                new SelectListItem() { Text = "Gas leak", Value = "Gas leak" },
                new SelectListItem() { Text = "Street light-Light Goes On and Off", Value = "Street light-Light Goes On and Off" },
                new SelectListItem() { Text = "Street light-Light Stays On", Value = "Street light-Light Stays On" },
                new SelectListItem() { Text = "Street light-Light Glass Broken", Value = "Street light-Light Glass Broken" },
                new SelectListItem() { Text = "Street light-Fixture Hanging", Value = "Street light-Fixture Hanging" },
                new SelectListItem() { Text = "Street light-String of Lights Out", Value = "Street light-String of Lights Out" }
            }, "Value", "Text");
            return outageTypes;
        }

        public static SelectList GetStates()
        {
            SelectList states = new SelectList(new List<SelectListItem>
            {
                new SelectListItem() { Text="Alabama", Value="AL" },
                new SelectListItem() { Text = "Alaska", Value="AK" },
                new SelectListItem() { Text = "Arizona", Value="AZ" },
                new SelectListItem() { Text="Arkansas", Value="AR" },
                new SelectListItem() { Text="California", Value="CA" },
                new SelectListItem() { Text="Colorado", Value="CO" },
                new SelectListItem() { Text="Connecticut", Value="CT" },
                new SelectListItem() { Text="District of Columbia", Value="DC" },
                new SelectListItem() { Text="Delaware", Value="DE" },
                new SelectListItem() { Text="Florida", Value="FL" },
                new SelectListItem() { Text="Georgia", Value="GA" },
                new SelectListItem() { Text="Hawaii", Value="HI" },
                new SelectListItem() { Text="Idaho", Value="ID" },
                new SelectListItem() { Text="Illinois", Value="IL" },
                new SelectListItem() { Text="Indiana", Value="IN" },
                new SelectListItem() { Text="Iowa", Value="IA" },
                new SelectListItem() { Text="Kansas", Value="KS" },
                new SelectListItem() { Text="Kentucky", Value="KY" },
                new SelectListItem() { Text="Louisiana", Value="LA" },
                new SelectListItem() { Text="Maine", Value="ME" },
                new SelectListItem() { Text="Maryland", Value="MD" },
                new SelectListItem() { Text="Massachusetts", Value="MA" },
                new SelectListItem() { Text="Michigan", Value="MI" },
                new SelectListItem() { Text="Minnesota", Value="MN" },
                new SelectListItem() { Text="Mississippi", Value="MS" },
                new SelectListItem() { Text="Missouri", Value="MO" },
                new SelectListItem() { Text="Montana", Value="MT" },
                new SelectListItem() { Text="Nebraska", Value="NE" },
                new SelectListItem() { Text="Nevada", Value="NV" },
                new SelectListItem() { Text="New Hampshire", Value="NH" },
                new SelectListItem() { Text="New Jersey", Value="NJ" },
                new SelectListItem() { Text="New Mexico", Value="NM" },
                new SelectListItem() { Text="New York", Value="NY" },
                new SelectListItem() { Text="North Carolina", Value="NC" },
                new SelectListItem() { Text="North Dakota", Value="ND" },
                new SelectListItem() { Text="Ohio", Value="OH" },
                new SelectListItem() { Text="Oklahoma", Value="OK" },
                new SelectListItem() { Text="Oregon", Value="OR" },
                new SelectListItem() { Text="Pennsylvania", Value="PA" },
                new SelectListItem() { Text="Rhode Island", Value="RI" },
                new SelectListItem() { Text="South Carolina", Value="SC" },
                new SelectListItem() { Text="South Dakota", Value="SD" },
                new SelectListItem() { Text="Tennessee", Value="TN" },
                new SelectListItem() { Text="Texas", Value="TX" },
                new SelectListItem() { Text="Utah", Value="UT" },
                new SelectListItem() { Text="Vermont", Value="VT" },
                new SelectListItem() { Text="Virginia", Value="VA" },
                new SelectListItem() { Text="Washington", Value="WA" },
                new SelectListItem() { Text="West Virginia", Value="WV" },
                new SelectListItem() { Text="Wisconsin", Value="WI" },
                new SelectListItem() { Text="Wyoming", Value="WY" }
            }, "Value", "Text");

            return states;
        }
    }
}
