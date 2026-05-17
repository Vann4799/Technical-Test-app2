Imports System.Text.Json.Serialization

Public Class ApiResponse(Of T)
    Public Property Success As Boolean
    Public Property Message As String = ""
    Public Property Data As T
End Class

Public Class ErrorResponse
    Public Property Success As Boolean
    Public Property Message As String = ""
    Public Property Errors As Dictionary(Of String, String) = New Dictionary(Of String, String)()
End Class

Public Class JurusanModel
    Public Property Id As Long
    Public Property NamaJurusan As String = ""
    Public Property Fakultas As String = ""
    Public Property CreatedAt As DateTime?
    Public Property UpdatedAt As DateTime?
End Class

Public Class MahasiswaModel
    Public Property Id As Long
    Public Property Nim As String = ""
    Public Property Nama As String = ""
    Public Property Email As String = ""
    Public Property Jurusan As JurusanModel
    Public Property CreatedAt As DateTime?
    Public Property UpdatedAt As DateTime?

    <JsonIgnore>
    Public ReadOnly Property NamaJurusan As String
        Get
            If Jurusan Is Nothing Then
                Return ""
            End If
            Return Jurusan.NamaJurusan
        End Get
    End Property
End Class

Public Class JurusanRequest
    Public Property NamaJurusan As String = ""
    Public Property Fakultas As String = ""
End Class

Public Class MahasiswaRequest
    Public Property Nim As String = ""
    Public Property Nama As String = ""
    Public Property Email As String = ""
    Public Property JurusanId As Long
End Class
