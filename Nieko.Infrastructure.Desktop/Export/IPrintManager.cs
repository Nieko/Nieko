using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Xps.Packaging;

namespace Nieko.Infrastructure.Export
{
    /// <summary>
    /// Generic simple print functionality of either the current main application region
    /// or a provided XPS Document
    /// </summary>
    public interface IPrintManager
    {
        /// <summary>
        /// Screen region that contains printable area (i.e. without menus or toolbars)
        /// </summary>
        object PrintArea { get; set; }
        /// <summary>
        /// Print current contents of <see cref="PrintArea"/>
        /// </summary>
        void Print();
        /// <summary>
        /// Send XPS Document to printer
        /// </summary>
        /// <param name="document"></param>
        void Print(XpsDocument document);
    }
}
