using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Composition
{
    /// <summary>
    /// Names of common Model Modules. These Modules contain most of the implementation
    /// of their associated Infrastructure framework.
    /// </summary>
    public class NiekoModuleNames
    {
        public string Architecture { get { return BindingHelper.Name(() => Architecture); } }

        public string Navigation { get { return BindingHelper.Name(() => Navigation); } }

        public string Versioning { get { return BindingHelper.Name(() => Versioning); } }

        public string Security { get { return BindingHelper.Name(() => Security); } }

        public string DataTransfer { get { return BindingHelper.Name(() => DataTransfer); } } 
    }
}
