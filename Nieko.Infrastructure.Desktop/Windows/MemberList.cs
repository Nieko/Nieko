using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using Nieko.Infrastructure.Windows.Data;

namespace Nieko.Infrastructure.Windows
{
    /// <summary>
    /// UI Control for editing an <seealso cref="IMembershipProvider"/>  list
    /// </summary>
    public class MemberList : Control, INotifyPropertyChanged
    {
        // Using a DependencyProperty as the backing store for Heading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeadingProperty = 
            DependencyProperty.Register("Heading", typeof(string), typeof(MemberList), new UIPropertyMetadata(""));

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty = 
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(MemberList), new UIPropertyMetadata());

        // Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateProperty = 
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(MemberList), new UIPropertyMetadata(null));

        // Using a DependencyProperty as the backing store for MembershipProvider.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MembershipProviderProperty = 
            DependencyProperty.Register("MembershipProvider", typeof(IMembershipProvider), typeof(MemberList), new UIPropertyMetadata(null, MembershipProviderChanged));

        // Using a DependencyProperty as the backing store for OptionDisplayMember.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OptionDisplayMemberProperty = 
            DependencyProperty.Register("OptionDisplayMember", typeof(string), typeof(MemberList), new UIPropertyMetadata(""));

        // Using a DependencyProperty as the backing store for OptionsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OptionsSourceProperty = 
            DependencyProperty.Register("OptionsSource", typeof(object), typeof(MemberList), new UIPropertyMetadata());

        // Using a DependencyProperty as the backing store for SelectedOption.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedOptionProperty = 
            DependencyProperty.Register("SelectedOption", typeof(object), typeof(MemberList), new UIPropertyMetadata());

        static MemberList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MemberList), new FrameworkPropertyMetadata(typeof(MemberList)));
        }

        public string Heading
        {
            get { return (string)GetValue(HeadingProperty); }
            set { SetValue(HeadingProperty, value); }
        }

        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public IMembershipProvider MembershipProvider
        {
            get { return (IMembershipProvider)GetValue(MembershipProviderProperty); }
            set { SetValue(MembershipProviderProperty, value); }
        }

        public string OptionDisplayMember
        {
            get { return (string)GetValue(OptionDisplayMemberProperty); }
            set { SetValue(OptionDisplayMemberProperty, value); }
        }

        public object OptionsSource
        {
            get { return (object)GetValue(OptionsSourceProperty); }
            set { SetValue(OptionsSourceProperty, value); }
        }

        private static void MembershipProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MemberList instance = ((MemberList)d);
            IMembershipProvider provider;

            provider = e.OldValue as IMembershipProvider;
            if (provider != null)
            {
                provider.Reset -= instance.ProviderReset;
            }

            provider = e.NewValue as IMembershipProvider;
            if (provider != null)
            {
                instance.InitialiseMemberships(provider);
                provider.Reset += instance.ProviderReset;
            }
        }

        private void ProviderReset(object sender, EventArgs args)
        {
            InitialiseMemberships(sender as IMembershipProvider);
        }

        private void InitialiseMemberships(IMembershipProvider membershipProvider)
        {
            OptionsSource = membershipProvider.Options;
            ItemsSource = membershipProvider.View;
            
            RaisePropertyChanged(() => OptionsSource);
            RaisePropertyChanged(() => ItemsSource);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void RaisePropertyChanged<TResult>(Expression<Func<TResult>> propertyExpression)
        {
            RaisePropertyChanged(BindingHelper.Name(propertyExpression));
        }

    }
}