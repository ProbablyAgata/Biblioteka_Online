# 📚 Biblioteka Online

Nowoczesna aplikacja webowa umożliwiająca użytkownikom wypożyczanie książek z biblioteki online, a administratorowi kompleksowe zarządzanie zasobami i wypożyczeniami. System zintegrowany z Google Books API do wyszukiwania i automatycznego importowania książek z pełnymi metadanymi.

---

## 🌟 Główne funkcje

### 👤 Dla czytelników:
- **Bezpieczna rejestracja i logowanie** z kartą biblioteczną
- **Intuicyjne przeglądanie** dostępnych książek z zaawansowanymi filtrami
- **Szczegółowe podstrony książek** z opisem, okładkami i informacją o dostępności
- **Zarządzanie wypożyczeniami** - podgląd aktualnych wypożyczeń i terminów zwrotu
- **Historia wypożyczeń** z możliwością oceny przeczytanych książek

### 🔧 Dla administratora:
- **Kompleksne zarządzanie biblioteką** (dodawanie, edytowanie, usuwanie książek)
- **Integracja z Google Books API** - automatyczne importowanie książek z metadanymi
- **Zarządzanie użytkownikami** i ich uprawnieniami
- **System wypożyczeń** - przypisywanie książek i ustalanie terminów zwrotu
- **Panel administracyjny** z raportami i statystykami
- **Powiadomienia email** o zbliżających się terminach zwrotu

---

## 🛠️ Stack technologiczny

- **Backend**: ASP.NET Core 8.0 (MVC + Web API)
- **Framework**: .NET 8.0
- **ORM**: Entity Framework Core 8.0.2
- **Uwierzytelnianie**: ASP.NET Core Identity
- **Frontend**: Bootstrap 5, jQuery, Responsive Design
- **Baza danych**: SQLite (produkcja gotowa na SQL Server/PostgreSQL)
- **API zewnętrzne**: Google Books API
- **Email**: Konfigurowalny system powiadomień

---

## 📦 Wymagania systemowe

- **.NET 8.0 SDK** lub nowszy
- **Visual Studio 2022** lub **Visual Studio Code** (zalecane)
- **Git** dla klonowania repozytorium

---

## 🚀 Szybkie uruchomienie

### 1. Klonowanie projektu
```bash
git clone [URL_REPOZYTORIUM]
cd Biblioteka_Online
```

### 2. Przywracanie pakietów
```bash
dotnet restore
```

### 3. Migracja bazy danych
```bash
dotnet ef database update
```

