using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Export;
using System.Windows;
using System.Windows.Controls;
using Nieko.Infrastructure;
using System.Windows.Media;
using System.Windows.Xps.Packaging;
using System.Windows.Documents;

namespace Nieko.Modules.Architecture.Export
{
    public class PrintManager : IPrintManager
    {
        private object _PrintArea;
        private Func<object> _TargetGetter;

        public object PrintArea 
        {
            get
            {
                return _PrintArea;
            }
            set
            {
                _PrintArea = value;

                if (typeof(ItemsControl).IsAssignableFrom(value.GetType()))
                {
                    _TargetGetter = () => (_PrintArea as ItemsControl).Items[0];
                }
                else
                {
                    _TargetGetter = () => (_PrintArea as ContentControl).Content;
                }
            }
        }

        public void Print()
        {
            object target = _TargetGetter();

            if (target is UserControl)
            {
                target = (target as UserControl).Content;
            }

            if (Attribute.IsDefined(target.GetType(), typeof(PrintChildAttribute)))
            {
                target = (target as ContentControl).Content;
            }

            var printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(target as Visual, "print");  
            }
        }

        public void Print(XpsDocument document)
        {
            // Create the print dialog object and set options
            PrintDialog pDialog = new PrintDialog();
            pDialog.PageRangeSelection = PageRangeSelection.AllPages;
            pDialog.UserPageRangeEnabled = true;

            // Display the dialog. This returns true if the user presses the Print button.
            Nullable<Boolean> print = pDialog.ShowDialog();
            if (print == true)
            {
                FixedDocumentSequence fixedDocSeq = document.GetFixedDocumentSequence();
                pDialog.PrintDocument(fixedDocSeq.DocumentPaginator, "Nieko Model Print job");
            }
        }
    }
}
