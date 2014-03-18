using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure
{
    /// <summary>
    /// Names of regions expected to be made available in the main application Shell Window
    /// </summary>
    public class CoreRegionNames
    {
        /// <summary>
        /// Bottom region; (i.e. for Record Navigation Bar)
        /// </summary>
        public static string BottomRegion { get { return "BottomRegion"; } }
        /// <summary>
        /// Region in which to load Views
        /// </summary>
        public static string MainRegion { get { return "MainRegion"; } }
        /// <summary>
        /// Left region (i.e. for Navigation Bar)
        /// </summary>
        public static string LeftRegion { get { return "LeftRegion"; } }
        /// <summary>
        /// Top region
        /// </summary>
        public static string TopRegion { get { return "LeftRegion"; } }
        /// <summary>
        /// Right region
        /// </summary>
        public static string RightRegion { get { return "LeftRegion"; } }
        /// <summary>
        /// Region to use for modal popup dialogs
        /// </summary>
        public static string PopupRegion { get { return "PopupRegion"; } }
    }
}
