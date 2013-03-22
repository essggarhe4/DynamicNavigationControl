using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint;
using DynamicNavigationControl.Data;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace DynamicNavigationControl.webparts.DynamicNavigation
{
    public partial class DynamicNavigationUserControl : UserControl
    {
        public String NavigationList { get; set; }

        List<DynamicItemNavigation> tmpInvertNavigation;
        int tmpInvertNavigationCounter= 0;

        List<DynamicItemNavigation> DynamicResult;

        string currentPage = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.NavigationList))
            {
                getInformationData();
            }
            else
            {                
                MenuMessages.Text = "<label>You need to configure the webpart</label>";
            }
        }        

        private void getInformationData()
        {
            currentPage = this.Page.Request.Url.AbsoluteUri;
            NavigationData ND = new NavigationData(SPContext.Current.Site.ID, SPContext.Current.Web.ID, this.NavigationList);
            DynamicResult = ND.GetDynamicNavigationInformation();
            if (DynamicResult.Count > 0)
            {

                System.Collections.Generic.List<DynamicItemNavigation> RootMenu = DynamicResult.FindAll(d => d.Depth == constants.variables.NavigationTop);

                if (isValidRoot(RootMenu))
                {
                    foreach (DynamicItemNavigation Ditem in RootMenu)
                    {
                        List<DynamicItemNavigation> submenu = DynamicResult.FindAll(s => s.Depth == Ditem.ID.ToString());

                        submenu.Sort((x, y) => x.Order.CompareTo(y.Order));
                        AddItemInDynamicMenu(Ditem, submenu);
                    }
                }
                else
                {
                    MenuMessages.Text = "<label>You have more that 2 values in the root</label>";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(ND.NavigationMessageError))
                {
                    MenuMessages.Text = string.Format("<label>{0}</label>",ND.NavigationMessageError);
                }
            }
        }

        private void AddItemInDynamicMenu(DynamicItemNavigation item, List<DynamicItemNavigation> submenu)
        {
            HtmlGenericControl newLi = new HtmlGenericControl("li");
            if(item.PageLink.ToLower().Equals(currentPage.ToLower()))
                newLi.Attributes["class"] = "menu-item-li dynamic-menu-selected";
            else
                newLi.Attributes["class"] = "menu-item-li";
            HtmlGenericControl a = new HtmlGenericControl("a");
            a.InnerText = item.PageName;
            if (isValidforUser(item.SharePointGroup))
            {
                a.Attributes["href"] = item.PageLink;
            }
            newLi.Controls.Add(a);                    

            if (submenu.Count > 0)
            {
                //submenu.Reverse();
                //tmpInvertNavigation = submenu;
                //tmpInvertNavigationCounter = 0;
                CreateSubMenu(submenu, newLi);
            }
            HtmlGenericControl ttcc = newLi;
            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            HtmlTextWriter hw = new HtmlTextWriter(tw);

            ttcc.RenderControl(hw);
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                if (sb.ToString().Contains("dynamic-menu-selected") && !newLi.Attributes["class"].Contains("dynamic-menu-selected"))
                    newLi.Attributes["class"] += " dynamic-menu-selected";
            }
            DynamicMenuRoot.Controls.Add(newLi); 
        }

        private void CreateSubMenu(List<DynamicItemNavigation> submenu, HtmlGenericControl prev)
        {
            if (submenu.Count > 0)
            {
                HtmlGenericControl ulsubmenu = new HtmlGenericControl("ul");
                ulsubmenu.Attributes["class"] = "sub-menu-item-ul";

                foreach (DynamicItemNavigation DN in submenu)
                {
                    HtmlGenericControl subLi = new HtmlGenericControl("li");
                    if (DN.PageLink.ToLower().Equals(currentPage.ToLower()))
                    {
                        subLi.Attributes["class"] = "sub-menu-item-li dynamic-menu-selected";                        
                    }
                    else
                    {
                        subLi.Attributes["class"] = "sub-menu-item-li";
                    }
                    HtmlGenericControl suba = new HtmlGenericControl("a");
                    suba.InnerText = DN.PageName;
                    if (isValidforUser(DN.SharePointGroup))
                    {
                        suba.Attributes["href"] = DN.PageLink;
                    }
                    subLi.Controls.Add(suba);

                    //here
                    List<DynamicItemNavigation> invert = DynamicResult.FindAll(s => s.Depth == DN.ID.ToString());
                    CreateSubMenu(invert, subLi);
                    //if ((tmpInvertNavigationCounter + 1) < tmpInvertNavigation.Count)
                    //{
                    //    tmpInvertNavigationCounter++;
                    //    CreateSubMenu(tmpInvertNavigation[tmpInvertNavigationCounter], subLi);
                    //}
                    ulsubmenu.Controls.Add(subLi);
                }

                


                prev.Controls.Add(ulsubmenu);
            }
        }

        private bool isValidRoot(List<DynamicItemNavigation> rootelements)
        {
            foreach (DynamicItemNavigation n in rootelements)
            {
                if (rootelements.FindAll(f => f.Brand == n.Brand).Count > 1)
                    return false;
            }
            return true;            
        }

        private bool isValidforUser(SPFieldUserValueCollection SharePointGroup)
        {
            if (SharePointGroup != null)
            {
                foreach (SPFieldUserValue svalue in SharePointGroup)
                {
                    SPUser cuser = SPContext.Current.Web.CurrentUser;
                    if (svalue.User != null)
                    {
                        if (cuser == svalue.User)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        SPGroup group = SPContext.Current.Web.SiteGroups.GetByID(svalue.LookupId);
                        if (isUserinGroup(group, cuser))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool isUserinGroup(SPGroup isgroup, SPUser oUser)
        {
            Boolean bUserIsInGroup = false;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {                
                if (oUser != null)
                {
                    foreach (SPUser item in isgroup.Users)
                    {
                        if (item.LoginName == oUser.LoginName)
                        {
                            bUserIsInGroup = true;
                            break;
                        }
                    }
                }                
            });
            return bUserIsInGroup;
        }
    }
}
