﻿using FitnessApp.Data;
using FitnessApp.Logic.ApiModels;
using FitnessApp.Logic.Builders;
using FitnessApp.Logic.Models;
using FitnessApp.Logic.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Logic.Services
{
    public class ProductCategoryService : BaseService, IProductCategoryService
    {
        private readonly ICustomValidator<ProductCategoryDto> _validator;

        public ProductCategoryService(ProductContext context, ICustomValidator<ProductCategoryDto> validator) : base(context)
        {
            _validator = validator;
        }

        /// <summary> Gets all product categories from DB. </summary>
        /// <returns> Returns collection of product categories. </returns>
        /// <exception cref="Exception"></exception>
        public async Task<ICollection<ProductCategoryDto>> GetAllAsync()
        {
            var categoryDbs = await _context.ProductCategories.ToListAsync().ConfigureAwait(false);

            return ProductCategoryBuilder.Build(categoryDbs) ?? throw new Exception($"There are not objects of product categories.");
        }

        /// <summary> Outputs paginated product categories from DB, depending on the selected conditions.</summary>
        /// <param name="request"></param>
        /// <returns> Returns a PaginationResponse object containing a sorted collection of product categories. </returns>
        public async Task<PaginationResponse<ProductCategoryDto>> GetPaginationAsync(PaginationRequest request)
        {
            var query = _context.ProductCategories.AsQueryable();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(c => c.Title.Contains(request.Query, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(request.SortBy))
            {
                switch (request.SortBy)
                {
                    case "title": query = request.Ascending ? query.OrderBy(c => c.Title) : query.OrderByDescending(c => c.Title); break;
                }
            }

            var categoryDbs = await query.ToListAsync().ConfigureAwait(false);

            var total = categoryDbs.Count;
            var categoryDtos = ProductCategoryBuilder.Build(categoryDbs.Skip(request.Skip ?? 0).Take(request.Take ?? 10)?.ToList());

            return new PaginationResponse<ProductCategoryDto>
            {
                Total = total,
                Values = categoryDtos
            };
        }

        /// <summary> Gets product category from DB by Id. </summary>
        /// <param name="productCategoryId"></param>
        /// <returns> Returns object of product category with Id: <paramref name="productCategoryId"/>. </returns>
        /// <exception cref="ValidationException"></exception>
        public async Task<ProductCategoryDto> GetByIdAsync(int? productCategoryId)
        {
            if (productCategoryId == null)
            {
                throw new ValidationException("Product Category Id can't be null.");
            }

            var categoryDb = await _context.ProductCategories.SingleOrDefaultAsync(_ => _.Id == productCategoryId).ConfigureAwait(false);
            
            return ProductCategoryBuilder.Build(categoryDb);
        }

        /// <summary> Creates new product category. </summary>
        /// <param name="productCategoryDto"></param>
        /// <returns> Returns operation status code. </returns>
        /// <exception cref="Exception"></exception>
        public async Task CreateAsync(ProductCategoryDto productCategoryDto)
        {
            _validator.Validate(productCategoryDto, "AddProductCategory");

            productCategoryDto.Created = DateTime.UtcNow;
            productCategoryDto.Updated = DateTime.UtcNow;

            await _context.ProductCategories.AddAsync(ProductCategoryBuilder.Build(productCategoryDto)).ConfigureAwait(false);
            
            try 
            {
                await _context.SaveChangesAsync().ConfigureAwait(false); 
            }
            catch (Exception ex) 
            { 
                throw new Exception($"Product category has not been created. {ex.Message}.");
            }
        }

        /// <summary> Updates product category in DB. </summary>
        /// <param name="productCategoryDto"></param>
        /// <returns> Returns operation status code. </returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ValidationException"></exception>
        public async Task UpdateAsync(ProductCategoryDto productCategoryDto)
        {
            _validator.Validate(productCategoryDto, "UpdateProductCategory");

            var categoryDb = await _context.ProductCategories.SingleOrDefaultAsync(_ => _.Id == productCategoryDto.Id).ConfigureAwait(false);
            
            if (categoryDb != null)
            {
                categoryDb.Title = productCategoryDto.Title;
                categoryDb.Updated = DateTime.UtcNow;

                try 
                { 
                    await _context.SaveChangesAsync().ConfigureAwait(false); 
                }
                catch (Exception ex) 
                {
                    throw new Exception($"Product category has not been updated. {ex.Message}.");
                }
            }
            else
            {
                throw new ValidationException($"There is not exist object, that you trying to update.");
            }   
        }

        /// <summary> Deletes product category from DB. </summary>
        /// <param name="productCategoryId"></param>
        /// <returns> Returns operation status code. </returns>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task DeleteAsync(int? productCategoryId)
        {
            if (productCategoryId == null)
            {
                throw new ValidationException("Invalid product category Id.");
            }

            var categoryDb = await _context.ProductCategories.SingleOrDefaultAsync(_ => _.Id == productCategoryId).ConfigureAwait(false);

            if (categoryDb != null)
            {
                _context.ProductCategories.Remove(categoryDb);

                try
                {
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Product category has not been deleted. {ex.Message}.");
                }
            }
            else
            {
                throw new ValidationException($"There is not exist object, that you trying to delete.");
            }    
        }
    }
}
