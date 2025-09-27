using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Catalog;
using Entegro.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface ITaskDescriptorRepository
    {
        Task<PagedResult<TaskDescriptor>> GetPagedAsync(GridCommand gridCommand);
    }
}
