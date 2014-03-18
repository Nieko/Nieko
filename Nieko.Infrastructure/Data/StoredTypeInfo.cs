using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Metadata for a type stored in a IDataStore
    /// </summary>
    public class StoredTypeInfo : IEquatable<StoredTypeInfo>  
    {
        public Type ItemType { get; private set;}
        public Func<IQueryable> ItemRetrieval { get; private set; }

        public StoredTypeInfo(Type itemType, Func<IQueryable> itemRetrieval)
        {
            ItemType = itemType;
            ItemRetrieval = itemRetrieval;
        }

        public bool Equals(StoredTypeInfo other)
        {
            if (ItemType == null)
            {
                return other.ItemType == null ? true : other.ItemType.Equals(ItemType);
            }

            return ItemType.Equals(other.ItemType == null ? null : other.ItemType);
        }

        public override int GetHashCode()
        {
            return ItemType == null ? 0 : ItemType.GetHashCode(); 
        }
    }
}
