using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Invoice;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Domain.Entities.Checkout;
using MapsterMapper;

namespace Entegro.Application.Services.Base
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;
        public InvoiceService(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository ?? throw new ArgumentNullException(nameof(invoiceRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<InvoiceDto> AddAsync(CreateInvoiceDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var brand = _mapper.Map<Invoice>(model);
            await _invoiceRepository.AddAsync(brand);

            return _mapper.Map<InvoiceDto>(brand);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null)
                throw new KeyNotFoundException($"ID {id} ile Fatura bulunamadı.");

            await _invoiceRepository.DeleteAsync(invoice);
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return await _invoiceRepository.ExistsByIdAsync(id);
        }

        public async Task<bool> ExistsByInvoiceNumberAsync(string invoiceNumber)
        {
            if (string.IsNullOrWhiteSpace(invoiceNumber))
                throw new ArgumentException("Fatura Numarası boş olamaz.", nameof(invoiceNumber));

            return await _invoiceRepository.ExistsByInvoiceNumberAsync(invoiceNumber);
        }

        public async Task<bool> ExistsByPackageNoAsync(string packageNo)
        {
            if (string.IsNullOrWhiteSpace(packageNo))
                throw new ArgumentException("Paket Numarası boş olamaz.", nameof(packageNo));

            return await _invoiceRepository.ExistsByPackageNoAsync(packageNo);
        }

        public async Task<InvoiceDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null)
            {
                return null;
            }
            var invoiceDto = _mapper.Map<InvoiceDto>(invoice);
            return invoiceDto;
        }

        public async Task<InvoiceDto?> GetByInvoiceNumberAsync(string invoiceNumber)
        {
            if (string.IsNullOrWhiteSpace(invoiceNumber))
                throw new ArgumentException("Fatura Numarası boş olamaz.", nameof(invoiceNumber));
            var invoice = await _invoiceRepository.GetByInvoiceNumberAsync(invoiceNumber);
            if (invoice == null)
            {
                return null;
            }
            var invoiceDto = _mapper.Map<InvoiceDto>(invoice);
            return invoiceDto;
        }

        public async Task<InvoiceDto?> GetByPackageNoAsync(string packageNo)
        {
            if (string.IsNullOrWhiteSpace(packageNo))
                throw new ArgumentException("Fatura Numarası boş olamaz.", nameof(packageNo));
            var invoice = await _invoiceRepository.GetByPackageNoAsync(packageNo);
            if (invoice == null)
            {
                return null;
            }
            var invoiceDto = _mapper.Map<InvoiceDto>(invoice);
            return invoiceDto;
        }

        public async Task<PagedResult<InvoiceDto>> GetPagedAsync(GridCommand gridCommand)
        {
            var invoices = await _invoiceRepository.GetPagedAsync(gridCommand);

            var items = await invoices.Items.SelectAwait(async x =>
            {
                var model = _mapper.Map<InvoiceDto>(x);
                return model;
            }).AsyncToList();

            return new PagedResult<InvoiceDto>
            {
                Items = items,
                TotalCount = invoices.TotalCount,
                PageNumber = invoices.PageNumber,
                PageSize = invoices.PageSize
            };
        }

        public async Task<InvoiceDto> UpdateAsync(UpdateInvoiceDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var existingInvoice = await _invoiceRepository.GetByIdAsync(model.Id);
            if (existingInvoice == null)
                throw new KeyNotFoundException($"ID {model.Id} ile Fatura bulunamadı.");

            _mapper.Map(model, existingInvoice);
            await _invoiceRepository.UpdateAsync(existingInvoice);
            return _mapper.Map<InvoiceDto>(existingInvoice);
        }
    }
}
