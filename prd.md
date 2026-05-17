# Product Requirements Document (PRD)

## Nama Proyek

Sistem Manajemen Data Mahasiswa

## Tujuan

Membangun aplikasi desktop untuk mengelola data mahasiswa dan jurusan dengan arsitektur modern berbasis REST API. Dokumen ini menjadi acuan utama agar implementasi tetap sesuai requirement technical test.

## Requirement Resmi Technical Test

- Frontend menggunakan VB.NET Desktop Application.
- Backend menggunakan Spring Boot.
- Database menggunakan PostgreSQL.
- Arsitektur menggunakan konsep REST API.
- Dokumentasi API menggunakan Swagger.
- Tidak diperbolehkan ada query database di sisi frontend.
- Frontend hanya berfungsi sebagai consumer API dan mengakses data melalui endpoint backend.

## Nilai Tambah

- Project menggunakan konsep MVC.
- Terdapat validasi input.
- Project dapat di-deploy ke Minikube.
- Data dapat di-export ke Excel, PDF, CSV, dan JSON.
- Tampilan aplikasi menarik dan responsif.

## Arsitektur Sistem

```text
VB.NET Windows Forms
        |
        | HTTP Request / Response
        v
Spring Boot REST API
        |
        | Spring Data JPA / Hibernate
        v
PostgreSQL
```

Prinsip utama:

- VB.NET hanya menangani UI, validasi ringan, dan pemanggilan API.
- Semua business logic berada di backend Spring Boot.
- Semua akses database hanya dilakukan oleh backend.
- PostgreSQL menjadi penyimpanan data permanen.

## Technology Stack

### Frontend

- VB.NET Windows Forms
- `System.Net.Http` atau RestSharp
- Newtonsoft.Json
- DataGridView

### Backend

- Java 21 atau Java 17
- Spring Boot 3
- Spring Web
- Spring Data JPA
- Spring Validation
- PostgreSQL Driver
- springdoc-openapi
- Lombok opsional

### Database

- PostgreSQL

## Modul dan Fitur

### Modul Jurusan

- Tambah jurusan.
- Edit jurusan.
- Hapus jurusan.
- Lihat daftar jurusan.
- Search jurusan.
- Data jurusan menyimpan nama jurusan dan fakultas.

### Modul Mahasiswa

- Tambah mahasiswa.
- Edit mahasiswa.
- Hapus mahasiswa.
- Lihat daftar mahasiswa.
- Search mahasiswa berdasarkan nama atau NIM.
- Relasi mahasiswa dengan jurusan.

### Export Data

Target nilai tambah:

- Export Excel.
- Export PDF.
- Export CSV.
- Export JSON.
- Export tersedia untuk data Mahasiswa dan Jurusan melalui REST API, lalu frontend hanya mengunduh file dari endpoint backend.

Prioritas implementasi export dapat dilakukan setelah CRUD, search, dan validasi utama stabil.

## Database Design

### Tabel `jurusan`

| Field | Type | Constraint |
| --- | --- | --- |
| `id` | SERIAL / BIGSERIAL | Primary Key |
| `nama_jurusan` | VARCHAR(100) | Unique, Not Null |
| `fakultas` | VARCHAR(100) |  |
| `created_at` | TIMESTAMP |  |
| `updated_at` | TIMESTAMP |  |

### Tabel `mahasiswa`

| Field | Type | Constraint |
| --- | --- | --- |
| `id` | SERIAL / BIGSERIAL | Primary Key |
| `nim` | VARCHAR(20) | Unique, Not Null |
| `nama` | VARCHAR(100) | Not Null |
| `email` | VARCHAR(100) | Unique |
| `jurusan_id` | INTEGER / BIGINT | Foreign Key ke `jurusan.id` |
| `created_at` | TIMESTAMP |  |
| `updated_at` | TIMESTAMP |  |

Relasi:

```text
Jurusan (1) -------- (N) Mahasiswa
```

## REST API Endpoints

### Jurusan

