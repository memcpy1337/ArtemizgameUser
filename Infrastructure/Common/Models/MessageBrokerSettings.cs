using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.Models
{
    public class MessageBrokerSettings
    {
        public string? Host { get; set; }
        public string? Password { get; set; }
        public string? Username { get; set; }

    }
}
