using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace DevCamp.API.Models
{
    public class ApiResponseMsg : StringContent
    {
        public ApiResponseMsg(string Content) : this(Content, Encoding.UTF8)
        {
            Id = Guid.NewGuid().ToString();
            Timestamp = DateTime.UtcNow;
            Message = Content;
        }

        public ApiResponseMsg(string content, Encoding encoding)
        : base(content, encoding, "application/json")
        {
            Id = Guid.NewGuid().ToString();
            Timestamp = DateTime.UtcNow;
            Message = content;
        }

        public string Message { get; set; }

        public string Id { get; set; }

        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            string json = JsonConvert.SerializeObject(this);
            return json;
        }
    }
}