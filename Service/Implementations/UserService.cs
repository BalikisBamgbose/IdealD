﻿using IdealDiscuss.Dtos;
using IdealDiscuss.Dtos.UserDto;
using IdealDiscuss.Entities;
using IdealDiscuss.Repository.Interfaces;
using IdealDiscuss.Service.Interface;

namespace IdealDiscuss.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            IUserRepository userRepository, 
            IRoleRepository roleRepository, 
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public BaseResponseModel AddUser(CreateUserDto request, string roleName)
        {
            try
            {
                var createdBy = _httpContextAccessor.HttpContext.User.Identity.Name;
                var createdDate = DateTime.Now;

                var formValueCheck = string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrEmpty(request.Email) || string.IsNullOrWhiteSpace(request.Password);

                if (formValueCheck)
                {
                    return new BaseResponseModel
                    {
                        Message = $"One or more form fields are required!",
                        Status = false
                    };
                }

                var userExist = _userRepository.Exists(x => x.UserName == request.UserName || x.Email == request.Email);

                if (userExist)
                {
                    return new BaseResponseModel
                    {
                        Message = $"User with {request.UserName} or {request.Email} already exist",
                        Status = false
                    };
                }

                roleName ??= "AppUser";

                var role = _roleRepository.Get(x => x.RoleName == roleName);

                var user = new User
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    Password = request.Password,
                    RoleId = role.Id,
                    CreatedBy = createdBy,
                    DateCreated = createdDate,
                };

                _userRepository.Create(user);
            }
            catch (Exception ex)
            {
                return new BaseResponseModel
                {
                    Message = $"Unable to create user: {ex.Message}"
                };
            }

            return new BaseResponseModel
            {
                Message = $"User with {request.UserName} added succesfully",
                Status = true
            };
        }

        public ViewUserDto GetUser(int userId)
        {
            var user = _userRepository.GetUser(x => x.Id == userId);

            if (user is null)
            {
                return new ViewUserDto
                {
                    Message = $"User with {userId} does not exist",
                    Status = false
                };
            }

            return new ViewUserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.Role.RoleName,
                Message = $"User successfully retrieved",
                Status = true
            };
        }

        public ViewUserDto Login(string username, string password)
        {
            var response = new ViewUserDto();

            try
            {
                var user = _userRepository.GetUser(x => (x.UserName.ToLower() == username.ToLower() || x.Email.ToLower() == username.ToLower()) && x.Password == password);

                if (user is null)
                {
                    return new ViewUserDto
                    {
                        Message = $"Invalid username or password",
                        Status = false
                    };
                }

                response.Id = user.Id;
                response.UserName = user.UserName;
                response.Email = user.Email;
                response.RoleId = user.RoleId;
                response.RoleName = user.Role.RoleName;
                response.Message = $"User successfully retrieved";
                response.Status = true;
            }
            catch (Exception ex)
            {
                response.Message = $"An error occured: {ex.Message}";
                return response;
            }

            return response;
        }
    }
}