| Method | Endpoint | Description |
| --- | --- | --- |
| GET | `/api/jurusan` | Get all jurusan |
| GET | `/api/jurusan/{id}` | Get detail jurusan |
| POST | `/api/jurusan` | Create jurusan |
| PUT | `/api/jurusan/{id}` | Update jurusan |
| DELETE | `/api/jurusan/{id}` | Delete jurusan |

### Mahasiswa

| Method | Endpoint | Description |
| --- | --- | --- |
| GET | `/api/mahasiswa` | Get all mahasiswa |
| GET | `/api/mahasiswa/{id}` | Get detail mahasiswa |
| GET | `/api/mahasiswa?search=keyword` | Search mahasiswa |
| POST | `/api/mahasiswa` | Create mahasiswa |
| PUT | `/api/mahasiswa/{id}` | Update mahasiswa |
| DELETE | `/api/mahasiswa/{id}` | Delete mahasiswa |

## API Response Format

### Success

```json
{
  "success": true,
  "message": "Data berhasil diproses",
  "data": {}
}
```

### Error

```json
{
  "success": false,
  "message": "Validation failed",
  "errors": {
    "nim": "NIM already exists"
  }
}
```

## Validasi

### Frontend

- Field wajib tidak boleh kosong.
- Format email harus valid.
- Jurusan wajib dipilih saat membuat atau mengubah mahasiswa.
- Konfirmasi sebelum delete.

### Backend

- `nim` wajib unik.
- `nama` mahasiswa wajib diisi.
- `nama_jurusan` wajib unik.
- `jurusan_id` wajib valid dan harus mengarah ke data jurusan yang ada.
- Bean Validation digunakan untuk request DTO.
- Business validation berada di service layer.

## Desain UI Frontend VB.NET

### Main Form

- Search panel:
  - TextBox Search.
  - Button Search.
- Input form:
  - TextBox NIM.
  - TextBox Nama.
  - TextBox Email.
  - ComboBox Jurusan.
- Action buttons:
  - New / Reset.
  - Save.
  - Update.
  - Delete.
- DataGridView:
  - Menampilkan daftar mahasiswa.
  - Klik row untuk mengisi form.

### Form atau Panel Jurusan

- TextBox nama jurusan.
- TextBox fakultas.
- Tombol New / Reset, Save, Update, Delete.
- Search jurusan.
- DataGridView daftar jurusan.

## User Flow

### Create Data

1. User mengisi form.
2. User klik Save.
3. VB.NET mengirim POST ke REST API.
4. Spring Boot melakukan validasi.
5. Data disimpan ke PostgreSQL.
6. Backend mengembalikan response JSON.
7. DataGridView di-refresh.

### Update Data

1. User memilih row pada DataGridView.
2. Form terisi otomatis.
3. User mengubah data.
4. User klik Update.
5. VB.NET mengirim PUT ke REST API.
6. Backend melakukan validasi dan update data.
7. DataGridView di-refresh.

### Delete Data

1. User memilih data.
2. User klik Delete.
3. Aplikasi menampilkan konfirmasi.
4. VB.NET mengirim DELETE ke REST API.
5. DataGridView di-refresh.

### Search

1. User mengetik keyword.
2. User klik Search atau menekan Enter.
3. VB.NET mengirim GET dengan query parameter.
4. Backend mengembalikan hasil filter.
5. DataGridView menampilkan hasil search.

## Backend Architecture

```text
Controller
   |
Service
   |
Repository
   |
Entity
   |
PostgreSQL
```

Tanggung jawab layer:

- Controller menerima request, memanggil service, dan mengembalikan response.
- Service menyimpan business logic dan validasi utama.
- Repository menangani query database melalui Spring Data JPA.
- Entity merepresentasikan tabel database.
- DTO digunakan untuk request dan response agar entity tidak langsung terekspos.

## Struktur Folder Target

### Backend

```text
backend/
├── src/main/java/com/example/app/
│   ├── controller/
│   ├── service/
│   ├── repository/
│   ├── entity/
│   ├── dto/
│   ├── config/
│   ├── exception/
│   └── AppApplication.java
├── src/main/resources/
│   └── application.properties
└── pom.xml
```

### Frontend

