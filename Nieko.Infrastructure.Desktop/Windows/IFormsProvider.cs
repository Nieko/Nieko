using System;
using System.Collections.Generic;
namespace Nieko.Infrastructure.Windows
{
    public interface IFormsProvider
    {
        IEnumerable<ViewModelForm> GetAllForms();
    }
}
