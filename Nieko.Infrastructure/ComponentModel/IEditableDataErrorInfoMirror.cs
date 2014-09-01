using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel
{
    public interface IEditableDataErrorInfoMirror : IEditableMirrorObject, IDataErrorInfo
    {
        void RecheckForErrors();
    }
}
