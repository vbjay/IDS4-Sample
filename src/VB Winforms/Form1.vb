Imports System.Text
Imports IdentityModel.OidcClient

Public Class Form1
    Dim _oidcClient As OidcClient
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()


    End Sub

    Private Async Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        btnLogin.Enabled = False
        Dim sb As New StringBuilder
        Dim msg As String = Nothing
        Try

            Dim lr = Await Auth.SignIn
            sb.AppendLine($"{vbCrLf}Sign In occured: {DateTimeOffset.Now}")
            sb.AppendLine($"{vbCrLf}Authentication Time: {lr.AuthenticationTime}")
            Activate()

            If lr.IsError Then
                sb.AppendLine($"{vbCrLf}{vbCrLf}Error:{vbCrLf}{lr.Error}")
            Else
                sb.AppendLine($"{vbCrLf}{vbCrLf}Claims:")

                For Each claim In lr.User.Claims
                    sb.AppendLine($"{claim.Type}: {claim.Value}")
                Next

                If Not String.IsNullOrWhiteSpace(lr.AccessToken) Then
                    sb.AppendLine($"Access token:{vbCrLf}{lr.AccessToken}")
                End If

                If Not String.IsNullOrWhiteSpace(lr.IdentityToken) Then
                    sb.AppendLine($"Identity token:{vbLf}{lr.IdentityToken}")
                End If

                If Not String.IsNullOrWhiteSpace(lr.RefreshToken) Then
                    sb.AppendLine($"Refresh token:{vbCrLf}{lr.RefreshToken}")
                End If
            End If
            sb.AppendLine($"{vbCrLf}Access Token Expires In:{lr.AccessTokenExpiration}")
            msg = $"Hi {lr.User.Identity.Name}.  You have a randomID value of {If(lr.User.Claims.FirstOrDefault(Function(c) c.Type = "RandomID")?.Value, "{null}")}."

        Catch ex As Exception
            sb.AppendLine($"{vbCrLf}{ex.Message}")
        End Try
        txtUser.AppendText(sb.ToString)
        txtUser.SelectionStart = txtUser.TextLength - 1
        txtUser.SelectionLength = 0
        If Not String.IsNullOrEmpty(msg) Then MessageBox.Show(msg)
        btnLogin.Enabled = True
    End Sub
End Class
