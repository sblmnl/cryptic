CREATE TABLE notes (
    id UUID NOT NULL PRIMARY KEY,
    content TEXT,
    delete_after INT NOT NULL,
    created_at TIMESTAMP NOT NULL
);
