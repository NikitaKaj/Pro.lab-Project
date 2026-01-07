# Ievads
## Problēmas nostadne
Maršruta optimizācija ir galvenais mērķis loģistikas un piegādes sistēmās. Uzņēmumiem ir jāizstrādā optimāli transporta maršruti, lai samazinātu degvielas izmaksas, saīsinātu piegādes laikus un uzlabotu klientu apkalpošanu. Galvenais izaicinājums ir klientu dažādās ģeogrāfiskās atrašanās vietas un pieņemamie piegādes laiki. Savukārt šīs sistēmas optimizācija var palīdzēt atrast visrentablākos maršrutus un uzlabot piegādes laikus un kvalitāti.
## Darba un novērtešanas mērķis
### Lietotāju stāsti
| Nr.   | Lietotāju stāsts <lietotājs> vēlas <sasniegt mērķi>, jo <ieguvums>   | Prioritāte |
|--------------|----------------|--------------------|
|1|Klients vēlas ātru pasūtījuma piegādi, jo tas ir ērti|1
|2|Klients vēlas pats izvēlēties piegādes laiku, jo tas padara pakalpojumu vairāk orientētu uz klientu|2
|3|Uzņēmums vēlas apmierināt klientus, jo tas palielinās viņu reitingu un atpazīstamību|6
|4|Uzņēmums vēlas optimizēt maršrutu, jo tas palīdzēs samazināt degvielas izmaksas|3
|5|Kurjers vēlas ērtu navigācijas sistēmu, jo tā samazinās pasūtījuma izpildes laiku|4
|6|Kurjers vēlas zināt laika logus, jo tas optimizē viņa darbu|5
### Mērķis 
Izstrādāt sistēmu, kas optimizē piegādes maršrutus, ņemot vērā dažādus faktorus, tostarp klienta ģeogrāfisko atrašanās vietu, izvēlēto laika logu un pieejamo kurjeru skaitu, kā arī citu loģistikai svarīgu informāciju.
### Uzdevumi 
#### 1. Analizēt problēmu un izvirzīt mērķi.
#### 2. Izpētīt esošos līdzīgos risinājumus.
#### 3. Izstrādāt un īstenot algoritmu
#### 4. Izveidot datu bāzi.
#### 5. Izstrādāt servera pusi (REST API)
#### 6. Izstrādāt klienta pusi (UX/UI).
#### 7. Vizuāli parādīt maršrutus kartē.
#### 8. Algoritma testēšana un optimizācija
#### 9. Veikt pašanalīzi un uzrakstīt secinājumus.

# Līdzīgo risinājumu pārskats
| Risinājumi   | Apraksts   | Priekšrocības  | Trūkumi |
|--------------|----------------|--------------------|--------------|
Google Maps|Bezmaksas tiešsaistes kartēšanas pakalpojums, kas ļauj atrast vietas, plānot maršrutus un pārvietoties pa pasauli|Plaši izmantots, vienkāršs un ērts interfeiss, nodrošina reāllaika satiksmes informāciju, bezmaksas pamata funkcijas|Nav piemērots sarežģītai maršrutu optimizācijai ar daudziem pieturas punktiem|
RouteXL|Bezmaksas tiešsaistes vairāku pieturu maršruta plānotājs, kas optimizē adrešu secību, lai samazinātu kopējo ceļojuma laiku un attālumu|Specializēts vairāku pieturas punktu maršrutu optimizācijai|Nepieciešams interneta savienojums, sarežģītāks interfeiss nekā Google Maps|
OpenStreetMap|Projekts, lai izveidotu un nodrošinātu bezmaksas un atvērtā koda pasaules karti|Atvērts un bezmaksas resurss, pieejams plašs ģeogrāfisko datu apjoms, lietotāji paši var papildināt un uzlabot kartes|Nav pilnībā standartizētu maršrutēšanas rīku, datu kvalitāte dažās vietās var būt nepilnīga vai neprecīza|
OptimoRoute|Pakalpojums piegādes un pakalpojumu maršrutu automātiskai plānošanai, optimizācijai un izsekošanai|Profesionāls rīks loģistikas un piegādes uzņēmumiem, atbalsta maršrutu plānošanu ar daudziem transportlīdzekļiem un ierobežojumiem|Sarežģītāka apmācība jaunajiem lietotājiem|
Route4Me|Maršruta optimizācijas platforma, kas paredzēta uzņēmumiem un privātpersonām|Ērts vairāku pieturas punktu maršrutēšanas rīks, mobilā aplikācija, ātra adrešu optimizācija, piemērots maziem un vidējiem uzņēmumiem|Pilnvērtīgās funkcijas pieejamas tikai maksas versijā, nav tik detalizētas satiksmes informācijas kā Google Maps|
# Tehniskais risinājums

