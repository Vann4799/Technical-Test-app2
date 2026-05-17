package com.example.app.service;

import com.example.app.dto.JurusanRequest;
import com.example.app.entity.Jurusan;
import com.example.app.exception.BadRequestException;
import com.example.app.exception.ResourceNotFoundException;
import com.example.app.repository.JurusanRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;
import java.util.Map;

@Service
public class JurusanService {

    private final JurusanRepository jurusanRepository;

    public JurusanService(JurusanRepository jurusanRepository) {
        this.jurusanRepository = jurusanRepository;
    }

    public List<Jurusan> findAll(String search) {
        if (search == null || search.isBlank()) {
            return jurusanRepository.findAll();
        }
        return jurusanRepository.findByNamaJurusanContainingIgnoreCase(search.trim());
    }

    @Transactional
    public Jurusan create(JurusanRequest request) {
        ensureNameAvailable(request.namaJurusan(), null);

        Jurusan jurusan = new Jurusan();
        jurusan.setNamaJurusan(request.namaJurusan().trim());
        return jurusanRepository.save(jurusan);
    }

    @Transactional
    public Jurusan update(Long id, JurusanRequest request) {
        Jurusan jurusan = findById(id);
        ensureNameAvailable(request.namaJurusan(), id);

        jurusan.setNamaJurusan(request.namaJurusan().trim());
        return jurusanRepository.save(jurusan);
    }

    @Transactional
    public void delete(Long id) {
        Jurusan jurusan = findById(id);
        jurusanRepository.delete(jurusan);
    }

    private Jurusan findById(Long id) {
        return jurusanRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Jurusan tidak ditemukan"));
    }

    private void ensureNameAvailable(String namaJurusan, Long currentId) {
        boolean exists = currentId == null
                ? jurusanRepository.existsByNamaJurusanIgnoreCase(namaJurusan.trim())
                : jurusanRepository.existsByNamaJurusanIgnoreCaseAndIdNot(namaJurusan.trim(), currentId);
        if (exists) {
            throw new BadRequestException(Map.of("namaJurusan", "Nama jurusan sudah digunakan"));
        }
    }
}
