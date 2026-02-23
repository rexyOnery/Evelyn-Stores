using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EvelynStores.Core.DTOs
{
    public class PurchaseDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        
        [Required]
        [StringLength(100)]
        public string SKU { get; set; } = string.Empty;

        public Guid CategoryId { get; set; }
        public Guid SubCategoryId { get; set; } 
        public Guid ProductId { get; set; }
        //public int ReOrderLevel { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Unit Cost must be a positive number")]
        public double UnitCost { get; set; }  

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number")]
        public decimal TotalAmount { get; set; } 
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Persisted custom fields
        public DateTime? ManufacturedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

}
