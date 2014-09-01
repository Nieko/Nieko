using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.ViewModel
{
    public class NiekoFormException : Exception
    {
        public NiekoFormException(string message) : base(message) { }
    }
}
