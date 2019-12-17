<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginChoice.aspx.cs" Inherits="SAML2ServiceProvider.LoginChoice" %>

<html>
<head runat="server">
    <title>Activants Service Provider</title>
	<link href="bootstrap/css/bootstrap.min.css" type="text/css" rel="stylesheet"/>
</head>
<body style="padding:0px;background-color:#8A39B7;">
    <div style="padding:8px;background-color:#FDD100">
        <center>
            <img src="Image/activants.png" />
        </center>
    </div>
    <div style="background-image:url(/Image/bg1.png); background-size: contain;background-repeat:no-repeat;">
        <div class="container body-content">
    
	<div class="jumbotron" style="background-color:transparent;">
    <div class="row">
        <div class="col-md-12">
            <center>
                <h1 style="color:white;text-shadow: 1px 1px 0px #FDD100;">Activants Pte Ltd</h1>
            </center>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12">
            <center>
                <img src="Image/network.png" ; width="100%" style="padding:0xp;margin:0px;" />
                <img src="Image/loader.gif" width="14%" />
                <img src="Image/loader.gif" width="14%" />
                <img src="Image/loader.gif" width="14%" />
                <img src="Image/loader.gif" width="14%" />
            </center>
        </div>
    </div>
    <br />
    <br />
    <div class="row">
        <div class="col-md-12">
            <center>
                <div style="width:35%;border-radius:10px;">
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
                    </div>
            </center>
        </div>
    </div>
        </div>
        <div>
            <hr />
            <footer>
                <center>
                    <p style="color:whitesmoke;font-size:15px;">&copy; @DateTime.Now.Year - Activants Pte Ltd | Service Provider | All Rights Reserved </p>
                </center>
            </footer>
        </div>
    </div>

   


</body>
</html>
