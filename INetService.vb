Imports System.Net

Public Interface INetService
        Sub Start(ipAddress As IPAddress, port As Integer)
        
        Sub WriteLine(output As String)
        
        Sub InteractAsync()
        Sub ReadSync()
        Sub Write(output As String)
        Sub Shutdown()
End Interface