# Aplikacja do śledzenia koncertów

### Opis


### Funkcjonalności


### Użyte zewnętrzne API
- **ticketmaster** - do zbierania danych o przyszłych koncertach wybranego artysty
- **spotify** - do zbierania 5 topowych piosenek artysty

### Instrukcja odpalenia aplikacji
1. Zklonuj repozytorium https://github.com/majamc/ticketmaster_55204_51270_51373 //potem zmienic nazwe
2. Otwórz kod w dowolnym edytorze kodu (np. Visual Studio 2022)
3. Upewnij się że masz zainstalowany dotnet ef, jeśli nie to zainstaluj go poleceniem 'dotnet tool install --global dotnet-ef'
4. Otwórz terminal w katalogu projektu i wpisz po kolei 'dotnet restore ConcertTracker.csproj', 'dotnet ef database update' aby utworzyć bazę danych i 'dotnet run' aby uruchomić aplikcję
5. Otwórz przeglądarke internetową i przejdź na stronę pod adresem http://localhost:5101/
