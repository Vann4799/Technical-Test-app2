package com.example.app.service;

import com.example.app.entity.Jurusan;
import com.example.app.entity.Mahasiswa;
import com.example.app.exception.BadRequestException;
import com.example.app.repository.JurusanRepository;
import com.example.app.repository.MahasiswaRepository;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.datatype.jsr310.JavaTimeModule;
import com.lowagie.text.Document;
import com.lowagie.text.Font;
import com.lowagie.text.FontFactory;
import com.lowagie.text.PageSize;
import com.lowagie.text.Paragraph;
import com.lowagie.text.Phrase;
import com.lowagie.text.pdf.PdfPCell;
import com.lowagie.text.pdf.PdfPTable;
import com.lowagie.text.pdf.PdfWriter;
import org.apache.poi.ss.usermodel.Cell;
import org.apache.poi.ss.usermodel.CellStyle;
import org.apache.poi.ss.usermodel.Row;
import org.apache.poi.ss.usermodel.Sheet;
import org.apache.poi.xssf.usermodel.XSSFWorkbook;
import org.springframework.http.MediaType;
import org.springframework.stereotype.Service;

import java.io.ByteArrayOutputStream;
import java.nio.charset.StandardCharsets;
import java.util.List;
import java.util.Map;

@Service
public class ExportService {

    private final JurusanRepository jurusanRepository;
    private final MahasiswaRepository mahasiswaRepository;
    private final ObjectMapper objectMapper;

    public ExportService(JurusanRepository jurusanRepository, MahasiswaRepository mahasiswaRepository) {
        this.jurusanRepository = jurusanRepository;
        this.mahasiswaRepository = mahasiswaRepository;
        this.objectMapper = new ObjectMapper().registerModule(new JavaTimeModule());
    }

