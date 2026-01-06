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
Eksperimenta mērķis ir novērtēt maršrutu plānošanas algoritma veiktspēju un efektivitāti, mainot galvenos sistēmas darbības parametrus – pieejamo kurjeru skaitu, kurjeru maiņas ilgumu un vidējo apkalpošanas laiku pie klienta. Šāda pieeja ļauj novērtēt sistēmas uzvedību dažādos darba scenārijos un noteikt optimālus konfigurācijas parametrus.

#### Eksperimenta mērķis

Novērtēt maršrutu plānošanas algoritma efektivitāti, analizējot tā darbību pie dažāda pieejamo kurjeru skaita, kurjeru maiņas ilguma un vidējā klienta apkalpošanas laika, lai nodrošinātu optimālu piegāžu sadali starp kurjeriem.

#### Ieejas parametri

- **Kurjeru skaits (K)** – kopējais pieejamo kurjeru skaits vienas maiņas laikā.
- **Kurjera maiņas ilgums (MI, h)** – maksimālais darba laiks vienam kurjeram maiņā.
- **Vidējais pieturas laiks pie klienta (VPT, min)** – vidējais laiks, kas nepieciešams viena klienta apkalpošanai (izkāpšana, nodošana, paraksts u.tml.).

#### Novērtēšanas mēri

- **Maršrutu plānošanas laiks (W, s)** – algoritma izpildes laiks, lai izveidotu maršrutus visiem kurjeriem.
- **Vidējais kurjeru noslogojums (U, %)** – kurjeru faktiskā darba laika attiecība pret pieejamo maiņas laiku (cik efektīvi tiek izmantoti kurjeri).

#### Eksperimentu plāns

Eksperimentu plāns tika izstrādāts, sistemātiski kombinējot trīs galvenos ieejas parametrus: kurjeru skaitu (K), maiņas ilgumu (MI) un vidējo apkalpošanas laiku pie klienta (VPT). Katrs eksperiments atbilst atšķirīgam darba scenārijam, kas ļauj analizēt maršrutu plānošanas algoritma uzvedību gan pie zemākas, gan pie augstākas sistēmas noslodzes. Šāda pieeja nodrošina salīdzināmus rezultātus un ļauj identificēt parametru ietekmi uz maršrutu plānošanas laiku un kurjeru noslogojumu.

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

### Novērtēšanas rezultāti
Eksperimentu rezultāti parāda skaidras sakarības starp ieejas parametriem un maršrutu plānošanas algoritma darbības rādītājiem. Palielinoties pieejamo kurjeru skaitam (K), maršrutu plānošanas laiks (W) pakāpeniski pieaug, jo algoritmam nepieciešams apstrādāt lielāku iespējamo piegāžu sadales variantu skaitu. Tomēr visos eksperimentu scenārijos algoritma izpildes laiks saglabājas dažu sekunžu robežās, kas norāda uz labu veiktspēju un praktisku pielietojamību.
Vidējais kurjeru noslogojums (U) samazinās, pieaugot kurjeru skaitam, jo pie nemainīga pasūtījumu apjoma darbs tiek sadalīts uz lielāku kurjeru skaitu. Tas īpaši izteikti novērojams scenārijos ar K = 8, kur noslogojums samazinās līdz aptuveni 42–62%, salīdzinot ar scenārijiem, kuros pieejami tikai 2 kurjeri.
Vidējais apkalpošanas laiks pie klienta (VPT) būtiski ietekmē kurjeru noslogojumu. Pie VPT = 10 min noslogojums ir ievērojami augstāks nekā pie VPT = 5 min, jo katra piegāde aizņem lielāku daļu no kurjera pieejamā darba laika. Tas norāda, ka algoritma efektivitāte ir cieši saistīta ne tikai ar maršruta ģeogrāfiju, bet arī ar operacionālajiem procesiem pie klienta.
Maiņas ilguma palielināšana no 6 līdz 8 stundām samazina relatīvo kurjeru noslogojumu, jo pieaug pieejamais darba laiks. Šāds risinājums var būt piemērots situācijās, kad svarīgi nodrošināt rezervi pīķa noslodzes gadījumiem, tomēr tas var radīt arī resursu neizmantotību pie zemāka pasūtījumu apjoma.

# Secinājumi
