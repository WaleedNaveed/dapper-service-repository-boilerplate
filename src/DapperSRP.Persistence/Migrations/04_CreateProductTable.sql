CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Quantity INT NULL,
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedBy INT NOT NULL,
    UpdatedAt DATETIME NOT NULL,
    CONSTRAINT FK_Products_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
    CONSTRAINT FK_Products_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(Id)
);
