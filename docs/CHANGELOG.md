# Changelog

Wszystkie znaczące zmiany w projekcie ChatElioraSystem będą dokumentowane w tym pliku.

Format jest oparty na [Keep a Changelog](https://keepachangelog.com/pl/1.0.0/),
a projekt przestrzega [Semantic Versioning](https://semver.org/lang/pl/).

## [Unreleased]

### Dodane
- Testy jednostkowe (23 testy)
- Dokumentacja architektury (ARCHITECTURE.md)
- CI/CD pipeline (GitHub Actions)
- CONTRIBUTING.md - przewodnik współpracy
- CODE_OF_CONDUCT.md - kodeks postępowania
- Szczegółowa dokumentacja przepływu danych w README

### Zmienione
- Usunięte hardcoded wartości (IP, DNS, ścieżki)
- Wszystkie prompty JSON są teraz embedded w kodzie (raw string literals)
- Zaktualizowana dokumentacja

### Naprawione
- Poprawki w konfiguracji ścieżek (użycie AppDomain.CurrentDomain.BaseDirectory)

## [1.0.0] - 2025-01-XX

### Dodane
- Podstawowa funkcjonalność czatu z AI
- Integracja z LM Studio (lokalne LLM)
- System RAG z Qdrant
- Aplikacja desktopowa (WPF)
- Aplikacja mobilna (MAUI)
- Automatyczna kategoryzacja rozmów
- Różne tryby promptów (Ogólna, Kod, Refleksyjna, ArchitekturaKodu)
- Streaming odpowiedzi w czasie rzeczywistym
- Formatowanie tekstu z kolorami
- Zarządzanie tematami rozmów
- Zapis/odczyt konwersacji do plików JSON
- Prompt Crossing - porównywanie odpowiedzi z różnych LLM
- System oceny odpowiedzi

### Funkcjonalności
- Clean Architecture z wyraźnym podziałem warstw
- Dependency Injection (Microsoft.Extensions.DependencyInjection)
- MVVM pattern dla UI
- Strategy Pattern dla różnych typów promptów
- Repository Pattern dla dostępu do danych
- Własny format JSON (MCP-inspired) dla operacji RAG

---

**Uwaga**: To jest pierwsza wersja projektu przygotowana do open source. Wszystkie funkcjonalności są w pełni działające, choć projekt jest w aktywnej fazie rozwoju.

