Imports System.Net
Imports System.Security.Claims
Imports System.Text
Imports System.Threading
Imports IdentityModel.OidcClient

Public Class Auth
    Public Shared Async Function SignIn() As Task(Of LoginResult)
        Dim br As New SystemBrowser


        Dim redirectUri As String = $"http://127.0.0.1:{br.Port}"
        Debug.WriteLine("redirect URI: " & redirectUri)

        '.Authority = "https://localhost:44310",
        '.ClientId = "vb-Winforms",
        '.ClientSecret = "f1bbb0bb-0471-40ed-b61b-70e55300ca71-399fc9d0-4016-4bc4-bea6-8bd375867689fac144cd-667d-4fa1-9165-64b05432e797",
        '.Scope = "openid profile email weather.read weather.write",

        '.Authority = "https://demo.identityserver.io",
        '.ClientId = "interactive.confidential.short",
        '.ClientSecret = "secret",
        '.Scope = "openid profile email api offline_access",
        Dim options = New OidcClientOptions With {
            .Authority = "https://localhost:44310",
            .ClientId = "vb-Winforms",
            .ClientSecret = "0559e8a8-dc72-4c6a-9b00-6d0917aa6588714e1e07-a4a2-47c0-9291-6eac956b2f29",
            .Scope = "openid profile email weather.read weather.write",
            .RedirectUri = redirectUri,
            .Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,
            .Browser = br
            }


        Dim client = New OidcClient(options)
        Dim result = Await client.LoginAsync()

        Return result

    End Function



End Class
