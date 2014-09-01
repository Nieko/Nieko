using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using Nieko.Infrastructure.Collections;

namespace Nieko.Infrastructure.ComponentModel
{
    public class UniqueOptions<T>
        where T : IComparable<T>
    {
        private HashSet<T> _allOptions;
        private Func<object, T> _UsedOption;

        public ObservableRangeCollection<T> Options { get; private set; }

        private UniqueOptions() { }

        public static UniqueOptions<T> Create<TLineItem>(ICollectionView usedOptions, Func<TLineItem, T> usedOption, IEnumerable<T> allOptions)
        {
            var newInstance = new UniqueOptions<T>()
            {
                Options = new ObservableRangeCollection<T>(allOptions)
            };

            usedOptions.CollectionChanged += newInstance.UsedOptionsChanged;
            newInstance._UsedOption = o => usedOption((TLineItem)o);
            newInstance._allOptions = new HashSet<T>(allOptions);

            return newInstance;
        }

        private void UsedOptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var inOrder = Options.ToList();

            if (e.OldItems != null)
            {
                foreach (var removed in e.OldItems)
                {
                    inOrder.Add(_UsedOption(removed));
                }
            }

            if (e.NewItems != null)
            {
                foreach (var added in e.NewItems)
                {
                    inOrder.Remove(_UsedOption(added));
                }
            }

            inOrder.Sort();

            Options.ReplaceRange(inOrder);
        }
    }
}
