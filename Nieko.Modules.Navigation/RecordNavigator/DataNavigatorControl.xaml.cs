namespace Nieko.Modules.Navigation.RecordNavigator
{
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
    using Nieko.Infrastructure.Navigation.RecordNavigation;

    /// <summary>
    /// Interaction logic for DataNavigatorControl.xaml
    /// </summary>
    public partial class DataNavigatorControl : UserControl, IDataNavigatorView
    {
        public DataNavigatorControl()
        {
            InitializeComponent();
        }

        public IDataNavigatorViewModel ViewModel 
        {
            get
            {
                return DataContext as IDataNavigatorViewModel;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}