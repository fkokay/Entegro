using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.User;

namespace Entegro.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(int userId);
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<PagedResult<UserDto>> GetUsersAsync(int pageNumber, int pageSize);
        Task<int> CreateUserAsync(CreateUserDto createUser);
        Task<bool> UpdateUserAsync(UpdateUserDto updateUser);
        Task<bool> DeleteUserAsync(int userId);

        Task<UserDto?> GetByEmailAndPasswordAsync(string email, string password);
    }
}
