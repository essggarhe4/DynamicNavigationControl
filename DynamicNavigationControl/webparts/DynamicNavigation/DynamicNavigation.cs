using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace DynamicNavigationControl.webparts.DynamicNavigation
{
    [ToolboxItemAttribute(false)]
    public class DynamicNavigation : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/DynamicNavigationControl.webparts/DynamicNavigation/DynamicNavigationUserControl.ascx";

        [WebBrowsable(true), Category("Configuration"), Personalizable(PersonalizationScope.Shared), DefaultValue(""), WebDisplayName("Navigation List"), WebDescription("List name where the webpart will get the information. Example: Dynamic Navigation")]
        public string NavigationList
        {
            get { return navigationlist; }
            set { navigationlist = value; }
        }
        public string navigationlist = string.Empty;

        protected override void CreateChildControls()
        {

            DynamicNavigationUserControl control = (DynamicNavigationUserControl)Page.LoadControl(_ascxPath);

            if (string.IsNullOrEmpty(this.NavigationList)) control.NavigationList = string.Empty;
            else control.NavigationList = this.NavigationList;

            Controls.Add(control);

            base.CreateChildControls(); 
        }

        protected override void OnPreRender(EventArgs e)
        {
            CssRegistration.Register("/_layouts/DynamicNavigationControl/css/DynamicNavigationControl.css");
            base.OnPreRender(e);
        }
    }
}
