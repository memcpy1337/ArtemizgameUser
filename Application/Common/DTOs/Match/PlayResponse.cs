using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.DTOs.Match
{
    public class PlayResponse
    {
        [SwaggerSchema(Required = new[] { "Ticket to acces server" })]
        public required string Ticket { get; set; }
    }
}
