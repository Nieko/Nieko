using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel
{
    /// <summary>
    /// Fluent interfaces for creation of an EndPoint
    /// </summary>
    public interface IEndPointFactory : IEndPointFactoryDescription
    {
        
    }

    /// <summary>
    /// Fluent interfaces for creation of an EndPoint
    /// </summary>
    public interface IEndPointFactoryDescription : IEndPointFactoryCreateMenuEntry
    {
        IEndPointFactoryCreateMenuEntry Description(string description);
    }

    /// <summary>
    /// Fluent interfaces for creation of an EndPoint
    /// </summary>
    public interface IEndPointFactoryCreateMenuEntry : IEndPointFactoryOrdinal
    {
        IEndPointFactoryOrdinal CreateMenuEntry(bool createMenuEntry);
    }

    /// <summary>
    /// Fluent interfaces for creation of an EndPoint
    /// </summary>
    public interface IEndPointFactoryOrdinal : IEndPointFactoryFinish
    {
        IEndPointFactoryFinish Ordinal(short ordinal);
    }

    /// <summary>
    /// Fluent interfaces for creation of an EndPoint
    /// </summary>
    public interface IEndPointFactoryFinish
    {
        EndPoint Build();
    }
}
