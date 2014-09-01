#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Modules.Navigation.RecordNavigator
{
    public class RecordSearchBaseItem
    {
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public int Units { get; set; }
    }
}
#endif