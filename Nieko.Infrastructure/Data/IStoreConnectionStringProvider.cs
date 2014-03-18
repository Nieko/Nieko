using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Data
{
    public interface IStoreConnectionStringProvider
    {
        string this[Type store] { get; }
    }
}
