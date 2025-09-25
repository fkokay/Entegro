using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.User;

namespace Entegro.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(int userId);
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<PagedResult<UserDto>> GetUsersAsync(int pageNumber, int pageSize);
        Task<PagedResult<UserDto>> GetPagedAsync(GridCommand gridCommand);
        Task<int> AddAsync(CreateUserDto createUser);
        Task<bool> UpdateAsync(UpdateUserDto updateUser);
        Task<bool> DeleteAsync(int userId);

        Task<UserDto?> GetByEmailAndPasswordAsync(string email, string password);
    }
}
