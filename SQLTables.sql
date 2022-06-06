CREATE TABLE user (
    uid VARCHAR(100) PRIMARY KEY NOT NULL UNIQUE,
    username VARCHAR(20) NOT NULL,
    tag VARCHAR(5) NOT NULL,
    friends JSON DEFAULT NULL,
    email VARCHAR(30) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    disabled TINYINT(1) NOT NULL DEFAULT 0,
    email_verified TINYINT(1) NOT NULL DEFAULT 0
);

CREATE TABLE note (
    nid VARCHAR(100) PRIMARY KEY NOT NULL UNIQUE,
    uid VARCHAR(100) NOT NULL,
    content JSON DEFAULT(NULL), --title, content, categories[]
    creation_date DATETIME NOT NULL,
    visibility ENUM('Public', 'Private') DEFAULT('Private'),
    last_edit DATETIME NOT NULL,
    FOREIGN KEY note(uid) REFERENCES user(uid)
);