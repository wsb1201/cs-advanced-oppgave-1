# Biblioteksystem 📚

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
