using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Windows
{
    /// <summary>
    /// Basic implementation of a Suite Form
    /// </summary>
    public class SuiteForm : ISuiteForm
    {
        public virtual int Ordinal { get; internal set; }

        public virtual string Caption { get; internal set; }

        public virtual Action ShowAction { get; internal set; }
    }
}