## Prasības
## Algoritms
## Konceptu modelis
![Link Error](https://i.ibb.co/rGXDfm6q/image.png)
### Klients: 
Viens Klients var izveidot vairākus Pasūtījumus (1:N).
### Pasūtījums: 
Katram Pasūtījumam ir viens Klients (N:1); <br>
Katrs Pasūtījums tiek piešķirts konkrētam Maršrutam (N:1);<br>
Katru pasūtījumu apstiprina Administrators (N:1).<br>
### Administrators:
Viens Administrators var apstrādāt vairākus Pasūtījumus (1:N);<br>
Var izveidot un modificēt vairākus Maršrutus (1:N).<br>
### Kurjeri:
Katrs Kurjers izmanto vienu Transportu (N:1);<br>
Katram Kurjeram ir viens vai vairāki Maršruti (1:N).<br>
### Maršruts:
Viens Maršruts sastāv no vairākiem Pasūtījuma punktiem (1:N);<br>
Katrs Maršruts pieder vienam Kurjeram (N:1);<br>
Katram Maršrutam var būt vairāki Pasūtījumi (1:N).<br>
### Pasūtījuma punkts:
Katrs Pasūtījuma punkts pieder konkrētam Maršrutam (N:1);<br>
Saistīts ar vienu Pasūtījumu (N:1).<br>
### Transports:
Katrs Transports var būt piesaistīts vienam vai vairākiem Kurjeriem (1:N, bet parasti 1:1 reālā laikā);<br>
Katrs Transports tiek izmantots vairākos Maršrutos (1:N).<br>
## Tehnoloģiju steks
![Link Error](https://i.ibb.co/F4XFPGVQ/image.png)
## Programmatūras

# Novērtējums
### Novērtēšanas plāns
Risinājuma novērtēšanas mērķis ir pārbaudīt izstrādātās sistēmas un maršrutu plānošanas algoritma spēju efektīvi sadalīt piegādes starp kurjeriem dažādos darba apstākļos. Novērtēšana balstās uz eksperimentālu pieeju, kurā tiek mainīti galvenie sistēmas darbības parametri, analizējot to ietekmi uz algoritma veiktspēju un resursu izmantošanas efektivitāti.
Eksperimenta ietvaros tiek analizēta sistēmas uzvedība pie dažāda pieejamo kurjeru skaita, maiņas ilguma un vidējā apkalpošanas laika pie klienta. Šāda pieeja ļauj novērtēt risinājuma piemērotību praktiskai izmantošanai loģistikas un piegādes sistēmās.

#### Eksperimenta mērķis
Novērtēt maršrutu plānošanas algoritma efektivitāti, analizējot tā darbību pie dažāda pieejamo kurjeru skaita, kurjeru maiņas ilguma un vidējā klienta apkalpošanas laika, lai nodrošinātu optimālu piegāžu sadali starp kurjeriem.

#### Ieejas parametri

- **Kurjeru skaits (K)** – kopējais pieejamo kurjeru skaits vienas maiņas laikā.
- **Kurjera maiņas ilgums (MI, h)** – maksimālais darba laiks vienam kurjeram maiņā.
- **Vidējais pieturas laiks pie klienta (VPT, min)** – vidējais laiks, kas nepieciešams viena klienta apkalpošanai (izkāpšana, nodošana, paraksts u.tml.).

#### Novērtēšanas mēri

- **Maršrutu plānošanas laiks (W, s)** – laiks, kas nepieciešams algoritmam, lai izveidotu maršrutus visiem kurjeriem.
- **Vidējais kurjeru noslogojums (U, %)** – kurjeru faktiskā darba laika attiecība pret kopējo pieejamo maiņas laiku.

#### Eksperimentu plāns

Eksperimentu plāns tika izveidots, sistemātiski kombinējot ieejas parametru vērtības. Katrs eksperiments raksturo atšķirīgu sistēmas noslodzes scenāriju – no situācijām ar nelielu kurjeru skaitu un augstu noslogojumu līdz scenārijiem ar lielāku kurjeru skaitu un lielāku sistēmas elastību. Šāda struktūra nodrošina iespēju salīdzināt rezultātus un identificēt galvenās parametru ietekmes uz algoritma darbību.

Eksperimentu rezultāti maršrutu plānošanas algoritma novērtēšanai tabula:

| Nr. | K | MI (h) | VPT (min) | W (s) | U (%) |
|----:|--:|-------:|----------:|------:|------:|
| 1   | 2 | 6      | 5         | 1.6   | 88    |
| 2   | 4 | 6      | 5         | 2.2   | 76    |
| 3   | 6 | 6      | 5         | 2.9   | 63    |
| 4   | 8 | 6      | 5         | 3.6   | 54    |
| 5   | 2 | 6      | 10        | 1.8   | 96    |
| 6   | 4 | 6      | 10        | 2.6   | 85    |
| 7   | 6 | 6      | 10        | 3.4   | 73    |
| 8   | 8 | 6      | 10        | 4.2   | 62    |
| 9   | 2 | 8      | 5         | 1.7   | 66    |
| 10  | 4 | 8      | 5         | 2.4   | 58    |
| 11  | 6 | 8      | 5         | 3.2   | 49    |
| 12  | 8 | 8      | 5         | 4.0   | 42    |
| 13  | 2 | 8      | 10        | 2.0   | 78    |
| 14  | 4 | 8      | 10        | 2.9   | 69    |
| 15  | 6 | 8      | 10        | 3.8   | 61    |
| 16  | 8 | 8      | 10        | 4.7   | 53    |

Tabulā apkopoti eksperimentu rezultāti, kuros tika mainīti galvenie sistēmas ieejas parametri
kurjeru skaits, maiņas ilgums un vidējais apkalpošanas laiks pie klienta. 
Katrs eksperiments raksturo atšķirīgu sistēmas noslodzes scenāriju, 
bet rādītāji W un U atspoguļo algoritma veiktspēju un kurjeru noslogojuma efektivitāti.

### Novērtēšanas rezultāti
Eksperimentu rezultāti parāda, ka izstrādātais maršrutu plānošanas algoritms korekti reaģē uz izmaiņām sistēmas konfigurācijā. Palielinoties kurjeru skaitam, pieaug maršrutu plānošanas laiks, tomēr tas visos gadījumos saglabājas dažu sekunžu robežās, kas norāda uz algoritma labu veiktspēju un piemērotību praktiskai lietošanai.
Vidējais kurjeru noslogojums samazinās, pieaugot kurjeru skaitam, jo piegādes tiek sadalītas uz lielāku izpildītāju skaitu. Savukārt, pie ilgāka apkalpošanas laika pie klienta, kurjeru noslogojums palielinās, jo katrs pasūtījums aizņem lielāku daļu no pieejamā darba laika.
Maiņas ilguma palielināšana nodrošina papildu elastību maršrutu plānošanā, taču pie nemainīga pasūtījumu apjoma var novest pie zemāka relatīvā noslogojuma. Tas norāda uz nepieciešamību pielāgot sistēmas konfigurāciju atbilstoši faktiskajam pieprasījumam.

# Secinājumi
