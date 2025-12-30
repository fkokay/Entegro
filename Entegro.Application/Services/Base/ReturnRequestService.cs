using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.ReturnRequest;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Domain.Entities.Checkout;
using MapsterMapper;

namespace Entegro.Application.Services.Base
{
    public class ReturnRequestService : IReturnRequestService
    {
        private readonly IReturnRequestRepository _returnRequestRepository;
        private readonly IMapper _mapper;
        public ReturnRequestService(IReturnRequestRepository returnRequestRepository, IMapper mapper)
        {
            _returnRequestRepository = returnRequestRepository ?? throw new ArgumentNullException(nameof(returnRequestRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task AddAsync(CreateReturnRequestDto returnRequest)
        {
            var request = _mapper.Map<ReturnRequest>(returnRequest);
            await _returnRequestRepository.AddAsync(request);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var request = await _returnRequestRepository.GetByIdAsync(id);
            if (request == null)
                throw new KeyNotFoundException($"ID {id} ile Brand bulunamadı.");

            await _returnRequestRepository.DeleteAsync(request);
        }

        public async Task<bool> ExistsByCustomerNameAsync(string customerName)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("CustomerName boş olamaz.", nameof(customerName));

            return await _returnRequestRepository.ExistsByCustomerNameAsync(customerName);
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            return await _returnRequestRepository.ExistsByIdAsync(id);
        }

        public async Task<bool> ExistsByOrderNumberAsync(string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
                throw new ArgumentException("OrderNumber boş olamaz.", nameof(orderNumber));

            return await _returnRequestRepository.ExistsByOrderNumberAsync(orderNumber);
        }

        public async Task<bool> ExistsByReturnRequestStatusAsync(int requestStatus)
        {
            if (requestStatus <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requestStatus));
            }
            return await _returnRequestRepository.ExistsByReturnRequestStatusAsync(requestStatus);
        }

        public async Task<ReturnRequestDto?> GetByCustomerNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Müşteri adı boş olamaz.", nameof(name));
            }

            var returnRequest = await _returnRequestRepository.GetByCustomerNameAsync(name);
            var returnRequestDto = _mapper.Map<ReturnRequestDto>(returnRequest);

            return returnRequestDto;
        }

        public async Task<ReturnRequestDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            var returnRequest = await _returnRequestRepository.GetByIdAsync(id);
            var returnRequestDto = _mapper.Map<ReturnRequestDto>(returnRequest);
            return returnRequestDto;
        }

        public async Task<ReturnRequestDto?> GetByOrderNumberAsync(string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
            {
                throw new ArgumentException("Sipariş Numarası boş olamaz.", nameof(orderNumber));
            }

            var returnRequest = await _returnRequestRepository.GetByOrderNumberAsync(orderNumber);
            var returnRequestDto = _mapper.Map<ReturnRequestDto>(returnRequest);
            return returnRequestDto;
        }

        public async Task<ReturnRequestDto?> GetByReturnRequestDtoStatusAsync(int requestStatus)
        {
            if (requestStatus <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requestStatus));
            }

            var returnRequest = await _returnRequestRepository.GetByReturnRequestStatusAsync(requestStatus);
            var returnRequestDto = _mapper.Map<ReturnRequestDto>(returnRequest);

            return returnRequestDto;
        }

        public async Task<PagedResult<ReturnRequestDto>> GetPagedAsync(GridCommand gridCommand)
        {
            var returnRequests = await _returnRequestRepository.GetPagedAsync(gridCommand);

            var items = await returnRequests.Items.SelectAwait(async x =>
            {
                var model = _mapper.Map<ReturnRequestDto>(x);
                model.CreatedOnUtc = x.CreatedOnUtc.ToLocalTime();
                model.UpdatedOnUtc = x.UpdatedOnUtc.ToLocalTime();
                //model.RequestedActionUpdatedOn = x.RequestedActionUpdatedOnUtc.ToLocalTime();
                return model;
            }).AsyncToList();

            return new PagedResult<ReturnRequestDto>
            {
                Items = items,
                TotalCount = returnRequests.TotalCount,
                PageNumber = returnRequests.PageNumber,
                PageSize = returnRequests.PageSize
            };
        }

        public async Task<PagedResult<ReturnRequestListDto>> GetPagedAsync(GridCommand gridCommand, ReturnRequestListFilterDto filters, int returnrequestStatusId)
        {
            var returnRequests = await _returnRequestRepository.GetPagedAsync(gridCommand, filters, returnrequestStatusId);

            var items = await returnRequests.Items.SelectAwait(async x =>
            {
                var model = _mapper.Map<ReturnRequestListDto>(x);
                model.CreatedOnUtc = x.CreatedOnUtc.ToLocalTime();
                model.UpdatedOnUtc = x.UpdatedOnUtc.ToLocalTime();
                model.ClaimDate = x.ClaimDate.ToLocalTime();
                model.OrderDate = x.OrderDate.ToLocalTime();
                return model;
            }).AsyncToList();

            return new PagedResult<ReturnRequestListDto>
            {
                Items = items,
                TotalCount = returnRequests.TotalCount,
                PageNumber = returnRequests.PageNumber,
                PageSize = returnRequests.PageSize
            };
        }

        public async Task<ReturnListPageDto> GetReturnRequestPageAsync()
        {
            var returnRequestPage = await _returnRequestRepository.GetReturnRequestPageAsync();
            return returnRequestPage;
        }

        public async Task<ReturnRequestDto> UpdateAsync(UpdateReturnRequestDto returnRequest)
        {
            if (returnRequest == null)
                throw new ArgumentNullException(nameof(returnRequest));

            var existingReturnRequest = await _returnRequestRepository.GetByIdAsync(returnRequest.Id);
            if (existingReturnRequest == null)
                throw new KeyNotFoundException($"ID {returnRequest.Id} ile ReturnRequest bulunamadı.");

            _mapper.Map(returnRequest, existingReturnRequest);
            await _returnRequestRepository.UpdateAsync(existingReturnRequest);

            return _mapper.Map<ReturnRequestDto>(existingReturnRequest);
        }
    }
}
