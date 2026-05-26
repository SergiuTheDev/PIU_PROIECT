# Portfolio Tracker

Acesta este un proiect academic dezvoltat pentru cursul de Programare Interfețe Utilizator. Aplicația reprezintă un sistem simplu de tip "Portfolio Tracker", conceput pentru a monitoriza investițiile financiare (acțiuni, criptomonede, ETF-uri) și pentru a calcula automat profitul sau pierderea în timp real.

## Descriere proiect din punctul de vedere al utilizatorului

Aplicația oferă o interfață grafică (GUI) intuitivă prin care un utilizator își poate gestiona investițiile. Funcționalitățile principale includ:
- **Adăugarea tranzacțiilor (Cumpărare):** Utilizatorul introduce simbolul activului (ex. AAPL pentru Apple), cantitatea și prețul de achiziție. Sistemul preia automat denumirea completă și prețul curent de pe piață.
- **Ștergerea tranzacțiilor (Vânzare):** Utilizatorul poate vinde complet o poziție deținută în portofoliu.
- **Căutare/Filtrare:** Printr-o bară de căutare dedicată, utilizatorul poate găsi rapid o acțiune după simbol sau denumire în propriul portofoliu.
- **Salvare persistentă:** Toate datele sunt salvate automat într-un fișier local, astfel încât la următoarea deschidere a aplicației, portofoliul este exact așa cum a fost lăsat.
- **Statistici în timp real:** Tabloul de bord afișează totalul investit, valoarea curentă a portofoliului și profitul/pierderea netă.
- **Actualizare prețuri:** Un buton dedicat permite actualizarea la zi a prețurilor pentru toate acțiunile deținute.

## Descriere proiect din punct de vedere al programatorului

Aplicația este dezvoltată în C# (WPF) respectând principiile de bază ale Programării Orientate pe Obiecte (OOP) și utilizează modelul arhitectural **MVVM (Model-View-ViewModel)** pentru a decupla logica de interfață (View) de logica de business (ViewModel/Model).

Elemente și algoritmi notabili:
- **Arhitectura MVVM:** `MainViewModel` gestionează starea aplicației, expunând proprietăți (prin `INotifyPropertyChanged`) și comenzi (`ICommand` via `RelayCommand`) către `MainWindow.xaml` prin DataBinding.
- **Integrarea API (Yahoo Finance):** Clasa `FinanceApiService` efectuează cereri HTTP asincrone (`HttpClient`, `Task`) către endpoint-ul Yahoo Finance pentru a obține prețul în timp real. Datele sunt parsate dinamic folosind `JsonDocument` din namespace-ul `System.Text.Json`.
- **Serializare/Deserializare JSON:** Clasa `NivelStocareData` gestionează persistența datelor. Portofoliul și lista de poziții sunt serializate și salvate local într-un fișier `portfolio.json`, fără a folosi căi absolute, asigurând portabilitatea proiectului.
- **Filtrare dinamică (Căutare):** Căutarea în interfață folosește `ICollectionView` și metoda `Filter` pentru a afișa selectiv datele din `ObservableCollection` fără a modifica structura de bază a colecției.
- **Recalcularea prețului mediu:** Algoritmul intern aplică principiul *Dollar Cost Averaging*. Când se cumpără cantități adiționale dintr-un activ deja deținut, noul preț mediu este recalculat proporțional cu cantitățile și prețurile istorice.

## Tehnologii Utilizate

- **Limbaj:** C#
- **Paradigmă:** MVVM (Model-View-ViewModel), OOP
- **UI Framework:** WPF (Windows Presentation Foundation)
- **Stocare Date:** JSON (`System.Text.Json`)
- **API Extern:** Yahoo Finance API
