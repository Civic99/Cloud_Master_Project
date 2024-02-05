using Common.DTO;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Fabric.Management.ServiceModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Mappers
{
    public static class ProductMapper
    {
        public static ProductDto ToDto(Product product)
        {
            var dto = new ProductDto();
            dto.Description = product.Description;
            dto.Category = product.Category;
            dto.Quantity = product.Quantity;
            dto.Price = product.Price;
            dto.Name = product.Name;
            dto.Id = product.Id;

            return dto;
        }

        public static Product FromDto(ProductDto dto)
        {
            var product = new Product();
            product.Description = dto.Description;
            product.Category = dto.Category;
            product.Quantity = dto.Quantity;
            product.Price = dto.Price;
            product.Name = dto.Name;
            product.Id = dto.Id;

            return product;
        }
    }
}
