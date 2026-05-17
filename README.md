# Technical Test App 2

Sistem Manajemen Data Mahasiswa dengan frontend VB.NET Windows Forms, backend Spring Boot, dan database PostgreSQL.

## Struktur

- `prd.md` - Product Requirements Document.
- `backend/` - Spring Boot REST API.
- `frontend/` - VB.NET Windows Forms desktop app.

## Backend

Default backend berjalan di port `8080` dan memakai PostgreSQL:

```properties
DB_URL=jdbc:postgresql://localhost:5432/mahasiswa_db
DB_USERNAME=postgres
DB_PASSWORD=password
```

Jalankan backend:

```powershell
cd backend
.\mvnw.cmd spring-boot:run
```

Swagger:

```text
http://localhost:8080/swagger-ui/index.html
```

Test backend:

```powershell
cd backend
.\mvnw.cmd test
```

## Frontend

Frontend hanya memanggil REST API dan tidak melakukan query database langsung.

Build frontend dengan .NET SDK:

```powershell
.\.dotnet\dotnet.exe build frontend\StudentManagementApp.vbproj
```

Jalankan frontend:

```powershell
.\.dotnet\dotnet.exe run --project frontend\StudentManagementApp.vbproj
```

Pastikan backend aktif sebelum menjalankan frontend. Base API default di aplikasi adalah:

```text
http://localhost:8080
```
