package com.example.app.service;

import com.example.app.dto.MahasiswaRequest;
import com.example.app.entity.Jurusan;
import com.example.app.entity.Mahasiswa;
import com.example.app.exception.BadRequestException;
import com.example.app.exception.ResourceNotFoundException;
import com.example.app.repository.JurusanRepository;
import com.example.app.repository.MahasiswaRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

@Service
public class MahasiswaService {

    private final MahasiswaRepository mahasiswaRepository;
    private final JurusanRepository jurusanRepository;

    public MahasiswaService(MahasiswaRepository mahasiswaRepository, JurusanRepository jurusanRepository) {
        this.mahasiswaRepository = mahasiswaRepository;
        this.jurusanRepository = jurusanRepository;
    }

    public List<Mahasiswa> findAll(String search) {
        if (search == null || search.isBlank()) {
            return mahasiswaRepository.findAll();
        }
        String keyword = search.trim();
        return mahasiswaRepository.findByNamaContainingIgnoreCaseOrNimContainingIgnoreCase(keyword, keyword);
    }

    public Mahasiswa findOne(Long id) {
        return findById(id);
    }

    @Transactional
    public Mahasiswa create(MahasiswaRequest request) {
        validateUniqueFields(request, null);
        Mahasiswa mahasiswa = new Mahasiswa();
        applyRequest(mahasiswa, request);
        return mahasiswaRepository.save(mahasiswa);
    }

    @Transactional
    public Mahasiswa update(Long id, MahasiswaRequest request) {
        Mahasiswa mahasiswa = findById(id);
        validateUniqueFields(request, id);
        applyRequest(mahasiswa, request);
        return mahasiswaRepository.save(mahasiswa);
    }

    @Transactional
    public void delete(Long id) {
        Mahasiswa mahasiswa = findById(id);
        mahasiswaRepository.delete(mahasiswa);
    }

    private Mahasiswa findById(Long id) {
        return mahasiswaRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Mahasiswa tidak ditemukan"));
    }

    private void applyRequest(Mahasiswa mahasiswa, MahasiswaRequest request) {
        Jurusan jurusan = jurusanRepository.findById(request.jurusanId())
                .orElseThrow(() -> new BadRequestException(Map.of("jurusanId", "Jurusan tidak ditemukan")));

        mahasiswa.setNim(request.nim().trim());
        mahasiswa.setNama(request.nama().trim());
        mahasiswa.setUmur(request.umur());
        mahasiswa.setTanggalLahir(request.tanggalLahir());
        mahasiswa.setAlamat(request.alamat().trim());
        mahasiswa.setJurusan(jurusan);
    }

    private void validateUniqueFields(MahasiswaRequest request, Long currentId) {
        Map<String, String> errors = new LinkedHashMap<>();
        String nim = request.nim().trim();

        boolean nimExists = currentId == null
                ? mahasiswaRepository.existsByNimIgnoreCase(nim)
                : mahasiswaRepository.existsByNimIgnoreCaseAndIdNot(nim, currentId);
        if (nimExists) {
            errors.put("nim", "NIM already exists");
        }

        if (!errors.isEmpty()) {
            throw new BadRequestException(errors);
        }
    }
}
