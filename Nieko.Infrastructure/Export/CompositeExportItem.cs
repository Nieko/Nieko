using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Export
{
    public class CompositeExportItem
    {
        public string Name { get; set; }
        [UseInstanceType]
        public object Data { get; set; }
    }
}
