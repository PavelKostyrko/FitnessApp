﻿using FitnessApp.Data;
using FitnessApp.Logic.Models;
using FitnessApp.Logic.Services;
using FitnessApp.Logic.Validators;
using FluentValidation;

namespace FitnessApp.Web.Configurations
{
    public static class ServicesConfiguration
    {
        public static void Configure(WebApplicationBuilder  builder)
        {
            builder.Services.AddTransient<ProductContext>();

            builder.Services.AddTransient<ICustomValidator<ProductCategoryDto>, CustomValidator<ProductCategoryDto>>();
            builder.Services.AddTransient<ICustomValidator<ProductSubCategoryDto>, CustomValidator<ProductSubCategoryDto>>();
            builder.Services.AddTransient<ICustomValidator<ProductDto>, CustomValidator<ProductDto>>();
            builder.Services.AddTransient<ICustomValidator<NutrientCategoryDto>, CustomValidator<NutrientCategoryDto>>();
            builder.Services.AddTransient<ICustomValidator<NutrientDto>, CustomValidator<NutrientDto>>();
            builder.Services.AddTransient<ICustomValidator<TreatingTypeDto>, CustomValidator<TreatingTypeDto>>();
            builder.Services.AddTransient<ICustomValidator<ProductNutrientDto>, CustomValidator<ProductNutrientDto>>();

            builder.Services.AddTransient<IProductCategoryService, ProductCategoryService>();
            builder.Services.AddTransient<IProductSubCategoryService, ProductSubCategoryService>();
            builder.Services.AddTransient<IProductService, ProductService>();
            builder.Services.AddTransient<INutrientCategoryService, NutrientCategoryService>();
            builder.Services.AddTransient<INutrientService, NutrientService>();
            builder.Services.AddTransient<ITreatingTypeService, TreatingTypeService>();
            builder.Services.AddTransient<IProductNutrientService, ProductNutrientService>();

            builder.Services.AddScoped<IValidator<ProductCategoryDto>, ProductCategoryValidator>();
            builder.Services.AddScoped<IValidator<ProductSubCategoryDto>, ProductSubCategoryValidator>();
            builder.Services.AddScoped<IValidator<ProductDto>, ProductValidator>();
            builder.Services.AddScoped<IValidator<NutrientCategoryDto>, NutrientCategoryValidator>();
            builder.Services.AddScoped<IValidator<NutrientDto>, NutrientValidator>();
            builder.Services.AddScoped<IValidator<TreatingTypeDto>, TreatingTypeValidator>();
            builder.Services.AddScoped<IValidator<ProductNutrientDto>, ProductNutrientValidator>();
        }
    }
}
