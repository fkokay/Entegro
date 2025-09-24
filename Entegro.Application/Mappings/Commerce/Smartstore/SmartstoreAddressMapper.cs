using Entegro.Application.DTOs.Address;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Microsoft.Extensions.Logging;

namespace Entegro.Application.Mappings.Commerce.Smartstore
{
    public static class SmartstoreAddressMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static AddressDto? ToDto(SmartstoreAddressDto smartstoreAddress)
        {
            try
            {
                if (smartstoreAddress == null)
                {
                    return null;
                }

                AddressDto address = new AddressDto();
                address.Id = smartstoreAddress.Id;
                address.Address1 = smartstoreAddress.Address1 ?? "";
                address.Address2 = smartstoreAddress.Address2 ?? "";
                address.ZipPostalCode = smartstoreAddress.ZipPostalCode;
                address.Company = smartstoreAddress.Company;
                address.Email = smartstoreAddress.Email;
                address.FirstName = smartstoreAddress.FirstName;
                address.LastName = smartstoreAddress.LastName;
                address.PhoneNumber = smartstoreAddress.PhoneNumber;
                address.City = "";
                address.District = "";
                address.Town = "";
                address.Country = "";
                address.Title = smartstoreAddress.Title;
                address.Salutation = smartstoreAddress.Salutation;
                address.TaxOffice = smartstoreAddress.TaxOffice;
                address.TaxOfficeNumber = smartstoreAddress.TaxNumber;
                address.FaxNumber = smartstoreAddress.FaxNumber;
                address.CreatedOnUtc = DateTime.UtcNow;
                address.UpdatedOnUtc = DateTime.UtcNow;
                return address;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Address mapping sırasında hata oluştu. AddressId: {AddressId}", smartstoreAddress.Id);
                return null;
            }
        }
        public static SmartstoreAddressDto? ToDto(AddressDto address)
        {
            try
            {
                if (address == null)
                {
                    return null;
                }

                SmartstoreAddressDto smartstoreAddress = new SmartstoreAddressDto();
                smartstoreAddress.Id = 0;
                smartstoreAddress.Address1 = address.Address1;
                smartstoreAddress.Address2 = address.Address2;
                smartstoreAddress.ZipPostalCode = address.ZipPostalCode;
                smartstoreAddress.Company = address.Company;
                smartstoreAddress.Email = address.Email;
                smartstoreAddress.FirstName = address.FirstName;
                smartstoreAddress.LastName = address.LastName;
                smartstoreAddress.PhoneNumber = address.PhoneNumber;
                smartstoreAddress.CreatedOnUtc = DateTime.Now;
                smartstoreAddress.CountryId = null;
                smartstoreAddress.DistrictId = null;
                smartstoreAddress.TownId = null;
                smartstoreAddress.CityId = null;
                smartstoreAddress.StateProvinceId = null;
                smartstoreAddress.FaxNumber = null;
                smartstoreAddress.Salutation = null;
                smartstoreAddress.TaxNumber = null;
                smartstoreAddress.TaxOffice = null;
                smartstoreAddress.Title = null;
                smartstoreAddress.CreatedOnUtc = DateTime.UtcNow;

                return smartstoreAddress;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Address mapping sırasında hata oluştu. AddressId: {AddressId}", address.Id);
                return null;
            }
        }

        public static IEnumerable<AddressDto> ToDtoList(IEnumerable<SmartstoreAddressDto> smartstoreAddresses)
        {
            if (smartstoreAddresses == null)
                yield break;

            foreach (var smartstoreAddress in smartstoreAddresses)
            {
                var dto = ToDto(smartstoreAddress);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
