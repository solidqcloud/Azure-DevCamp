using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCamp.API.Models
{
    public class QMsgIncidentImage
    {
        public string IncidentId { get; set; }
        public string BlobContainerName { get; set; }
        public string BlobName { get; set; }
    }
}
