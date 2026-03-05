using EvelynStores.Core.DTOs;
using EvelynStores.Core.Entities;
using EvelynStores.Core.Services;
using EvelynStores.Infrastructure.Data;
using EvelynStores.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;

namespace EvelynStores.Infrastructure.Services;

public class SaleService : ISaleService
{
    private readonly EvelynStoresDbContext _context;

    public SaleService(EvelynStoresDbContext context)
    {
        _context = context;
    }

    public async Task<SaleDto> CreateSaleAsync(CreateSaleDto dto)
    {
        var transactionId = GenerateTransactionId();

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionId,
            Subtotal = dto.Subtotal,
            TaxRate = dto.TaxRate,
            TaxAmount = dto.TaxAmount,
            ShippingAmount = dto.ShippingAmount,
            DiscountAmount = dto.DiscountAmount,
            GrandTotal = dto.GrandTotal,
            ReceivedAmount = dto.ReceivedAmount,
            ChangeAmount = dto.ChangeAmount,
            PaymentMethod = dto.PaymentMethod,
            PaymentReceiver = dto.PaymentReceiver,
            PaymentNote = dto.PaymentNote,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var item in dto.Items)
        {
            sale.Items.Add(new SaleItem
            {
                Id = Guid.NewGuid(),
                SaleId = sale.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                ProductImageUrl = item.ProductImageUrl,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                LineTotal = item.LineTotal
            });
        }

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        // Reduce product stock
        foreach (var item in dto.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                product.Quantity -= item.Quantity;
                if (product.Quantity < 0) product.Quantity = 0;
            }
        }

        // Reduce product Levels
        foreach (var item in dto.Items)
        {
            var productLevel = await _context.ProductLevels.FindAsync(item.ProductId);
            if (productLevel != null)
            {
                productLevel.InStockQuantity -= item.Quantity;
                if (productLevel.InStockQuantity < 0) productLevel.InStockQuantity = 0;
            }
        }

        await _context.SaveChangesAsync();

        return MapToDto(sale);
    }

    public async Task<SaleDto?> GetSaleByIdAsync(Guid id)
    {
        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id);

        return sale == null ? null : MapToDto(sale);
    }

    public async Task<SaleDto?> GetSaleByTransactionIdAsync(string transactionId)
    {
        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.TransactionId == transactionId);

        return sale == null ? null : MapToDto(sale);
    }

    public async Task<IEnumerable<SaleDto>> GetAllSalesAsync()
    {
        var sales = await _context.Sales
            .Include(s => s.Items)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return sales.Select(MapToDto);
    }

    public async Task<IEnumerable<SaleDto>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var sales = await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return sales.Select(MapToDto);
    }

    private static string GenerateTransactionId()
    {
        return $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }

    private static SaleDto MapToDto(Sale sale)
    {
        return new SaleDto
        {
            Id = sale.Id,
            TransactionId = sale.TransactionId,
            Subtotal = sale.Subtotal,
            TaxRate = sale.TaxRate,
            TaxAmount = sale.TaxAmount,
            ShippingAmount = sale.ShippingAmount,
            DiscountAmount = sale.DiscountAmount,
            GrandTotal = sale.GrandTotal,
            ReceivedAmount = sale.ReceivedAmount,
            ChangeAmount = sale.ChangeAmount,
            PaymentMethod = sale.PaymentMethod,
            PaymentReceiver = sale.PaymentReceiver,
            PaymentNote = sale.PaymentNote,
            CreatedBy = sale.CreatedBy,
            CreatedAt = sale.CreatedAt,
            Items = sale.Items.Select(i => new SaleItemDto
            {
                Id = i.Id,
                SaleId = i.SaleId,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductImageUrl = i.ProductImageUrl,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                LineTotal = i.LineTotal
            }).ToList()
        };
    }
}
