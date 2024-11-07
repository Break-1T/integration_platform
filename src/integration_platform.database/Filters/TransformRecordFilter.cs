using integration_platform.database.Enums;
using integration_platform.database.Models;
using System.Collections.Generic;
using System.Linq;

namespace integration_platform.database.Filters;

public class TransformRecordFilter : BaseFilter<TransformRecordFilter>
{
    public TransformRecordFilter() { }

    public List<long> TransformRecordIds { get; set; }

    public List<TransformRecordStatus> Statuses { get; set; }

    public static TransformRecordFilter CreateFilter() => new ();

    public TransformRecordFilter WithId(params long[] transferRecordIds)
    {
        this.TransformRecordIds = [..transferRecordIds];
        return this;
    }

    public TransformRecordFilter WithStatus(params TransformRecordStatus[] statuses)
    {
        this.Statuses = [.. statuses];
        return this;
    }

    public override IQueryable<TRecord> ApplyFilters<TRecord>(IQueryable<TRecord> query)
    {
        var result = base.ApplyFilters(query.Cast<TransformRecord>());

        if (TransformRecordIds != null)
        {
            result = result.Where(tr => TransformRecordIds.Any(filterId => filterId == tr.TransformRecordId));
        }

        if (Statuses != null)
        {
            result = result.Where(rt => Statuses.Contains(rt.Status));
        }

        if (SourceList != null)
        {
            result = result.Where(rt => SourceList.Contains(rt.Source));
        }

        if (TargetList != null)
        {
            result = result.Where(rt => TargetList.Contains(rt.Target));
        }

        if (TypeList != null)
        {
            result = result.Where(rt => TypeList.Contains(rt.RecordType));
        }

        return result.Cast<TRecord>();
    }
}
