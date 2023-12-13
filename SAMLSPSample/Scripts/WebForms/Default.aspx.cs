using ComponentSpace.SAML2.Exceptions;
using ComponentSpace.SAML2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SAMLSPSample.Scripts.WebForms
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IDictionary<string, string> attributes = (IDictionary<string, string>)Session[SAMLSP.AssertionConsumerService.AttributesSessionKey];
            if (attributes == null)
            {
                return;
            }
            string htmlTable = "<table id='myTable' style='border: 1px solid black;' > <tr class='header'><th style = 'width:20%;'> Attribute </th><th style = 'width:40%;'> Value </th> </tr>";
            HttpCookie ramesauthcookie = new HttpCookie("myrameshcokieguid");

            foreach (KeyValuePair<string, string> entry in attributes)
            {
                // do something with entry.Value or entry.Key
                if (entry.Key == "guid")
                {
                    ramesauthcookie.Value = entry.Value;
                    Response.Cookies[entry.Key].Value = entry.Value;
                }
                if (entry.Key == "ot_cookie_consent_token")
                {
                    ramesauthcookie.Value = entry.Value;
                    Response.Cookies[entry.Key].Value = entry.Value;
                }
                // do something with entry.Value or entry.Key
                htmlTable += "<tr><td>" + entry.Key + "</td><td>" + entry.Value + "</td></tr>";
            }
            //First get user claims    
            var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
            var sesSionOj = HttpContext.Current.Session;
            var req = (User.Identity as ClaimsIdentity).Claims.Where(c => c.Type == "FirmName").FirstOrDefault();
           // string reqValue = req.Value;
            //Filter specific claim    
            //  claims?.FirstOrDefault(x => x.Type.Equals("FirmName", StringComparison.OrdinalIgnoreCase))?.Value
            htmlTable += "<tr><td>"+ " " +  "</td></tr>";

            htmlTable += "</table>";
            lblTable.Text = htmlTable;
        }



        protected void logoutButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Logout locally.
                FormsAuthentication.SignOut();

                if (SAMLServiceProvider.CanSLO(WebConfigurationManager.AppSettings[AppSettings.PartnerIdP]))
                {
                    // Request logout at the identity provider.
                    string partnerIdP = WebConfigurationManager.AppSettings[AppSettings.PartnerIdP];
                    SAMLServiceProvider.InitiateSLO(Response, null, null, partnerIdP);

                    
                }
                else
                {
                    
                    Response.Redirect("../../../loginSP.aspx");

                }
            }
            catch (SAMLConfigurationException samlconfigex)
            {
                throw samlconfigex;
            }
            catch (SAMLException samlex)
            {
                throw samlex;
            }

        }
    }
}