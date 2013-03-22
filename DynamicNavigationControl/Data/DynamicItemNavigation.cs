using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace DynamicNavigationControl.Data
{
    public class DynamicItemNavigation
    {
        public int ID { set; get; }
        public string Brand {set; get;}
        public string PageName {set; get;}
        public string PageLink {set; get;}
        public int Order {set; get;}
        public string Depth {set; get;}
        public SPFieldUserValueCollection SharePointGroup { set; get; }
        public bool Show { set; get; }
    }
}
