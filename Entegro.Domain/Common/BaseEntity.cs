using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Common
{
    public abstract partial class BaseEntity : INamedEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetEntityName()
        {
            return GetType().Name;
        }
    }
}
