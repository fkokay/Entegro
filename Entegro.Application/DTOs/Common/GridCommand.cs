using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Common
{
    public class GridCommand
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public List<GridColumn> Columns { get; set; } = new();
        public GridSearch? Search { get; set; }
        public List<GridOrder> Order { get; set; } = new();
    }

    public class GridColumn
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public GridSearch Search { get; set; }
    }

    public class GridSearch
    {
        public string Value { get; set; }
        public bool Regex { get; set; }
    }

    public class GridOrder
    {
        public int Column { get; set; }
        public string Dir { get; set; }
    }
}
