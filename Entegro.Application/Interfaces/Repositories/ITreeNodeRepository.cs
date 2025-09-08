using Entegro.Domain.Entities.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface ITreeNodeRepository<TNode> where TNode : class, ITreeNode
    {
        TNode? GetParent(TNode node);
        IEnumerable<TNode> GetChildren(TNode node);
    }
}
