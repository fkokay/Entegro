using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Infrastructure.Repositories
{
    public class TreeNodeRepository<TNode> : ITreeNodeRepository<TNode> where TNode : class, ITreeNode
    {
        private readonly EntegroDbContext _db;
        public TreeNodeRepository(EntegroDbContext db) 
        {
            _db = db;
        }
        public IEnumerable<TNode> GetChildren(TNode node)
        {
            return _db.Set<TNode>().Where(x => x.ParentId == node.Id).ToList();
        }

        public TNode? GetParent(TNode node)
        {
            if (node.ParentId == null)
                return null;

            return _db.Set<TNode>().Find(node.ParentId);
        }
    }
}
