using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Export
{
    public interface IDataExporter
    {
        bool IsEnabled { get; }
        void Export(object data);
    }
}
