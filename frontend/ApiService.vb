Imports System.Net
Imports System.Net.Http
Imports System.Text
Imports System.Text.Json

Public Class ApiService
    Private Shared ReadOnly JsonOptions As New JsonSerializerOptions With {
        .PropertyNameCaseInsensitive = True,
        .PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    }

    Private ReadOnly _client As HttpClient

    Public Sub New(baseUrl As String)
        _client = New HttpClient With {
            .BaseAddress = New Uri(NormalizeBaseUrl(baseUrl)),
            .Timeout = TimeSpan.FromSeconds(20)
        }
    End Sub

    Public Async Function GetJurusanAsync(search As String) As Task(Of List(Of JurusanModel))
        Dim url = "api/jurusan"
        If Not String.IsNullOrWhiteSpace(search) Then
            url &= "?search=" & Uri.EscapeDataString(search.Trim())
        End If

        Dim response = Await _client.GetAsync(url)
        Return Await ReadDataAsync(Of List(Of JurusanModel))(response)
    End Function

    Public Async Function CreateJurusanAsync(request As JurusanRequest) As Task(Of JurusanModel)
        Dim response = Await _client.PostAsync("api/jurusan", ToJsonContent(request))
        Return Await ReadDataAsync(Of JurusanModel)(response)
    End Function

    Public Async Function UpdateJurusanAsync(id As Long, request As JurusanRequest) As Task(Of JurusanModel)
        Dim response = Await _client.PutAsync($"api/jurusan/{id}", ToJsonContent(request))
        Return Await ReadDataAsync(Of JurusanModel)(response)
    End Function

    Public Async Function DeleteJurusanAsync(id As Long) As Task
        Dim response = Await _client.DeleteAsync($"api/jurusan/{id}")
        Await EnsureSuccessAsync(response)
    End Function

    Public Async Function GetMahasiswaAsync(search As String) As Task(Of List(Of MahasiswaModel))
        Dim url = "api/mahasiswa"
        If Not String.IsNullOrWhiteSpace(search) Then
            url &= "?search=" & Uri.EscapeDataString(search.Trim())
        End If

        Dim response = Await _client.GetAsync(url)
        Return Await ReadDataAsync(Of List(Of MahasiswaModel))(response)
    End Function

    Public Async Function CreateMahasiswaAsync(request As MahasiswaRequest) As Task(Of MahasiswaModel)
        Dim response = Await _client.PostAsync("api/mahasiswa", ToJsonContent(request))
        Return Await ReadDataAsync(Of MahasiswaModel)(response)
    End Function

    Public Async Function UpdateMahasiswaAsync(id As Long, request As MahasiswaRequest) As Task(Of MahasiswaModel)
        Dim response = Await _client.PutAsync($"api/mahasiswa/{id}", ToJsonContent(request))
        Return Await ReadDataAsync(Of MahasiswaModel)(response)
    End Function

    Public Async Function DeleteMahasiswaAsync(id As Long) As Task
        Dim response = Await _client.DeleteAsync($"api/mahasiswa/{id}")
        Await EnsureSuccessAsync(response)
    End Function

    Public Async Function DownloadExportAsync(resource As String, format As String) As Task(Of Byte())
        Dim response = Await _client.GetAsync($"api/export/{Uri.EscapeDataString(resource)}/{Uri.EscapeDataString(format)}")
        Await EnsureSuccessAsync(response)
        Return Await response.Content.ReadAsByteArrayAsync()
    End Function

    Private Shared Function ToJsonContent(value As Object) As StringContent
        Dim json = JsonSerializer.Serialize(value, JsonOptions)
        Return New StringContent(json, Encoding.UTF8, "application/json")
    End Function

    Private Shared Async Function ReadDataAsync(Of T)(response As HttpResponseMessage) As Task(Of T)
        Dim content = Await response.Content.ReadAsStringAsync()
        If Not response.IsSuccessStatusCode Then
            Throw New ApplicationException(ParseErrorMessage(content, response.StatusCode))
        End If

        Dim apiResponse = JsonSerializer.Deserialize(Of ApiResponse(Of T))(content, JsonOptions)
        If apiResponse Is Nothing Then
            Throw New ApplicationException("Response API tidak valid.")
        End If
        Return apiResponse.Data
    End Function

    Private Shared Async Function EnsureSuccessAsync(response As HttpResponseMessage) As Task
        If response.IsSuccessStatusCode Then
            Return
        End If

        Dim content = Await response.Content.ReadAsStringAsync()
        Throw New ApplicationException(ParseErrorMessage(content, response.StatusCode))
    End Function

    Private Shared Function ParseErrorMessage(content As String, statusCode As HttpStatusCode) As String
        Try
            Dim errorResponse = JsonSerializer.Deserialize(Of ErrorResponse)(content, JsonOptions)
            If errorResponse IsNot Nothing AndAlso errorResponse.Errors IsNot Nothing AndAlso errorResponse.Errors.Count > 0 Then
                Return String.Join(Environment.NewLine, errorResponse.Errors.Select(Function(item) $"{item.Key}: {item.Value}"))
            End If

            If errorResponse IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(errorResponse.Message) Then
                Return errorResponse.Message
            End If
        Catch
        End Try

        Return $"Request gagal. Status: {CInt(statusCode)}"
    End Function

    Private Shared Function NormalizeBaseUrl(baseUrl As String) As String
        If String.IsNullOrWhiteSpace(baseUrl) Then
            Return "http://localhost:8080/"
        End If

        Dim normalized = baseUrl.Trim()
        If Not normalized.EndsWith("/") Then
            normalized &= "/"
        End If
        Return normalized
    End Function
End Class