    public ExportFile export(String resource, String format) {
        String normalizedResource = normalize(resource);
        String normalizedFormat = normalize(format);
        List<String[]> rows = rowsFor(normalizedResource);

        return switch (normalizedFormat) {
            case "csv" -> new ExportFile(fileName(normalizedResource, "csv"), "text/csv", toCsv(rows));
            case "json" -> new ExportFile(fileName(normalizedResource, "json"), MediaType.APPLICATION_JSON_VALUE, toJson(normalizedResource));
            case "xlsx" -> new ExportFile(fileName(normalizedResource, "xlsx"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", toExcel(normalizedResource, rows));
            case "pdf" -> new ExportFile(fileName(normalizedResource, "pdf"), MediaType.APPLICATION_PDF_VALUE, toPdf(normalizedResource, rows));
            default -> throw new BadRequestException(Map.of("format", "Format export harus csv, json, xlsx, atau pdf"));
        };
    }

    private List<String[]> rowsFor(String resource) {
        return switch (resource) {
            case "jurusan" -> {
                List<String[]> rows = jurusanRepository.findAll().stream()
                        .map(item -> new String[] {
                                text(item.getId()),
                                item.getNamaJurusan(),
                                item.getFakultas()
                        })
                        .toList();
                yield withHeader(new String[] {"ID", "Nama Jurusan", "Fakultas"}, rows);
            }
            case "mahasiswa" -> {
                List<String[]> rows = mahasiswaRepository.findAll().stream()
                        .map(item -> new String[] {
                                text(item.getId()),
                                item.getNim(),
                                item.getNama(),
                                item.getEmail(),
                                item.getJurusan() == null ? "" : item.getJurusan().getNamaJurusan(),
                                item.getJurusan() == null ? "" : item.getJurusan().getFakultas()
                        })
                        .toList();
                yield withHeader(new String[] {"ID", "NIM", "Nama", "Email", "Jurusan", "Fakultas"}, rows);
            }
            default -> throw new BadRequestException(Map.of("resource", "Resource export harus jurusan atau mahasiswa"));
        };
    }

    private byte[] toCsv(List<String[]> rows) {
        StringBuilder builder = new StringBuilder();
        for (String[] row : rows) {
            for (int i = 0; i < row.length; i++) {
                if (i > 0) {
                    builder.append(',');
                }
                builder.append(csvValue(row[i]));
            }
            builder.append(System.lineSeparator());
        }
        return builder.toString().getBytes(StandardCharsets.UTF_8);
    }

    private byte[] toJson(String resource) {
        try {
            Object data = "jurusan".equals(resource) ? jurusanRepository.findAll() : mahasiswaRepository.findAll();
            return objectMapper.writerWithDefaultPrettyPrinter().writeValueAsBytes(data);
        } catch (Exception ex) {
            throw new IllegalStateException("Gagal membuat export JSON", ex);
        }
    }

    private byte[] toExcel(String resource, List<String[]> rows) {
        try (XSSFWorkbook workbook = new XSSFWorkbook(); ByteArrayOutputStream output = new ByteArrayOutputStream()) {
            Sheet sheet = workbook.createSheet(title(resource));
            CellStyle headerStyle = workbook.createCellStyle();
            org.apache.poi.ss.usermodel.Font headerFont = workbook.createFont();
            headerFont.setBold(true);
            headerStyle.setFont(headerFont);

            for (int rowIndex = 0; rowIndex < rows.size(); rowIndex++) {
                Row sheetRow = sheet.createRow(rowIndex);
                String[] sourceRow = rows.get(rowIndex);
                for (int columnIndex = 0; columnIndex < sourceRow.length; columnIndex++) {
                    Cell cell = sheetRow.createCell(columnIndex);
                    cell.setCellValue(nullToEmpty(sourceRow[columnIndex]));
                    if (rowIndex == 0) {
                        cell.setCellStyle(headerStyle);
                    }
                }
            }

            for (int i = 0; i < rows.get(0).length; i++) {
                sheet.autoSizeColumn(i);
            }
            workbook.write(output);
            return output.toByteArray();
        } catch (Exception ex) {
            throw new IllegalStateException("Gagal membuat export Excel", ex);
        }
    }

    private byte[] toPdf(String resource, List<String[]> rows) {
        try (ByteArrayOutputStream output = new ByteArrayOutputStream()) {
            Document document = new Document(PageSize.A4.rotate());
            PdfWriter.getInstance(document, output);
            document.open();

            Font titleFont = FontFactory.getFont(FontFactory.HELVETICA_BOLD, 16);
            document.add(new Paragraph("Export Data " + title(resource), titleFont));
            document.add(new Paragraph(" "));

            PdfPTable table = new PdfPTable(rows.get(0).length);
            table.setWidthPercentage(100);
            Font headerFont = FontFactory.getFont(FontFactory.HELVETICA_BOLD, 10);
            Font bodyFont = FontFactory.getFont(FontFactory.HELVETICA, 9);

            for (int rowIndex = 0; rowIndex < rows.size(); rowIndex++) {
                for (String value : rows.get(rowIndex)) {
                    PdfPCell cell = new PdfPCell(new Phrase(nullToEmpty(value), rowIndex == 0 ? headerFont : bodyFont));
                    cell.setPadding(6);
                    table.addCell(cell);
                }
            }

            document.add(table);
            document.close();
            return output.toByteArray();
        } catch (Exception ex) {
            throw new IllegalStateException("Gagal membuat export PDF", ex);
        }
    }

    private static List<String[]> withHeader(String[] header, List<String[]> rows) {
        java.util.ArrayList<String[]> allRows = new java.util.ArrayList<>();
        allRows.add(header);
        allRows.addAll(rows);
        return allRows;
    }

    private static String csvValue(String value) {
        String safe = nullToEmpty(value);
        if (safe.contains(",") || safe.contains("\"") || safe.contains("\n") || safe.contains("\r")) {
            return "\"" + safe.replace("\"", "\"\"") + "\"";
        }
        return safe;
    }

    private static String fileName(String resource, String extension) {
        return "export-" + resource + "." + extension;
    }

    private static String normalize(String value) {
        return value == null ? "" : value.trim().toLowerCase();
    }

    private static String title(String resource) {
        return "jurusan".equals(resource) ? "Jurusan" : "Mahasiswa";
    }

    private static String nullToEmpty(String value) {
        return value == null ? "" : value;
    }

    private static String text(Object value) {
        return value == null ? "" : String.valueOf(value);
    }

    public record ExportFile(String fileName, String contentType, byte[] content) {
    }
}
