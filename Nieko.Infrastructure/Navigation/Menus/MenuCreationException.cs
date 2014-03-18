using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Navigation.Menus
{
    /// <summary>
    /// Raised when the menu tree hierarchy cannot be resolved from definitions
    /// </summary>
    public class MenuCreationException : Exception
    {
        public MenuCreationException(string message)
            : base(message)
        {
        }

    }
}