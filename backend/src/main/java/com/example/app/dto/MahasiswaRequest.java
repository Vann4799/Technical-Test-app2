package com.example.app.dto;

import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import java.time.LocalDate;

public record MahasiswaRequest(
        @NotBlank(message = "NIM wajib diisi")
        @Size(max = 20, message = "NIM maksimal 20 karakter")
        String nim,

        @NotBlank(message = "Nama wajib diisi")
        @Size(max = 100, message = "Nama maksimal 100 karakter")
        String nama,

        @NotNull(message = "Umur wajib diisi")
        @Min(value = 1, message = "Umur harus lebih dari 0")
        Integer umur,

        @NotNull(message = "Tanggal lahir wajib diisi")
        LocalDate tanggalLahir,

        @NotBlank(message = "Alamat wajib diisi")
        @Size(max = 255, message = "Alamat maksimal 255 karakter")
        String alamat,

        @NotNull(message = "Jurusan wajib dipilih")
        Long jurusanId
) {
}
