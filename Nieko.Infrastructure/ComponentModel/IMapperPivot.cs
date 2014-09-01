using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel
{
    public interface IMapperPivot<TFrom, TTo>
    {
        void From(TFrom from, TTo to);
        void To(TFrom from, TTo to);
    }
}
