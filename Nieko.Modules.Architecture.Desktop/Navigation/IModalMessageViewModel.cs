using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Modules.Architecture.Navigation
{
    public interface IModalMessageViewModel
    {
        string Title { get; set; }
        string Message { get; set; }
        IList<ModalMessageButtonItem> Buttons { get; }
    }
}
