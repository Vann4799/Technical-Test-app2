package com.example.app.controller;

import com.example.app.entity.Jurusan;
import com.example.app.repository.JurusanRepository;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.AutoConfigureMockMvc;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.http.MediaType;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.test.web.servlet.MockMvc;

import java.time.LocalDate;

import static org.hamcrest.Matchers.empty;
import static org.hamcrest.Matchers.hasSize;
import static org.hamcrest.Matchers.notNullValue;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.delete;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.post;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.put;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.jsonPath;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

@SpringBootTest
@AutoConfigureMockMvc
class MahasiswaControllerTest {

    @Autowired
    private MockMvc mockMvc;

    @Autowired
    private JurusanRepository jurusanRepository;

    @Autowired
    private JdbcTemplate jdbcTemplate;

    @Autowired
    private ObjectMapper objectMapper;

    @BeforeEach
    void setUp() {
        jdbcTemplate.execute("DELETE FROM mahasiswa");
        jurusanRepository.deleteAll();
    }

    @Test
    void getAllMahasiswaReturnsSuccessResponseWithDataArray() throws Exception {
        mockMvc.perform(get("/api/mahasiswa"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.success").value(true))
                .andExpect(jsonPath("$.data", empty()));
    }

    @Test
    void createMahasiswaPersistsNewData() throws Exception {
        Jurusan jurusan = saveJurusan("Teknik Informatika");

        mockMvc.perform(post("/api/mahasiswa")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content("""
                                {
                                  "nim": "2026001",
                                  "nama": "Ivan Pratama",
                                  "umur": 21,
                                  "tanggalLahir": "2005-01-15",
                                  "alamat": "Jl. Merdeka 1",
                                  "jurusanId": %d
                                }
                                """.formatted(jurusan.getId())))
                .andExpect(status().isCreated())
                .andExpect(jsonPath("$.success").value(true))
                .andExpect(jsonPath("$.data.id", notNullValue()))
                .andExpect(jsonPath("$.data.nim").value("2026001"))
                .andExpect(jsonPath("$.data.nama").value("Ivan Pratama"))
                .andExpect(jsonPath("$.data.umur").value(21))
                .andExpect(jsonPath("$.data.tanggalLahir").value("2005-01-15"))
                .andExpect(jsonPath("$.data.alamat").value("Jl. Merdeka 1"))
                .andExpect(jsonPath("$.data.jurusan.namaJurusan").value("Teknik Informatika"));
    }

    @Test
    void createMahasiswaRejectsBlankNama() throws Exception {
        Jurusan jurusan = saveJurusan("Sistem Informasi");

        mockMvc.perform(post("/api/mahasiswa")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content("""
                                {
                                  "nim": "2026002",
                                  "nama": "",
                                  "umur": 20,
                                  "tanggalLahir": "2006-02-20",
                                  "alamat": "Jl. Melati 2",
                                  "jurusanId": %d
                                }
                                """.formatted(jurusan.getId())))
                .andExpect(status().isBadRequest())
                .andExpect(jsonPath("$.success").value(false))
                .andExpect(jsonPath("$.message").value("Validation failed"))
                .andExpect(jsonPath("$.errors.nama").value("Nama wajib diisi"));
    }

    @Test
    void createMahasiswaRejectsDuplicateNim() throws Exception {
        Jurusan jurusan = saveJurusan("Akuntansi");
        createMahasiswa("2026003", "Budi", jurusan.getId());

        mockMvc.perform(post("/api/mahasiswa")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content("""
                                {
                                  "nim": "2026003",
                                  "nama": "Budi Santoso",
                                  "umur": 22,
                                  "tanggalLahir": "2004-03-10",
                                  "alamat": "Jl. Kenanga 3",
                                  "jurusanId": %d
                                }
                                """.formatted(jurusan.getId())))
                .andExpect(status().isBadRequest())
                .andExpect(jsonPath("$.errors.nim").value("NIM already exists"));
    }

    @Test
    void updateMahasiswaChangesExistingData() throws Exception {
        Jurusan jurusan = saveJurusan("Manajemen");
        int id = createMahasiswa("2026004", "Sari", jurusan.getId());

        mockMvc.perform(put("/api/mahasiswa/{id}", id)
                        .contentType(MediaType.APPLICATION_JSON)
                        .content("""
                                {
                                  "nim": "2026004",
                                  "nama": "Sari Wulandari",
                                  "umur": 23,
                                  "tanggalLahir": "2003-04-12",
                                  "alamat": "Jl. Mawar 4",
                                  "jurusanId": %d
                                }
                                """.formatted(jurusan.getId())))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.data.nama").value("Sari Wulandari"))
                .andExpect(jsonPath("$.data.umur").value(23))
                .andExpect(jsonPath("$.data.alamat").value("Jl. Mawar 4"));
    }

    @Test
    void deleteMahasiswaRemovesExistingData() throws Exception {
        Jurusan jurusan = saveJurusan("Desain Komunikasi Visual");
        int id = createMahasiswa("2026005", "Dina", jurusan.getId());

        mockMvc.perform(delete("/api/mahasiswa/{id}", id))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.success").value(true));

        mockMvc.perform(get("/api/mahasiswa"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.data", empty()));
    }

    @Test
    void searchMahasiswaFiltersByNamaOrNim() throws Exception {
        Jurusan jurusan = saveJurusan("Teknik Informatika");
        createMahasiswa("2026006", "Andi Saputra", jurusan.getId());
        createMahasiswa("2026007", "Maya Putri", jurusan.getId());

        mockMvc.perform(get("/api/mahasiswa").param("search", "maya"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.data", hasSize(1)))
                .andExpect(jsonPath("$.data[0].nim").value("2026007"));

        mockMvc.perform(get("/api/mahasiswa").param("search", "2026006"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.data", hasSize(1)))
                .andExpect(jsonPath("$.data[0].nama").value("Andi Saputra"));
    }

    private Jurusan saveJurusan(String namaJurusan) {
        Jurusan jurusan = new Jurusan();
        jurusan.setNamaJurusan(namaJurusan);
        jurusan.setFakultas("Fakultas " + namaJurusan);
        jurusan.setJenjang("S1");
        return jurusanRepository.save(jurusan);
    }

    private int createMahasiswa(String nim, String nama, Long jurusanId) throws Exception {
        String response = mockMvc.perform(post("/api/mahasiswa")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content("""
                                {
                                  "nim": "%s",
                                  "nama": "%s",
                                  "umur": 21,
                                  "tanggalLahir": "2005-01-15",
                                  "alamat": "Jl. Test",
                                  "jurusanId": %d
                                }
                                """.formatted(nim, nama, jurusanId)))
                .andExpect(status().isCreated())
                .andReturn()
                .getResponse()
                .getContentAsString();

        JsonNode root = objectMapper.readTree(response);
        return root.path("data").path("id").asInt();
    }
}
