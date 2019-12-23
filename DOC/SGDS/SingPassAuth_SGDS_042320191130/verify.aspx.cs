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
using System.Xml;
using System.Web.Security.AntiXss;


public class BasePage : Page
{
    protected override void Render(HtmlTextWriter writer)
    {
        StringBuilder sb = new StringBuilder();
        StringWriter sw = new StringWriter(sb);
        HtmlTextWriter hWriter = new HtmlTextWriter(sw);
        base.Render(hWriter);
        string html = sb.ToString();
        html = Regex.Replace(html, "<input[^>]*id=\"(__VIEWSTATE)\"[^>]*>", string.Empty, RegexOptions.IgnoreCase);
        html = Regex.Replace(html, "<input[^>]*id=\"(__VIEWSTATEGENERATOR)\"[^>]*>", string.Empty, RegexOptions.IgnoreCase);
        html = Regex.Replace(html, "<input[^>]*id=\"(__EVENTVALIDATION)\"[^>]*>", string.Empty, RegexOptions.IgnoreCase);
        writer.Write(html);
    }
}

public partial class verify : BasePage
{
    string ENV = ConfigurationManager.AppSettings["ENVIRONMENT"].ToString();
    protected void Page_Load(object sender, EventArgs e)
    {
		Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
		Response.Cache.SetNoStore();
        string log = "verify.aspx.cs Page_Load()";
        MyLog(log);
 
        try
        {
            log += System.Environment.NewLine + "IP: " + (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
               Request.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim();
    		MyLog(log);
            if (!IsPostBack)
            {
                Session["Source"] = "I";
                string strAuthStatus = null;
		        string strAuthType = null;
                string strErrorMessage = null;
                string strTargetURL = null;
                string strFunction = null;

                // AuthStatus = "Y" if authentication success. NRIC will be passed in "UserID" querystring
                // AuthStatus = "N" if authentication fail, error message will be passed in "StatusMessage" querystring
                //strAuthStatus = Request.QueryString["AuthStatus"];
		        //strAuthType = Request.QueryString["AuthType"];
                //strTargetURL = Request.QueryString["TargetURL"];
                //strFunction = Request.QueryString["function"].ToString();

                //strAuthStatus = SanitizeValue(Request.Form["AuthStatus"]);
                //strAuthType = SanitizeValue(Request.Form["AuthType"]);
                //strTargetURL = SanitizeValue(Request.Form["TargetURL"]);
                //strFunction = SanitizeValue(Request.QueryString["function"]);

                strAuthStatus=AntiXssEncoder.HtmlEncode(Request.Form["AuthStatus"],false);
                strAuthType = AntiXssEncoder.HtmlEncode(Request.Form["AuthType"], false);
                strTargetURL = AntiXssEncoder.HtmlEncode(Request.Form["TargetURL"], false);
                strFunction = AntiXssEncoder.HtmlEncode(Request.QueryString["function"], false);
                
                log += System.Environment.NewLine + "AuthStatus: " + strAuthStatus;
		        log += System.Environment.NewLine + "AuthType: " + strAuthType;
                log += System.Environment.NewLine + "TargetURL: " + strTargetURL;
                log += System.Environment.NewLine + "function: " + strFunction;

                strTargetURL = getFunctionURL(strFunction);
                log += System.Environment.NewLine + "strTargetURL: " + strTargetURL;
			
				if ((strFunction == "sgds_login_org") || (strFunction == "sgds_register_org"))
				{
					if (strAuthType == "CP")
					{
						if (strAuthStatus == "Y")
						{
							string strUserID = AntiXssEncoder.HtmlEncode(Request.Form["UserID"].ToString(),false);
							string strUEN = AntiXssEncoder.HtmlEncode(Request.Form["UEN"].ToString(),false);

							log += System.Environment.NewLine + "UserID: " + strUserID;
							log += System.Environment.NewLine + "UEN: " + strUEN;
							log += System.Environment.NewLine + "Encrypted strTargetURL: " + strTargetURL;
                            strTargetURL = strTargetURL + "?euid=" + HttpContext.Current.Server.UrlEncode(getEncryptedValue(strUserID)) + "&euen=" + HttpContext.Current.Server.UrlEncode(getEncryptedValue(strUEN)) + "&efn=" + HttpContext.Current.Server.UrlEncode(getEncryptedValue(strFunction)); 
							MyLog(log);

							string jsSetSession1 = "window.location='" + strTargetURL + "';";
							log += System.Environment.NewLine + "jsSetSession1: " + jsSetSession1;
							ScriptManager.RegisterStartupScript(this, typeof(Page), "OnClientClicking", jsSetSession1, true);	
						}
						else
						{
							// Get the error message
							strErrorMessage = AntiXssEncoder.HtmlEncode(Request.Form["ErrorMessage"],false);
							log += System.Environment.NewLine + "ErrorMessage: " + strErrorMessage;
					
							MyLog(log);

							Response.Redirect(ConfigurationManager.AppSettings["SGDS_HOME_PAGE_" + ENV].ToString());
						}
					}
                    else
                    {
                        // Get the error message
                        strErrorMessage = AntiXssEncoder.HtmlEncode(Request.Form["ErrorMessage"], false);
                        log += System.Environment.NewLine + "ErrorMessage: " + strErrorMessage;

                        MyLog(log);

                        Response.Redirect(ConfigurationManager.AppSettings["SGDS_HOME_PAGE_" + ENV].ToString());
                    }
				}
				else
				{
					if (strAuthType == "SP")
					{	
						if (strAuthStatus == "Y")
						{
							//Start - Add MyInfo QueryString
							string strMyInfo = "";
							//foreach (String key in Request.QueryString.AllKeys)
							//{
							//strMyInfo = strMyInfo + key + "=" + Request.QueryString[key] + "&";
							//}
							foreach (string key in System.Web.HttpContext.Current.Request.Form.AllKeys)
							{
								strMyInfo = strMyInfo + key + "=" + AntiXssEncoder.HtmlEncode(System.Web.HttpContext.Current.Request.Form[key],false) + "&";
							}
							strTargetURL = strTargetURL + "?" + strMyInfo;
							log += System.Environment.NewLine + "strTargetURL: " + strTargetURL;
							//End - Add MyInfo QueryString

							//string strUserID = Request.QueryString["UserID"].ToString();
							//string strUserID = SanitizeValue(Request.Form["UserID"]);
							string strUserID = AntiXssEncoder.HtmlEncode(Request.Form["UserID"],false);
							 
							strTargetURL = strTargetURL + "&euid=" + getEncryptedValue(strUserID);
							log += System.Environment.NewLine + "strTargetURL: " + strTargetURL;

							string sURL = strTargetURL;
							string sURL__;
	 
							try
							{
								sURL__ = sURL.Substring(sURL.IndexOf("__VIEWSTATE="), sURL.IndexOf("&") - sURL.IndexOf("__VIEWSTATE="));
								sURL = sURL.Replace(sURL__ + "&", "");
							}
							catch (Exception ex)
							{
								log += System.Environment.NewLine + "ERROR: sURL : sURL__ " + ex.Message;
								MyLog(log);
							}

							try
							{
								sURL__ = sURL.Substring(sURL.IndexOf("__VIEWSTATEGENERATOR"), sURL.IndexOf("&") - sURL.IndexOf("__VIEWSTATEGENERATOR"));
								sURL = sURL.Replace(sURL__ + "&", "");
							}
							catch (Exception ex)
							{
								log += System.Environment.NewLine + "ERROR: sURL : sURL__ " + ex.Message;
								MyLog(log);
							}

							try
							{
								sURL__ = sURL.Substring(sURL.IndexOf("__VIEWSTATEENCRYPTED"), sURL.IndexOf("&") - sURL.IndexOf("__VIEWSTATEENCRYPTED"));
								sURL = sURL.Replace(sURL__ + "&", "");
							}
							catch (Exception ex)
							{
								log += System.Environment.NewLine + "ERROR: sURL : sURL__ " + ex.Message;
								MyLog(log);
							}


							try
							{
								sURL__ = sURL.Substring(sURL.IndexOf("__EVENTVALIDATION"), sURL.IndexOf("&") - sURL.IndexOf("__EVENTVALIDATION"));
								sURL = sURL.Replace(sURL__ + "&", "");
							}
							catch (Exception ex)
							{
								log += System.Environment.NewLine + "ERROR: sURL : sURL__ " + ex.Message;
								MyLog(log);
							}

							strTargetURL = sURL;
							 
							log += System.Environment.NewLine + "UserID: " + strUserID;
							log += System.Environment.NewLine + "strTargetURL with encrypted Value: " + strTargetURL;
							MyLog(log);

							//string jsSetSession = "sessionStorage.setItem('MDA_LOGIN_SINGPASS_ID', '" + strUserID + "'); window.location='" + strTargetURL + "';";
							string jsSetSession = "window.location='" + strTargetURL + "';";

							ScriptManager.RegisterStartupScript(this, typeof(Page), "OnClientClicking", jsSetSession, true);
						}
						else
						{
							// Get the error message
							//strErrorMessage = Request.QueryString["ErrorMessage"].ToString();
							strErrorMessage = AntiXssEncoder.HtmlEncode(Request.Form["ErrorMessage"].ToString(),false);
							log += System.Environment.NewLine + "ErrorMessage: " + strErrorMessage;
							MyLog(log);

							Response.Redirect(ConfigurationManager.AppSettings["HOME_PAGE_" + ENV].ToString());
						}
					}
					else if (strAuthType == "CP")
					{
						if (strAuthStatus == "Y")
						{
							//string strUserID = Request.QueryString["UserID"].ToString();
							//string strUEN = Request.QueryString["UEN"].ToString();
							string strUserID = AntiXssEncoder.HtmlEncode(Request.Form["UserID"].ToString(),false);
							string strUEN = AntiXssEncoder.HtmlEncode(Request.Form["UEN"].ToString(),false);

							log += System.Environment.NewLine + "UserID: " + strUserID;
							log += System.Environment.NewLine + "UEN: " + strUEN;
							log += System.Environment.NewLine + "Encrypted strTargetURL: " + strTargetURL;
							strTargetURL = strTargetURL + "?euid=" + getEncryptedValue(strUserID) + "&euen=" + getEncryptedValue(strUEN); 
							MyLog(log);

							//string jsSetSession = "sessionStorage.setItem('MDA_LOGIN_SINGPASS_ID', '" + strUserID + "'); window.location='" + strTargetURL + "';";
							//string jsSetSession1 = "sessionStorage.setItem('MDA_LOGIN_CORPPASS_ID','" + strUEN +"');  sessionStorage.setItem('MDA_LOGIN_SINGPASS_ID', '" + strUserID + "');window.location='" + strTargetURL + "';";
								
							//log += System.Environment.NewLine + "jsSetSession: " + jsSetSession;
							
							string jsSetSession1 = "window.location='" + strTargetURL + "';";
							log += System.Environment.NewLine + "jsSetSession1: " + jsSetSession1;
							ScriptManager.RegisterStartupScript(this, typeof(Page), "OnClientClicking", jsSetSession1, true);	
						}
						else
						{
							// Get the error message
							//strErrorMessage = Request.QueryString["ErrorMessage"].ToString();
							strErrorMessage = AntiXssEncoder.HtmlEncode(Request.Form["ErrorMessage"],false);
							log += System.Environment.NewLine + "ErrorMessage: " + strErrorMessage;
					
							MyLog(log);

							Response.Redirect(ConfigurationManager.AppSettings["HOME_PAGE_" + ENV].ToString());
						}
					}
                    else
                    {
                        // Get the error message
                        //strErrorMessage = Request.QueryString["ErrorMessage"].ToString();
                        strErrorMessage = AntiXssEncoder.HtmlEncode(Request.Form["ErrorMessage"], false);
                        log += System.Environment.NewLine + "ErrorMessage: " + strErrorMessage;

                        MyLog(log);

                        Response.Redirect(ConfigurationManager.AppSettings["HOME_PAGE_" + ENV].ToString());
                    }
				}
            }
        }
        catch (Exception ex)
        {
            log += System.Environment.NewLine + "ERROR: " + ex.Message;
            MyLog(log);
        }
        finally
        {
        }

    }

    private string getFunctionURL(string strFunction)
    {
        switch (strFunction)
        {
       		case "loginorg":
                return ConfigurationManager.AppSettings["LOGIN_URL_" + ENV].ToString();
	        case "loginind":
		        return ConfigurationManager.AppSettings["LOGIN_URL_IND_" + ENV].ToString();
            case "individualregistration":
                return ConfigurationManager.AppSettings["IND_REG_URL_" + ENV].ToString();
            case "registerorganisation":
                return ConfigurationManager.AppSettings["ORG_REG_URL_" + ENV].ToString();
       		case "sgds_login_org":
                return ConfigurationManager.AppSettings["SGDS_LOGIN_URL_" + ENV].ToString();
            case "sgds_register_org":
                return ConfigurationManager.AppSettings["SGDS_ORG_REG_URL_" + ENV].ToString();
            default:
                return "FAIL";
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

    private string getEncryptedValue(string val)
    {
        string EncryptedValue="";
        EncryptedValue=Encrypt(val);
        return EncryptedValue;
    }

    public string Encrypt(string clearText)
    {
        try
        {
           //var ENCRYPTION_KEY = "";
            var doc=new XmlDocument();
            //var path = ConfigurationManager.AppSettings["CON_XML"];
            //var filePath = Path.Combine(path, "Config.xml");
            //if (File.Exists(filePath))
            //{
            //  doc.Load(filePath);
            //  ENCRYPTION_KEY = doc.SelectSingleNode("//SetUp/EncryptionKey").InnerText;
            //}
            var ENCRYPTION_KEY = ConfigurationManager.AppSettings["EncryptionKey"];
             byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(ENCRYPTION_KEY, new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
        }
        catch (Exception ex)
        {
MyLog(ex.Message);
        }

        return clearText;
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
            retValue = param;
            retValue = retValue.Replace(@"..\", "");
            retValue = retValue.Replace("../", "");

            retValue = retValue.Replace("&", "");
            retValue = retValue.Replace("<", "");
            retValue = retValue.Replace(">", "");
            retValue = retValue.Replace("\"", "");
            retValue = retValue.Replace("''", "");


            // HTTP RESPONSE SPLITTING
            retValue = retValue.Replace("%0d%0a", "");
            retValue = Regex.Replace(retValue, "[^a-zA-Z0-9/:-_.#, ]", "");

            }
        }
    catch (Exception ex)
    {
    }
    return retValue;
}
}
