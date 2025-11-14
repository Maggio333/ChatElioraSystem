# Przewodnik współpracy (Contributing Guide)

Dziękujemy za zainteresowanie projektem ChatElioraSystem! 🎉

## Jak możesz pomóc?

### Zgłaszanie błędów

Jeśli znalazłeś błąd:

1. Sprawdź czy nie został już zgłoszony w [Issues](https://github.com/Maggio333/ChatElioraSystem/issues)
2. Utwórz nowe issue z:
   - Opisem problemu
   - Krokami do reprodukcji
   - Oczekiwanym zachowaniem
   - Rzeczywistym zachowaniem
   - Informacjami o środowisku (.NET wersja, OS, itp.)
   - Logami (jeśli dostępne)

### Proponowanie nowych funkcji

1. Sprawdź czy funkcja nie została już zaproponowana
2. Utwórz issue z:
   - Opisem funkcjonalności
   - Uzasadnieniem dlaczego jest potrzebna
   - Przykładami użycia
   - Ewentualnymi propozycjami implementacji

### Wysyłanie Pull Requestów

#### Proces

1. **Fork** repozytorium
2. **Sklonuj** swoje forka:
   ```bash
   git clone https://github.com/Maggio333/ChatElioraSystem.git
   cd ChatElioraSystem
   ```
3. **Utwórz branch** dla swojej zmiany:
   ```bash
   git checkout -b feature/nazwa-funkcji
   # lub
   git checkout -b fix/opis-bledu
   ```
4. **Wprowadź zmiany** zgodnie z wytycznymi poniżej
5. **Uruchom testy**:
   ```bash
   dotnet test
   ```
6. **Commit** zmiany:
   ```bash
   git add .
   git commit -m "Opis zmiany"
   ```
7. **Push** do swojego forka:
   ```bash
   git push origin feature/nazwa-funkcji
   ```
8. **Utwórz Pull Request** na GitHubie

#### Wytyczne dotyczące kodu

##### Nazewnictwo

- **Język**: Używamy języka polskiego dla:
  - Komentarzy w kodzie
  - Nazw zmiennych i metod (gdzie to możliwe)
  - Komunikatów błędów
  - Dokumentacji
- **Angielski**: Używamy dla:
  - Nazw klas i interfejsów (standard .NET)
  - Nazw przestrzeni nazw
  - Komunikatów commitów (opcjonalnie)

##### Struktura kodu

- **Clean Architecture**: Przestrzegaj podziału na warstwy:
  - `Domain` - logika biznesowa, modele, interfejsy
  - `Application` - use case'y, serwisy aplikacyjne
  - `Infrastructure` - implementacje zewnętrzne (LLM, Qdrant, pliki)
  - `Presentation` - UI (WPF/MAUI)

##### Testy

- **Dodaj testy** dla nowych funkcjonalności
- **Utrzymuj pokrycie testami** na poziomie co najmniej 70% dla nowego kodu
- **Używaj polskich nazw** dla testów (np. `Powinien_Zwrocic_Wiadomosc`)
- **Struktura AAA**: Arrange-Act-Assert

##### Komentarze

- Komentarze po polsku
- Dokumentuj złożoną logikę biznesową
- Używaj XML comments dla publicznych API:
  ```csharp
  /// <summary>
  /// Zapisuje wiadomości do pliku JSON
  /// </summary>
  /// <param name="messages">Kolekcja wiadomości do zapisania</param>
  /// <returns>Ścieżka do zapisanego pliku</returns>
  public string Save(IEnumerable<IChatMessage> messages)
  ```

##### Formatowanie

- Używaj automatycznego formatowania IDE (Ctrl+K, Ctrl+D w Visual Studio)
- Przestrzegaj konwencji C# (PascalCase dla klas, camelCase dla zmiennych lokalnych)
- Maksymalna długość linii: 120 znaków

#### Przykład dobrego commita

```
feat: Dodano obsługę nowego typu promptu "Analiza"

- Utworzono PromptAnalysisService
- Dodano SesjaTematu.Analiza
- Dodano testy jednostkowe
- Zaktualizowano dokumentację
```

#### Typy commitów

- `feat:` - Nowa funkcjonalność
- `fix:` - Naprawa błędu
- `docs:` - Zmiany w dokumentacji
- `test:` - Dodanie/zmiana testów
- `refactor:` - Refaktoryzacja kodu
- `style:` - Zmiany formatowania
- `chore:` - Zmiany w build/konfiguracji

### Testowanie przed PR

Przed wysłaniem Pull Requesta upewnij się, że:

- ✅ Wszystkie testy przechodzą (`dotnet test`)
- ✅ Kod się kompiluje bez błędów
- ✅ Nie ma ostrzeżeń kompilatora (lub są uzasadnione)
- ✅ Kod jest sformatowany
- ✅ Dodano testy dla nowych funkcjonalności
- ✅ Zaktualizowano dokumentację (jeśli potrzeba)

### Code Review

- Bądź otwarty na feedback
- Odpowiadaj na komentarze konstruktywnie
- Nie bierz krytyki osobiście - to o kod, nie o Tobie
- Jeśli nie zgadzasz się z sugestią, wyjaśnij dlaczego

**Uwaga**: Projekt jest obecnie zarządzany przez jednego autora. Code review będzie wykonywany przez autora projektu, który może potrzebować czasu na przejrzenie zmian.

### Pytania?

Jeśli masz pytania:
- Utwórz issue z pytaniem
- Sprawdź dokumentację w README.md
- Sprawdź istniejące Issues i PR

---

**Dziękujemy za wkład w rozwój projektu!** 🙏

