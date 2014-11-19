// Copyright (c) 2014, Outercurve Foundation.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// - Redistributions of source code must  retain  the  above copyright notice, this
//   list of conditions and the following disclaimer.
//
// - Redistributions in binary form  must  reproduce the  above  copyright  notice,
//   this list of conditions  and  the  following  disclaimer in  the documentation
//   and/or other materials provided with the distribution.
//
// - Neither  the  name  of  the  Outercurve Foundation  nor   the   names  of  its
//   contributors may be used to endorse or  promote  products  derived  from  this
//   software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,  BUT  NOT  LIMITED TO, THE IMPLIED
// WARRANTIES  OF  MERCHANTABILITY   AND  FITNESS  FOR  A  PARTICULAR  PURPOSE  ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL,  SPECIAL,  EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO,  PROCUREMENT  OF  SUBSTITUTE  GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)  HOWEVER  CAUSED AND ON
// ANY  THEORY  OF  LIABILITY,  WHETHER  IN  CONTRACT,  STRICT  LIABILITY,  OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE)  ARISING  IN  ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Web.UI.WebControls;
using WebsitePanel.EnterpriseServer;
using WebsitePanel.Providers.Common;
using WebsitePanel.Providers.HostedSolution;
using WebsitePanel.Providers.OS;
using WebsitePanel.Providers.RemoteDesktopServices;
using WebsitePanel.WebPortal;

namespace WebsitePanel.Portal.RDS
{
    public partial class AssignedRDSServers : WebsitePanelModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }

            PackageContext cntx = PackagesHelper.GetCachedPackageContext(PanelSecurity.PackageId);
            if (cntx.Quotas.ContainsKey(Quotas.RDS_SERVERS))
            {
                btnAddServerToOrg.Enabled = (!(cntx.Quotas[Quotas.RDS_SERVERS].QuotaAllocatedValue <= gvRDSAssignedServers.Rows.Count) || (cntx.Quotas[Quotas.RDS_SERVERS].QuotaAllocatedValue == -1));
            }
        }

        protected void btnAddServerToOrg_Click(object sender, EventArgs e)
        {
            Response.Redirect(EditUrl("ItemID", PanelRequest.ItemID.ToString(), "rds_add_server",
                "SpaceID=" + PanelSecurity.PackageId));
        }

        protected void gvRDSAssignedServers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteItem")
            {
                // delete RDS Server
                int rdsServerId = int.Parse(e.CommandArgument.ToString());

                try
                {
                    RdsServer rdsServer = ES.Services.RDS.GetRdsServer(rdsServerId);
                    if (rdsServer.RdsCollectionId != null)
                    {
                        messageBox.ShowErrorMessage("RDS_UNASSIGN_SERVER_FROM_ORG_SERVER_IS_IN_COLLECTION");
                        return;
                    }

                    ResultObject result = ES.Services.RDS.RemoveRdsServerFromOrganization(rdsServerId);
                    if (!result.IsSuccess)
                    {
                        messageBox.ShowMessage(result, "REMOTE_DESKTOP_SERVICES_UNASSIGN_SERVER_FROM_ORG", "RDS");
                        return;
                    }

                    gvRDSAssignedServers.DataBind();
                    
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("REMOTE_DESKTOP_SERVICES_UNASSIGN_SERVER_FROM_ORG", ex);
                }
            }
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvRDSAssignedServers.PageSize = Convert.ToInt16(ddlPageSize.SelectedValue);

            gvRDSAssignedServers.DataBind();
        }
    }
}