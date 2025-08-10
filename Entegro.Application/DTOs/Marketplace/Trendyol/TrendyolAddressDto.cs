using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.Trendyol
{
    public class TrendyolAddressDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public TrendyolAddressLinesDto AddressLines { get; set; }
        public string City { get; set; }
        public int? CityCode { get; set; }
        public string District { get; set; }
        public int? DistrictId { get; set; }
        public int? CountyId { get; set; }
        public string CountyName { get; set; }
        public string ShortAddress { get; set; }
        public string StateName { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public long? NeighborhoodId { get; set; }
        public string Neighborhood { get; set; }
        public string Phone { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string FullName { get; set; }
        public string FullAddress { get; set; }
        public string TaxOffice { get; set; }
        public string TaxNumber { get; set; }
    }
}
