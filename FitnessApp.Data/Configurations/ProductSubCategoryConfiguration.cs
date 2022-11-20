﻿using FitnessApp.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Data.Configurations
{
    internal class ProductSubCategoryConfiguration : IEntityTypeConfiguration<ProductSubCategoryDb>
    {
        public void Configure(EntityTypeBuilder<ProductSubCategoryDb> builder)
        {
            builder.ToTable("ProductsSubCategories").HasKey(t => t.Id); // configure table name and set primary key
            builder.Property(_ => _.Id).ValueGeneratedOnAdd(); // auto creating id when entity is added
            builder.Property(_ => _.Title).IsRequired().HasMaxLength(30); // field to require for fill. Set numbers of max symbols
            builder.HasOne(_ => _.ProductCategory).WithMany(_ => _.ProductSubCategories).HasForeignKey(_ => _.ProductCategoryId);
        }
    }
}
