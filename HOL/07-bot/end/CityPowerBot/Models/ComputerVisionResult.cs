using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityPowerBot
{
	public class ComputerVisionResult
	{
		public String Description { get; set; } = String.Empty;
		public List<String> Tags { get; set; } = new List<String>();
		public String Text { get; set; } = String.Empty;
	}
}