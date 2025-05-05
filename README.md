# Biblioteka_Online# 📚 Biblioteka Online

Aplikacja webowa umożliwiająca użytkownikom wypożyczanie książek z biblioteki, a administratorowi zarządzanie zasobami i wypożyczeniami.

---

## 🚀 Funkcje

### Dla użytkowników:
- Rejestracja i logowanie (karta biblioteczna)
- Przeglądanie dostępnych książek
- Podstrona książki z opisem, ilością dostępnych egzemplarzy i terminami zwrotów
- Podgląd własnych wypożyczeń i dat ich zwrotu

### Dla administratora:
- Zarządzanie bazą książek (dodawanie, edytowanie, usuwanie)
- Przypisywanie książek użytkownikom
- Ustalanie terminów zwrotu
- Panel administracyjny

---

## 🛠️ Stack technologiczny

- **Backend**: ASP.NET Core 8 (MVC)
- **ORM**: Entity Framework Core 8
- **Frontend**: React (skompilowany do `/wwwroot`)
- **Baza danych**: SQLite / SQL Server (do wyboru)

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

4. Uruchom aplikację:
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
  - `Borrowings`: relacja 1:N z `Borrowing`

- **Borrowing**
  - `Id`: int (PK)
  - `UserId`: int (FK)
  - `BookId`: int (FK)
  - `BorrowDate`: DateTime
  - `ReturnDate`: DateTime
  - `Returned`: bool

---

## 📦 Deployment

Pliki React (frontend) umieszczone w katalogu `/wwwroot`, nie wymagają uruchamiania node.js. Aplikacja jest gotowa do uruchomienia jednym poleceniem `dotnet run`.

---

## 📜 Licencja

Projekt na potrzeby kursu zaliczeniowego – użytek edukacyjny.
