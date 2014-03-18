using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Export
{
    public class CompositeExport
    {
        private List<CompositeExportItem> _Items = new List<CompositeExportItem>();

        public IEnumerable<CompositeExportItem> Items { get { return _Items; } }

        public void AddItem(string name, object data)
        {
            _Items.Add(new CompositeExportItem()
            {
                Name = name,
                Data = data ?? string.Empty
            });
        }
    }
}
