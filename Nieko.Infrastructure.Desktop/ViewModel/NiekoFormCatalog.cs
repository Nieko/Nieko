using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Composition;
using Nieko.Infrastructure.Reflection;
using Nieko.Infrastructure.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Nieko.Infrastructure.ViewModel
{
    public class NiekoFormCatalog : EndPointProvider<NiekoFormCatalog>, IFormsProvider
    {
        private class InterceptedEndPointFactory : IEndPointFactory
        {
            private object _CurrentStage;
            private Action<EndPoint> _AddEndPointAction;

            public InterceptedEndPointFactory(IEndPointFactory implementation, Action<EndPoint> addEndPointAction)
            {
                _CurrentStage = implementation;
                _AddEndPointAction = addEndPointAction;
            }

            public IEndPointFactoryCreateMenuEntry Description(string description)
            {
                _CurrentStage = (_CurrentStage as IEndPointFactory).Description(description);

                return this;
            }

            public IEndPointFactoryOrdinal CreateMenuEntry(bool createMenuEntry)
            {
                _CurrentStage = (_CurrentStage as IEndPointFactoryCreateMenuEntry).CreateMenuEntry(createMenuEntry);

                return this;
            }

            public IEndPointFactoryFinish Ordinal(short ordinal)
            {
                _CurrentStage = (_CurrentStage as IEndPointFactoryOrdinal).Ordinal(ordinal);

                return this;
            }

            public EndPoint Build()
            {
                var result = (_CurrentStage as IEndPointFactoryFinish).Build();

                _AddEndPointAction(result);

                return result;
            }
        }

        private static object _Lock = new object();
        private static bool _RegistrationsProcessed = false;
        private static List<EndPoint> _EndPoints = new List<EndPoint>();
        private static List<ViewModelForm> _Forms = new List<ViewModelForm>();
        private static List<Func<IViewModelFormFactory, ViewModelForm>> _EndPointRegistrations = new List<Func<IViewModelFormFactory, ViewModelForm>>();

        private IViewModelFormFactory _FormFactory;

        internal static void RegisterForm(Func<IViewModelFormFactory, ViewModelForm> endPointBuilder)
        {
            DoLocked(() => _EndPointRegistrations.Add(endPointBuilder));
        }

        internal static IEndPointFactory GetEndPoint(EndPoint parent, Expression<Func<EndPoint>> property)
        {
            IEndPointFactory factory = null;

            DoLocked(() => 
            {
                factory = new InterceptedEndPointFactory(Get(parent, property, EndPointType.Form), ep => _EndPoints.Add(ep));
            });

            return factory;
        }

        public NiekoFormCatalog(IViewModelFormFactory formFactory)
        {
            _FormFactory = formFactory;
        }

        IEnumerable<ViewModelForm> IFormsProvider.GetAllForms()
        {
            DoLocked(() =>
                {
                    if(!_RegistrationsProcessed)
                    {
                        var formEndPoints = AssemblyHelper.FindTypes(t => t.IsGenericType
                            && t.GetGenericTypeDefinition() == typeof(NiekoForm<>)
                            && !t.IsGenericTypeDefinition
                            && !t.IsAbstract && !t.IsInterface)
                                .Select(t => new
                                {
                                    FormType = t,
                                    EndPoints = t.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                                    .Where(p => p.PropertyType == typeof(EndPoint))
                                });

                        if (formEndPoints
                            .Any(fep => fep.EndPoints.Count() != 1))
                        {
                            throw new NiekoFormException("One or more " + typeof(NiekoForm<>).BasicName() + " do not implement exactly one static " + typeof(EndPoint).Name + " property. " + Environment.NewLine +
                                formEndPoints
                                    .Where(fep => fep.EndPoints.Count() != 1)
                                    .Aggregate(string.Empty, (seed, fep) =>
                                        {
                                            return (seed == string.Empty ?
                                                string.Empty :
                                                Environment.NewLine) + fep.FormType.FullName + " has " + fep.EndPoints.Count() + " EndPoints defined";
                                        }));
                        }

                        _Forms.AddRange(_EndPointRegistrations
                            .Select(epr => epr(_FormFactory)));

                        _RegistrationsProcessed = true;
                    }
                });

            return _Forms;
        }

        private static void DoLocked(Action action)
        {
            lock(_Lock)
            {
                action();
            }
        }
    }
}
