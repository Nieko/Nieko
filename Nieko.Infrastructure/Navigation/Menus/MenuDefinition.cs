using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.ComponentModel;

namespace Nieko.Infrastructure.Navigation.Menus
{
    /// <summary>
    /// Contains details of an End Point necessary for creation
    /// of its IMenu entry
    /// </summary>
    public class MenuDefinition
    {
        public EndPoint EndPoint { get; private set; }

        public string Caption { get; private set; }

        public string ParentMenuPath { get; private set; }

        public short Position { get; private set; }

        public MenuDefinition(EndPoint endPoint)
        {
            EndPoint = endPoint;
            ParentMenuPath = string.Empty;

            if (endPoint == EndPoint.Root)
            {
                Caption = string.Empty;
                return;
            }

            Caption = endPoint.Description;

            var parentEndPoint = endPoint.Parent;

            while (true)
            {
                if (parentEndPoint == EndPoint.Root)
                {
                    ParentMenuPath = "/" + ParentMenuPath;
                    break;
                }

                ParentMenuPath = parentEndPoint.Description + ((ParentMenuPath == string.Empty) ? string.Empty : ("/" + ParentMenuPath));
                parentEndPoint = parentEndPoint.Parent;
            }
            Position = endPoint.Ordinal;
        }
    }
}
