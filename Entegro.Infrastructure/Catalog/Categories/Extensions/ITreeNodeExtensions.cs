using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Infrastructure.Catalog.Categories.Extensions
{
    public static class ITreeNodeExtensions
    {
        const char PathSeparator = '/';

        public static string BuildTreePath<TNode>(this ITreeNodeRepository<TNode> repo, TNode node, bool update = true) where TNode : class, ITreeNode
        {
            string path;

            var parent = repo.GetParent(node);
            if (parent == null)
            {
                path = $"/{node.Id}/";
            }
            else
            {
                var parentPath = string.IsNullOrEmpty(parent.TreePath)? repo.BuildTreePath(parent, update): parent.TreePath;

                if (!parentPath.EndsWith(PathSeparator))
                    parentPath += PathSeparator;

                path = $"{parentPath}{node.Id}/";
            }

            if (update)
                node.TreePath = path;

            return path;
        }
    }
}
