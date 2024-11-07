using integration_platform.database.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace integration_platform.database.Filters;

public class BaseFilter<T> where T: BaseFilter<T>
{
    public BaseFilter() { }

    public int PageNumber { get; set; } = 0;
    public int PageSize { get; set; } = 10;

    public string SortBy { get; set; } = "RecCreated";
    public bool SortDesc { get; set; } = true;

    public List<string> SourceList { get; set; }
    public List<string> TargetList { get; set; }
    public List<string> TypeList { get; set; }

    public T SetPageNumber(int number)
    {
        this.PageNumber = number;
        return this as T;
    }

    public T TopPageSize(int number)
    {
        this.PageSize = number;
        return this as T;
    }

    public T WithSource(params string[] sources)
    {
        this.SourceList = [.. sources];
        return this as T;
    }

    public T WithTarget(params string[] targets)
    {
        this.TargetList = [.. targets];
        return this as T;
    }

    public T WithType(params string[] types)
    {
        this.TypeList = [.. types];
        return this as T;
    }

    public T WithSort(string sortField, bool sortDesc = true)
    {
        this.SortBy = sortField;
        this.SortDesc = sortDesc;

        return this as T;
    }

    public virtual IQueryable<TRecord> ApplyFilters<TRecord>(IQueryable<TRecord> query)
        where TRecord : BaseRecord
    {
        return this.SortDesc
            ? query.OrderByDescending(q => EF.Property<T>(q, this.SortBy))
            : query.OrderBy(q => EF.Property<T>(q, this.SortBy));
    }
}
