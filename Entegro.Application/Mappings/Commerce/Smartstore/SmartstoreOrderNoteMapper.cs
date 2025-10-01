using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.OrderNote;
using Microsoft.Extensions.Logging;

namespace Entegro.Application.Mappings.Commerce.Smartstore
{
    public static class SmartstoreOrderNoteMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static OrderNoteDto? ToDto(SmartstoreOrderNoteDto smartstoreOrderNote)
        {
            try
            {
                if (smartstoreOrderNote == null)
                {
                    return null;
                }

                OrderNoteDto orderNote = new OrderNoteDto();
                orderNote.Id = 0;
                orderNote.OrderId = smartstoreOrderNote.OrderId;
                orderNote.Note = smartstoreOrderNote.Note;
                orderNote.CreatedOn = smartstoreOrderNote.CreatedOnUtc;


                return orderNote;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "OrderNote mapping sırasında hata oluştu. OrderNoteId: {OrderNoteId}", smartstoreOrderNote.Id);
                return null;
            }
        }

        public static IEnumerable<OrderNoteDto> ToDtoList(IEnumerable<SmartstoreOrderNoteDto> smartstoreOrderNotes)
        {
            if (smartstoreOrderNotes == null)
                yield break;

            foreach (var smartstoreOrderNote in smartstoreOrderNotes)
            {
                var dto = ToDto(smartstoreOrderNote);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
