using Nieko.Infrastructure.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Threading
{
    public static class VirtualCollectionCopy
    {
        public static VirtualCollectionCopy<T> CreateCopier<T>(IEnumerable<T> source)
        {
            return new VirtualCollectionCopy<T>()
            {
                Source = source.GetEnumerator()
            };
        }

        public static List<T> CreateDestination<T>(IEnumerable<T> example)
        {
            return new List<T>();
        }
    }

    public class VirtualCollectionCopy<T>
    {
        public IViewNavigator ViewNavigator { get; set; }

        public IEnumerator<T> Source { get; set; }

        public long BatchSize { get; set; }

        public Action<ICollection<T>, IList<T>> BatchAction { get; set; }

        public Action PostCopyAction { get; set; }

        public VirtualCollectionCopy()
        {
            BatchSize = 2000;
            BatchAction = (d, b) => { foreach (var item in b) d.Add(item); };
        }

        public void Copy(ICollection<T> destination)
        {
            if(destination == null)
            {
                throw new ArgumentNullException();
            }

            if (ViewNavigator == null)
            {
                throw new InvalidOperationException("ViewNavigator is null");
            }

            List<T> batch = null;
            var i = 1;
            var hasMore = Source.MoveNext();
            Action backgroundProcessing = () => { };

            Action processBatch = () =>
            {
                if (!hasMore)
                {
                    return;
                }

                batch = new List<T>();
                i = 1;

                while (hasMore && i < BatchSize)
                {
                    batch.Add(Source.Current);
                    hasMore = Source.MoveNext();
                    i++;
                }

                BatchAction(destination, batch);
            };

            backgroundProcessing = () =>
            {
                ViewNavigator.EnqueuePostLayoutWork(() =>
                {
                    processBatch();
                    if (hasMore)
                    {
                        backgroundProcessing();
                    }
                    else
                    {
                        if (PostCopyAction != null)
                        {
                            PostCopyAction();
                        }
                    }
                });
            };

            processBatch();
            backgroundProcessing();
        }

    }
}
