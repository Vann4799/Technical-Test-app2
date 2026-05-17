package com.example.app.controller;

import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.AutoConfigureMockMvc;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.http.MediaType;
import org.springframework.test.web.servlet.MockMvc;

import com.example.app.entity.Jurusan;
import com.example.app.repository.JurusanRepository;

import static org.hamcrest.Matchers.empty;
import static org.hamcrest.Matchers.hasSize;
import static org.hamcrest.Matchers.notNullValue;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;
import org.junit.jupiter.api.BeforeEach;

import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.delete;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.post;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.put;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.jsonPath;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

@SpringBootTest
@AutoConfigureMockMvc
class JurusanControllerTest {

    @Autowired
    private MockMvc mockMvc;

    @Autowired
    private JurusanRepository jurusanRepository;

    @BeforeEach
    void setUp() {
        jurusanRepository.deleteAll();
    }

    @Test
    void getAllJurusanReturnsSuccessResponseWithDataArray() throws Exception {
        mockMvc.perform(get("/api/jurusan"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.success").value(true))
                .andExpect(jsonPath("$.message").value("Data berhasil diproses"))
                .andExpect(jsonPath("$.data", empty()));
    }

    @Test
    void createJurusanPersistsNewData() throws Exception {
        mockMvc.perform(post("/api/jurusan")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content("""
                                {
                                  "namaJurusan": "Teknik Informatika",
                                  "fakultas": "Sains dan Teknologi"
                                }
                                """))
                .andExpect(status().isCreated())
                .andExpect(jsonPath("$.success").value(true))
                .andExpect(jsonPath("$.data.id", notNullValue()))
                .andExpect(jsonPath("$.data.namaJurusan").value("Teknik Informatika"))
                .andExpect(jsonPath("$.data.fakultas").value("Sains dan Teknologi"));

        assertTrue(jurusanRepository.findAll().stream()
                .anyMatch(jurusan -> jurusan.getNamaJurusan().equals("Teknik Informatika")));
    }

    @Test
    void createJurusanRejectsBlankName() throws Exception {
        mockMvc.perform(post("/api/jurusan")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content("""
                                {
                                  "namaJurusan": ""
                                }
                                """))
                .andExpect(status().isBadRequest())
                .andExpect(jsonPath("$.success").value(false))
                .andExpect(jsonPath("$.message").value("Validation failed"))
                .andExpect(jsonPath("$.errors.namaJurusan").value("Nama jurusan wajib diisi"));
    }

    @Test
    void createJurusanRejectsDuplicateName() throws Exception {
        saveJurusan("Sistem Informasi");

        mockMvc.perform(post("/api/jurusan")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content("""
                                {
                                  "namaJurusan": "sistem informasi"
                                }
                                """))
                .andExpect(status().isBadRequest())
                .andExpect(jsonPath("$.success").value(false))
                .andExpect(jsonPath("$.errors.namaJurusan").value("Nama jurusan sudah digunakan"));
    }

    @Test
    void updateJurusanChangesExistingData() throws Exception {
        Jurusan jurusan = saveJurusan("Manajemen");

        mockMvc.perform(put("/api/jurusan/{id}", jurusan.getId())
                        .contentType(MediaType.APPLICATION_JSON)
                        .content("""
                                {
                                  "namaJurusan": "Akuntansi",
                                  "fakultas": "Ekonomi dan Bisnis"
                                }
                                """))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.success").value(true))
                .andExpect(jsonPath("$.data.id").value(jurusan.getId()))
                .andExpect(jsonPath("$.data.namaJurusan").value("Akuntansi"))
                .andExpect(jsonPath("$.data.fakultas").value("Ekonomi dan Bisnis"));
    }

    @Test
    void deleteJurusanRemovesExistingData() throws Exception {
        Jurusan jurusan = saveJurusan("Desain Komunikasi Visual");

        mockMvc.perform(delete("/api/jurusan/{id}", jurusan.getId()))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.success").value(true));

        assertFalse(jurusanRepository.existsById(jurusan.getId()));
    }

    @Test
    void searchJurusanFiltersByName() throws Exception {
        saveJurusan("Teknik Informatika");
        saveJurusan("Akuntansi");

        mockMvc.perform(get("/api/jurusan").param("search", "teknik"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.data", hasSize(1)))
                .andExpect(jsonPath("$.data[0].namaJurusan").value("Teknik Informatika"));
    }

    private Jurusan saveJurusan(String namaJurusan) {
        Jurusan jurusan = new Jurusan();
        jurusan.setNamaJurusan(namaJurusan);
        return jurusanRepository.save(jurusan);
    }
}
