using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Nieko.Infrastructure.Navigation;
using Microsoft.Practices.Prism.Commands;

namespace Nieko.Modules.Architecture.Navigation
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class ModalMessage : Window
    {
        public ModalMessageButton Result { get; private set; }

        public ModalMessage()
        {
            InitializeComponent();
        }

        public IModalMessageViewModel ViewModel
        {
            get
            {
                return DataContext as IModalMessageViewModel; 
            }
            set
            {
                DataContext = value;

                var viewModel = DataContext as IModalMessageViewModel;

                if (viewModel != null)
                {
                    foreach (var button in viewModel.Buttons)
                    {
                        var option = button.Option;
                        button.Clicked = new DelegateCommand(() => ButtonClicked(option));
                    }
                }
            }
        }

        private void ButtonClicked(ModalMessageButton option)
        {
            Result = option;
            this.Close();
        }
    }
}
