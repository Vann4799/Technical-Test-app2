Imports System.ComponentModel
Imports System.Drawing
Imports System.Text.RegularExpressions
Imports System.Windows.Forms

Public Class MainForm
    Inherits Form

    Private txtApiUrl As TextBox
    Private lblStatus As ToolStripStatusLabel

    Private txtMahasiswaSearch As TextBox
    Private gridMahasiswa As DataGridView
    Private txtNim As TextBox
    Private txtNama As TextBox
    Private txtEmail As TextBox
    Private cmbMahasiswaJurusan As ComboBox
    Private currentMahasiswaId As Long?

    Private txtJurusanSearch As TextBox
    Private gridJurusan As DataGridView
    Private txtNamaJurusan As TextBox
    Private currentJurusanId As Long?

    Public Sub New()
        InitializeUi()
        AddHandler Shown, Async Sub(sender, e) Await LoadInitialDataAsync()
    End Sub

    Private Sub InitializeUi()
        Text = "Sistem Manajemen Data Mahasiswa"
        StartPosition = FormStartPosition.CenterScreen
        MinimumSize = New Size(1100, 720)
        Size = New Size(1220, 780)
        Font = New Font("Segoe UI", 10)

        Dim root = New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .RowCount = 3,
            .Padding = New Padding(12)
        }
        root.RowStyles.Add(New RowStyle(SizeType.Absolute, 54))
        root.RowStyles.Add(New RowStyle(SizeType.Percent, 100))
        root.RowStyles.Add(New RowStyle(SizeType.Absolute, 28))

        Dim apiPanel = New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 3}
        apiPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 90))
        apiPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
        apiPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 160))
        apiPanel.Controls.Add(New Label With {.Text = "Base API", .Dock = DockStyle.Fill, .TextAlign = ContentAlignment.MiddleLeft}, 0, 0)

        txtApiUrl = New TextBox With {.Dock = DockStyle.Fill, .Text = "http://localhost:8080", .Margin = New Padding(0, 8, 10, 8)}
        apiPanel.Controls.Add(txtApiUrl, 1, 0)

        Dim btnReloadAll = New Button With {.Text = "Reload", .Dock = DockStyle.Fill, .Margin = New Padding(0, 8, 0, 8)}
        AddHandler btnReloadAll.Click, Async Sub(sender, e) Await LoadInitialDataAsync()
        apiPanel.Controls.Add(btnReloadAll, 2, 0)

        Dim tabs = New TabControl With {.Dock = DockStyle.Fill}
        tabs.TabPages.Add(BuildMahasiswaTab())
        tabs.TabPages.Add(BuildJurusanTab())

        Dim status = New StatusStrip()
        lblStatus = New ToolStripStatusLabel With {.Text = "Ready"}
        status.Items.Add(lblStatus)

        root.Controls.Add(apiPanel, 0, 0)
        root.Controls.Add(tabs, 0, 1)
        root.Controls.Add(status, 0, 2)
        Controls.Add(root)
    End Sub

    Private Function BuildMahasiswaTab() As TabPage
        Dim tab = New TabPage("Mahasiswa")
        Dim layout = New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2, .Padding = New Padding(10)}
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 64))
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 36))

        Dim left = New TableLayoutPanel With {.Dock = DockStyle.Fill, .RowCount = 2}
        left.RowStyles.Add(New RowStyle(SizeType.Absolute, 48))
        left.RowStyles.Add(New RowStyle(SizeType.Percent, 100))

        Dim searchPanel = New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 3}
        searchPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
        searchPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 110))
        searchPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 110))
        txtMahasiswaSearch = New TextBox With {.Dock = DockStyle.Fill, .PlaceholderText = "Search nama atau NIM", .Margin = New Padding(0, 6, 8, 6)}
        Dim btnSearch = New Button With {.Text = "Search", .Dock = DockStyle.Fill, .Margin = New Padding(0, 6, 8, 6)}
        Dim btnRefresh = New Button With {.Text = "Refresh", .Dock = DockStyle.Fill, .Margin = New Padding(0, 6, 0, 6)}
        AddHandler btnSearch.Click, Async Sub(sender, e) Await LoadMahasiswaAsync()
        AddHandler btnRefresh.Click, Async Sub(sender, e)
                                         txtMahasiswaSearch.Clear()
                                         Await LoadMahasiswaAsync()
                                     End Sub
        AddHandler txtMahasiswaSearch.KeyDown, Async Sub(sender, e)
                                                   If e.KeyCode = Keys.Enter Then
                                                       e.SuppressKeyPress = True
                                                       Await LoadMahasiswaAsync()
                                                   End If
                                               End Sub
        searchPanel.Controls.Add(txtMahasiswaSearch, 0, 0)
        searchPanel.Controls.Add(btnSearch, 1, 0)
        searchPanel.Controls.Add(btnRefresh, 2, 0)

        gridMahasiswa = New DataGridView With {.Dock = DockStyle.Fill, .AutoGenerateColumns = False, .ReadOnly = True, .SelectionMode = DataGridViewSelectionMode.FullRowSelect, .MultiSelect = False, .AllowUserToAddRows = False}
        gridMahasiswa.Columns.Add(TextColumn("NIM", "Nim", 110))
        gridMahasiswa.Columns.Add(TextColumn("Nama", "Nama", 190))
        gridMahasiswa.Columns.Add(TextColumn("Email", "Email", 210))
        gridMahasiswa.Columns.Add(TextColumn("Jurusan", "NamaJurusan", 180))
        AddHandler gridMahasiswa.SelectionChanged, AddressOf MahasiswaSelectionChanged

        left.Controls.Add(searchPanel, 0, 0)
        left.Controls.Add(gridMahasiswa, 0, 1)

        Dim formPanel = New TableLayoutPanel With {.Dock = DockStyle.Top, .AutoSize = True, .ColumnCount = 1, .Padding = New Padding(16)}
        formPanel.Controls.Add(HeaderLabel("Form Mahasiswa"))
        txtNim = InputBox("NIM")
        txtNama = InputBox("Nama")
        txtEmail = InputBox("Email")
        cmbMahasiswaJurusan = New ComboBox With {.Dock = DockStyle.Top, .DropDownStyle = ComboBoxStyle.DropDownList, .Height = 32}

        formPanel.Controls.Add(LabeledControl("NIM", txtNim))
        formPanel.Controls.Add(LabeledControl("Nama", txtNama))
        formPanel.Controls.Add(LabeledControl("Email", txtEmail))
        formPanel.Controls.Add(LabeledControl("Jurusan", cmbMahasiswaJurusan))
        formPanel.Controls.Add(MahasiswaButtons())

        layout.Controls.Add(left, 0, 0)
        layout.Controls.Add(formPanel, 1, 0)
        tab.Controls.Add(layout)
        Return tab
    End Function

    Private Function BuildJurusanTab() As TabPage
        Dim tab = New TabPage("Jurusan")
        Dim layout = New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2, .Padding = New Padding(10)}
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 64))
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 36))

        Dim left = New TableLayoutPanel With {.Dock = DockStyle.Fill, .RowCount = 2}
        left.RowStyles.Add(New RowStyle(SizeType.Absolute, 48))
        left.RowStyles.Add(New RowStyle(SizeType.Percent, 100))

        Dim searchPanel = New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 3}
        searchPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
        searchPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 110))
        searchPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 110))
        txtJurusanSearch = New TextBox With {.Dock = DockStyle.Fill, .PlaceholderText = "Search jurusan", .Margin = New Padding(0, 6, 8, 6)}
        Dim btnSearch = New Button With {.Text = "Search", .Dock = DockStyle.Fill, .Margin = New Padding(0, 6, 8, 6)}
        Dim btnRefresh = New Button With {.Text = "Refresh", .Dock = DockStyle.Fill, .Margin = New Padding(0, 6, 0, 6)}
        AddHandler btnSearch.Click, Async Sub(sender, e) Await LoadJurusanAsync()
        AddHandler btnRefresh.Click, Async Sub(sender, e)
                                         txtJurusanSearch.Clear()
                                         Await LoadJurusanAsync()
                                     End Sub
        searchPanel.Controls.Add(txtJurusanSearch, 0, 0)
        searchPanel.Controls.Add(btnSearch, 1, 0)
        searchPanel.Controls.Add(btnRefresh, 2, 0)

        gridJurusan = New DataGridView With {.Dock = DockStyle.Fill, .AutoGenerateColumns = False, .ReadOnly = True, .SelectionMode = DataGridViewSelectionMode.FullRowSelect, .MultiSelect = False, .AllowUserToAddRows = False}
        gridJurusan.Columns.Add(TextColumn("ID", "Id", 80))
        gridJurusan.Columns.Add(TextColumn("Nama Jurusan", "NamaJurusan", 320))
        AddHandler gridJurusan.SelectionChanged, AddressOf JurusanSelectionChanged

        left.Controls.Add(searchPanel, 0, 0)
        left.Controls.Add(gridJurusan, 0, 1)

        Dim formPanel = New TableLayoutPanel With {.Dock = DockStyle.Top, .AutoSize = True, .ColumnCount = 1, .Padding = New Padding(16)}
        formPanel.Controls.Add(HeaderLabel("Form Jurusan"))
        txtNamaJurusan = InputBox("Nama Jurusan")
        formPanel.Controls.Add(LabeledControl("Nama Jurusan", txtNamaJurusan))
        formPanel.Controls.Add(JurusanButtons())

        layout.Controls.Add(left, 0, 0)
        layout.Controls.Add(formPanel, 1, 0)
        tab.Controls.Add(layout)
        Return tab
    End Function

    Private Function MahasiswaButtons() As Control
        Dim panel = New FlowLayoutPanel With {.Dock = DockStyle.Top, .AutoSize = True, .Padding = New Padding(0, 14, 0, 0)}
        Dim btnNew = ActionButton("New")
        Dim btnSave = ActionButton("Save")
        Dim btnUpdate = ActionButton("Update")
        Dim btnDelete = ActionButton("Delete")
        AddHandler btnNew.Click, Sub(sender, e) ResetMahasiswaForm()
        AddHandler btnSave.Click, Async Sub(sender, e) Await SaveMahasiswaAsync()
        AddHandler btnUpdate.Click, Async Sub(sender, e) Await UpdateMahasiswaAsync()
        AddHandler btnDelete.Click, Async Sub(sender, e) Await DeleteMahasiswaAsync()
        panel.Controls.AddRange({btnNew, btnSave, btnUpdate, btnDelete})
        Return panel
    End Function

    Private Function JurusanButtons() As Control
        Dim panel = New FlowLayoutPanel With {.Dock = DockStyle.Top, .AutoSize = True, .Padding = New Padding(0, 14, 0, 0)}
        Dim btnNew = ActionButton("New")
        Dim btnSave = ActionButton("Save")
        Dim btnUpdate = ActionButton("Update")
        Dim btnDelete = ActionButton("Delete")
        AddHandler btnNew.Click, Sub(sender, e) ResetJurusanForm()
        AddHandler btnSave.Click, Async Sub(sender, e) Await SaveJurusanAsync()
        AddHandler btnUpdate.Click, Async Sub(sender, e) Await UpdateJurusanAsync()
        AddHandler btnDelete.Click, Async Sub(sender, e) Await DeleteJurusanAsync()
        panel.Controls.AddRange({btnNew, btnSave, btnUpdate, btnDelete})
        Return panel
    End Function

    Private Async Function LoadInitialDataAsync() As Task
        Await RunSafelyAsync(Async Function()
                                 Await LoadJurusanAsync()
                                 Await LoadMahasiswaAsync()
                             End Function)
    End Function

    Private Async Function LoadJurusanAsync() As Task
        Await RunSafelyAsync(Async Function()
                                 Dim data = Await Service().GetJurusanAsync(txtJurusanSearch.Text)
                                 gridJurusan.DataSource = New BindingList(Of JurusanModel)(data)
                                 cmbMahasiswaJurusan.DisplayMember = "NamaJurusan"
                                 cmbMahasiswaJurusan.ValueMember = "Id"
                                 cmbMahasiswaJurusan.DataSource = New BindingList(Of JurusanModel)(data.ToList())
                                 SetStatus($"Loaded {data.Count} jurusan.")
                             End Function)
    End Function

    Private Async Function LoadMahasiswaAsync() As Task
        Await RunSafelyAsync(Async Function()
                                 Dim data = Await Service().GetMahasiswaAsync(txtMahasiswaSearch.Text)
                                 gridMahasiswa.DataSource = New BindingList(Of MahasiswaModel)(data)
                                 SetStatus($"Loaded {data.Count} mahasiswa.")
                             End Function)
    End Function

    Private Async Function SaveJurusanAsync() As Task
        If String.IsNullOrWhiteSpace(txtNamaJurusan.Text) Then
            MessageBox.Show("Nama jurusan wajib diisi.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Await RunSafelyAsync(Async Function()
                                 Await Service().CreateJurusanAsync(New JurusanRequest With {.NamaJurusan = txtNamaJurusan.Text.Trim()})
                                 ResetJurusanForm()
                                 Await LoadJurusanAsync()
                             End Function)
    End Function

    Private Async Function UpdateJurusanAsync() As Task
        If Not currentJurusanId.HasValue Then
            MessageBox.Show("Pilih data jurusan terlebih dahulu.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Await RunSafelyAsync(Async Function()
                                 Await Service().UpdateJurusanAsync(currentJurusanId.Value, New JurusanRequest With {.NamaJurusan = txtNamaJurusan.Text.Trim()})
                                 ResetJurusanForm()
                                 Await LoadJurusanAsync()
                             End Function)
    End Function

    Private Async Function DeleteJurusanAsync() As Task
        If Not currentJurusanId.HasValue Then
            MessageBox.Show("Pilih data jurusan terlebih dahulu.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If MessageBox.Show("Hapus jurusan terpilih?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Return
        End If

        Await RunSafelyAsync(Async Function()
                                 Await Service().DeleteJurusanAsync(currentJurusanId.Value)
                                 ResetJurusanForm()
                                 Await LoadJurusanAsync()
                                 Await LoadMahasiswaAsync()
                             End Function)
    End Function

    Private Async Function SaveMahasiswaAsync() As Task
        Dim request = BuildMahasiswaRequest()
        If request Is Nothing Then Return

        Await RunSafelyAsync(Async Function()
                                 Await Service().CreateMahasiswaAsync(request)
                                 ResetMahasiswaForm()
                                 Await LoadMahasiswaAsync()
                             End Function)
    End Function

    Private Async Function UpdateMahasiswaAsync() As Task
        If Not currentMahasiswaId.HasValue Then
            MessageBox.Show("Pilih data mahasiswa terlebih dahulu.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim request = BuildMahasiswaRequest()
        If request Is Nothing Then Return

        Await RunSafelyAsync(Async Function()
                                 Await Service().UpdateMahasiswaAsync(currentMahasiswaId.Value, request)
                                 ResetMahasiswaForm()
                                 Await LoadMahasiswaAsync()
                             End Function)
    End Function

    Private Async Function DeleteMahasiswaAsync() As Task
        If Not currentMahasiswaId.HasValue Then
            MessageBox.Show("Pilih data mahasiswa terlebih dahulu.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If MessageBox.Show("Hapus mahasiswa terpilih?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Return
        End If

        Await RunSafelyAsync(Async Function()
                                 Await Service().DeleteMahasiswaAsync(currentMahasiswaId.Value)
                                 ResetMahasiswaForm()
                                 Await LoadMahasiswaAsync()
                             End Function)
    End Function

    Private Function BuildMahasiswaRequest() As MahasiswaRequest
        If String.IsNullOrWhiteSpace(txtNim.Text) OrElse String.IsNullOrWhiteSpace(txtNama.Text) Then
            MessageBox.Show("NIM dan nama wajib diisi.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return Nothing
        End If

        If cmbMahasiswaJurusan.SelectedValue Is Nothing Then
            MessageBox.Show("Jurusan wajib dipilih.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return Nothing
        End If

        If Not String.IsNullOrWhiteSpace(txtEmail.Text) AndAlso Not Regex.IsMatch(txtEmail.Text.Trim(), "^[^@\s]+@[^@\s]+\.[^@\s]+$") Then
            MessageBox.Show("Format email tidak valid.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return Nothing
        End If

        Return New MahasiswaRequest With {
            .Nim = txtNim.Text.Trim(),
            .Nama = txtNama.Text.Trim(),
            .Email = txtEmail.Text.Trim(),
            .JurusanId = Convert.ToInt64(cmbMahasiswaJurusan.SelectedValue)
        }
    End Function

    Private Sub MahasiswaSelectionChanged(sender As Object, e As EventArgs)
        Dim item = TryCast(CurrentGridItem(gridMahasiswa), MahasiswaModel)
        If item Is Nothing Then Return

        currentMahasiswaId = item.Id
        txtNim.Text = item.Nim
        txtNama.Text = item.Nama
        txtEmail.Text = item.Email
        If item.Jurusan IsNot Nothing Then
            cmbMahasiswaJurusan.SelectedValue = item.Jurusan.Id
        End If
    End Sub

    Private Sub JurusanSelectionChanged(sender As Object, e As EventArgs)
        Dim item = TryCast(CurrentGridItem(gridJurusan), JurusanModel)
        If item Is Nothing Then Return

        currentJurusanId = item.Id
        txtNamaJurusan.Text = item.NamaJurusan
    End Sub

    Private Sub ResetMahasiswaForm()
        currentMahasiswaId = Nothing
        txtNim.Clear()
        txtNama.Clear()
        txtEmail.Clear()
        If cmbMahasiswaJurusan.Items.Count > 0 Then cmbMahasiswaJurusan.SelectedIndex = 0
        gridMahasiswa.ClearSelection()
    End Sub

    Private Sub ResetJurusanForm()
        currentJurusanId = Nothing
        txtNamaJurusan.Clear()
        gridJurusan.ClearSelection()
    End Sub

    Private Async Function RunSafelyAsync(action As Func(Of Task)) As Task
        Try
            UseWaitCursor = True
            SetStatus("Loading...")
            Await action()
        Catch ex As Exception
            SetStatus("Error")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            UseWaitCursor = False
        End Try
    End Function

    Private Function Service() As ApiService
        Return New ApiService(txtApiUrl.Text)
    End Function

    Private Sub SetStatus(message As String)
        lblStatus.Text = message
    End Sub

    Private Shared Function CurrentGridItem(grid As DataGridView) As Object
        If grid.CurrentRow Is Nothing Then Return Nothing
        Return grid.CurrentRow.DataBoundItem
    End Function

    Private Shared Function TextColumn(header As String, propertyName As String, width As Integer) As DataGridViewTextBoxColumn
        Return New DataGridViewTextBoxColumn With {
            .HeaderText = header,
            .DataPropertyName = propertyName,
            .Width = width,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        }
    End Function

    Private Shared Function InputBox(placeholder As String) As TextBox
        Return New TextBox With {.Dock = DockStyle.Top, .PlaceholderText = placeholder, .Height = 32}
    End Function

    Private Shared Function HeaderLabel(text As String) As Label
        Return New Label With {.Text = text, .Dock = DockStyle.Top, .Height = 42, .Font = New Font("Segoe UI", 14, FontStyle.Bold)}
    End Function

    Private Shared Function LabeledControl(labelText As String, control As Control) As Control
        Dim panel = New TableLayoutPanel With {.Dock = DockStyle.Top, .AutoSize = True, .ColumnCount = 1, .Padding = New Padding(0, 0, 0, 12)}
        panel.Controls.Add(New Label With {.Text = labelText, .Dock = DockStyle.Top, .Height = 24})
        panel.Controls.Add(control)
        Return panel
    End Function

    Private Shared Function ActionButton(text As String) As Button
        Return New Button With {.Text = text, .Width = 86, .Height = 34, .Margin = New Padding(0, 0, 8, 8)}
    End Function
End Class
