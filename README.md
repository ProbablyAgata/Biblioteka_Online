# 📚 Biblioteka Online

Aplikacja webowa umożliwiająca użytkownikom wypożyczanie książek z biblioteki, a administratorowi zarządzanie zasobami i wypożyczeniami. Zintegrowana z Google Books API do wyszukiwania i importowania książek.

---

## 🚀 Funkcje

### Dla użytkowników:
- Rejestracja i logowanie (karta biblioteczna)
- Przeglądanie dostępnych książek
- Podstrona książki z opisem, ilością dostępnych egzemplarzy i terminami zwrotów
- Podgląd własnych wypożyczeń i dat ich zwrotu

### Dla administratora:
- Zarządzanie bazą książek (dodawanie, edytowanie, usuwanie)
- Wyszukiwanie i importowanie książek z Google Books API
- Przypisywanie książek użytkownikom
- Ustalanie terminów zwrotu
- Panel administracyjny

---

## 🛠️ Stack technologiczny

- **Backend**: ASP.NET Core 8 (MVC)
- **ORM**: Entity Framework Core 8
- **Frontend**: Bootstrap 5, jQuery
- **Baza danych**: SQLite
- **API**: Google Books API

---

## 🧪 Uruchomienie aplikacji

1. Sklonuj repozytorium:
    ```bash
    git clone https://github.com/twoj-login/biblioteka-online.git
    ```

2. Przejdź do katalogu projektu:
    ```bash
    cd biblioteka-online
    ```

3. W terminalu Visual Studio:
    - Zastosuj migracje:
      ```bash
      dotnet ef database update
      ```
    - (Opcjonalnie) uruchom seeder danych testowych

4. Konfiguracja Google Books API (opcjonalnie):
   - Uzyskaj klucz API z [Google Cloud Console](https://console.cloud.google.com/)
   - Dodaj klucz do pliku `appsettings.json` w sekcji `GoogleBooks:ApiKey`

5. Uruchom aplikację:
    ```bash
    dotnet run
    ```

---

## 👥 Role użytkowników

- `User` – czytelnik, może wypożyczać i przeglądać książki
- `Admin` – administrator, może zarządzać książkami i wypożyczeniami

---

## ✅ Konto testowe

**Admin:**
- Login: `admin@admin.com`
- Hasło: `Admin123!`

**Użytkownik:**
- Login: `user@user.com`
- Hasło: `User123!`

---

## 🧱 Struktura bazy danych

- **User**
  - `Id`: int (PK)
  - `Email`: string
  - `PasswordHash`: string
  - `Role`: string (`User`, `Admin`)
  - `Borrowings`: relacja 1:N z `Borrowing`

- **Book**
  - `Id`: int (PK)
  - `Title`: string
  - `Author`: string
  - `Description`: string
  - `TotalCopies`: int
  - `GoogleBookId`: string
  - `ISBN`: string
  - `Publisher`: string
  - `PublishedDate`: string
  - `PageCount`: int
  - `Categories`: string
  - `Language`: string
  - `ThumbnailUrl`: string
  - `PreviewLink`: string
  - `Borrowings`: relacja 1:N z `Borrowing`

- **Borrowing**
  - `Id`: int (PK)
  - `UserId`: int (FK)
  - `BookId`: int (FK)
  - `BorrowDate`: DateTime
  - `ReturnDate`: DateTime
  - `Returned`: bool

---

## 🔍 Integracja z Google Books API

Aplikacja oferuje następujące funkcje związane z Google Books API:

- Wyszukiwanie książek po tytule, autorze lub ISBN
- Przeglądanie szczegółowych informacji o książkach
- Importowanie książek do biblioteki z zachowaniem wszystkich metadanych
- Wyświetlanie okładek i linków do podglądu książek

Funkcje API dostępne są dla administratorów w panelu "Google Books".

## 📁 Struktura projektu

- **Controllers/** - Kontrolery MVC i API
  - **Api/** - Kontrolery Web API (np. GoogleBooksController)
  - AccountController.cs - Obsługa logowania i rejestracji
  - BooksController.cs - Zarządzanie książkami
  - BorrowingsController.cs - Zarządzanie wypożyczeniami
  - HomeController.cs - Strona główna i dashboard

- **Models/** - Modele danych
  - **GoogleBooks/** - Modele dla Google Books API
  - Book.cs - Model książki
  - Borrowing.cs - Model wypożyczenia
  - User.cs - Model użytkownika

- **Services/** - Usługi aplikacji
  - GoogleBooksService.cs - Obsługa komunikacji z Google Books API

- **Views/** - Widoki MVC
  - **Books/** - Widoki dla książek
  - **Borrowings/** - Widoki dla wypożyczeń
  - **Home/** - Widoki dla strony głównej
  - **Shared/** - Współdzielone elementy widoków

- **Areas/** - Obszary aplikacji
  - **Identity/** - Obsługa uwierzytelniania użytkowników

---

## 📦 Deployment

Aplikacja jest gotowa do uruchomienia jednym poleceniem `dotnet run`. Używa wbudowanego serwera Kestrel.

---

## 📜 Licencja

Projekt na potrzeby kursu zaliczeniowego – użytek edukacyjny.
