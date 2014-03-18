using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Nieko.Infrastructure.ComponentModel
{
    internal class EndPointFactory<T> : IEndPointFactory
        where T : IEndPointProvider
    {
        private static Dictionary<string, EndPoint> _ExistingEndPoints = new Dictionary<string, EndPoint>();
        private Func<EndPoint> _EndPointBuild = null;

        internal EndPointFactory() { }

        internal IEndPointFactory Create<TOwner>(EndPoint parent, Expression<Func<EndPoint>> property, EndPointType type)
        {
            EndPoint existing;
            string propertyName = BindingHelper.Name(property);

            if (_ExistingEndPoints.TryGetValue(propertyName, out existing))
            {
                return new ExistingEndPointFactory(existing); 
            }

            _EndPointBuild = () =>
                {
                    var endPoint = new EndPoint()
                    {
                        Parent = parent,
                        Name = propertyName,
                        Type = type,
                        CreateMenuEntry = type != EndPointType.ReportPage,
                        CreatedBy = typeof(TOwner)
                    };

                    endPoint.Description = endPoint.Name;

                    return endPoint;
                };

            return this;
        }

        public IEndPointFactoryCreateMenuEntry Description(string description)
        {
            var currentBuild = _EndPointBuild;

            _EndPointBuild = () =>
                {
                    var endPoint = currentBuild();
                    endPoint.Description = description;

                    return endPoint;
                };

            return this;
        }

        public IEndPointFactoryOrdinal CreateMenuEntry(bool createMenuEntry)
        {
            var currentBuild = _EndPointBuild;

            _EndPointBuild = () =>
            {
                var endPoint = currentBuild();
                endPoint.CreateMenuEntry = createMenuEntry;

                return endPoint;
            };

            return this;
        }

        public IEndPointFactoryFinish Ordinal(short ordinal)
        {
            var currentBuild = _EndPointBuild;

            _EndPointBuild = () =>
            {
                var endPoint = currentBuild();
                endPoint.Ordinal = ordinal;

                return endPoint;
            };

            return this;
        }

        public EndPoint Build()
        {
            var endPoint = _EndPointBuild();

            if (!EndPoint.CanAddCheck(endPoint))
            {
                throw new InvalidOperationException("Creation of " + typeof(EndPoint).Name + " denied");
            }

            _ExistingEndPoints.Add(endPoint.Name, endPoint);

            return endPoint;
        }
    }
}
