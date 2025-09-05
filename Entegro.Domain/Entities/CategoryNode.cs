using Entegro.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    public interface ICategoryNode
    {
        new int Id { get; }
        int? ParentId { get; }
        string Name { get; }
        int? MediaFileId { get; }
        bool Published { get; }
        int DisplayOrder { get; }
        DateTime UpdatedOnUtc { get; }
    }

    [Serializable]
    public class CategoryNode : ICategoryNode, IKeyedNode
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public int? MediaFileId { get; set; }
        public bool Published { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        /// <inheritdoc/>
        object IKeyedNode.GetNodeKey()
        {
            return Id;
        }

        /// <inheritdoc/>
        public string GetDisplayName()
        {
            return Name;
        }

        /// <inheritdoc/>
        public string[] GetDisplayNameMemberNames()
        {
            return new[] { nameof(Name) };
        }

        /// <inheritdoc/>
        public string GetEntityName()
        {
            return nameof(Category);
        }
    }
}
