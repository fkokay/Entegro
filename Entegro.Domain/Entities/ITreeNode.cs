using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    public interface ITreeNode
    {
        int Id { get; }
        int? ParentId { get; }
        string TreePath { get; set; }
        ITreeNode? GetParentNode();
        IEnumerable<ITreeNode> GetChildNodes();
    }

    public static class ITreeNodeExtensions
    {
        const char PathSeparator = '/';

        public static string BuildTreePath(this ITreeNode node)
        {
            if (node.ParentId == null || node.GetParentNode() is not ITreeNode parentNode)
            {
                return $"/{node.Id}/";
            }

            var parentPath = parentNode.TreePath.NullEmpty() ?? parentNode.BuildTreePath();
            parentPath = parentPath.EmptyNull().EnsureEndsWith(PathSeparator);

            return $"{parentPath}{node.Id}/";
        }
    }
}
