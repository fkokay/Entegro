using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Commerce.Smartstore
{
    public class SmartstoreCustomerDto
    {
        public Guid CustomerGuid { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string ClientIdent { get; set; }
        public string AdminComment { get; set; }
        public bool IsTaxExempt { get; set; }
        public int AffiliateId { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public bool IsSystemAccount { get; set; }
        public string SystemName { get; set; }
        public string LastIpAddress { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime? LastLoginDateUtc { get; set; }
        public DateTime? LastActivityDateUtc { get; set; }
        public string LastVisitedPage { get; set; }
        public string Salutation { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Company { get; set; }
        public string CustomerNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public int VatNumberStatusId { get; set; }
        public string TimeZoneId { get; set; }
        public int TaxDisplayTypeId { get; set; }
        public DateTime? LastForumVisit { get; set; }
        public string LastUserAgent { get; set; }
        public string LastUserDeviceType { get; set; }
        public int BillingAddressId { get; set; }
        public int ShippingAddressId { get; set; }
        public bool LimitedToStores { get; set; }
        public int Id { get; set; }
    }
}
