using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Navigation;
using System.Windows.Input;

namespace Nieko.Modules.Architecture.Navigation
{
    public class ModalMessageButtonItem
    {
        public string Caption { get; set; }

        public ModalMessageButton Option { get; set; }

        public ICommand Clicked { get; set; }
    }
}
