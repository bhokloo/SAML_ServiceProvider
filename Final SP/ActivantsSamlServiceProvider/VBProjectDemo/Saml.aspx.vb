Imports System.Net
Imports System.Web.Configuration

Public Class Saml
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub


    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim samlConfigurationId = WebConfigurationManager.AppSettings("samlConfigurationId")
        Dim SP As String = "https://localhost:44364/Saml/InitiateSingleSignOn?returnURL=abc.com&samlConfigurationId=" + samlConfigurationId
        Response.Redirect(SP)
    End Sub

End Class