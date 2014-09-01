using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Nieko.Infrastructure.Navigation.RecordNavigation
{
    public interface IRecordFilter
    {
        string Description { get; set; }
        BinaryExpression BuildPathExpression(ParameterExpression itemParameter);
    }
}
