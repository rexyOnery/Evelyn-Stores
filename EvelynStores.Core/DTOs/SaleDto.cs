using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EvelynStores.Core.DTOs
{
    public class SaleDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TransactionId { get; set; } = string.Empty;
        public List<SaleItemDto> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal ChangeAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentReceiver { get; set; } = string.Empty;
        public string PaymentNote { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class SaleItemDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SaleId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class CreateSaleDto
    {
        [Required]
        public List<CreateSaleItemDto> Items { get; set; } = new();

        [Range(0, double.MaxValue)]
        public decimal Subtotal { get; set; }

        [Range(0, 1)]
        public decimal TaxRate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TaxAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ShippingAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DiscountAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal GrandTotal { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ReceivedAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ChangeAmount { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = string.Empty;

        public string PaymentReceiver { get; set; } = string.Empty;
        public string PaymentNote { get; set; } = string.Empty;
    }

    public class CreateSaleItemDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        public string ProductImageUrl { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal LineTotal { get; set; }
    }
}
