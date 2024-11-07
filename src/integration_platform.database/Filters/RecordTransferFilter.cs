using integration_platform.database.Enums;
using integration_platform.database.Models;
using System.Collections.Generic;
using System.Linq;

namespace integration_platform.database.Filters;

public class RecordTransferFilter : BaseFilter<RecordTransferFilter>
{
    public RecordTransferFilter() { }

    public List<long> RecordTransferIds { get; set; }

    public List<RecordTransferStatus> Statuses { get; set; }

    public static RecordTransferFilter CreateFilter() => new();

    public RecordTransferFilter WithId(params long[] recortTransferIds)
    {
        this.RecordTransferIds = [.. recortTransferIds];
        return this;
    }

    public RecordTransferFilter WithStatus(params RecordTransferStatus[] statuses)
    {
        this.Statuses = [.. statuses];
        return this;
    }

    public override IQueryable<TRecord> ApplyFilters<TRecord>(IQueryable<TRecord> query)
    {
        var result = base.ApplyFilters(query.Cast<RecordTransfer>());

        if (RecordTransferIds != null)
        {
            result = result.Where(rt => RecordTransferIds.Any(filterId => filterId == rt.RecordTransferId));
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
