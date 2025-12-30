using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface ISchedulingRunner
    {
        Task<string> RunAsync(string type, int taskId, int? parameter);
    }
}
