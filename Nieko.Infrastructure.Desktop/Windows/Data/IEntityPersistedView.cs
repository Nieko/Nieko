using Nieko.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Windows.Data
{
    public interface IEntityPersistedView : IPersistedView
    {
        Type EntityType { get; }
        Action<Action<IDataStore>> UnitOfWorkStart { get; }
    }
}
