# Product Requirements Document (PRD)

# Aplikasi 2: Sistem Manajemen Data Mahasiswa
## VB.NET Desktop + Spring Boot + PostgreSQL + REST API + Swagger

---

## 1. Project Overview

### Nama Proyek
Sistem Manajemen Data Mahasiswa (Desktop Application)

### Tujuan
Membangun aplikasi desktop untuk mengelola data Mahasiswa dan Jurusan dengan arsitektur REST API.

### Requirement Resmi dari Technical Test
- Frontend: VB.NET (Desktop Application)
- Backend: Spring Boot
- Database: PostgreSQL
- Arsitektur: REST API
- Dokumentasi API: Swagger
- Frontend tidak boleh melakukan query database secara langsung
- Frontend hanya berfungsi sebagai consumer API

---

## 2. Arsitektur Sistem

```text
VB.NET Desktop Application
        ↓ HTTP Request (GET, POST, PUT, DELETE)
Spring Boot REST API
        ↓ JPA / Hibernate
PostgreSQL Database
```

---

## 3. Model Data dan Normalisasi

### Tabel Jurusan
- id_jurusan
- nama_jurusan
- fakultas
- jenjang

### Tabel Mahasiswa
- id
- nama
- umur
- nim
- tanggal_lahir
- alamat
- id_jurusan (foreign key)

### Relasi
Satu Jurusan memiliki banyak Mahasiswa (One-to-Many).

### Catatan Penting
`Fakultas` dan `Jenjang` adalah atribut milik Jurusan, bukan Mahasiswa.

Artinya:
- Data `fakultas` dan `jenjang` disimpan hanya di tabel `jurusan`.
- Tabel `mahasiswa` hanya menyimpan `id_jurusan`.
- Saat input Mahasiswa, `fakultas` dan `jenjang` hanya ditampilkan otomatis (readonly).

Ini adalah desain database yang benar untuk menghindari redundansi data.

---

## 4. Form Jurusan

Digunakan untuk mengelola master data jurusan.

### Field
- Nama Jurusan
- Fakultas
- Jenjang

### Fitur
- Create
- Read
- Update
- Delete
- Search

---

## 5. Form Mahasiswa

Digunakan untuk mengelola data mahasiswa.

### Field Input
- Nama
- Umur
- NIM
- Tanggal Lahir
- Alamat
- Jurusan (ComboBox)

### Field Informasi (Readonly)
- Fakultas
- Jenjang

### Tombol
- Reset
- Save
- Update
- Delete

### DataGridView
- Menampilkan daftar mahasiswa beserta nama jurusan.

---

## 6. Alur Form Mahasiswa

1. Form dibuka.
2. VB.NET memanggil API `/api/jurusan`.
3. ComboBox Jurusan terisi.
4. User memilih Jurusan.
5. Fakultas dan Jenjang tampil otomatis.
6. User mengisi data Mahasiswa.
7. Klik Save.
8. Frontend hanya mengirim `id_jurusan`, bukan `fakultas` dan `jenjang`.
9. Spring Boot menyimpan data ke PostgreSQL.

---

## 7. REST API Endpoints

### Jurusan
- GET /api/jurusan
- GET /api/jurusan/{id}
- POST /api/jurusan
- PUT /api/jurusan/{id}
- DELETE /api/jurusan/{id}

### Mahasiswa
- GET /api/mahasiswa
- GET /api/mahasiswa/{id}
- GET /api/mahasiswa?search=keyword
- POST /api/mahasiswa
- PUT /api/mahasiswa/{id}
- DELETE /api/mahasiswa/{id}

---

## 8. Backend Architecture

```text
Controller
   ↓
Service
   ↓
Repository
   ↓
Entity (JPA)
   ↓
PostgreSQL
```

---

## 9. Frontend Architecture

```text
Forms
 ├── FrmJurusan
 └── FrmMahasiswa

Services
 └── ApiService

Models
 ├── Jurusan
 └── Mahasiswa
```

---

## 10. Validation Rules

### Frontend
- Semua field wajib diisi
- Umur harus numerik
- Jurusan wajib dipilih

### Backend
- NIM harus unik
- Nama wajib diisi
- Bean Validation

---

## 11. Swagger

Endpoint dokumentasi:
- /swagger-ui/index.html

---

## 12. Bonus Features
- MVC Pattern
- Input Validation
- Export CSV, Excel, PDF, JSON
- Deployment ke Minikube
- UI menarik dan rapi

---

## 13. Development Phases

### Phase 1
- Setup Spring Boot
- Setup PostgreSQL
- Setup VB.NET

### Phase 2
- CRUD Jurusan

### Phase 3
- CRUD Mahasiswa

### Phase 4
- Search dan Validation

### Phase 5
- Swagger

### Phase 6
- Export Data

### Phase 7
- Testing dan Presentasi

---

## 14. Success Criteria
- CRUD Jurusan berjalan
- CRUD Mahasiswa berjalan
- Search berjalan
- Swagger tersedia
- VB.NET hanya mengakses REST API
- PostgreSQL menyimpan data
- Fakultas dan Jenjang dikelola melalui Form Jurusan
- Pada Form Mahasiswa, Fakultas dan Jenjang tampil otomatis sebagai readonly

---

## 15. Prompt untuk Codex

Build a Student Management Desktop Application using VB.NET Windows Forms as frontend and Spring Boot as backend. The frontend must only consume REST APIs and must not access PostgreSQL directly. Implement CRUD for Jurusan and Mahasiswa. Fakultas and Jenjang belong to Jurusan and are managed in the Jurusan form. In the Mahasiswa form, Fakultas and Jenjang are displayed automatically as readonly fields after a Jurusan is selected. Only id_jurusan is sent when saving Mahasiswa.
