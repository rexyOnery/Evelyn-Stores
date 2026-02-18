namespace EvelynStores.Core.DTOs;

using System.Collections.Generic;

public class PagedResult<T>
{
    // Matches typical server shape: { items: [...], total: 0, page: 1, pageSize: 10 }
    public List<T> Items { get; set; } = new List<T>();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
