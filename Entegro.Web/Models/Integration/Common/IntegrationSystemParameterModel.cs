namespace Entegro.Web.Models.Integration.Common
{
    public class IntegrationSystemParameterModel
    {
        public int Id { get; set; }
        public int IntegrationSystemTypeId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
