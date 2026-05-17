package com.example.app.controller;

import com.example.app.dto.ApiResponse;
import com.example.app.dto.JurusanRequest;
import com.example.app.entity.Jurusan;
import com.example.app.service.JurusanService;
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
@RequestMapping("/api/jurusan")
public class JurusanController {

    private final JurusanService jurusanService;

    public JurusanController(JurusanService jurusanService) {
        this.jurusanService = jurusanService;
    }

    @GetMapping
    public ApiResponse<List<Jurusan>> findAll(@RequestParam(required = false) String search) {
        return ApiResponse.success("Data berhasil diproses", jurusanService.findAll(search));
    }

    @PostMapping
    public ResponseEntity<ApiResponse<Jurusan>> create(@Valid @RequestBody JurusanRequest request) {
        return ResponseEntity.status(HttpStatus.CREATED)
                .body(ApiResponse.success("Data berhasil diproses", jurusanService.create(request)));
    }

    @PutMapping("/{id}")
    public ApiResponse<Jurusan> update(@PathVariable Long id, @Valid @RequestBody JurusanRequest request) {
        return ApiResponse.success("Data berhasil diproses", jurusanService.update(id, request));
    }

    @DeleteMapping("/{id}")
    public ApiResponse<Void> delete(@PathVariable Long id) {
        jurusanService.delete(id);
        return ApiResponse.success("Data berhasil diproses", null);
    }
}
