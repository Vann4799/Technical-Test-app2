package com.example.app.dto;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Size;

public record JurusanRequest(
        @NotBlank(message = "Nama jurusan wajib diisi")
        @Size(max = 100, message = "Nama jurusan maksimal 100 karakter")
        String namaJurusan
) {
}
