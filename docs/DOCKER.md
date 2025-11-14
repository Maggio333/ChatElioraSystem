# 🐳 Docker Setup

Ten dokument opisuje sposób uruchomienia Qdrant (bazy wektorowej) przy użyciu Docker Compose.

**Uwaga:** ChatElioraSystem to aplikacja desktopowa (WPF) i mobilna (MAUI), która wymaga GUI. Aplikacja **nie może być uruchomiona w kontenerze Docker** - tylko Qdrant może działać w Dockerze.

## Wymagania

- [Docker](https://www.docker.com/get-started) (wersja 20.10 lub nowsza)
- [Docker Compose](https://docs.docker.com/compose/install/) (wersja 2.0 lub nowsza)

## Szybki start

### 1. Uruchom Qdrant

```bash
# Uruchom Qdrant w tle
docker-compose up -d

# Sprawdź status
docker-compose ps

# Zobacz logi
docker-compose logs -f qdrant
```

### 2. Sprawdź czy działa

```bash
# Health check
curl http://localhost:6333/health

# Dashboard
# Otwórz w przeglądarce: http://localhost:6333/dashboard
```

### 3. Zatrzymaj Qdrant

```bash
# Zatrzymaj kontenery
docker-compose down

# Zatrzymaj i usuń dane (⚠️ usuwa wszystkie dane!)
docker-compose down -v
```

## Konfiguracja

### Porty

Domyślnie Qdrant używa następujących portów:
- **6333** - REST API
- **6334** - gRPC

Możesz zmienić porty w pliku `docker-compose.yml`:

```yaml
ports:
  - "6333:6333"  # Zmień pierwszy numer na inny port
  - "6334:6334"
```

### Persystencja danych

Dane Qdrant są przechowywane w wolumenie Docker `qdrant_storage`. Dane są zachowywane nawet po zatrzymaniu kontenera.

Aby usunąć wszystkie dane:
```bash
docker-compose down -v
```

## Troubleshooting

### Port już zajęty

Jeśli port 6333 lub 6334 jest już zajęty:

1. Sprawdź co używa portu:
   ```bash
   # Windows
   netstat -ano | findstr :6333
   
   # Linux/macOS
   lsof -i :6333
   ```

2. Zmień port w `docker-compose.yml` lub zatrzymaj proces używający portu

### Kontener nie startuje

Sprawdź logi:
```bash
docker-compose logs qdrant
```

### Reset Qdrant

Aby całkowicie zresetować Qdrant (usuwa wszystkie dane):
```bash
docker-compose down -v
docker-compose up -d
```

## Integracja z aplikacją

Po uruchomieniu Qdrant w Dockerze, aplikacja automatycznie połączy się z nim na `http://localhost:6333`.

Kolekcje (`PierwszaKolekcjaOnline` i `TopicCollection`) są tworzone automatycznie przy pierwszym uruchomieniu aplikacji.

## ⚠️ Ograniczenia

ChatElioraSystem to aplikacja desktopowa (WPF) i mobilna (MAUI), która wymaga interfejsu graficznego. **Aplikacja nie może być uruchomiona w kontenerze Docker**.

### Co można uruchomić w Dockerze:

✅ **Qdrant** - baza wektorowa (w `docker-compose.yml`)

### Co NIE można uruchomić w Dockerze:

❌ **Aplikacja WPF** - wymaga Windows GUI  
❌ **Aplikacja MAUI** - wymaga natywnej platformy  
❌ **LM Studio** - działa lokalnie na hoście

## 📝 Rekomendacja

Dla pełnego środowiska deweloperskiego:

1. **Qdrant** - uruchom w Dockerze (`docker-compose up -d qdrant`)
2. **LM Studio** - uruchom lokalnie na hoście (port 8123)
3. **Aplikacja WPF/MAUI** - uruchom lokalnie w Visual Studio lub Rider
4. **Testy** - uruchom lokalnie (`dotnet test`)

## Alternatywy

Jeśli nie chcesz używać Dockera, możesz zainstalować Qdrant lokalnie zgodnie z [oficjalną dokumentacją](https://qdrant.tech/documentation/).

