# Biblioteksystem 📚

## REST API

```
POST /books
Request: {"title":"","author":""}
Response:
 - Status 201 Created
 - Returns uuid string
```

```
GET /books/{id}
Response:
 - Status 201 Created
 - Returns {"title":"","author":"","available":true}
```

```
POST /loans
Request: {"bookId":"","patron":"","expiryDate":""}
Response:
 - Status 201 Created
 - Returns uuid string
```

```
DELETE /loans/{id}
Response:
 - Status 200 Ok
 - Returns {"patron":"","late":true}
```

## Sekvensdiagram (MVP)

```mermaid
sequenceDiagram
    actor Ansatt as Bibliotekansatt
    participant System as Biblioteksystem
    participant BokRegister as Bokregister

    Ansatt->>System: Registrer bok(tittel, forfatter)
    System->>BokRegister: Opprett bok
    BokRegister-->>Ansatt: Vis bok-ID

    Ansatt->>System: Lån ut bok(bok-ID, låntakernavn)
    System->>BokRegister: Finn bok(bok-ID)
    BokRegister-->>System: Returner bok

    alt Bok tilgjengelig
        System->>BokRegister: Sett bokstatus til Utlånt
        System-->>Ansatt: Utlån registrert
    else Bok utlånt
        System-->>Ansatt: Utlån avvist
    end
```
