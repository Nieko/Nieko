using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Logging;

namespace Nieko.Infrastructure.Logging
{
    public interface IPrismLogger : ILoggerFacade, ILogger
    {
    }
}
