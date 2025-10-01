using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Event
{
    public interface IEventHandler<TEvent>
    {
        Task HandleAsync(TEvent @event);
    }
}
