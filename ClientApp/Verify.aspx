<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Verify.aspx.cs" Inherits="ClientApp.Verify" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent">
      <form id="defaultForm">
	<div>
		<h1>Welcome to the Service Provider Site</h1>
		<p>
			You are logged in as <%= Context.User.Identity.Name %>.
		</p>    
		<br />
		

		<asp:Button ID="logoutButton" Text="Logout" OnClick="logoutButton_Click" />
	</div>
    </form>

   

</asp:Content>

  