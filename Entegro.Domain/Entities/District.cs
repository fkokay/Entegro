using Entegro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    [Table("District")]
    public class District : BaseEntity
    {
        public int TownId { get; set; }
        private Town? _town;
        public Town? Town
        {
            get => _town ?? LazyLoader?.Load(this, ref _town);
            set => _town = value;
        }

        public string Name { get; set; }
        public bool Published { get; set; }
    }
}
