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

namespace Nieko.Infrastructure.Windows.Background
{
    /// <summary>
    /// Interaction logic for Desktop.xaml
    /// </summary>
    public partial class Desktop : UserControl, IViewBase<IDesktopViewModel>
    {
        public Desktop()
        {
            InitializeComponent();
        }

        public IDesktopViewModel ViewModel 
        {
            get
            {
                return DataContext as IDesktopViewModel;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}