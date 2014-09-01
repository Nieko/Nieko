using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Nieko.Infrastructure.Navigation.RecordNavigation
{
    public interface IRecordFilterGroup
    {
        int Ordinal { get; set; }
        IList<IRecordFilter> Filters { get; }
    }
}
