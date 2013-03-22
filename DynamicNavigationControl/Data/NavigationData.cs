using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace DynamicNavigationControl.Data
{
    public class NavigationData
    {
        private Guid siteID { set; get; }
        private Guid webID { set; get; }
        private string listName { set; get; }

        public string NavigationMessageError { get; set; }

        public NavigationData(Guid siteid, Guid webid, string listname)
        {
            this.NavigationMessageError = string.Empty;
            this.siteID = siteid;
            this.webID = webid;
            this.listName = listname;
        }

        public List<DynamicItemNavigation> GetDynamicNavigationInformation()
        {
            List<DynamicItemNavigation> dynamicList = new List<DynamicItemNavigation>();
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(this.siteID))
                {
                    using (SPWeb web = site.OpenWeb(this.webID))
                    {
                        if (web.Lists.TryGetList(this.listName) != null)
                        {
                            SPList navigationList = web.Lists[this.listName];
                            if(isValidList(navigationList))
                            {
                                SPQuery listQuery = new SPQuery();
                                listQuery.Query = "<Where><Eq><FieldRef Name='Show' /><Value Type='Boolean'>1</Value></Eq></Where>";
                                SPListItemCollection cgenericCollection = navigationList.GetItems(listQuery);
                                foreach (SPListItem navigationItem in cgenericCollection)
                                {
                                    string localBrand = string.Empty;
                                    string localPageName = string.Empty;
                                    string localPageLink = string.Empty;
                                    int localOrder = 0;
                                    string localDepth = string.Empty;
                                    SPFieldUserValueCollection localSharePointGroup = null;
                                    bool localShow = false;

                                    if (navigationItem.Fields.ContainsField(constants.columns.NavigationList.Brand) && navigationItem[constants.columns.NavigationList.Brand] != null)
                                    {
                                        localBrand = navigationItem[constants.columns.NavigationList.Brand].ToString();                                        
                                    }

                                    if (navigationItem.Fields.ContainsField(constants.columns.NavigationList.PageName) && navigationItem[constants.columns.NavigationList.PageName] != null)
                                    {
                                        localPageName = navigationItem[constants.columns.NavigationList.PageName].ToString();
                                    }

                                    if (navigationItem.Fields.ContainsField(constants.columns.NavigationList.PageLink) && navigationItem[constants.columns.NavigationList.PageLink] != null)
                                    {
                                        localPageLink = navigationItem[constants.columns.NavigationList.PageLink].ToString();
                                    }

                                    if (navigationItem.Fields.ContainsField(constants.columns.NavigationList.Order) && navigationItem[constants.columns.NavigationList.Order] != null)
                                    {
                                        localOrder = int.Parse(navigationItem[constants.columns.NavigationList.Order].ToString());
                                    }

                                    if (navigationItem.Fields.ContainsField(constants.columns.NavigationList.Depth) && navigationItem[constants.columns.NavigationList.Depth] != null)
                                    {
                                        localDepth = navigationItem[constants.columns.NavigationList.Depth].ToString();
                                    }

                                    if (navigationItem.Fields.ContainsField(constants.columns.NavigationList.SharePointGroup) && navigationItem[constants.columns.NavigationList.SharePointGroup] != null)
                                    {
                                        localSharePointGroup = new SPFieldUserValueCollection(navigationItem.Web,navigationItem[constants.columns.NavigationList.SharePointGroup].ToString());
                                    }

                                    if (navigationItem.Fields.ContainsField(constants.columns.NavigationList.Show) && navigationItem[constants.columns.NavigationList.Show] != null)
                                    {
                                        localShow = bool.Parse(navigationItem[constants.columns.NavigationList.Show].ToString());
                                    }

                                    dynamicList.Add(new DynamicItemNavigation{
                                        ID = navigationItem.ID,
                                        Brand = localBrand,
                                        PageName = localPageName,
                                        PageLink = localPageLink,
                                        Order = localOrder,
                                        Depth = localDepth,
                                        SharePointGroup = localSharePointGroup,
                                        Show = localShow
                                    });
                                }
                            }
                            else
                            {
                                //This list is not valid. It has not all teh necesasary columns
                                this.NavigationMessageError = "The list does not have the correct columns";
                            }
                        }
                        else
                        {
                            //This list doesn't exist
                            this.NavigationMessageError = "The list doesn't exist";
                        }
                    }
                }
            });
            return dynamicList;
        }

        private bool isValidList(SPList navigationList)
        {
            if (navigationList.Fields.ContainsField(constants.columns.NavigationList.Brand)
                && navigationList.Fields.ContainsField(constants.columns.NavigationList.PageName)
                && navigationList.Fields.ContainsField(constants.columns.NavigationList.PageLink)
                && navigationList.Fields.ContainsField(constants.columns.NavigationList.Order)
                && navigationList.Fields.ContainsField(constants.columns.NavigationList.Depth)
                && navigationList.Fields.ContainsField(constants.columns.NavigationList.SharePointGroup)
                && navigationList.Fields.ContainsField(constants.columns.NavigationList.Show)
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
