﻿using FitnessApp.Logic.ApiModels;
using FitnessApp.Logic.Models;
using FitnessApp.Logic.Services;
using FitnessApp.Logic.Validators;
using FitnessApp.Web.SwaggerExamples;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Filters;
using FitnessApp.Localization;
using EventBus.Base.Standard;
using FitnessApp.Logging.Events;

namespace FitnessApp.Web.Controllers
{
    [ApiController]
    [Route("api/v1.0/productSubCategory")]
    public class ProductSubCategoryController : ControllerBase
    {
        private readonly IProductSubCategoryService _productSubCategoryService;
        private readonly ICustomValidator<ProductSubCategoryDto> _validator;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly IEventBus _eventBus;

        public ProductSubCategoryController(IProductSubCategoryService productSubCategoryService, ICustomValidator<ProductSubCategoryDto> validator, 
            IStringLocalizer<SharedResource> sharedLocalizer, IEventBus eventBus)
        {
            _productSubCategoryService = productSubCategoryService;
            _validator = validator;
            _sharedLocalizer = sharedLocalizer;
            _eventBus = eventBus;
        }

        /// <summary> Gets all product subcategories from DB. </summary>
        /// <returns> Returns collection of product subcategories. </returns>
        /// <response code="200"> Sucsess. </response>
        /// <response code="404"> Not found collection of objects. </response>
        /// <response code="500"> Something wrong on the Server. </response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ICollection<ProductSubCategoryDto>> GetAllAsync()
        {
            return await _productSubCategoryService.GetAllAsync();
        }

        /// <summary> Outputs paginated product subcategories from DB, depending on the selected conditions.</summary>
        /// <param name="request"></param>
        /// <returns> Returns a PaginationResponse object containing a sorted collection of product subcategories. </returns>
        /// <exception cref="Exception"></exception>
        /// <response code="200"> Sucsess. </response>
        /// <response code="404"> Not found collection of objects. </response>
        /// <response code="500"> Something wrong on the Server. </response>
        [HttpPost("pagination")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<PaginationResponse<ProductSubCategoryDto>> GetPaginationAsync([FromBody] PaginationRequest request)
        {
            return await _productSubCategoryService.GetPaginationAsync(request);
        }

        /// <summary> Gets product subcategory from DB by Id. </summary>
        /// <param name="productSubCategoryId" example="666">The product subcategory Id. </param>
        /// <returns> Returns object of product subcategory with Id: <paramref name="productSubCategoryId"/>. </returns>
        /// <remarks> Field "id" must be only positive number </remarks>
        /// <response code="200"> Sucsess. </response>
        /// <response code="404"> Object with this Id not found. </response>
        /// <response code="500"> Something wrong on the Server. </response>
        [HttpGet("{productSubCategoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ProductSubCategoryDto> GetByIdAsync(int? productSubCategoryId)
        {
            if (productSubCategoryId == null)
                throw new ValidationException(_sharedLocalizer["ObjectIdCantBeNull"]);

            return await _productSubCategoryService.GetByIdAsync(productSubCategoryId) ?? throw new Exception(_sharedLocalizer["NotExistObjectWithThisId"]);
        }

        /// <summary> Creates new product subcategory. </summary>
        /// <param name="productSubCategoryDto"></param>
        /// <returns> Returns operation status code. </returns>
        /// <remarks> Required fields: "title" (Lenght:1-30 symbols; restriction: only letters); "productCategoryId" (must be positive number). </remarks>
        /// <response code="200"> Sucsess. </response>
        /// <response code="400"> Incorrect request of object creation. </response>
        /// <response code="500"> Something wrong on the Server. </response>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(ProductSubCategoryDto), typeof(ProductSubCategoryCreateExample))]
        public async Task CreateAsync([FromBody] ProductSubCategoryDto productSubCategoryDto)
        {
            _validator.Validate(productSubCategoryDto, "AddProductSubCategory");

            await _productSubCategoryService.CreateAsync(productSubCategoryDto);
            _eventBus.Publish(new LogEvent(Statuses.Success, Actions.Creation, EntityTypes.ProductSubCategory, productSubCategoryDto));
        }

        /// <summary> Updates product subcategory in DB. </summary>
        /// <param name="productSubCategoryDto"></param>
        /// <returns> Returns operation status code. </returns>
        /// <remarks> Required fields: "title" (Lenght:1-30 symbols; restriction: only letters); "id", "productCategoryId" (must be positive number). </remarks>
        /// <response code="200"> Sucsess. </response>
        /// <response code="404"> That object not found.  </response>
        /// <response code="500"> Something wrong on the Server. </response>
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(ProductSubCategoryDto), typeof(ProductSubCategoryUpdateExample))]
        public async Task UpdateAsync([FromBody] ProductSubCategoryDto productSubCategoryDto)
        {
            _validator.Validate(productSubCategoryDto, "UpdateProductSubCategory");

            await _productSubCategoryService.UpdateAsync(productSubCategoryDto);
            _eventBus.Publish(new LogEvent(Statuses.Success, Actions.Update, EntityTypes.ProductSubCategory, productSubCategoryDto));
        }

        /// <summary> Deletes product subcategory from DB. </summary>
        /// <param name="productSubCategoryId" example="666"> The product subcategory Id. </param>
        /// <returns> Returns operation status code. </returns>
        /// <remarks> Field "id" must be only positive number. </remarks>
        /// <exception cref="ValidationException"></exception>
        /// <response code="200"> Sucsess. </response>
        /// <response code="404"> Object with this Id not found. </response>
        /// <response code="500"> Something wrong on the Server. </response>
        [HttpDelete("{productSubCategoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task DeleteAsync(int? productSubCategoryId)
        {
            if (productSubCategoryId == null)
                throw new ValidationException(_sharedLocalizer["ObjectIdCantBeNull"]);

            await _productSubCategoryService.DeleteAsync(productSubCategoryId);
            _eventBus.Publish(new LogEvent(Statuses.Success, Actions.Deletion, EntityTypes.ProductSubCategory, $"with ID: {productSubCategoryId}"));
        }
    }
}
