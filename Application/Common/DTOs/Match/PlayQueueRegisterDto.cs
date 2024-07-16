using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.DTOs.Match;

public record PlayQueueRegisterDto
{
    public required string UserId { get; set; }
    public int Elo { get; set; }
    public int GameRegime { get; set; }
    public int PlayerType { get; set; }
}
