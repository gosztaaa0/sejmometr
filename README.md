# Sejmometr - Analizator Głosowań Sejmowych 🏛️🇵🇱

Sejmometr to aplikacja desktopowa napisana w technologii **C# WPF**, która pozwala na bieżąco śledzić i analizować wyniki głosowań w polskim Sejmie (X kadencja). Program pobiera dane w czasie rzeczywistym bezpośrednio z oficjalnego **API Sejmu**.

## ✨ Funkcje aplikacji

- **Pobieranie listy posiedzeń:** Automatyczne pobieranie aktualnej listy posiedzeń Sejmu.
- **Przegląd głosowań:** Możliwość wybrania konkretnego posiedzenia i wyświetlenia wszystkich odbytych w jego ramach głosowań.
- **Szczegółowe wyniki:** Wyświetlanie listy posłów wraz z ich zdjęciami, przynależnością klubową i oddanym głosem.
- **Wykresy kołowe (LiveCharts):** Wizualizacja rozkładu głosów (Za, Przeciw, Wstrzymało się, Nieobecni) w formie interaktywnego wykresu.
- **Wykrywanie "Buntowników":** System automatycznie analizuje linię partyjną (najczęstszy głos w klubie) i oznacza posłów, którzy zagłosowali inaczej niż ich koledzy z klubu.
- **Filtrowanie wyników:** Możliwość filtrowania listy według rodzaju głosu lub wyświetlenie tylko "buntowników".
- **Eksport do PDF:** Generowanie prostego raportu z wynikami głosowania gotowego do wydruku.

## 🛠️ Technologie

- **Język:** C#
- **Interfejs:** WPF (Windows Presentation Foundation)
- **Biblioteki:**
  - `Newtonsoft.Json` - do obsługi danych JSON z API.
  - `LiveCharts.Wpf` - do renderowania wykresów.
  - `System.Net.Http` - do komunikacji z serwerami Sejmu.

## 🚀 Jak uruchomić projekt

1. Sklonuj repozytorium:
   ```bash
   git clone [https://github.com/gosztaaa0/sejmometr.git](https://github.com/gosztaaa0/sejmometr.git)
