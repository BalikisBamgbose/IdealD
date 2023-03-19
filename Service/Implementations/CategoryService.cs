﻿using IdealDiscuss.Dtos;
using IdealDiscuss.Dtos.CategoryDto;
using IdealDiscuss.Dtos.FlagDto;
using IdealDiscuss.Entities;
using IdealDiscuss.Repository.Implementations;
using IdealDiscuss.Repository.Interfaces;
using IdealDiscuss.Service.Interface;

namespace IdealDiscuss.Service.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICategoryQuestionRepository _categoryQuestionRepository;

        public CategoryService(ICategoryRepository categoryRepository, IHttpContextAccessor httpContextAccessor,ICategoryQuestionRepository categoryQuestionRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _categoryQuestionRepository = categoryQuestionRepository;
            _categoryRepository = categoryRepository;
         
        }

        public BaseResponseModel CreateCategory(CreateCategoryDto createCategoryDto)
        {
            var response = new BaseResponseModel();
            var createdBy = _httpContextAccessor.HttpContext.User.Identity.Name;

            var isCategoryExist = _categoryRepository.Exists(c => c.Name == createCategoryDto.Name);

            if (isCategoryExist)
            {
                response.Message = "Category already exist!";
                return response;
            }

            if (string.IsNullOrWhiteSpace(createCategoryDto.Name))
            {
                response.Message = "Category name is required!";
                return response;
            }

            var category = new Category
            {
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description,
                CreatedBy = createdBy,
                DateCreated = DateTime.Now
            };

            try
            {
                _categoryRepository.Create(category);
            }
            catch (Exception ex)
            {
                response.Message = $"Failed to create category at this time: {ex.Message}";
                return response;
            }

            response.Status = true;
            response.Message = "Category created successfully.";

            return response;
        }

        public BaseResponseModel DeleteCategory(int categoryId)
        {
            var response = new BaseResponseModel();

            if (!_categoryRepository.Exists(c => c.Id == categoryId))
            {
                response.Message = "Category does not exist.";
                return response;
            }

            var category = _categoryRepository.Get(categoryId);
            category.IsDeleted = true;

            try
            {
                _categoryRepository.Update(category);
            }
            catch (Exception ex)
            {
                response.Message = $"Can not delete category: {ex.Message}";
                return response;
            }

            response.Status = true;
            response.Message = "Category successfully deleted.";
            return response;
        }

        public CategoriesResponseModel GetAllCategory()
        {
            var response = new CategoriesResponseModel();

            var category = _categoryRepository.GetAll();

            response.Data = category.Select(category => new ViewCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            }).ToList();

            response.Status = true;
            response.Message = "Success";

            return response;
        }

        public CategoryResponseModel GetCategory(int categoryId)
        {
			var response = new CategoryResponseModel();
            var categoryExist = _categoryRepository.Exists(c => c.Id == categoryId);
			var categoryIsDeleted = _categoryRepository.Exists(c => c.Id == categoryId && c.IsDeleted == true);

			if (!categoryExist || categoryIsDeleted)
			{
                response.Message = $"Category with id {categoryId} does not exist.";
                return response;
            }
            var category = _categoryRepository.Get(categoryId);

            response.Message = "Success";
            response.Status = true;
            response.Data = new ViewCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
            };

            return response;
        }

        public BaseResponseModel UpdateCategory(int categoryId, UpdateCategoryDto updateCategoryDto)
        {
            var response = new BaseResponseModel();
            string modifiedBy = _httpContextAccessor.HttpContext.User.Identity.Name;

            if (!_categoryRepository.Exists(c => c.Id == categoryId))
            {
                response.Message = "Category does not exist.";
                return response;
            }

            var category = _categoryRepository.Get(categoryId);
            category.Description = updateCategoryDto.Description;
            category.ModifiedBy = modifiedBy;
            category.LastModified = DateTime.Now;
            try
            {
                _categoryRepository.Update(category);
            }
            catch (Exception ex)
            {
                response.Message = $"Could not update the category: {ex.Message}";
                return response;
            }
            response.Message = "Category updated successfully.";
            return response;
        }
        
    }
}
