using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.Models;

public class RedisSettings
{
    public string Host { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
}
