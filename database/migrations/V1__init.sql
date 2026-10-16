CREATE TABLE vehicle (
    id CHAR(36) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
    type VARCHAR(30) NOT NULL DEFAULT 'CAR',
    brand VARCHAR(100) NOT NULL,
    model VARCHAR(150) NOT NULL,
    license_plate VARCHAR(20) NOT NULL,
    color VARCHAR(50) NOT NULL,
    status VARCHAR(30) NOT NULL DEFAULT 'AVAILABLE',
    price DECIMAL(12, 2) NOT NULL,
    year YEAR NOT NULL,
    created_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    updated_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),

    PRIMARY KEY (id),
    UNIQUE KEY uk_vehicle_license_plate (license_plate),
    KEY idx_vehicle_status_price (status, price)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
