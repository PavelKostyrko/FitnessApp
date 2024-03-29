﻿using EventBus.Base.Standard;
using FitnessApp.Localization;
using FitnessApp.Logging.Events;
using FitnessApp.Logic.ApiModels;
using FitnessApp.Logic.Models;
using FitnessApp.Logic.Services;
using FitnessApp.Logic.Validators;
using FitnessApp.Web.SwaggerExamples;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Filters;

namespace FitnessApp.Web.Controllers
{
    [ApiController]
    [Route("api/v1.0/productNutrient")]
    public class ProductNutrientController : ControllerBase
    {
        private readonly IProductNutrientService _productNutrientService;
        private readonly ICustomValidator<ProductNutrientDto> _validator;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly IEventBus _eventBus;

        public ProductNutrientController(IProductNutrientService productNutrientService, ICustomValidator<ProductNutrientDto> validator, 
            IStringLocalizer<SharedResource> sharedLocalizer, IEventBus eventBus)
        {
            _productNutrientService = productNutrientService;
            _validator = validator;
            _sharedLocalizer = sharedLocalizer;
            _eventBus = eventBus;
        }

        /// <summary> Gets all Product-Nutrients from DB. </summary>
        /// <returns> Returns collection of Product-Nutrients. </returns>
        /// <response code="200"> Sucsess. </response>
        /// <response code="404"> Not found collection of objects. </response>
        /// <response code="500"> Something wrong on the Server. </response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ICollection<ProductNutrientDto>> GetAllAsync()
        {
            return await _productNutrientService.GetAllAsync();
        }

        /// <summary> Outputs paginated Product-Nutrients from DB, depending on the selected conditions.</summary>
        /// <param name="request"></param>
        /// <returns> Returns a PaginationResponse object containing a sorted collection of Product-Nutrients. </returns>
        /// <exception cref="Exception"></exception>
        ///  <response code="200"> Sucsess. </response>
        /// <response code="404"> Not found collection of objects. </response>
        /// <response code="500"> Something wrong on the Server. </response>
        [HttpPost("pagination")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<PaginationResponse<ProductNutrientDto>> GetPaginationAsync([FromBody] PaginationRequest request)
        {
            return await _productNutrientService.GetPaginationAsync(request);
        }

        /// <summary> Gets Product-Nutrient from DB by Id. </summary>
        /// <param name="productNutrientId" example="666">The Product-Nutrient Id. </param>
        /// <returns> Returns object of Product-Nutrient with Id: <paramref name="productNutrientId"/>. </returns>
        /// <remarks> Field "id" must be only positive number </remarks>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="Exception"></exception>
        /// <response code="200"> Sucsess. </response>
        /// <response code="404"> Object with this Id not found. </response>
        /// <response code="500"> Something wrong on the Server. </response>
        [HttpGet("{productNutrientId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ProductNutrientDto> GetByIdAsync(int? productNutrientId)
        {
            if (productNutrientId == null)
                throw new ValidationException(_sharedLocalizer["ObjectIdCantBeNull"]);

            return await _productNutrientService.GetByIdAsync(productNutrientId) ?? throw new Exception(_sharedLocalizer["NotExistObjectWithThisId"]);
        }

        /// <summary> Creates new Product-Nutrient. </summary>
        /// <param name="productNutrientDto"></param>
        /// <returns> Returns operation status code. </returns>
        /// <remarks> Required fields: "productId", "nutrientId", "treatingTypeId" (must be positive number); "quality" (must be equals or more than zero). </remarks>
        /// <response code="200"> Sucsess. </response>
        /// <response code="400"> Incorrect request of object creation. </response>
        /// <response code="500"> Something wrong on the Server. </response>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(ProductNutrientDto), typeof(ProductNutrientCreateExample))]
        public async Task CreateAsync([FromBody] ProductNutrientDto productNutrientDto)
        {
            _validator.Validate(productNutrientDto, "AddProductNutrient");

            await _productNutrientService.CreateAsync(productNutrientDto);
            _eventBus.Publish(new LogEvent(Statuses.Success, Actions.Creation, EntityTypes.ProductNutrient, productNutrientDto));
        }

        /// <summary> Updates Product-Nutrient in DB. </summary>
        /// <param name="productNutrientDto"></param>
        /// <returns> Returns operation status code. </returns>
        /// <remarks> Required fields: "id", "productId", "nutrientId", "treatingTypeId" (must be positive number); "quality" (must be equals or more than zero). </remarks>
        /// <response code="200"> Sucsess. </response>
        /// <response code="404"> That object not found.  </response>
        /// <response code="500"> Something wrong on the Server. </response>
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(ProductNutrientDto), typeof(ProductNutrientUpdateExample))]
        public async Task UpdateAsync([FromBody] ProductNutrientDto productNutrientDto)
        {
            _validator.Validate(productNutrientDto, "UpdateProductNutrient");

            await _productNutrientService.UpdateAsync(productNutrientDto);
            _eventBus.Publish(new LogEvent(Statuses.Success, Actions.Update, EntityTypes.ProductNutrient, productNutrientDto));
        }

        /// <summary> Deletes Product-Nutrient from DB. </summary>
        /// <param name="productNutrientId" example="666"> The Product-Nutrient Id. </param>
        /// <returns> Returns operation status code. </returns>
        /// <remarks> Field "id" must be only positive number. </remarks>
        /// <exception cref="ValidationException"></exception>
        /// <response code="200"> Sucsess. </response>
        /// <response code="404"> Object with this Id not found. </response>
        /// <response code="500"> Something wrong on the Server. </response> 
        [HttpDelete("{productNutrientId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task DeleteAsync(int? productNutrientId)
        {
            if (productNutrientId == null)
                throw new ValidationException(_sharedLocalizer["ObjectIdCantBeNull"]);

            await _productNutrientService.DeleteAsync(productNutrientId);
            _eventBus.Publish(new LogEvent(Statuses.Success, Actions.Deletion, EntityTypes.ProductNutrient, $"with ID: {productNutrientId}"));
        }
    }
}

