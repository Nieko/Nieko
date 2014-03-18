#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Navigation;

namespace Nieko.Modules.Architecture.Navigation
{
    public class ModalMessageViewModelSample : IModalMessageViewModel
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public IList<ModalMessageButtonItem> Buttons { get; private set; }

        public ModalMessageViewModelSample()
        {
            Title = "Something has happened!";
            Message = "There has been a happening." + Environment.NewLine + "It appears to be a something. Continue?";
            Buttons = new[] { ModalMessageButton.Yes, ModalMessageButton.No }
                .Select(mmb => new ModalMessageButtonItem()
                {
                    Caption = mmb.ToString(),
                    Option = mmb
                })
                .ToList();
        }
    }
}
#endif