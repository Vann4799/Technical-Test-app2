package com.example.app.dto;

import java.util.Map;

public record ErrorResponse(
        boolean success,
        String message,
        Map<String, String> errors
) {
    public static ErrorResponse validation(Map<String, String> errors) {
        return new ErrorResponse(false, "Validation failed", errors);
    }
}
