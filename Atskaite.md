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
## Novērtēšanas plāns
## Novērtējums

### Novērtēšanas plāns

#### Eksperimenta mērķis

Novērtēt maršrutu plānošanas algoritma veiktspēju, mainot pieejamo kurjeru skaitu, kurjeru maiņas garumu un vidējo apkalpošanas laiku pie klienta, lai pēc iespējas efektīvāk sadalītu piegādes kurjeru starpā.

#### Ieejas parametri

- **Kurjeru skaits (K)** – kopējais pieejamo kurjeru skaits vienas maiņas laikā.
- **Kurjera maiņas ilgums (MI, h)** – maksimālais darba laiks vienam kurjeram maiņā.
- **Vidējais pieturas laiks pie klienta (VPT, min)** – vidējais laiks, kas nepieciešams viena klienta apkalpošanai (izkāpšana, nodošana, paraksts u.tml.).

#### Novērtēšanas mēri

- **Maršrutu plānošanas laiks (W, s)** – algoritma izpildes laiks, lai izveidotu maršrutus visiem kurjeriem.
- **Vidējais kurjeru noslogojums (U, %)** – kurjeru faktiskā darba laika attiecība pret pieejamo maiņas laiku (cik efektīvi tiek izmantoti kurjeri).

#### Eksperimentu plāns

| Nr. | K  | MI (h) | VPT (min) | W (s) | U (%) |
|----:|---:|-------:|----------:|:-----:|:-----:|
| 1   | 2  | 6      | 5         |       |       |
| 2   | 4  | 6      | 5         |       |       |
| 3   | 6  | 6      | 5         |       |       |
| 4   | 8  | 6      | 5         |       |       |
| 5   | 2  | 6      | 10        |       |       |
| 6   | 4  | 6      | 10        |       |       |
| 7   | 6  | 6      | 10        |       |       |
| 8   | 8  | 6      | 10        |       |       |
| 9   | 2  | 8      | 5         |       |       |
| 10  | 4  | 8      | 5         |       |       |
| 11  | 6  | 8      | 5         |       |       |
| 12  | 8  | 8      | 5         |       |       |
| 13  | 2  | 8      | 10        |       |       |
| 14  | 4  | 8      | 10        |       |       |
| 15  | 6  | 8      | 10        |       |       |
| 16  | 8  | 8      | 10        |       |       |

## Novērtēšanas rezultāti

# Secinājumi
