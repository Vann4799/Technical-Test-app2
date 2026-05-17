package com.example.app.repository;

import com.example.app.entity.Mahasiswa;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface MahasiswaRepository extends JpaRepository<Mahasiswa, Long> {
    boolean existsByNimIgnoreCase(String nim);

    boolean existsByNimIgnoreCaseAndIdNot(String nim, Long id);

    boolean existsByEmailIgnoreCase(String email);

    boolean existsByEmailIgnoreCaseAndIdNot(String email, Long id);

    List<Mahasiswa> findByNamaContainingIgnoreCaseOrNimContainingIgnoreCase(String nama, String nim);
}
