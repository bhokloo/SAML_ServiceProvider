Public Class _Default
    Inherits System.Web.UI.Page

    Public str_maintenance_message As String = String.Empty
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Using sgdsClient As New SGDS_ServiceReference.SGDServiceClient()
                sgdsClient.Get_Maintenance_Message(str_maintenance_message, GenerateServiceKey(True))
                If String.IsNullOrEmpty(str_maintenance_message.Trim()) Then
                    System.Web.UI.ScriptManager.RegisterStartupScript(Me, [GetType](), Guid.NewGuid().ToString(), "document.getElementById('divMaintenanceMessage').style.display='none'", True)
                Else
                    System.Web.UI.ScriptManager.RegisterStartupScript(Me, [GetType](), Guid.NewGuid().ToString(), "document.getElementById('btn_log').style.pointerEvents = 'none';document.getElementById('btn_log').style.opacity = '0.8';", True)
                    System.Web.UI.ScriptManager.RegisterStartupScript(Me, [GetType](), Guid.NewGuid().ToString(), "document.getElementById('btn_register').style.pointerEvents = 'none';document.getElementById('btn_register').style.opacity = '0.8';", True)
                End If
            End Using
        Catch ex As Exception
            Log(ex)
        End Try
    End Sub
End Class