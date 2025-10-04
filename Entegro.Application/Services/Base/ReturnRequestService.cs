using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ReturnRequest;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using MapsterMapper;

namespace Entegro.Application.Services.Base
{
    public class ReturnRequestService : IReturnRequestService
    {
        private readonly IReturnRequestRepository _returRequestRepository;
        private readonly IMapper _mapper;
        public ReturnRequestService(IReturnRequestRepository returRequestRepository, IMapper mapper)
        {
            _returRequestRepository = returRequestRepository ?? throw new ArgumentNullException(nameof(returRequestRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task AddAsync(CreateReturnRequestDto returRequest)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var request = await _returRequestRepository.GetByIdAsync(id);
            if (request == null)
                throw new KeyNotFoundException($"ID {id} ile Brand bulunamadı.");

            await _returRequestRepository.DeleteAsync(request);
        }

        public async Task<bool> ExistsByCustomerNameAsync(string customerName)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("CustomerName boş olamaz.", nameof(customerName));

            return await _returRequestRepository.ExistsByCustomerNameAsync(customerName);
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            return await _returRequestRepository.ExistsByIdAsync(id);
        }

        public async Task<bool> ExistsByOrderNumberAsync(string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
                throw new ArgumentException("OrderNumber boş olamaz.", nameof(orderNumber));

            return await _returRequestRepository.ExistsByOrderNumberAsync(orderNumber);
        }

        public async Task<bool> ExistsByReturnRequestStatusAsync(int requestStatus)
        {
            if (requestStatus <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requestStatus));
            }
            return await _returRequestRepository.ExistsByReturnRequestStatusAsync(requestStatus);
        }

        public async Task<ReturnRequestDto?> GetByCustomerNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Müşteri adı boş olamaz.", nameof(name));
            }

            var returnRequest = await _returRequestRepository.GetByCustomerNameAsync(name);
            var returnRequestDto = _mapper.Map<ReturnRequestDto>(returnRequest);

            return returnRequestDto;
        }

        public async Task<ReturnRequestDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            var returnRequest = await _returRequestRepository.GetByIdAsync(id);
            var returnRequestDto = _mapper.Map<ReturnRequestDto>(returnRequest);
            return returnRequestDto;
        }

        public async Task<ReturnRequestDto?> GetByOrderNumberAsync(string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
            {
                throw new ArgumentException("Sipariş Numarası boş olamaz.", nameof(orderNumber));
            }

            var returnRequest = await _returRequestRepository.GetByOrderNumberAsync(orderNumber);
            var returnRequestDto = _mapper.Map<ReturnRequestDto>(returnRequest);
            return returnRequestDto;
        }

        public async Task<ReturnRequestDto?> GetByReturnRequestDtoStatusAsync(int requestStatus)
        {
            if (requestStatus <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requestStatus));
            }

            var returnRequest = await _returRequestRepository.GetByReturnRequestStatusAsync(requestStatus);
            var returnRequestDto = _mapper.Map<ReturnRequestDto>(returnRequest);

            return returnRequestDto;
        }

        public async Task<PagedResult<ReturnRequestDto>> GetPagedAsync(GridCommand gridCommand)
        {
            var returnRequests = await _returRequestRepository.GetPagedAsync(gridCommand);

            var items = await returnRequests.Items.SelectAwait(async x =>
            {
                var model = _mapper.Map<ReturnRequestDto>(x);
                model.CreatedOn = x.CreatedOnUtc.ToLocalTime();
                model.UpdatedOn = x.UpdatedOnUtc.ToLocalTime();
                model.RequestedActionUpdatedOn = x.RequestedActionUpdatedOnUtc.ToLocalTime();
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

        public Task UpdateAsync(UpdateReturnRequestDto returRequest)
        {
            throw new NotImplementedException();
        }
    }
}
