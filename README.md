# Technical Test App 2

Sistem Manajemen Data Mahasiswa dengan frontend VB.NET Windows Forms, backend Spring Boot, dan database PostgreSQL.

## Struktur

- `prd.md` - Product Requirements Document.
- `backend/` - Spring Boot REST API.
- `frontend/` - VB.NET Windows Forms desktop app.
- `k8s/` - Manifest Kubernetes untuk deployment Minikube.

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

Untuk demo lokal tanpa PostgreSQL, backend bisa dijalankan di port `8081` dengan H2 in-memory:

```powershell
cd backend
$env:DB_URL='jdbc:h2:mem:demo;DB_CLOSE_DELAY=-1;DB_CLOSE_ON_EXIT=FALSE'
$env:DB_USERNAME='sa'
$env:DB_PASSWORD=''
java -jar target\app-0.0.1-SNAPSHOT.jar --server.port=8081 --spring.datasource.driver-class-name=org.h2.Driver --spring.jpa.hibernate.ddl-auto=create-drop
```

Test backend:

```powershell
cd backend
.\mvnw.cmd test
```

## Deployment Minikube

Build JAR dan Docker image backend:

```powershell
cd backend
.\mvnw.cmd package -DskipTests
cd ..
minikube image build -t student-management-api:latest backend
```

Deploy PostgreSQL dan backend ke Minikube:

```powershell
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/postgres-secret.yaml
kubectl apply -f k8s/postgres-deployment.yaml
kubectl apply -f k8s/backend-deployment.yaml
```

Akses API dari host:

```powershell
minikube service student-api -n student-management --url
```

Swagger tersedia di:

```text
http://<minikube-service-url>/swagger-ui/index.html
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
http://localhost:8081
```

## Fitur

- CRUD Jurusan dengan field Nama Jurusan, Fakultas, dan Jenjang.
- CRUD Mahasiswa dengan field Nama, Umur, NIM, Tanggal Lahir, Alamat, dan relasi ke Jurusan.
- Fakultas dan Jenjang tampil otomatis readonly di Form Mahasiswa berdasarkan Jurusan yang dipilih.
- Search Jurusan dan Mahasiswa.
- Validasi input di frontend dan backend.
- Swagger/OpenAPI untuk dokumentasi REST API.
- Deployment Minikube tersedia melalui `backend/Dockerfile` dan manifest `k8s/`.
- Export data Jurusan dan Mahasiswa ke Excel, PDF, CSV, dan JSON melalui endpoint backend:

```text
GET /api/export/{jurusan|mahasiswa}/{xlsx|pdf|csv|json}
```
