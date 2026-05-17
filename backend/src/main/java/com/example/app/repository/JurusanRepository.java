package com.example.app.repository;

import com.example.app.entity.Jurusan;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface JurusanRepository extends JpaRepository<Jurusan, Long> {
    boolean existsByNamaJurusanIgnoreCase(String namaJurusan);

    boolean existsByNamaJurusanIgnoreCaseAndIdNot(String namaJurusan, Long id);

    List<Jurusan> findByNamaJurusanContainingIgnoreCase(String namaJurusan);
}
