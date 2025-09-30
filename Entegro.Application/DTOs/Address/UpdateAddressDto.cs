namespace Entegro.Application.DTOs.Address
{
    public class UpdateAddressDto
    {
        public int Id { get; set; }
        public string? Salutation { get; set; }
        public string? Title { get; set; }
        public string? AddressType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Company { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Town { get; set; }
        public string? District { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string? ZipPostalCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FaxNumber { get; set; }
        public string? TaxOffice { get; set; }
        public string? TaxOfficeNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
