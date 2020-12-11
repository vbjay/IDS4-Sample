Imports IdentityModel.OidcClient.Browser
Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http
Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading


Public Class SystemBrowser
    Implements IBrowser

    Public ReadOnly Property Port As Integer
    Private ReadOnly _path As String

    Public Sub New(ByVal Optional port As Integer? = Nothing, ByVal Optional path As String = Nothing)
        _path = path

        If Not port.HasValue Then
            Me.Port = GetRandomUnusedPort()
        Else
            Me.Port = port.Value
        End If
    End Sub

    Private Function GetRandomUnusedPort() As Integer
        Dim listener = New TcpListener(IPAddress.Loopback, 0)
        listener.Start()
        Dim port = CType(listener.LocalEndpoint, IPEndPoint).Port
        listener.Stop()
        Return port
    End Function

    Private Async Function InvokeAsync(options As BrowserOptions, Optional cancellationToken As CancellationToken = Nothing) As Task(Of BrowserResult) Implements IBrowser.InvokeAsync
        Using listener = New LoopbackHttpListener(Port, _path)
            Dim proc = OpenBrowser(options.StartUrl)

            Try
                Dim result = Await listener.WaitForCallbackAsync()
                ' proc.Kill()

                If String.IsNullOrWhiteSpace(result) Then
                    Return New BrowserResult With {
                        .ResultType = BrowserResultType.UnknownError,
                        .[Error] = "Empty response."
                    }
                End If

                Return New BrowserResult With {
                    .Response = result,
                    .ResultType = BrowserResultType.Success
                }
            Catch ex As TaskCanceledException
                Return New BrowserResult With {
                    .ResultType = BrowserResultType.Timeout,
                    .[Error] = ex.Message
                }
            Catch ex As Exception
                Return New BrowserResult With {
                    .ResultType = BrowserResultType.UnknownError,
                    .[Error] = ex.Message
                }
            End Try
        End Using
    End Function

    Public Shared Function OpenBrowser(ByVal url As String) As Process
        Dim proc As Process
        Try
            proc = Process.Start(url)
        Catch

            If RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
                url = url.Replace("&", "^&")
                proc = Process.Start(New ProcessStartInfo("cmd", $"/c start {url}") With {
                    .CreateNoWindow = True
                })
            ElseIf RuntimeInformation.IsOSPlatform(OSPlatform.Linux) Then
                proc = Process.Start("xdg-open", url)
            ElseIf RuntimeInformation.IsOSPlatform(OSPlatform.OSX) Then
                proc = Process.Start("open", url)
            Else
                Throw
            End If
        End Try
        Return proc
    End Function


End Class

Public Class LoopbackHttpListener
    Implements IDisposable

    Const DefaultTimeout As Integer = 60 * 5
    Private _host As IWebHost
    Private _source As TaskCompletionSource(Of String) = New TaskCompletionSource(Of String)()
    Private _url As String

    Public ReadOnly Property Url As String
        Get
            Return _url
        End Get
    End Property

    Public Sub New(ByVal port As Integer, ByVal Optional path As String = Nothing)
        path = If(path, String.Empty)
        If path.StartsWith("/") Then path = path.Substring(1)
        _url = $"http://127.0.0.1:{port}/{path}"
        _host = New WebHostBuilder().UseKestrel().UseUrls(_url).Configure(AddressOf Configure).Build()
        _host.Start()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Task.Run(Async Function()
                     Await Task.Delay(500)
                     _host.Dispose()
                 End Function)
    End Sub

    Private Sub Configure(ByVal app As IApplicationBuilder)
        app.Run(Async Function(ctx)

                    If ctx.Request.Method = "GET" Then
                        SetResult(ctx.Request.QueryString.Value, ctx)
                    ElseIf ctx.Request.Method = "POST" Then

                        If Not ctx.Request.ContentType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase) Then
                            ctx.Response.StatusCode = 415
                        Else

                            Using sr = New StreamReader(ctx.Request.Body, Encoding.UTF8)
                                Dim body = Await sr.ReadToEndAsync()
                                SetResult(body, ctx)
                            End Using
                        End If
                    Else
                        ctx.Response.StatusCode = 405
                    End If
                End Function)
    End Sub

    Private Sub SetResult(ByVal value As String, ByVal ctx As HttpContext)
        Dim rsp As String = "
<html>
<head>
    <script type=""text/javascript"">
        function closeMe() {
            window.close();
        }
        setTimeout(closeMe, 500);
    </script>
<body>
    <h1>
        You can now return to the application.
    </h1>
</body>
</html>".Replace(vbCrLf, "")
        Try
            ctx.Response.StatusCode = 200
            ctx.Response.ContentType = "text/html"
            ctx.Response.WriteAsync(rsp)
            ctx.Response.Body.Flush()
            _source.TrySetResult(value)
        Catch
            ctx.Response.StatusCode = 400
            ctx.Response.ContentType = "text/html"
            ctx.Response.WriteAsync("<h1>Invalid request.</h1>")
            ctx.Response.Body.Flush()
        End Try
    End Sub

    Public Function WaitForCallbackAsync(ByVal Optional timeoutInSeconds As Integer = DefaultTimeout) As Task(Of String)
        Task.Run(Async Function()
                     Await Task.Delay(timeoutInSeconds * 1000)
                     _source.TrySetCanceled()
                 End Function)
        Return _source.Task
    End Function
End Class
