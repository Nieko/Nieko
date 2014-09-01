using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Navigation;

namespace Nieko.Modules.Architecture.Navigation
{
    public class Dialogs : IDialogs
    {
        private readonly Func<IWaitDialog> _WaitDialogFactory;

        public Dialogs(Func<IWaitDialog> waitDialogFactory)
        {
            _WaitDialogFactory = waitDialogFactory;
        }

        public IWaitDialog CreateWaitDialog()
        {
            return _WaitDialogFactory();
        }

        public ModalMessageButton ShowModalMessage(string message)
        {
            return ShowModalMessage(message, ModalMessageButton.Ok);
        }

        public ModalMessageButton ShowModalMessage(string message, ModalMessageButton buttons)
        {
            return ShowModalMessage(string.Empty, message, buttons);
        }

        public ModalMessageButton ShowModalMessage(string title, string message, ModalMessageButton buttons)
        {
            ModalMessageButton result = ModalMessageButton.Ok;
            var viewModel = new ModalMessageViewModel()
            {
                Title = title,
                Message = message
            };

            foreach (ModalMessageButton option in Enum.GetValues(typeof(ModalMessageButton)))
            {
                if (buttons.HasFlag(option))
                {
                    viewModel.Buttons.Add(new ModalMessageButtonItem()
                    {
                        Caption = option.ToString(),
                        Option = option
                    });
                }
            }

            System.Windows.Application.Current.Dispatcher.Invoke(
                new Action(() =>
                {
                    var dialog = new ModalMessage();
                    dialog.ViewModel = viewModel;
                    dialog.ShowDialog();

                    result = dialog.Result;
                }));

            return result;
        }
    }
}
