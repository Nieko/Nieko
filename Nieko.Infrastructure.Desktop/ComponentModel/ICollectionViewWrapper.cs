using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel
{
    public interface ICollectionViewWrapper : IEnumerable, IEditableCollectionViewAddNewItem, IEditableCollectionView, ICollectionView, IItemProperties
    {
        int Count { get; }
    }
}
