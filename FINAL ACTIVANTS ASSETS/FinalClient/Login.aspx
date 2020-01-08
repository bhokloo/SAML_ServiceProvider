<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="FinalClient.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html>
<head runat="server">
    <title>Activants Service Provider</title>
	<link href="bootstrap/css/bootstrap.min.css" type="text/css" rel="stylesheet"/>
</head>
<body style="padding:100px;background-color:#8A39B7;">
    <div class="row">
        <div class="col-md-12">
            <center>
                <form id="loginChoiceForm" runat="server" defaultbutton="continueButton">
		        <asp:Button ID="continueButton" runat="server" 
                    Text="INITIATE SAML LOGIN"
                    CssClass="btn btn-block" 
                    OnClick="continueButton_Click" 
                    style="background-color:#FDD100;color:#8A39B7;padding:9px;font-size:19px;text-shadow: 0px 1px 1px black;">
                     </asp:Button>
                    <% if(errorMessageLabel.Text != "") { %>
                       <div style="background-color:red;color:white;padding:0px;border-bottom-left-radius:10px;border-bottom-right-radius:10px;">
                                  <asp:Label ID="errorMessageLabel" runat="server" ForeColor="white">
			                </asp:Label> 
                             </div>
                    <% } %>

               </form>
            </center>
        </div>
    </div>
</body>
</html>
