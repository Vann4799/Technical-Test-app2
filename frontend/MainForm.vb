Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Text
Imports System.Windows.Forms

Public Class MainForm
    Inherits Form

    Private Shared ReadOnly SilverBackground As Color = Color.FromArgb(244, 246, 248)
    Private Shared ReadOnly Surface As Color = Color.FromArgb(255, 255, 255)
    Private Shared ReadOnly SurfaceMuted As Color = Color.FromArgb(250, 251, 252)
    Private Shared ReadOnly ReadOnlySurface As Color = Color.FromArgb(246, 248, 250)
    Private Shared ReadOnly ReadOnlyBorder As Color = Color.FromArgb(205, 213, 223)
    Private Shared ReadOnly BorderSoft As Color = Color.FromArgb(220, 225, 230)
    Private Shared ReadOnly TextStrong As Color = Color.FromArgb(30, 41, 59)
    Private Shared ReadOnly TextMuted As Color = Color.FromArgb(100, 116, 139)
    Private Shared ReadOnly Accent As Color = Color.FromArgb(73, 101, 133)
    Private Shared ReadOnly AccentDark As Color = Color.FromArgb(53, 75, 101)
    Private Shared ReadOnly Danger As Color = Color.FromArgb(176, 64, 72)
    Private Shared ReadOnly NeutralButton As Color = Color.FromArgb(232, 236, 240)
    Private Shared ReadOnly AppFonts As PrivateFontCollection = LoadAppFonts()
    Private Shared ReadOnly UiFontFamily As FontFamily = ResolveUiFontFamily()

    Private txtApiUrl As TextBox
    Private lblStatus As ToolStripStatusLabel

    Private txtMahasiswaSearch As TextBox
    Private gridMahasiswa As DataGridView
    Private lblMahasiswaCount As Label
    Private txtNim As TextBox
    Private txtNama As TextBox
    Private txtUmur As TextBox
    Private dtpTanggalLahir As DateTimePicker
    Private txtAlamat As TextBox
    Private cmbMahasiswaJurusan As ComboBox
    Private txtMahasiswaFakultas As TextBox
    Private txtMahasiswaJenjang As TextBox
    Private currentMahasiswaId As Long?
    Private isResettingMahasiswaForm As Boolean

    Private txtJurusanSearch As TextBox
    Private gridJurusan As DataGridView
    Private lblJurusanCount As Label
    Private txtNamaJurusan As TextBox
    Private txtFakultas As TextBox
    Private txtJenjang As TextBox
    Private currentJurusanId As Long?
    Private isResettingJurusanForm As Boolean

    Public Sub New()
        InitializeUi()
        AddHandler Shown, Async Sub(sender, e) Await LoadInitialDataAsync()
    End Sub

    Private Sub InitializeUi()
        Text = "Sistem Manajemen Data Mahasiswa"
        StartPosition = FormStartPosition.CenterScreen
        MinimumSize = New Size(1180, 740)
        Size = New Size(1320, 820)
        Font = UiFont(10)
        BackColor = SilverBackground

        Dim root = New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .RowCount = 3,
            .Padding = New Padding(20),
            .BackColor = SilverBackground
        }
        root.RowStyles.Add(New RowStyle(SizeType.Absolute, 86))
        root.RowStyles.Add(New RowStyle(SizeType.Percent, 100))
        root.RowStyles.Add(New RowStyle(SizeType.Absolute, 28))

        Dim apiPanel = New Panel With {.Dock = DockStyle.Fill, .Padding = New Padding(24, 14, 16, 14), .BackColor = Surface}
        Dim headerLayout = New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 4, .BackColor = Surface}
        headerLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 320))
        headerLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 92))
        headerLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
        headerLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 126))

        headerLayout.Controls.Add(New Label With {.Text = "Student Management", .Dock = DockStyle.Fill, .TextAlign = ContentAlignment.MiddleLeft, .Font = UiFont(15, FontStyle.Bold), .ForeColor = TextStrong, .Margin = New Padding(0)}, 0, 0)
        headerLayout.Controls.Add(New Label With {.Text = "Base API", .Dock = DockStyle.Fill, .TextAlign = ContentAlignment.MiddleRight, .ForeColor = TextMuted, .Margin = New Padding(0, 0, 14, 0), .AutoEllipsis = False}, 1, 0)

        txtApiUrl = New TextBox With {.Dock = DockStyle.Fill, .Text = "http://localhost:8081"}
        StyleTextBox(txtApiUrl)
        Dim apiUrlShell = InputShell(txtApiUrl)
        apiUrlShell.Margin = New Padding(0, 12, 12, 12)
        headerLayout.Controls.Add(apiUrlShell, 2, 0)
        AddHandler txtApiUrl.TextChanged, AddressOf BaseApiUrlChanged

        Dim btnReloadAll = ActionButton("Reload", ButtonKind.Primary)
        btnReloadAll.Dock = DockStyle.Fill
        btnReloadAll.Margin = New Padding(0, 10, 0, 10)
        AddHandler btnReloadAll.Click, Async Sub(sender, e) Await LoadInitialDataAsync()
        headerLayout.Controls.Add(btnReloadAll, 3, 0)
        apiPanel.Controls.Add(headerLayout)

        Dim tabs = New TabControl With {.Dock = DockStyle.Fill, .Appearance = TabAppearance.Normal, .Padding = New Point(22, 10)}
        tabs.Font = UiFont(10, FontStyle.Bold)
        tabs.Multiline = False
        tabs.TabPages.Add(BuildMahasiswaTab())
        tabs.TabPages.Add(BuildJurusanTab())
        tabs.Margin = New Padding(0, 12, 0, 0)

        Dim status = New StatusStrip With {.BackColor = SilverBackground, .SizingGrip = False}
        lblStatus = New ToolStripStatusLabel With {.Text = "Ready"}
        lblStatus.ForeColor = TextMuted
        status.Items.Add(lblStatus)

        root.Controls.Add(apiPanel, 0, 0)
        root.Controls.Add(tabs, 0, 1)
        root.Controls.Add(status, 0, 2)
        Controls.Add(root)
    End Sub

    Private Function BuildMahasiswaTab() As TabPage
        Dim tab = New TabPage("Mahasiswa") With {.BackColor = SilverBackground}
        Dim layout = New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2, .Padding = New Padding(14, 16, 14, 14), .BackColor = SilverBackground}
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 68))
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 32))

        Dim left = CardPanel()
        left.RowCount = 3
        left.RowStyles.Add(New RowStyle(SizeType.Absolute, 58))
        left.RowStyles.Add(New RowStyle(SizeType.Absolute, 72))
        left.RowStyles.Add(New RowStyle(SizeType.Percent, 100))
        lblMahasiswaCount = CountBadge("0 data")
        left.Controls.Add(SectionHeader("Data Mahasiswa", lblMahasiswaCount), 0, 0)

        Dim searchPanel = New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 4, .Padding = New Padding(16, 8, 16, 8), .BackColor = Surface}
        searchPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
        searchPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 112))
        searchPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 112))
        searchPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 112))
        txtMahasiswaSearch = New TextBox With {.Dock = DockStyle.Fill, .PlaceholderText = "Search nama atau NIM"}
        StyleTextBox(txtMahasiswaSearch)
        Dim mahasiswaSearchShell = InputShell(txtMahasiswaSearch)
        mahasiswaSearchShell.Margin = New Padding(0, 4, 10, 4)
        Dim btnSearch = ActionButton("Search", ButtonKind.Primary)
        btnSearch.Dock = DockStyle.Fill
        btnSearch.Margin = New Padding(0, 4, 10, 4)
        Dim btnRefresh = ActionButton("Refresh", ButtonKind.Neutral)
        btnRefresh.Dock = DockStyle.Fill
        btnRefresh.Margin = New Padding(0, 4, 10, 4)
        Dim btnExport = ActionButton("Export", ButtonKind.Neutral)
        btnExport.Dock = DockStyle.Fill
        btnExport.Margin = New Padding(0, 4, 0, 4)
        AttachExportMenu(btnExport, "mahasiswa")
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
        searchPanel.Controls.Add(mahasiswaSearchShell, 0, 0)
        searchPanel.Controls.Add(btnSearch, 1, 0)
        searchPanel.Controls.Add(btnRefresh, 2, 0)
        searchPanel.Controls.Add(btnExport, 3, 0)

        gridMahasiswa = New DataGridView With {.Dock = DockStyle.Fill, .AutoGenerateColumns = False, .ReadOnly = True, .SelectionMode = DataGridViewSelectionMode.FullRowSelect, .MultiSelect = False, .AllowUserToAddRows = False}
        gridMahasiswa.Columns.Add(TextColumn("NIM", "Nim", 110))
        gridMahasiswa.Columns.Add(TextColumn("Nama", "Nama", 170))
        gridMahasiswa.Columns.Add(TextColumn("Umur", "Umur", 80))
        gridMahasiswa.Columns.Add(TextColumn("Tanggal Lahir", "TanggalLahirDisplay", 130))
        gridMahasiswa.Columns.Add(TextColumn("Alamat", "Alamat", 220))
        gridMahasiswa.Columns.Add(TextColumn("Jurusan", "NamaJurusan", 170))
        StyleGrid(gridMahasiswa)
        AddHandler gridMahasiswa.SelectionChanged, AddressOf MahasiswaSelectionChanged

        left.Controls.Add(searchPanel, 0, 1)
        left.Controls.Add(gridMahasiswa, 0, 2)

        Dim formPanel = FormStackPanel()
        formPanel.Controls.Add(HeaderLabel("Form Mahasiswa"))
        txtNama = InputBox("Nama")
        txtUmur = InputBox("Umur")
        txtNim = InputBox("NIM")
        dtpTanggalLahir = New DateTimePicker With {.Dock = DockStyle.Fill, .Height = 34, .Format = DateTimePickerFormat.Custom, .CustomFormat = "yyyy-MM-dd", .Font = UiFont(10), .CalendarForeColor = TextStrong}
        txtAlamat = InputBox("Alamat")
        cmbMahasiswaJurusan = New ComboBox With {.Dock = DockStyle.Fill, .DropDownStyle = ComboBoxStyle.DropDownList, .Height = 34, .FlatStyle = FlatStyle.Flat, .Font = UiFont(10), .BackColor = Surface, .ForeColor = TextStrong}
        txtMahasiswaFakultas = InputBox("Fakultas")
        StyleReadOnlyTextBox(txtMahasiswaFakultas)
        txtMahasiswaJenjang = InputBox("Jenjang")
        StyleReadOnlyTextBox(txtMahasiswaJenjang)
        AddHandler cmbMahasiswaJurusan.SelectedIndexChanged, AddressOf MahasiswaJurusanChanged

        formPanel.Controls.Add(LabeledControl("Nama", txtNama))
        formPanel.Controls.Add(LabeledControl("Umur", txtUmur))
        formPanel.Controls.Add(LabeledControl("NIM", txtNim))
        formPanel.Controls.Add(LabeledControl("Tanggal Lahir", dtpTanggalLahir))
        formPanel.Controls.Add(LabeledControl("Alamat", txtAlamat))
        formPanel.Controls.Add(LabeledControl("Jurusan", cmbMahasiswaJurusan))
        formPanel.Controls.Add(LabeledControl("Fakultas", txtMahasiswaFakultas))
        formPanel.Controls.Add(LabeledControl("Jenjang", txtMahasiswaJenjang))
        formPanel.Controls.Add(MahasiswaButtons())
        ResizeFormStack(formPanel)
        Dim formShell = CreateFormShell(formPanel)
        formShell.Margin = New Padding(18, 0, 0, 0)

        layout.Controls.Add(left, 0, 0)
        layout.Controls.Add(formShell, 1, 0)
        tab.Controls.Add(layout)
        Return tab
    End Function

    Private Function BuildJurusanTab() As TabPage
        Dim tab = New TabPage("Jurusan") With {.BackColor = SilverBackground}
        Dim layout = New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2, .Padding = New Padding(14, 16, 14, 14), .BackColor = SilverBackground}
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 68))
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 32))

        Dim left = CardPanel()
        left.RowCount = 3
        left.RowStyles.Add(New RowStyle(SizeType.Absolute, 58))
        left.RowStyles.Add(New RowStyle(SizeType.Absolute, 72))
        left.RowStyles.Add(New RowStyle(SizeType.Percent, 100))
        lblJurusanCount = CountBadge("0 data")
        left.Controls.Add(SectionHeader("Data Jurusan", lblJurusanCount), 0, 0)

        Dim searchPanel = New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 4, .Padding = New Padding(16, 8, 16, 8), .BackColor = Surface}
        searchPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
        searchPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 112))
        searchPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 112))
        searchPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 112))
        txtJurusanSearch = New TextBox With {.Dock = DockStyle.Fill, .PlaceholderText = "Search jurusan"}
        StyleTextBox(txtJurusanSearch)
        Dim jurusanSearchShell = InputShell(txtJurusanSearch)
        jurusanSearchShell.Margin = New Padding(0, 4, 10, 4)
        Dim btnSearch = ActionButton("Search", ButtonKind.Primary)
        btnSearch.Dock = DockStyle.Fill
        btnSearch.Margin = New Padding(0, 4, 10, 4)
        Dim btnRefresh = ActionButton("Refresh", ButtonKind.Neutral)
        btnRefresh.Dock = DockStyle.Fill
        btnRefresh.Margin = New Padding(0, 4, 10, 4)
        Dim btnExport = ActionButton("Export", ButtonKind.Neutral)
        btnExport.Dock = DockStyle.Fill
        btnExport.Margin = New Padding(0, 4, 0, 4)
        AttachExportMenu(btnExport, "jurusan")
        AddHandler btnSearch.Click, Async Sub(sender, e) Await LoadJurusanAsync()
        AddHandler btnRefresh.Click, Async Sub(sender, e)
                                         txtJurusanSearch.Clear()
                                         Await LoadJurusanAsync()
                                     End Sub
        AddHandler txtJurusanSearch.KeyDown, Async Sub(sender, e)
                                                If e.KeyCode = Keys.Enter Then
                                                    e.SuppressKeyPress = True
                                                    Await LoadJurusanAsync()
                                                End If
                                            End Sub
        searchPanel.Controls.Add(jurusanSearchShell, 0, 0)
        searchPanel.Controls.Add(btnSearch, 1, 0)
        searchPanel.Controls.Add(btnRefresh, 2, 0)
        searchPanel.Controls.Add(btnExport, 3, 0)

        gridJurusan = New DataGridView With {.Dock = DockStyle.Fill, .AutoGenerateColumns = False, .ReadOnly = True, .SelectionMode = DataGridViewSelectionMode.FullRowSelect, .MultiSelect = False, .AllowUserToAddRows = False}
        gridJurusan.Columns.Add(TextColumn("ID", "Id", 80))
        gridJurusan.Columns.Add(TextColumn("Nama Jurusan", "NamaJurusan", 240))
        gridJurusan.Columns.Add(TextColumn("Fakultas", "Fakultas", 220))
        gridJurusan.Columns.Add(TextColumn("Jenjang", "Jenjang", 140))
        StyleGrid(gridJurusan)
        AddHandler gridJurusan.SelectionChanged, AddressOf JurusanSelectionChanged

        left.Controls.Add(searchPanel, 0, 1)
        left.Controls.Add(gridJurusan, 0, 2)

        Dim formPanel = FormStackPanel()
        formPanel.Controls.Add(HeaderLabel("Form Jurusan"))
        txtNamaJurusan = InputBox("Nama Jurusan")
        txtFakultas = InputBox("Fakultas")
        txtJenjang = InputBox("Jenjang")
        formPanel.Controls.Add(LabeledControl("Nama Jurusan", txtNamaJurusan))
        formPanel.Controls.Add(LabeledControl("Fakultas", txtFakultas))
        formPanel.Controls.Add(LabeledControl("Jenjang", txtJenjang))
        formPanel.Controls.Add(JurusanButtons())
        ResizeFormStack(formPanel)
        Dim formShell = CreateFormShell(formPanel)
        formShell.Margin = New Padding(18, 0, 0, 0)

        layout.Controls.Add(left, 0, 0)
        layout.Controls.Add(formShell, 1, 0)
        tab.Controls.Add(layout)
        Return tab
    End Function

    Private Function MahasiswaButtons() As Control
        Dim panel = ButtonPanel()
        Dim btnNew = ActionButton("New", ButtonKind.Neutral)
        Dim btnSave = ActionButton("Save", ButtonKind.Primary)
        Dim btnUpdate = ActionButton("Update", ButtonKind.Neutral)
        Dim btnDelete = ActionButton("Delete", ButtonKind.Danger)
        AddHandler btnNew.Click, Sub(sender, e) ResetMahasiswaForm()
        AddHandler btnSave.Click, Async Sub(sender, e) Await SaveMahasiswaAsync()
        AddHandler btnUpdate.Click, Async Sub(sender, e) Await UpdateMahasiswaAsync()
        AddHandler btnDelete.Click, Async Sub(sender, e) Await DeleteMahasiswaAsync()
        panel.Controls.AddRange({btnNew, btnSave, btnUpdate, btnDelete})
        Return panel
    End Function

    Private Function JurusanButtons() As Control
        Dim panel = ButtonPanel()
        Dim btnNew = ActionButton("New", ButtonKind.Neutral)
        Dim btnSave = ActionButton("Save", ButtonKind.Primary)
        Dim btnUpdate = ActionButton("Update", ButtonKind.Neutral)
        Dim btnDelete = ActionButton("Delete", ButtonKind.Danger)
        AddHandler btnNew.Click, Sub(sender, e) ResetJurusanForm()
        AddHandler btnSave.Click, Async Sub(sender, e) Await SaveJurusanAsync()
        AddHandler btnUpdate.Click, Async Sub(sender, e) Await UpdateJurusanAsync()
        AddHandler btnDelete.Click, Async Sub(sender, e) Await DeleteJurusanAsync()
        panel.Controls.AddRange({btnNew, btnSave, btnUpdate, btnDelete})
        Return panel
    End Function

    Private Async Function LoadInitialDataAsync() As Task
        If Not HasValidBaseApiUrl(True) Then Return

        Await RunSafelyAsync(Async Function()
                                 Await LoadJurusanAsync()
                                 Await LoadMahasiswaAsync()
                             End Function)
    End Function

    Private Async Function LoadJurusanAsync() As Task
        If Not HasValidBaseApiUrl(True) Then Return

        Await RunSafelyAsync(Async Function()
                                 Dim data = Await Service().GetJurusanAsync(txtJurusanSearch.Text)
                                 gridJurusan.DataSource = New BindingList(Of JurusanModel)(data)
                                 lblJurusanCount.Text = $"{data.Count} data"
                                 cmbMahasiswaJurusan.DisplayMember = "NamaJurusan"
                                 cmbMahasiswaJurusan.ValueMember = "Id"
                                 cmbMahasiswaJurusan.DataSource = New BindingList(Of JurusanModel)(data.ToList())
                                 SetStatus($"Loaded {data.Count} jurusan.")
                             End Function)
    End Function

    Private Async Function LoadMahasiswaAsync() As Task
        If Not HasValidBaseApiUrl(True) Then Return

        Await RunSafelyAsync(Async Function()
                                 Dim data = Await Service().GetMahasiswaAsync(txtMahasiswaSearch.Text)
                                 gridMahasiswa.DataSource = New BindingList(Of MahasiswaModel)(data)
                                 lblMahasiswaCount.Text = $"{data.Count} data"
                                 SetStatus($"Loaded {data.Count} mahasiswa.")
                             End Function)
    End Function

    Private Async Function SaveJurusanAsync() As Task
        If String.IsNullOrWhiteSpace(txtNamaJurusan.Text) OrElse String.IsNullOrWhiteSpace(txtFakultas.Text) OrElse String.IsNullOrWhiteSpace(txtJenjang.Text) Then
            MessageBox.Show("Nama jurusan, fakultas, dan jenjang wajib diisi.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Await RunSafelyAsync(Async Function()
                                 Await Service().CreateJurusanAsync(New JurusanRequest With {.NamaJurusan = txtNamaJurusan.Text.Trim(), .Fakultas = txtFakultas.Text.Trim(), .Jenjang = txtJenjang.Text.Trim()})
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
                                 Await Service().UpdateJurusanAsync(currentJurusanId.Value, New JurusanRequest With {.NamaJurusan = txtNamaJurusan.Text.Trim(), .Fakultas = txtFakultas.Text.Trim(), .Jenjang = txtJenjang.Text.Trim()})
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

    Private Sub AttachExportMenu(button As Button, resource As String)
        Dim menu = New ContextMenuStrip With {.Font = UiFont(9.5F), .BackColor = Surface, .ForeColor = TextStrong}
        AddExportMenuItem(menu, resource, "Excel", "xlsx")
        AddExportMenuItem(menu, resource, "PDF", "pdf")
        AddExportMenuItem(menu, resource, "CSV", "csv")
        AddExportMenuItem(menu, resource, "JSON", "json")
        AddHandler button.Click, Sub(sender, e) menu.Show(button, New Point(0, button.Height))
    End Sub

    Private Sub AddExportMenuItem(menu As ContextMenuStrip, resource As String, label As String, format As String)
        Dim item = New ToolStripMenuItem(label)
        AddHandler item.Click, Async Sub(sender, e) Await ExportDataAsync(resource, format)
        menu.Items.Add(item)
    End Sub

    Private Async Function ExportDataAsync(resource As String, format As String) As Task
        Await RunSafelyAsync(Async Function()
                                 Dim fileBytes = Await Service().DownloadExportAsync(resource, format)
                                 Using dialog As New SaveFileDialog With {
                                     .FileName = $"export-{resource}.{format}",
                                     .Filter = ExportFilter(format),
                                     .OverwritePrompt = True
                                 }
                                     If dialog.ShowDialog(Me) <> DialogResult.OK Then
                                         SetStatus("Export dibatalkan.")
                                         Return
                                     End If

                                     IO.File.WriteAllBytes(dialog.FileName, fileBytes)
                                     SetStatus($"Export {resource} {format.ToUpperInvariant()} berhasil.")
                                 End Using
                             End Function)
    End Function

    Private Shared Function ExportFilter(format As String) As String
        Select Case format
            Case "xlsx"
                Return "Excel Workbook (*.xlsx)|*.xlsx"
            Case "pdf"
                Return "PDF Document (*.pdf)|*.pdf"
            Case "csv"
                Return "CSV File (*.csv)|*.csv"
            Case "json"
                Return "JSON File (*.json)|*.json"
            Case Else
                Return "All Files (*.*)|*.*"
        End Select
    End Function

    Private Function BuildMahasiswaRequest() As MahasiswaRequest
        If String.IsNullOrWhiteSpace(txtNama.Text) OrElse String.IsNullOrWhiteSpace(txtUmur.Text) OrElse String.IsNullOrWhiteSpace(txtNim.Text) OrElse String.IsNullOrWhiteSpace(txtAlamat.Text) Then
            MessageBox.Show("Nama, umur, NIM, dan alamat wajib diisi.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return Nothing
        End If

        Dim umur As Integer
        If Not Integer.TryParse(txtUmur.Text.Trim(), umur) OrElse umur <= 0 Then
            MessageBox.Show("Umur harus berupa angka lebih dari 0.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return Nothing
        End If

        If cmbMahasiswaJurusan.SelectedValue Is Nothing Then
            MessageBox.Show("Jurusan wajib dipilih.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return Nothing
        End If

        Return New MahasiswaRequest With {
            .Nim = txtNim.Text.Trim(),
            .Nama = txtNama.Text.Trim(),
            .Umur = umur,
            .TanggalLahir = dtpTanggalLahir.Value.ToString("yyyy-MM-dd"),
            .Alamat = txtAlamat.Text.Trim(),
            .JurusanId = Convert.ToInt64(cmbMahasiswaJurusan.SelectedValue)
        }
    End Function

    Private Sub MahasiswaSelectionChanged(sender As Object, e As EventArgs)
        If isResettingMahasiswaForm Then Return

        Dim item = TryCast(CurrentGridItem(gridMahasiswa), MahasiswaModel)
        If item Is Nothing Then Return

        currentMahasiswaId = item.Id
        txtNim.Text = item.Nim
        txtNama.Text = item.Nama
        txtUmur.Text = item.Umur.ToString()
        If item.TanggalLahir.HasValue Then
            dtpTanggalLahir.Value = item.TanggalLahir.Value
        End If
        txtAlamat.Text = item.Alamat
        If item.Jurusan IsNot Nothing Then
            cmbMahasiswaJurusan.SelectedValue = item.Jurusan.Id
        End If
    End Sub

    Private Sub JurusanSelectionChanged(sender As Object, e As EventArgs)
        If isResettingJurusanForm Then Return

        Dim item = TryCast(CurrentGridItem(gridJurusan), JurusanModel)
        If item Is Nothing Then Return

        currentJurusanId = item.Id
        txtNamaJurusan.Text = item.NamaJurusan
        txtFakultas.Text = item.Fakultas
        txtJenjang.Text = item.Jenjang
    End Sub

    Private Sub MahasiswaJurusanChanged(sender As Object, e As EventArgs)
        Dim selectedJurusan = TryCast(cmbMahasiswaJurusan.SelectedItem, JurusanModel)
        If selectedJurusan Is Nothing Then
            txtMahasiswaFakultas.Clear()
            txtMahasiswaJenjang.Clear()
            Return
        End If

        txtMahasiswaFakultas.Text = selectedJurusan.Fakultas
        txtMahasiswaJenjang.Text = selectedJurusan.Jenjang
    End Sub

    Private Sub ResetMahasiswaForm()
        Try
            isResettingMahasiswaForm = True
            currentMahasiswaId = Nothing
            gridMahasiswa.ClearSelection()
            txtNim.Clear()
            txtNama.Clear()
            txtUmur.Clear()
            dtpTanggalLahir.Value = Date.Today
            txtAlamat.Clear()
            cmbMahasiswaJurusan.SelectedIndex = -1
            txtMahasiswaFakultas.Clear()
            txtMahasiswaJenjang.Clear()
        Finally
            isResettingMahasiswaForm = False
        End Try
    End Sub

    Private Sub ResetJurusanForm()
        Try
            isResettingJurusanForm = True
            currentJurusanId = Nothing
            gridJurusan.ClearSelection()
            txtNamaJurusan.Clear()
            txtFakultas.Clear()
            txtJenjang.Clear()
        Finally
            isResettingJurusanForm = False
        End Try
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

    Private Sub BaseApiUrlChanged(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txtApiUrl.Text) Then
            ClearLoadedData()
            SetStatus("Base API kosong. Isi URL API lalu klik Reload.")
        End If
    End Sub

    Private Function HasValidBaseApiUrl(showMessage As Boolean) As Boolean
        Dim baseUrl = txtApiUrl.Text.Trim()
        Dim parsedUri As Uri = Nothing
        Dim valid = Uri.TryCreate(baseUrl, UriKind.Absolute, parsedUri) AndAlso (parsedUri.Scheme = Uri.UriSchemeHttp OrElse parsedUri.Scheme = Uri.UriSchemeHttps)
        If valid AndAlso IsLocalhostPort8080(parsedUri) Then
            valid = False
        End If
        If valid Then Return True

        ClearLoadedData()
        SetStatus("Base API belum valid.")
        If showMessage Then
            MessageBox.Show("Base API project lokal ini pakai http://localhost:8081. Jangan pakai localhost:8080 karena port itu bisa bentrok dengan service lain.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtApiUrl.Focus()
        End If

        Return False
    End Function

    Private Sub ClearLoadedData()
        gridMahasiswa.DataSource = New BindingList(Of MahasiswaModel)()
        gridJurusan.DataSource = New BindingList(Of JurusanModel)()
        cmbMahasiswaJurusan.DataSource = New BindingList(Of JurusanModel)()
        If lblMahasiswaCount IsNot Nothing Then lblMahasiswaCount.Text = "0 data"
        If lblJurusanCount IsNot Nothing Then lblJurusanCount.Text = "0 data"
        ResetMahasiswaForm()
        ResetJurusanForm()
    End Sub

    Private Shared Function IsLocalhostPort8080(uri As Uri) As Boolean
        Return uri.Port = 8080 AndAlso (String.Equals(uri.Host, "localhost", StringComparison.OrdinalIgnoreCase) OrElse uri.Host = "127.0.0.1")
    End Function

    Private Function Service() As ApiService
        Return New ApiService(txtApiUrl.Text.Trim())
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
        Dim box = New TextBox With {.Dock = DockStyle.Fill, .PlaceholderText = placeholder, .Font = UiFont(10), .Multiline = True, .Height = 34, .ScrollBars = ScrollBars.None, .WordWrap = False}
        StyleTextBox(box)
        Return box
    End Function

    Private Shared Function HeaderLabel(text As String) As Label
        Return New Label With {.Text = text, .Height = 40, .Font = UiFont(16, FontStyle.Bold), .ForeColor = TextStrong, .TextAlign = ContentAlignment.MiddleLeft, .Margin = New Padding(0, 0, 0, 14)}
    End Function

    Private Shared Function LabeledControl(labelText As String, control As Control) As Control
        Dim panel = New Panel With {.Height = 78, .BackColor = Surface, .Margin = New Padding(0, 0, 0, 10)}
        Dim label = New Label With {.Text = labelText, .Dock = DockStyle.Top, .Height = 22, .ForeColor = TextMuted, .Font = UiFont(9.25F, FontStyle.Bold), .TextAlign = ContentAlignment.MiddleLeft}
        Dim shell = InputShell(control)
        shell.Dock = DockStyle.Top
        panel.Controls.Add(shell)
        panel.Controls.Add(label)
        Return panel
    End Function

    Private Shared Function SectionHeader(title As String, countLabel As Label) As Control
        Dim panel = New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 2,
            .Padding = New Padding(16, 12, 16, 4),
            .BackColor = Surface
        }
        panel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
        panel.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 104))
        panel.Controls.Add(New Label With {.Text = title, .Dock = DockStyle.Fill, .Font = UiFont(12.5F, FontStyle.Bold), .ForeColor = TextStrong, .TextAlign = ContentAlignment.MiddleLeft, .Margin = New Padding(0)}, 0, 0)
        panel.Controls.Add(countLabel, 1, 0)
        Return panel
    End Function

    Private Shared Function CountBadge(text As String) As Label
        Return New Label With {
            .Text = text,
            .Dock = DockStyle.Fill,
            .Font = UiFont(9.25F, FontStyle.Bold),
            .ForeColor = TextMuted,
            .TextAlign = ContentAlignment.MiddleRight,
            .Margin = New Padding(0)
        }
    End Function

    Private Shared Function ButtonPanel() As FlowLayoutPanel
        Return New FlowLayoutPanel With {
            .Dock = DockStyle.None,
            .Height = 52,
            .AutoSize = False,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False,
            .Padding = New Padding(0, 6, 0, 4),
            .BackColor = Surface,
            .Margin = New Padding(0, 8, 0, 0)
        }
    End Function

    Private Shared Function CreateFormShell(body As FlowLayoutPanel) As Panel
        Dim shell = New Panel With {
            .Dock = DockStyle.Fill,
            .BackColor = Surface,
            .Padding = New Padding(0)
        }
        shell.Controls.Add(body)
        Return shell
    End Function

    Private Shared Function FormStackPanel() As FlowLayoutPanel
        Dim panel = New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .AutoScroll = True,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False,
            .Padding = New Padding(26, 26, 26, 22),
            .BackColor = Surface
        }
        AddHandler panel.Resize, Sub(sender, e) ResizeFormStack(DirectCast(sender, FlowLayoutPanel))
        Return panel
    End Function

    Private Shared Sub ResizeFormStack(panel As FlowLayoutPanel)
        Dim availableWidth = Math.Max(300, panel.ClientSize.Width - panel.Padding.Left - panel.Padding.Right - SystemInformation.VerticalScrollBarWidth - 8)
        For Each child As Control In panel.Controls
            child.Width = availableWidth
        Next
    End Sub

    Private Shared Function CardPanel() As TableLayoutPanel
        Return New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .BackColor = Surface,
            .Padding = New Padding(0),
            .CellBorderStyle = TableLayoutPanelCellBorderStyle.None
        }
    End Function

    Private Shared Sub StyleGrid(grid As DataGridView)
        grid.BackgroundColor = Surface
        grid.BorderStyle = BorderStyle.None
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        grid.GridColor = BorderSoft
        grid.RowHeadersVisible = False
        grid.AllowUserToResizeRows = False
        grid.EnableHeadersVisualStyles = False
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(238, 242, 246)
        grid.ColumnHeadersDefaultCellStyle.ForeColor = TextStrong
        grid.ColumnHeadersDefaultCellStyle.Font = UiFont(9.5F, FontStyle.Bold)
        grid.DefaultCellStyle.Font = UiFont(9.5F)
        grid.ColumnHeadersDefaultCellStyle.Padding = New Padding(8, 0, 8, 0)
        grid.ColumnHeadersHeight = 42
        grid.DefaultCellStyle.BackColor = Surface
        grid.DefaultCellStyle.ForeColor = TextStrong
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(215, 227, 240)
        grid.DefaultCellStyle.SelectionForeColor = TextStrong
        grid.DefaultCellStyle.Padding = New Padding(8, 0, 8, 0)
        grid.AlternatingRowsDefaultCellStyle.BackColor = SurfaceMuted
        grid.RowTemplate.Height = 40
    End Sub

    Private Shared Sub StyleTextBox(box As TextBox)
        box.BorderStyle = BorderStyle.None
        box.BackColor = Surface
        box.ForeColor = TextStrong
        box.Font = UiFont(10)
        box.Margin = New Padding(0)
    End Sub

    Private Shared Sub StyleReadOnlyTextBox(box As TextBox)
        box.ReadOnly = True
        box.BackColor = ReadOnlySurface
        box.ForeColor = TextMuted
        box.Cursor = Cursors.Default
        box.TabStop = False
    End Sub

    Private Shared Function InputShell(control As Control) As Panel
        control.Dock = DockStyle.None
        control.Margin = New Padding(0)
        Dim readOnlyInput = IsReadOnlyInput(control)
        Dim shellBack = If(readOnlyInput, ReadOnlySurface, Surface)
        control.BackColor = shellBack
        If Not TypeOf control Is TextBox Then
            control.Font = UiFont(10)
            control.Height = 34
        End If

        Dim shell = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 48,
            .BackColor = shellBack,
            .Padding = New Padding(14, 0, 14, 0),
            .Margin = New Padding(0)
        }
        shell.Controls.Add(control)
        AddHandler shell.Paint, AddressOf PaintInputShell
        AddHandler shell.Resize, Sub(sender, e) LayoutInputShell(DirectCast(sender, Panel))
        LayoutInputShell(shell)
        Return shell
    End Function

    Private Shared Sub LayoutInputShell(shell As Panel)
        If shell.Controls.Count = 0 Then Return

        Dim child = shell.Controls(0)
        child.Width = Math.Max(10, shell.ClientSize.Width - shell.Padding.Left - shell.Padding.Right)
        child.Left = shell.Padding.Left
        child.Top = Math.Max(0, CInt((shell.ClientSize.Height - child.Height) / 2) + 1)
    End Sub

    Private Shared Sub PaintInputShell(sender As Object, e As PaintEventArgs)
        Dim panel = DirectCast(sender, Panel)
        Dim borderColor = If(panel.Controls.Count > 0 AndAlso IsReadOnlyInput(panel.Controls(0)), ReadOnlyBorder, BorderSoft)
        Using pen As New Pen(borderColor, 1)
            Dim rect = New Rectangle(0, 0, panel.Width - 1, panel.Height - 1)
            e.Graphics.DrawRectangle(pen, rect)
        End Using
    End Sub

    Private Shared Function IsReadOnlyInput(control As Control) As Boolean
        If TypeOf control Is TextBox Then
            Return DirectCast(control, TextBox).ReadOnly
        End If

        Return False
    End Function

    Private Shared Function ActionButton(text As String, kind As ButtonKind) As Button
        Dim button = New Button With {.Text = text, .Width = 88, .Height = 36, .Margin = New Padding(0, 0, 10, 10), .FlatStyle = FlatStyle.Flat, .Cursor = Cursors.Hand}
        button.FlatAppearance.BorderSize = 0
        button.Font = UiFont(9.5F, FontStyle.Bold)

        Select Case kind
            Case ButtonKind.Primary
                button.BackColor = Accent
                button.ForeColor = Color.White
                button.FlatAppearance.MouseOverBackColor = AccentDark
            Case ButtonKind.Danger
                button.BackColor = Danger
                button.ForeColor = Color.White
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(146, 48, 56)
            Case Else
                button.BackColor = NeutralButton
                button.ForeColor = TextStrong
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(218, 224, 230)
        End Select

        Return button
    End Function

    Private Enum ButtonKind
        Primary
        Neutral
        Danger
    End Enum

    Private Shared Function UiFont(size As Single, Optional style As FontStyle = FontStyle.Regular) As Font
        Return New Font(UiFontFamily, size, style, GraphicsUnit.Point)
    End Function

    Private Shared Function LoadAppFonts() As PrivateFontCollection
        Dim collection = New PrivateFontCollection()
        Dim fontDir = IO.Path.Combine(AppContext.BaseDirectory, "Assets", "Fonts")
        If IO.Directory.Exists(fontDir) Then
            For Each fontFile In IO.Directory.GetFiles(fontDir, "*.ttf")
                collection.AddFontFile(fontFile)
            Next
        End If
        Return collection
    End Function

    Private Shared Function ResolveUiFontFamily() As FontFamily
        If AppFonts.Families.Any(Function(family) String.Equals(family.Name, "Poppins", StringComparison.OrdinalIgnoreCase)) Then
            Return AppFonts.Families.First(Function(family) String.Equals(family.Name, "Poppins", StringComparison.OrdinalIgnoreCase))
        End If

        Dim preferredFonts = {"Poppins", "Rubik", "Montserrat", "Segoe UI"}
        Using fonts As New InstalledFontCollection()
            Dim installed = fonts.Families.ToDictionary(Function(family) family.Name, Function(family) family, StringComparer.OrdinalIgnoreCase)
            For Each fontName In preferredFonts
                If installed.ContainsKey(fontName) Then
                    Return installed(fontName)
                End If
            Next
        End Using
        Return FontFamily.GenericSansSerif
    End Function
End Class
