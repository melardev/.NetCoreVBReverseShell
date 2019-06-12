Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Public Class TcpSyncNetService
    Implements INetService

    Public Property Client As TcpClient

    Private Property NetStream As NetworkStream

    Private Property Reader As StreamReader

    Private Property Writer As StreamWriter

    Public Event OutputDataReceived As TcpSyncNetService.LineReceivedHandler


    Public Event Connected As TcpSyncNetService.ConnectedHandler

    Public Event ClientDisconnected As TcpSyncNetService.DisconnectionHandler

    Public Delegate Sub LineReceivedHandler(line As String)

    Public Delegate Sub ConnectedHandler()

    Public Delegate Sub DisconnectionHandler()

    Public Sub Start(iPAddress As IPAddress, port As Integer) Implements INetService.Start
        Me.Client = New TcpClient(AddressFamily.InterNetwork)
        Me.Client.Connect(iPAddress, port)
        Me.NetStream = Me.Client.GetStream()
        Me.Reader = New StreamReader(Me.NetStream)
        Me.Writer = New StreamWriter(Me.NetStream)


        RaiseEvent Connected()
    End Sub

    Public Sub WriteLine(line As String) Implements INetService.WriteLine
        Dim flag As Boolean = Not line.EndsWith(vbLf)
        If flag Then
            line += vbLf
        End If
        Me.Write(line)
    End Sub

    Public Sub Write(output As String) Implements INetService.Write
        Try
            Me.Writer.Write(output)
            Me.Writer.Flush()
        Catch exception As IOException
            Me.CloseAndNotify()
        End Try
    End Sub

    Public Sub ReadSync() Implements INetService.ReadSync
        Try
            While True
                Dim line As String = Me.Reader.ReadLine()
                RaiseEvent OutputDataReceived(line)
            End While
        Catch exception As IOException
            Me.CloseAndNotify()
        End Try
    End Sub

    Public Sub InteractAsync() Implements INetService.InteractAsync
        Call new Thread(sub() 
            Me.ReadSync()
        End Sub).Start()
    End Sub

    Private Sub CloseAndNotify()
        Me.Close()
        RaiseEvent ClientDisconnected()
    End Sub

    Private Sub Close()
        Me.Writer.Close()
        Me.Reader.Close()
        Me.NetStream.Close()
    End Sub

    Public Sub Shutdown() Implements INetService.Shutdown
        Me.Close()
    End Sub
End Class
