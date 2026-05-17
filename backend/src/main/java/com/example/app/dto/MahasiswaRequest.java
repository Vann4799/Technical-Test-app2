package com.example.app.dto;

import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

public record MahasiswaRequest(
        @NotBlank(message = "NIM wajib diisi")
        @Size(max = 20, message = "NIM maksimal 20 karakter")
        String nim,

        @NotBlank(message = "Nama wajib diisi")
        @Size(max = 100, message = "Nama maksimal 100 karakter")
        String nama,

        @Email(message = "Format email tidak valid")
        @Size(max = 100, message = "Email maksimal 100 karakter")
        String email,

        @NotNull(message = "Jurusan wajib dipilih")
        Long jurusanId
) {
}
