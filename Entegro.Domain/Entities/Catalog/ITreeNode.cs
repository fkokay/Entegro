using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities.Catalog
{
    public interface ITreeNode
    {
        int Id { get; }
        int? ParentId { get; }
        string TreePath { get; set; }
        ITreeNode? GetParentNode();
        IEnumerable<ITreeNode> GetChildNodes();
    }

   
}
