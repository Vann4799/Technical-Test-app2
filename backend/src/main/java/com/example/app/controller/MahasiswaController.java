package com.example.app.controller;

import com.example.app.dto.ApiResponse;
import com.example.app.dto.MahasiswaRequest;
import com.example.app.entity.Mahasiswa;
import com.example.app.service.MahasiswaService;
import jakarta.validation.Valid;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/api/mahasiswa")
public class MahasiswaController {

    private final MahasiswaService mahasiswaService;

    public MahasiswaController(MahasiswaService mahasiswaService) {
        this.mahasiswaService = mahasiswaService;
    }

    @GetMapping
    public ApiResponse<List<Mahasiswa>> findAll(@RequestParam(required = false) String search) {
        return ApiResponse.success("Data berhasil diproses", mahasiswaService.findAll(search));
    }

    @GetMapping("/{id}")
    public ApiResponse<Mahasiswa> findById(@PathVariable Long id) {
        return ApiResponse.success("Data berhasil diproses", mahasiswaService.findOne(id));
    }

    @PostMapping
    public ResponseEntity<ApiResponse<Mahasiswa>> create(@Valid @RequestBody MahasiswaRequest request) {
        return ResponseEntity.status(HttpStatus.CREATED)
                .body(ApiResponse.success("Data berhasil diproses", mahasiswaService.create(request)));
    }

    @PutMapping("/{id}")
    public ApiResponse<Mahasiswa> update(@PathVariable Long id, @Valid @RequestBody MahasiswaRequest request) {
        return ApiResponse.success("Data berhasil diproses", mahasiswaService.update(id, request));
    }

    @DeleteMapping("/{id}")
    public ApiResponse<Void> delete(@PathVariable Long id) {
        mahasiswaService.delete(id);
        return ApiResponse.success("Data berhasil diproses", null);
    }
}
