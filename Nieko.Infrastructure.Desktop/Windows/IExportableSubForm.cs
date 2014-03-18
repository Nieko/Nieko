using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Windows
{
    public interface IExportableSubForm
    {
        object GetExportData();
    }
}
