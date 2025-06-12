# Aplikacja do śledzenia koncertów

### Opis
Aplikacja do śledzenia koncertów to proste i intuicyjne narzędzie, które umożliwia użytkownikom szybkie wyszukiwanie nadchodzących koncertów wybranych artystów lub zespołów. Dzięki integracji z zewnętrznymi źródłami danych aplikacja prezentuje również listę najpopularniejszych utworów danego wykonawcy, co pozwala lepiej przygotować się do wydarzenia i przewidzieć, jakie piosenki mogą zostać zagrane na żywo.

### Funkcjonalności
- **logowanie** - aby uzyskać dostęp do funkcji aplikacji, użytkownik musi się zalogować.
- **tworzenie konta** - nowi użytkownicy mogą łatwo założyć konto, które pozwoli im logować się w przyszłości i korzystać z aplikacji.
- **wyszukiwanie koncertów** - aplikacja umożliwia wyszukiwanie nadchodzących koncertów na podstawie nazwy artysty lub zespołu.
- **wyświetlanie top piosenek** -  wraz z informacjami o koncertach użytkownik otrzymuje listę najpopularniejszych utworów danego artysty.

### Użyte zewnętrzne API
- **ticketmaster** - do zbierania danych o przyszłych koncertach wybranego artysty
- **spotify** - do zbierania 5 topowych piosenek artysty

### Instrukcja odpalenia aplikacji
1. Zklonuj repozytorium https://github.com/majamc/ticketmaster_55204_51270_51373 //potem zmienic nazwe
2. Otwórz kod w dowolnym edytorze kodu (np. Visual Studio 2022)
3. Upewnij się że masz zainstalowany dotnet ef, jeśli nie to zainstaluj go poleceniem 'dotnet tool install --global dotnet-ef'
4. Otwórz terminal w katalogu projektu i wpisz po kolei 'dotnet restore ConcertTracker.csproj', 'dotnet ef database update' aby utworzyć bazę danych i 'dotnet run' aby uruchomić aplikcję
5. Otwórz przeglądarke internetową i przejdź na stronę pod adresem http://localhost:5101/
