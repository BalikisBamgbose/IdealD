﻿using IdealDiscuss.Dtos;
using IdealDiscuss.Dtos.FlagDto;
using IdealDiscuss.Entities;
using IdealDiscuss.Repository.Interfaces;
using IdealDiscuss.Service.Interface;
using System.Linq.Expressions;

namespace IdealDiscuss.Service.Implementations
{
    public class FlagService : IFlagService
    {
        private readonly IFlagRepository _flagRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FlagService(IFlagRepository flagRepository, IHttpContextAccessor httpContextAccessor)
        {
            _flagRepository = flagRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public BaseResponseModel CreateFlag(CreateFlagDto createFlagDto)
        {
            var response = new BaseResponseModel();
            var createdBy = _httpContextAccessor.HttpContext.User.Identity.Name;
            var createdDate = DateTime.Now;

            var isFlagExist = _flagRepository.Exists(c => c.FlagName == createFlagDto.FlagName);

            if (isFlagExist)
            {
                response.Message = $"Flag with {createFlagDto.FlagName} already exist!";
                return response;
            }

            if (string.IsNullOrWhiteSpace(createFlagDto.FlagName))
            {
                response.Message = "Flag name is required!";
                return response;
            }

            var flag = new Flag()
            {
                FlagName = createFlagDto.FlagName,
                Description = createFlagDto.Description,
                CreatedBy = createdBy,
                DateCreated = createdDate
            };

            try
            {
                _flagRepository.Create(flag);
            }
            catch (Exception ex)
            {
                response.Message = $"Failed to create Flag. {ex.Message}";
                return response;
            }

            response.Status = true;
            response.Message = "Flag created successfully.";

            return response;

        }

        public BaseResponseModel DeleteFlag(int flagId)
        {
            var response = new BaseResponseModel();
            var flagExist = _flagRepository.Exists(x => x.Id == flagId);

            if (!flagExist)
            {
                response.Message = "Flag does not exist!";
                return response;
            }

            var flags = _flagRepository.Get(flagId);
            flags.IsDeleted = true;

            try
            {
                _flagRepository.Update(flags);
            }
            catch (Exception)
            {
                response.Message = "Flag delete failed";
                return response;
            }

            response.Status = true;
            response.Message = "Flag deleted successfully.";
            return response;
        }

        public FlagsResponseModel GetAllFlag()
        {
            var response = new FlagsResponseModel();

            try
            {
                var flags = _flagRepository.GetAll(f => f.IsDeleted == false);

                if (flags is null || flags.Count == 0)
                {
                    response.Message = "No flags found!";
                    return response;
                }

                response.Data = flags
                   .Select(f => new ViewFlagDto
                   {
                       Id = f.Id,
                       FlagName = f.FlagName,
                       Description = f.Description
                   }).ToList();

                response.Status = true;
                response.Message = "Success";
            }
            catch(Exception ex)
            {
                response.Message = ex.StackTrace;
                return response;
            }

            return response;
        }

        public FlagResponseModel GetFlag(int flagId)
        {
            var response = new FlagResponseModel();
            var flagExist = _flagRepository.Exists(f => (f.Id == flagId) && (f.Id == flagId && f.IsDeleted == false));

            if (!flagExist)
            {
                response.Message = $"Flag with id {flagId} does not exist.";
                return response;
            }

            try
            {
                var flags = _flagRepository.Get(flagId);

                response.Message = "Success";
                response.Status = true;
                response.Data = new ViewFlagDto
                {
                    Id = flags.Id,
                    FlagName = flags.FlagName,
                    Description = flags.Description
                };
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                return response;
            }

            return response;
        }

        public BaseResponseModel UpdateFlag(int flagId, UpdateFlagDto updateFlagDto)
        {
            var response = new BaseResponseModel();
            var modifiedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
            var modifiedDate = DateTime.Now;
            Expression<Func<Flag, bool>> expression = f => (f.Id == flagId) 
                                                && (f.Id == flagId 
                                                && f.IsDeleted == false);
            var isFlagExist = _flagRepository.Exists(expression);

            if (!isFlagExist)
            {
                response.Message = "Flag does not exist!";
                return response;
            }

            if(string.IsNullOrWhiteSpace(updateFlagDto.FlagName))
            {
                response.Message = "Flag name cannot be null!";
                return response;
            }

            var flag = _flagRepository.Get(flagId);

            flag.FlagName = updateFlagDto.FlagName;
            flag.Description = updateFlagDto.Description;
            flag.ModifiedBy = modifiedBy;
            flag.LastModified = modifiedDate;

            try
            {
                _flagRepository.Update(flag);
            }
            catch (Exception ex)
            {
                response.Message = $"Could not update the flag: {ex.Message}";
                return response;
            }
            response.Message = "Flag updated successfully.";
            return response;
        }
    }
}

