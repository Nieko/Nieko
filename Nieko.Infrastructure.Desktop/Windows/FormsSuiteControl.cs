using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Nieko.Infrastructure.ComponentModel;
using System.ComponentModel;

namespace Nieko.Infrastructure.Windows
{
    /// <summary>
    /// UI Control for displaying a Forms Suite (see also <seealso cref="IFormsSuite"/>)
    /// </summary>
    public class FormsSuiteControl : ContentControl, INotifyPropertyChanged
    {
        private NotifyingFields _Fields;

        public IFormsSuite FormsSuite
        {
            get { return (IFormsSuite)GetValue(FormsSuiteProperty); }
            set { SetValue(FormsSuiteProperty, value); }
        }

        public IList<string> SubFormNames
        {
            get
            {
                return _Fields.Get(() => SubFormNames);
            }
            private set
            {
                _Fields.Set(() => SubFormNames, value);
            }
        }

        // Using a DependencyProperty as the backing store for FormsSuite.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FormsSuiteProperty =
            DependencyProperty.Register("FormsSuite", typeof(IFormsSuite), typeof(FormsSuiteControl), new UIPropertyMetadata(null, FormsSuiteChanged));

        static FormsSuiteControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FormsSuiteControl), new FrameworkPropertyMetadata(typeof(FormsSuiteControl)));
        }

        public FormsSuiteControl()
        {
            _Fields = new NotifyingFields(this, RaisePropertyChanged);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private static IFormsSuite _FormsSuite;

        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private static void FormsSuiteChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var formsSuiteControl = (FormsSuiteControl)sender;
            if (formsSuiteControl == null)
            {
                return;
            }
            _FormsSuite = (IFormsSuite)args.NewValue;

            if (_FormsSuite == null)
            {
                formsSuiteControl.SubFormNames = new List<string>();
                return;
            }

            formsSuiteControl.SubFormNames = _FormsSuite.Forms
                .OrderBy(sf => sf.Ordinal)
                .Select(sf => sf.Caption)
                .ToList();
        }
    }
}
