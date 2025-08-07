using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Smartstore
{
    public class SmartstoreCategoryDto
    {
        public int? ParentId { get; set; }
        public string TreePath { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public string BottomDescription { get; set; }
        public string ExternalLink { get; set; }
        public string BadgeText { get; set; }
        public int BadgeStyle { get; set; }
        public string Alias { get; set; }
        public int CategoryTemplateId { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public int? MediaFileId { get; set; }
        public int PageSize { get; set; }
        public bool AllowCustomersToSelectPageSize { get; set; }
        public string PageSizeOptions { get; set; }
        public bool ShowOnHomePage { get; set; }
        public bool LimitedToStores { get; set; }
        public bool SubjectToAcl { get; set; }
        public bool Published { get; set; }
        public bool IgnoreInMenus { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public string DefaultViewMode { get; set; }
        public bool HasDiscountsApplied { get; set; }
        public int Id { get; set; }
    }
}