```text
frontend/
├── Forms/
│   └── FrmMahasiswa.vb
├── Services/
│   └── ApiService.vb
├── Models/
│   └── Mahasiswa.vb
├── Utils/
└── App.config
```

## Swagger

Swagger UI harus tersedia di:

```text
http://localhost:8080/swagger-ui/index.html
```

Tujuan:

- Dokumentasi API otomatis.
- Testing endpoint langsung dari browser.
- Bukti bahwa backend mengikuti requirement dokumentasi API.

## Environment Configuration

Contoh konfigurasi backend:

```properties
spring.datasource.url=jdbc:postgresql://localhost:5432/mahasiswa_db
spring.datasource.username=postgres
spring.datasource.password=password
spring.jpa.hibernate.ddl-auto=update
server.port=8080
```

Catatan:

- Credential database dapat disesuaikan dengan environment lokal.
- Untuk deployment Minikube, konfigurasi database dan backend harus dipisahkan melalui environment variable atau ConfigMap/Secret.

## Development Phases

### Phase 1 - Setup Project

- Buat project Spring Boot.
- Buat project VB.NET Windows Forms.
- Setup PostgreSQL database.

### Phase 2 - Database dan Entity

- Buat entity `Jurusan`.
- Buat entity `Mahasiswa`.
- Setup relasi one-to-many.
- Setup repository.

### Phase 3 - Backend Development

- CRUD Jurusan.
- CRUD Mahasiswa.
- Search mahasiswa.
- Search jurusan.
- Validasi backend.
- Response wrapper.
- Global exception handler.
- Swagger/OpenAPI.

### Phase 4 - Frontend Development

- Windows Forms UI.
- Service HTTP client.
- Integrasi CRUD Jurusan.
- Integrasi CRUD Mahasiswa.
- Search.
- Validasi frontend.
- Error handling response API.

### Phase 5 - Export dan Nilai Tambah

- Export CSV.
- Export JSON.
- Export Excel.
- Export PDF.
- Persiapan deployment Minikube.

### Phase 6 - Testing dan Presentasi

- CRUD testing.
- Validation testing.
- Swagger testing.
- Demo alur aplikasi.
- Penjelasan arsitektur REST API.

## Testing Checklist

- Create jurusan berhasil.
- Update jurusan berhasil.
- Delete jurusan berhasil.
- Search jurusan berhasil.
- Create mahasiswa berhasil.
- Update mahasiswa berhasil.
- Delete mahasiswa berhasil.
- Search mahasiswa by nama berhasil.
- Search mahasiswa by NIM berhasil.
- Validasi NIM unik berjalan.
- Validasi nama wajib berjalan.
- Validasi jurusan wajib berjalan.
- Swagger dapat diakses.
- Frontend tidak melakukan koneksi langsung ke PostgreSQL.
- Frontend hanya menggunakan REST API.

## Success Criteria

Project dianggap selesai jika:

- CRUD Jurusan berjalan.
- CRUD Mahasiswa berjalan.
- Data tersimpan di PostgreSQL.
- VB.NET hanya menggunakan REST API.
- Swagger tersedia.
- Search berfungsi.
- Validasi input berjalan di frontend dan backend.
- Struktur project dapat dijelaskan dengan konsep MVC/layered architecture.
- Aplikasi dapat didemokan untuk kebutuhan interview.

## Presentation Talking Points

- VB.NET dipakai karena sesuai requirement dan cocok untuk desktop application.
- Spring Boot dipakai untuk mempercepat pembuatan REST API dengan struktur backend yang rapi.
- PostgreSQL dipakai sebagai database relasional yang stabil.
- REST API memisahkan frontend dan backend sehingga aplikasi lebih modular.
- Frontend tidak menyentuh database langsung, sehingga tanggung jawab data dan business logic terpusat di backend.

## Prompt Implementasi Utama

Build a Student Management Desktop Application using VB.NET Windows Forms as frontend and Spring Boot as backend. The frontend must only consume REST APIs and must not access PostgreSQL directly. Implement CRUD for Jurusan and Mahasiswa, including search and validation. Document APIs using Swagger.
