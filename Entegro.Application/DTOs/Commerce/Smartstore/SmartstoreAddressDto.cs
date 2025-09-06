using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Commerce.Smartstore
{
    public class SmartstoreAddressDto
    {
        public int Id { get; set; }
        public string? Salutation { get; set; }
        public string? Title { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Company { get; set; }
        public int? CountryId { get; set; }
        public int? StateProvinceId { get; set; }
        public int? CityId { get; set; }
        public int? TownId { get; set; }
        public int? DistrictId { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? ZipPostalCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FaxNumber { get; set; }
        public string? TaxOffice { get; set; }
        public string? TaxNumber { get; set; }
        public DateTime CreatedOnUtc { get; set; }

        public SmartstoreCityDto? City { get; set; }
        public SmartstoreTownDto? Town { get; set; }
        public SmartstoreDistrictDto? District { get; set; }
    }
}
