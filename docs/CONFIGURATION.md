# Konfiguracja ChatElioraSystem

Ten dokument opisuje sposób konfiguracji aplikacji ChatElioraSystem.

## Zmienne środowiskowe

Aplikacja może być konfigurowana za pomocą zmiennych środowiskowych:

### LM Studio
- `LM_STUDIO_URL` - Adres URL serwera LM Studio (domyślnie: `http://localhost:8123`)
- `LM_STUDIO_PORT` - Port serwera LM Studio (domyślnie: `8123`)

### Qdrant
- `QDRANT_REST_URL` - Adres URL REST API Qdrant (domyślnie: `http://localhost:6333`)
- `QDRANT_GRPC_URL` - Adres URL gRPC Qdrant (domyślnie: `http://localhost:6334`)

**Kolekcje Qdrant:**
System automatycznie tworzy następujące kolekcje przy pierwszym uruchomieniu:
- `PierwszaKolekcjaOnline` - główna kolekcja dla kontekstu rozmów (1024 wymiarów, Cosine)
- `TopicCollection` - kolekcja tematów rozmów (1024 wymiarów, Cosine, z indeksem Timestamp)

Kolekcje są tworzone automatycznie przez metodę `InitializeAsync()` w `QdrantVectorDbService`, która jest wywoływana przy starcie aplikacji.

### Tailscale
- `TAILSCALE_DNS` - Nazwa DNS urządzenia w Tailscale (np. `your-device.tailXXXXXX.ts.net`)
- `TAILSCALE_USE_CLI` - Czy używać CLI Tailscale do automatycznego wykrywania IP (domyślnie: `true`)

## Konfiguracja dla różnych środowisk

### Development (Lokalne)
Domyślnie aplikacja używa `localhost` dla wszystkich serwisów:
- LM Studio: `http://localhost:8123`
- Qdrant: `http://localhost:6333` (REST), `http://localhost:6334` (gRPC)

### Production (Z Tailscale)
1. Skonfiguruj Tailscale DNS w `TailscaleService.cs` lub ustaw zmienną środowiskową `TAILSCALE_DNS`
2. Upewnij się, że wszystkie urządzenia są w tej samej sieci Tailscale
3. Zaktualizuj adresy serwerów w konfiguracji

### Mobile (Android Emulator)
Dla emulatora Android użyj adresu `10.0.2.2` zamiast `localhost`:
- LM Studio: `http://10.0.2.2:8123`
- Qdrant: `http://10.0.2.2:6333`

## Przykładowa konfiguracja

Skopiuj plik `appsettings.example.json` do `appsettings.json` i dostosuj wartości:

```bash
cp appsettings.example.json appsettings.json
```

Następnie edytuj `appsettings.json` zgodnie z Twoim środowiskiem.

## Uwagi bezpieczeństwa

⚠️ **WAŻNE**: Nigdy nie commituj pliku `appsettings.json` do repozytorium! Zawiera on wrażliwe dane konfiguracyjne.

Plik `appsettings.json` jest już dodany do `.gitignore` i nie będzie commitowany.

