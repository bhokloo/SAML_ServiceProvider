using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Web.Security.AntiXss;


public partial class sign : System.Web.UI.Page
{
    string strSingpassURL, strTargetURL, strReturnURL, strFunction;
    string ENV = ConfigurationManager.AppSettings["ENVIRONMENT"].ToString();
    protected void Page_Load(object sender, EventArgs e)
    {
    	Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
		Response.Cache.SetNoStore();

        string log = "sign.aspx.cs Page_Load()";
        bool is_valid = false;

        try
        {
            log += System.Environment.NewLine + "IP: " + (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
               Request.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim();

            if (!Page.IsPostBack)
            {
                Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            }
            //strFunction = SanitizeValue(Request.QueryString["function"]);
            strFunction = AntiXssEncoder.HtmlEncode(Request.QueryString["function"],false);
            log += strFunction;
            strTargetURL = ConfigurationManager.AppSettings["retURL_" + ENV].ToString() + "?function=" + strFunction;

            if (strFunction == "loginorg")
            {
		
                strReturnURL = ConfigurationManager.AppSettings["SAMLSP_CP_URL_" + ENV].ToString();
                strSingpassURL = ConfigurationManager.AppSettings["CorppassURL_" + ENV].ToString();
                strSingpassURL = strSingpassURL + "?RequestBinding=HTTPArtifact&ResponseBinding=HTTPArtifact&PartnerId=" + strReturnURL + "&Target=" + strTargetURL + "&NameIdFormat=Email&esrvcID=" + ConfigurationManager.AppSettings["SERVICE_ID_CP_" + ENV].ToString() + "&param1=NULL&param2=NULL";
            }
	        else if (strFunction == "registerorganisation")
            {
		
                strReturnURL = ConfigurationManager.AppSettings["SAMLSP_CP_URL_" + ENV].ToString();
                strSingpassURL = ConfigurationManager.AppSettings["CorppassURL_" + ENV].ToString();
                strSingpassURL = strSingpassURL + "?RequestBinding=HTTPArtifact&ResponseBinding=HTTPArtifact&PartnerId=" + strReturnURL + "&Target=" + strTargetURL + "&NameIdFormat=Email&esrvcID=" + ConfigurationManager.AppSettings["SERVICE_ID_CP_" + ENV].ToString() + "&param1=NULL&param2=NULL";
            }
	        else if (strFunction == "sgds_login_org")
            {
		
                strReturnURL = ConfigurationManager.AppSettings["SAMLSP_CP_URL_" + ENV].ToString();
                strSingpassURL = ConfigurationManager.AppSettings["CorppassURL_" + ENV].ToString();
                strSingpassURL = strSingpassURL + "?RequestBinding=HTTPArtifact&ResponseBinding=HTTPArtifact&PartnerId=" + strReturnURL + "&Target=" + strTargetURL + "&NameIdFormat=Email&esrvcID=" + ConfigurationManager.AppSettings["SERVICE_ID_CP_" + ENV].ToString() + "&param1=NULL&param2=NULL";
            }
	        else if (strFunction == "sgds_register_org")
            {
		
                strReturnURL = ConfigurationManager.AppSettings["SAMLSP_CP_URL_" + ENV].ToString();
                strSingpassURL = ConfigurationManager.AppSettings["CorppassURL_" + ENV].ToString();
                strSingpassURL = strSingpassURL + "?RequestBinding=HTTPArtifact&ResponseBinding=HTTPArtifact&PartnerId=" + strReturnURL + "&Target=" + strTargetURL + "&NameIdFormat=Email&esrvcID=" + ConfigurationManager.AppSettings["SERVICE_ID_CP_" + ENV].ToString() + "&param1=NULL&param2=NULL";
            }				
            else
            {
		        //SP
                //<add key="SAMLSP_URL_UAT" value="https://samlsppilot.imda.gov.sg/SAMLSP/SAML/AssertionConsumerService.aspx" />
                strReturnURL = ConfigurationManager.AppSettings["SAMLSP_URL_" + ENV].ToString();

                //IDP
                //<add key="SingpassURL_UAT" value="https://stg-saml.singpass.gov.sg/FIM/sps/SingpassIDPFed/saml20/logininitial" />
                strSingpassURL = ConfigurationManager.AppSettings["SingpassURL_" + ENV].ToString();

                //Target URL = https://schemes-uat.imda.gov.sg/SingPassAuth/verify.aspx

                //SP
                //<add key="SERVICE_ID_UAT" value="OSAM" />
                strSingpassURL = strSingpassURL + "?RequestBinding=HTTPArtifact&ResponseBinding=HTTPArtifact&PartnerId=" + strReturnURL + "&Target=" + strTargetURL + "&NameIdFormat=Email&esrvcID=" + ConfigurationManager.AppSettings["SERVICE_ID_" + ENV].ToString() + "&param1=&param2=";
                //https://stg-saml.singpass.gov.sg/FIM/sps/SingpassIDPFed/saml20/logininitial?RequestBinding=HTTPArtifact&ResponseBinding=HTTPArtifact&PartnerId=https://samlsppilot.imda.gov.sg/SAMLSP/SAML/AssertionConsumerService.aspx&Target=https://schemes-uat.imda.gov.sg/SingPassAuth/verify.aspx?function=&NameIdFormat=Email&esrvcID=OSAM&param1=abc&param2=def
            }
			
            log += System.Environment.NewLine + "strSingpassURL: " + strSingpassURL;
            is_valid = true;
        }
        catch (Exception ex)
        {
            log += System.Environment.NewLine + "ERROR: " + ex.Message;
        }
        finally
        {
            MyLog(log);
            if (is_valid == true)
            {
                Response.Redirect(strSingpassURL);
            }
            else
            {
                Response.End();
            }
        }
    }

    private void MyLog(string msg)
    {
        using (StreamWriter w = new StreamWriter(ConfigurationManager.AppSettings["LOG_" + ENV].ToString() + System.DateTime.Now.ToString("yyyyMMdd") + "_SP.txt", true))
        {
            w.WriteLine(System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss tt"));
            w.WriteLine(msg);
            w.WriteLine("--------------------------------------------");
            w.WriteLine("");
        }
    }

 public string SanitizeValue(string param)
{
    string retValue = string.Empty;
    try
    {
        if (string.IsNullOrEmpty(param))
            param = "";
        else
        {
            // PATH TRAVERSAL
            retValue = param.Replace(@"..\", "");
            retValue = param.Replace("../", "");

            retValue = param.Replace("&", "");
            retValue = param.Replace("<", "");
            retValue = param.Replace(">", "");
            retValue = param.Replace("\"", "");
            retValue = param.Replace("''", "");


            // HTTP RESPONSE SPLITTING
            retValue = param.Replace("%0d%0a", "");
            retValue = Regex.Replace(param, "[^a-zA-Z0-9/:-_.#,]", "");

            }
        }
    catch (Exception ex)
    {
    }
    return retValue;
}

}