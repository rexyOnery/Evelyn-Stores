using System;
using System.Collections.Generic;
using System.Text;

namespace EvelynStores.Core.DTOs
{
    public class CategoryStatisticsBySaleItemsDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public int TotalCategorySaleItems { get; set; }
    }
}
