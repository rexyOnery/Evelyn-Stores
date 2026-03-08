using System;
using System.Collections.Generic;
using System.Text;

namespace EvelynStores.Core.DTOs
{
    public class CategoryStatisticsDto
    {
       public List<CategoryStatisticsBySaleItemsDto> CategorySales { get; set; } = new();
        public int TotalCategories { get; set; }
        public int TotalCategoryProductsCount { get; set; }

    }

    
}
