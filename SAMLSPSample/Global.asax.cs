using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

using ComponentSpace.SAML2.Notifications;
using ComponentSpace.SAML2.Protocols;
using SAMLSPSample.SAMLSP;

namespace SAMLSPSample
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            SAMLObservable.Subscribe(new SAMLObserver());
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            try
            {
                string s =HttpContext.Current.Request.Headers.ToString();
                System.Diagnostics.Debug.WriteLine("args1: {0} ", s);
                HttpContext.Current.Request.Headers["Accept-Encoding"] = "";
                HttpContext.Current.Request.Headers.Remove("Accept-Encoding");
                HttpContext.Current.Response.Headers.Remove("Accept-Encoding");
                s = HttpContext.Current.Request.Headers.ToString();
                System.Diagnostics.Debug.WriteLine("args2: {0} ", s);

            }
            catch (Exception) { }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            try
            {
                string cookieName = FormsAuthentication.FormsCookieName;
                string cookieVal;
                HttpCookie authCookie;
                // extract the FormsAuthentication cookie
                if (HttpContext.Current.Request.IsAuthenticated) { }
                authCookie = HttpContext.Current.Request.Cookies[cookieName];
                if (authCookie == null)
                {
                    if (HttpContext.Current.Request.QueryString.Get("Auth") == null)
                    {
                        if ((HttpContext.Current.Session == null) || (HttpContext.Current.Session.Contents["AuthCookie"] == null))
                            // no authorization cookie is found
                            return;
                        cookieVal = Convert.ToString(HttpContext.Current.Session.Contents["AuthCookie"]);
                    }
                    else
                        cookieVal = HttpContext.Current.Request.QueryString.Get("Auth");
                    authCookie = new HttpCookie("cookiename", cookieVal);
                }
                FormsAuthenticationTicket tkt;
                try
                {
                    tkt = FormsAuthentication.Decrypt(authCookie.Value);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                // has the ticket expired ??
                if (tkt.Expired)
                    return;
                //parse list of permissions
                string[] roles;
                FormsAuthentication.RenewTicketIfOld(tkt);
                roles = tkt.UserData.Split(new char[] { ',' });
                // create identity object
                FormsIdentity id = new FormsIdentity(tkt);
                //this principal will flow through the request
                GenericPrincipal principal = new GenericPrincipal(id, roles);
                //attach the new principal to the current HttpContext object
                HttpContext.Current.User = principal;
            }
            catch (Exception ex) { 
                
                throw ex;
            }
        }
    }
}