using Domain.Entities;
using Netjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

[InjectAsScoped]
public interface IRegisterUserService
{
    Task RegisterAsync(User user, CancellationToken cancellationToken);
}
