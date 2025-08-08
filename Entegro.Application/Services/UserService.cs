using AutoMapper;
using Entegro.Application.DTOs.User;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.User;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository,IMapper mapper) 
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<int> CreateUserAsync(CreateUserDto createUser)
        {
            var user = _mapper.Map<User>(createUser);
            await _userRepository.AddAsync(user);

            return user.Id;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }
            await _userRepository.DeleteAsync(user);
            return true;
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
            return userDtos;
        }

        public async Task<PagedResult<UserDto>> GetUsersAsync(int pageNumber, int pageSize)
        {
            var users = await _userRepository.GetAllAsync(pageNumber, pageSize);
            var userDtos = _mapper.Map<PagedResult<UserDto>>(users);
            return userDtos;
        }

        public async Task<bool> UpdateUserAsync(UpdateUserDto updateUser)
        {
            await _userRepository.UpdateAsync(_mapper.Map<User>(updateUser));
            return true;
        }
    }
}