### 4. (Opcjonalnie) Konfiguracja Google Books API
1. Uzyskaj klucz API z [Google Cloud Console](https://console.cloud.google.com/)
2. Włącz Google Books API dla swojego projektu
3. Dodaj klucz do `appsettings.json`:
```json
{
  "GoogleBooks": {
    "ApiKey": "TWÓJ_KLUCZ_API"
  }
}
```

### 5. Uruchomienie aplikacji
```bash
dotnet run
```

Aplikacja będzie dostępna pod adresem: `https://localhost:5179` (port zostanie wyświetlony w konsoli)

---

## 👥 System ról i uprawnień

### 🔑 Role użytkowników:
- **User** – Czytelnik z możliwością wypożyczania i przeglądania książek
- **Admin** – Administrator z pełnymi uprawnieniami do zarządzania systemem

### 🧪 Konta testowe:

**Administrator:**
- Email: `admin@admin.com`
- Hasło: `Admin123!`

**Użytkownik:**
- Email: `user@user.com`
- Hasło: `User123!`

> **Uwaga:** Konta testowe są automatycznie tworzone przy pierwszym uruchomieniu aplikacji

---

## 🗄️ Architektura bazy danych

### Główne encje:

#### **User** (Użytkownik)
- `Id`: int (PK) - Unikalny identyfikator
- `Email`: string - Adres email (login)
- `PasswordHash`: string - Zahashowane hasło
- `Role`: string - Rola użytkownika
- `Borrowings`: Navigation Property → Wypożyczenia

#### **Book** (Książka)
- `Id`: int (PK) - Unikalny identyfikator
- `Title`: string - Tytuł książki
- `Author`: string - Autor
- `Description`: string - Opis
- `TotalCopies`: int - Liczba egzemplarzy
- `ISBN`: string - Numer ISBN
- `Publisher`: string - Wydawnictwo
- `PublishedDate`: string - Data publikacji
- `PageCount`: int - Liczba stron
- `Categories`: string - Kategorie
- `Language`: string - Język
- `ThumbnailUrl`: string - URL okładki
- `PreviewLink`: string - Link do podglądu
- `GoogleBookId`: string - ID w Google Books

#### **Borrowing** (Wypożyczenie)
- `Id`: int (PK) - Unikalny identyfikator
- `UserId`: int (FK) → User - Wypożyczający
- `BookId`: int (FK) → Book - Wypożyczona książka
- `BorrowDate`: DateTime - Data wypożyczenia
- `ReturnDate`: DateTime - Planowana data zwrotu
- `Returned`: bool - Status zwrotu

---

## 🔍 Funkcje Google Books API

System oferuje zaawansowaną integrację z Google Books API:

### 🔎 **Wyszukiwanie książek**
- Wyszukiwanie po tytule, autorze, ISBN
- Filtrowanie po kategorii i języku
- Podgląd szczegółowych informacji

### 📥 **Import książek**
- Automatyczne pobieranie metadanych
- Import okładek w wysokiej jakości
- Zachowanie linków do podglądu książek
- Batch import wielu książek jednocześnie

### 🖼️ **Dodatkowe funkcje**
- Wyświetlanie okładek książek
- Linki do podglądu w Google Books
- Automatyczne uzupełnianie danych książek

---

## 📁 Szczegółowa struktura projektu

```
Biblioteka_Online/
├── Controllers/              # Kontrolery MVC i Web API
│   ├── Api/                 # Kontrolery Web API
│   │   └── GoogleBooksController.cs
│   ├── AccountController.cs # Rejestracja i logowanie
│   ├── BooksController.cs   # Zarządzanie książkami
│   ├── BorrowingsController.cs # Zarządzanie wypożyczeniami
│   └── HomeController.cs    # Strona główna
├── Models/                  # Modele danych i ViewModels
│   ├── GoogleBooks/         # Modele dla Google Books API
│   ├── Book.cs             # Model książki
│   ├── Borrowing.cs        # Model wypożyczenia
│   ├── User.cs             # Model użytkownika
│   ├── LoginModel.cs       # ViewModel logowania
│   └── RegisterModel.cs    # ViewModel rejestracji
├── Services/               # Usługi biznesowe
│   ├── GoogleBooksService.cs # Komunikacja z Google Books API
│   └── EmailSender.cs      # Wysyłanie powiadomień email
├── Data/                   # Kontekst bazy danych i seeder
├── Views/                  # Widoki MVC (Razor)
│   ├── Books/             # Widoki dla książek
│   ├── Borrowings/        # Widoki dla wypożyczeń
│   ├── Home/              # Strona główna i dashboard
│   └── Shared/            # Wspólne komponenty UI
├── Areas/                  # Obszary aplikacji
│   └── Identity/          # Widoki dla uwierzytelniania
├── wwwroot/               # Pliki statyczne (CSS, JS, obrazy)
├── Migrations/            # Migracje Entity Framework
└── Properties/            # Konfiguracja uruchomienia
```

---

## ⚙️ Konfiguracja i ustawienia

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=biblioteka.db"
  },
  "GoogleBooks": {
    "ApiKey": "[TWÓJ_KLUCZ_API]",
    "BaseUrl": "https://www.googleapis.com/books/v1/"
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "Username": "[EMAIL]",
    "Password": "[HASŁO_APLIKACJI]"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

---

## 🚀 Deployment i produkcja

### Przygotowanie do wdrożenia:
1. Zmień connection string na produkcyjną bazę danych
2. Ustaw środowisko na `Production`
3. Skonfiguruj HTTPS i certyfikaty SSL
4. Ustaw właściwe klucze API i hasła
5. Skonfiguruj system kopii zapasowych

### Obsługiwane bazy danych:
- **SQLite** (development/małe wdrożenia)
- **SQL Server** (zalecane dla produkcji)
- **PostgreSQL** (alternatywa open-source)
- **MySQL** (z drobnymi modyfikacjami)

---

## 🔧 Rozwój i kontrybucje

### Uruchomienie środowiska deweloperskiego:
```bash
# Klonowanie repozytorium
git clone [URL]
cd Biblioteka_Online

# Instalacja zależności
dotnet restore

# Uruchomienie w trybie deweloperskim
dotnet run --environment Development
```

### Testowanie:
```bash
# Uruchomienie testów jednostkowych
dotnet test

# Sprawdzenie stylu kodu
dotnet format --verify-no-changes
```

---

## 📄 Licencja

Ten projekt został stworzony na potrzeby edukacyjne jako część kursu ASP.NET Core. 

**Licencja MIT** - szczegóły w pliku `LICENSE`

---

## 📞 Wsparcie

W przypadku problemów lub pytań:
1. Sprawdź [Issues](../../issues) w repozytorium
2. Przeczytaj dokumentację ASP.NET Core
3. Sprawdź logi aplikacji w katalogu `Logs/`

---

**Wersja README:** 2.0  
**Ostatnia aktualizacja:** Grudzień 2024  
**Kompatybilność:** .NET 8.0+
