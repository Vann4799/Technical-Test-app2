package com.example.app.exception;

import java.util.Map;

public class BadRequestException extends RuntimeException {

    private final Map<String, String> errors;

    public BadRequestException(Map<String, String> errors) {
        super("Validation failed");
        this.errors = errors;
    }

    public Map<String, String> getErrors() {
        return errors;
    }
}
