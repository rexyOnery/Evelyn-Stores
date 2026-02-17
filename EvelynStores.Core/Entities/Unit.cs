using System;

namespace EvelynStores.Core.Entities;

public class Unit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int NoOfProducts { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
