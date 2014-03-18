using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Modules.Architecture.Navigation
{
    public class ModalMessageViewModel : IModalMessageViewModel
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public IList<ModalMessageButtonItem> Buttons { get; private set; }

        public ModalMessageViewModel()
        {
            Buttons = new List<ModalMessageButtonItem>(); 
        }
    }
}
