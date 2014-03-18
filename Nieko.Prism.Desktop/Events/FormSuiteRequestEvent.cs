using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.EventAggregation;
using Nieko.Infrastructure.Windows;
using Microsoft.Practices.Prism.Events;

namespace Nieko.Prism.Events
{
    public class FormSuiteRequestEvent : CompositePresentationEvent<FormSuiteRequestEvent>, IFormSuiteRequestEvent
    {
        private object _Result;

        public Type TypeRequested { get; private set; }

        public object Result
        {
            get
            {
                return _Result;
            }
            set
            {
                if (_Result != null)
                {
                    throw new InvalidOperationException("Result has already been set"); 
                }

                _Result = value;
            }
        }

        public void Initialize<T>() 
            where T : IFormsSuite
        {
            if (TypeRequested != null)
            {
                throw new InvalidOperationException("Event has already been initialized");
            }

            TypeRequested = typeof(T);
        }
    }
}
