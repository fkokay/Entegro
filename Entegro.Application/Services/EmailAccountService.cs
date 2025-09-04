
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.EmailAccount;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;
using MapsterMapper;

namespace Entegro.Application.Services
{
    public class EmailAccountService : IEmailAccountService
    {
        private readonly IEmailAccountRepository _emailAccountRepository;
        private readonly IMapper _mapper;
        public EmailAccountService(IEmailAccountRepository emailAccountRepository, IMapper mapper)
        {
            _emailAccountRepository = emailAccountRepository ?? throw new ArgumentNullException(nameof(emailAccountRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<EmailAccountDto> AddAsync(CreateEmailAccountDto emailAccount)
        {
            if (emailAccount == null)
                throw new ArgumentNullException(nameof(emailAccount));

            var model = _mapper.Map<EmailAccount>(emailAccount);
            await _emailAccountRepository.AddAsync(model);

            return _mapper.Map<EmailAccountDto>(model);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var emailAccount = await _emailAccountRepository.GetByIdAsync(id);
            if (emailAccount == null)
                throw new KeyNotFoundException($"ID {id} ile Email bulunamadı.");

            await _emailAccountRepository.DeleteAsync(emailAccount);
        }

        public async Task<List<EmailAccountDto>> GetAllAsync()
        {
            var emailAccounts = await _emailAccountRepository.GetAllAsync();
            var emailAccountDtos = _mapper.Map<List<EmailAccountDto>>(emailAccounts);
            return emailAccountDtos;
        }

        public async Task<EmailAccountDto> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var emailAccount = await _emailAccountRepository.GetByIdAsync(id);
            if (emailAccount == null)
            {
                return null;
            }
            var emailAccountDto = _mapper.Map<EmailAccountDto>(emailAccount);

            return emailAccountDto;
        }

        public async Task<PagedResult<EmailAccountDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7)
        {
            if (pageNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize));


            var emails = await _emailAccountRepository.GetAllAsync(pageNumber, pageSize);
            return new PagedResult<EmailAccountDto>
            {
                Items = _mapper.Map<IEnumerable<EmailAccountDto>>(emails.Items),
                TotalCount = emails.TotalCount,
                PageNumber = emails.PageNumber,
                PageSize = emails.PageSize
            };
        }

        public async Task<EmailAccountDto> UpdateAsync(UpdateEmailAccountDto emailAccount)
        {
            if (emailAccount == null)
                throw new ArgumentNullException(nameof(emailAccount));

            var existingEmailAccount = await _emailAccountRepository.GetByIdAsync(emailAccount.Id);
            if (existingEmailAccount == null)
                throw new KeyNotFoundException($"ID {emailAccount.Id} ile EmailAccount bulunamadı.");

            _mapper.Map(emailAccount, existingEmailAccount);
            await _emailAccountRepository.UpdateAsync(existingEmailAccount);

            return _mapper.Map<EmailAccountDto>(existingEmailAccount);
        }

        Task<PagedResult<EmailAccountDto>> IEmailAccountService.GetPagedAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
