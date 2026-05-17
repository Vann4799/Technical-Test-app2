package com.example.app.controller;

import com.example.app.entity.Jurusan;
import com.example.app.entity.Mahasiswa;
import com.example.app.repository.JurusanRepository;
import com.example.app.repository.MahasiswaRepository;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.AutoConfigureMockMvc;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.http.HttpHeaders;
import org.springframework.test.web.servlet.MockMvc;

import java.nio.charset.StandardCharsets;

import static org.assertj.core.api.Assertions.assertThat;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.header;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

@SpringBootTest
@AutoConfigureMockMvc
class ExportControllerTest {

    @Autowired
    private MockMvc mockMvc;

    @Autowired
    private JurusanRepository jurusanRepository;

    @Autowired
    private MahasiswaRepository mahasiswaRepository;

    @BeforeEach
    void setUp() {
        mahasiswaRepository.deleteAll();
        jurusanRepository.deleteAll();
    }

    @Test
    void exportJurusanCsvReturnsDownloadableFile() throws Exception {
        saveJurusan("Teknik Informatika", "Teknik");

        byte[] response = mockMvc.perform(get("/api/export/jurusan/csv"))
                .andExpect(status().isOk())
                .andExpect(header().string(HttpHeaders.CONTENT_DISPOSITION, "attachment; filename=\"export-jurusan.csv\""))
                .andReturn()
                .getResponse()
                .getContentAsByteArray();

        String csv = new String(response, StandardCharsets.UTF_8);
        assertThat(csv).contains("ID,Nama Jurusan,Fakultas");
        assertThat(csv).contains("Teknik Informatika,Teknik");
    }

    @Test
    void exportMahasiswaJsonReturnsData() throws Exception {
        Jurusan jurusan = saveJurusan("Sistem Informasi", "Ilmu Komputer");
        saveMahasiswa("2026001", "Ayu", "ayu@example.com", jurusan);

        byte[] response = mockMvc.perform(get("/api/export/mahasiswa/json"))
                .andExpect(status().isOk())
                .andExpect(header().string(HttpHeaders.CONTENT_DISPOSITION, "attachment; filename=\"export-mahasiswa.json\""))
                .andReturn()
                .getResponse()
                .getContentAsByteArray();

        String json = new String(response, StandardCharsets.UTF_8);
        assertThat(json).contains("2026001");
        assertThat(json).contains("Sistem Informasi");
    }

    @Test
    void exportExcelAndPdfFormatsAreSupported() throws Exception {
        saveJurusan("Akuntansi", "Ekonomi");

        mockMvc.perform(get("/api/export/jurusan/xlsx"))
                .andExpect(status().isOk())
                .andExpect(header().string(HttpHeaders.CONTENT_DISPOSITION, "attachment; filename=\"export-jurusan.xlsx\""));

        mockMvc.perform(get("/api/export/jurusan/pdf"))
                .andExpect(status().isOk())
                .andExpect(header().string(HttpHeaders.CONTENT_DISPOSITION, "attachment; filename=\"export-jurusan.pdf\""));
    }

    @Test
    void exportRejectsUnsupportedFormat() throws Exception {
        mockMvc.perform(get("/api/export/jurusan/txt"))
                .andExpect(status().isBadRequest());
    }

    private Jurusan saveJurusan(String namaJurusan, String fakultas) {
        Jurusan jurusan = new Jurusan();
        jurusan.setNamaJurusan(namaJurusan);
        jurusan.setFakultas(fakultas);
        return jurusanRepository.save(jurusan);
    }

    private void saveMahasiswa(String nim, String nama, String email, Jurusan jurusan) {
        Mahasiswa mahasiswa = new Mahasiswa();
        mahasiswa.setNim(nim);
        mahasiswa.setNama(nama);
        mahasiswa.setEmail(email);
        mahasiswa.setJurusan(jurusan);
        mahasiswaRepository.save(mahasiswa);
    }
}
