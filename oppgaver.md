
# Oppgave 1

## 1.1 En rask test på at alt er satt opp riktig

1.  Remote client: velg den du ønsker å bruke:
     *  Start, evt. installér "Remote Development" extension i VS Code
     *  Start / installer PuTTY
         *  [https://www.chiark.greenend.org.uk/~sgtatham/putty/latest.html](https://www.chiark.greenend.org.uk/~sgtatham/putty/latest.html)
     *  Bruk SSH i GIT Bash eller Powershell 
     *  Bruk evt Docker Desktop om du har den installert fra før.
1.  SSH til VM: 
     *  Brukernavn "localadmin"
     *  IP og passord er utdelt
1.  Hello World!
     *  `docker run --name hello hello-world`
1.  Ta en rask kikk på hva som akkurat ble nedlastet:
     *  `docker image ls`
1.  En container ble kjørt, men er ikke aktivt nå:
     *  `docker container ps`
     *  Listen bør være tom.
1.  Vi kan likevel undersøke den inaktive containeren:
     *  `docker container ps -a`

Du vet nå at Docker (Engine) fungerer greit på maskinen, og du har fått prøvd noen kommandoer.

## 1.2 Test en webserver

1.  La oss prøve å kjøre en web server:
     *  `docker pull nginx`
     *  `docker run -d --name web -p 80:80 nginx`
1.  Bruk curl lokalt til å kalle webserveren
     *  `curl localhost`
     *  Du bør se HTML i terminalen
1.  Prøv å aksessere webserveren fra din fysiske PC
     *  Lim inn VM'ens IP i nettleseren's adressefelt
     *  IP er utdelt. I teorien skal dette fungere.
1.  Vi kan nå se at containeren kjører og er aktiv:
     *  `docker container ps`
     *  Listen bør ha én linje.
1.  Stopp "web"
     *  `docker stop web`

Du så nå på en container som var persistent, og som kunne bare ha fortsatt å kjøre om den ikke ble stoppet. Du så også containere i prinsippet kan lytte på nettverksporter.

# Oppgave 2

## 2.1 Undersøk containere litt grundigere

1.  Start opp igjen nginx containeren fra tidligere:
     *  `docker start web`
1.  `curl -g localhost` for å bekrefte at den lytter.
1.  Åpne shell inni "web" containeren:
     *  `docker exec -it web /bin/bash`
1.  De følgende kommandoene skal kjøres inni containeren:
1.  Bekreft at du er inni containeren:
     *  Kommandoen `hostname` bør returnere et heksadesimalt tall
1.  `ps` brukes for å se på prosesser som kjører i Linux. Den må installeres først:
     *  `apt update && apt install -y procps`
1.  Bruk `ps` til å se nginx kjørende i containeren, og noter deg PID'ene (Prosess ID'ene):
     *  `ps ax`
    Lukk shellet / gå ut av containeren:
     *  `exit`
1.  Du er tilbake i host (VM'en). Kjør `ps ax | grep nginx` der, og se PID'ene på nginx prosessene på host.
1.  Avslutt web:
     *  `docker stop web`

Du så nå at prosess ID'ene var forskjellige inni container og host - men det er faktisk de samme prosessene. 

## 2.2 Lag image fra en container

1.  Hent Alpine Linux base image
     *  `docker pull alpine:latest`
1.  Start opp Alpine (starter et interaktivt shell):
     *  `docker run -it --name alpine alpine`
     *  -it = "interactive tty (teletype)"
1.  De følgende kommandoene kjøres inni containeren:
     *  Alltid greit å sjekke `hostname`: bør være heksadesimalt
1.  Sjekk om den har curl: `curl -v nrk.no`
     *  nei..
1.  Installer curl: 
     *  `apk update && apk add curl`
     *  `curl -v nrk.no` bør fungere
1.  Installer jq også (et JSON verktøy):
     *  `apk add jq`
1.  Slett et trolig ubrukt directory:
     *  `rmdir /media/floppy`
1.  Gå ut av shellet
     *  `exit`
1.  Se på containere i Exited state:
     *  `docker container ps -a | grep alpine`
1.  Se DIFF, de endringene du gjorde på containeren: 
     *  `docker container diff alpine`
     *  litt mye output -- en bedre teknikk kommer straks
1.  Se nåværende images
     *  `docker image ls` / `docker images`
1.  Opprett image fra den containeren vi jobbet på nå:
     *  `docker container commit alpine alpine-with-curl`
1.  List images igjen:
     *  `docker image ls`
1.  Ta en rask kikk på layers i imaget: 
     *  `docker image history alpine-with-curl`     

Du så nå hvordan du kunne gjøre endringer på writeable layer i en container, og hvordan et nytt image enkelt kunne lages fra en container, uten å bruke en Dockerfile. Du så også litt nærmere på layers-begrepet.

## 2.3 Kjør container fra ditt image

*Bare gjør denne om tiden tillater*

1.  Start en container fra imaget du akkurat lagde
     *  `docker run -it --rm --name awc alpine-with-curl`
1.  Du står i et shell inni containeren.
     *  Sjekk eventuelt `hostname`
1.  Test curl og jq: 
     *  `curl -g https://official-joke-api.appspot.com/jokes/random | jq`
1.  Logg ut av container-shell:
     *  `exit`


# Oppgave 3: Bygg og deploy med Dockerfiles


## 3.1 Bygg lokalt

```shell
GIT_URL=url til GitHub, se presentasjonen
```

1.  Hent filene fra GIT
     *  GITURL er delt i presentasjonen. Skal være et public repository.
     *  `git clone $GIT_URL`
1.  Sjekk om du har .NET SDK installert på VM:
     *  `dotnet --version`
     *  Det er ikke installert, men det er ikke noe problem. Ikke installer den.
1.  Bygg imaget lokalt én gang (ikke installer dotnet SDK'en):
     *  Stå i samme directory som `DockerTrivia.sln` filen
     *  `docker build . -f DockerTrivia.API/Dockerfile -t trivia:0.1`
     *  Legg merke til tidsbruken
1.  Gjør en liten endring på koden.
     *  F.eks. legg inn en linje whitespace i `Program.cs`.
     *  Du kan enten lage en ny commit og pull'e denne fra GitHub, eller gjøre endringen direkte i VM'en:
         *  `nano DockerTrivia.API/Program.cs`
         *  Legg til en whitespace eller ny linje
         *  `Ctrl+o` og `Enter` for å lagre, `Ctrl+x` for å avslutte `nano`.
     *  Bygg på nytt, og legg merke til hva som skjer.
     *  `docker build . -f DockerTrivia.API/Dockerfile -t trivia:0.2`
         *  **Obs!** legg merge til at tag er endret til `0.2`.
1.  Kjør Trivia API lokalt
     *  `docker container run -e HTTPLISTENPORT=80 -d --name trivia --rm -p 80:80 trivia:0.2`
     *  -e setter en environment variabel (konfigurasjon)
     *  -d kjører container "detached" (i bakgrunnen)
     *  --rm sørger for å slette container etter den stoppes
     *  -p mapper host port 80 til container port 80 (NAT firewall)
1.  Prøv å besøke IP-adressen til VM'en fra din fysiske PC i nettleseren:
     *  F.eks. `100.100.100.100/swagger/index.html`
     *  I teorien skal det fungere. Du kan da gjøre et kall til API'et i Swagger.
1.  Stopp containeren
     *  `docker container stop trivia`

Du brukte nå en Dockerfile (multi-stage) til å bygge et .NET API - et svært enkelt API riktignok, men prinsippet og stegene er stort sett de samme for å bygge store applikasjoner.

## 3.2 Bygg enda litt mer

1.  Dockerfile brukt i 3.1 var multi-stage. Prøv nå å bygge samme app, men med en single stage Dockerfile.. 
     *  Husk å stå i directory der `.sln` filen er.
     *  `docker build . -f DockerTrivia.API/Dockerfile-singlestage -t trivia-singlestage:latest`
1.  Ta en kikk på størrelsen på dette image, i motsetning til det fra oppg 3.1:
     *  `docker image ls`
     *  Hva kan størrelsesforskjellen komme av?
1.  Imaget kan evt slettes (ikke viktig):
     *  `docker image remove trivia-singlestage:latest`

Du så nå en av ulempene dersom man hadde bruk SDK-baserte image til å både bygge og deployere applikasjoner.

## 3.3 Push til repository

Du kan bruke din egen ACR i denne øvelsen, eller bruke en som er allerede satt opp (enklest):

Definér variabler i shellet:

```shell
REGISTRY_USERNAME=kopier fra sted vist i presentasjonen
REGISTRY_PASSWORD=kopier fra samme sted
UNIQUETAG=din egen tag
```

Finn på noe unikt for UNIQUETAG, f.eks. en string og noen siffer.


1.  Lag ny "latest" tag til `trivia:0.2` eller `0.1` fra tidligere:
     *  `docker tag trivia:0.2 trivia:latest`
1.  Lag enda en tag med adressen til ACR fra trivia:
     *  `docker tag trivia:latest triviaacr.azurecr.io/trivia:$UNIQUETAG`
1.  Login til Docker
     *  `docker login -u $REGISTRY_USERNAME -p $REGISTRY_PASSWORD triviaacr.azurecr.io`
     *  Se presentasjonen for hvor du finner passordet.
1.  Push til ACR repositoriet:
     *  `docker push triviaacr.azurecr.io/trivia:$UNIQUETAG`
    
----

Ikke del av oppgaven: For å bruke din egen ACR, istendenfor `triviaacr.azurecr.io`, må du lage en scopeMap og en Token som bruker dette scopeMap'et. I Portalen: 

1.  ACR
1.  Repos. permissions
1.  Scope Maps
     *  Gi READ og WRITE permission til scope map.
1.  Tokens
     *  Bruke scope map fra forrige steg til tokenet, med repos.name `trivia` f.eks.

## 3.4 Pull fra repository

1.  Du må være logget inn på ACR'en, om du ikke har gjort det allerede:
     *  `docker login -u $REGISTRY_USERNAME -p $REGISTRY_PASSWORD triviaacr.azurecr.io`
1.  Prøv å hente ned imaget laget av en av de andre: 
     *  `docker pull triviaacr.azurecr.io/trivia:OTHERTAG`
     *  OTHERTAG er den tag'en de andre brukte
     *  Alternativt: 
         *  `docker image rm triviaacr.azurecr.io/trivia:$UNIQUETAG`
         *  `docker image rm trivia:0.1`
         *  `docker image rm trivia:0.2`
         *  `docker image rm trivia:latest`    
         *  `docker pull triviaacr.azurecr.io/trivia:$UNIQUETAG`
1.  Prøv eventuelt å kjøre image som ble pulled:
     *  Vær sikker på at webserveren fra tidligere er stoppet:
     *  `docker stop web`
     *  Start image som du pull-et fra ACR:
     *  `docker container run -e HTTPLISTENPORT=80 -d --name trivia-acr --rm -p 80:80 triviaacr.azurecr.io/trivia:OTHERTAG`
1.  Besøk `<IP>/swagger/index.html` i nettleseren på din fysiske maskin, eller `curl localhost/swagger/index.html` på VM'en
     *  Det skal gå an å invokere `/api/questions` fra Swagger eller `curl`.
     *  `curl -g http://localhost:80/api/questions | jq`

## 3.5 Deploy til ACI

Denne går ut på å deployere trivia-api til en Azure container instance (ACI) ved å bruke AZ CLI. Din domene-bruker må ha tilgang til Sør Agder's subscription i Azure for at dette skal fungere: 

```shell
az login --use-device-code
```

Velg ".. Sør Agder .." subscription om du blir spurt om å velge av `az login`.

Definér noen variabler i shellet. ACI_UNIQUENAME = VM-name ("vm-1", "vm-2" etc.) + "-aci". F.eks. "vm-10-aci":

```shell
# Dobbelsjekk at du er logget inn: 
az account show

# Definér flere variabler i shellet:
RESOURCE_GROUP=se presentasjonen (ressursgruppen)
ACI_UNIQUENAME=vm-N-aci
```

Kommandoen er som følger:


```shell
az container create \
    --resource-group $RESOURCE_GROUP \
    --name $ACI_UNIQUENAME \
    --image triviaacr.azurecr.io/trivia:$UNIQUETAG \
    --cpu 1 \
    --memory 1 \
    --registry-login-server triviaacr.azurecr.io \
    --registry-username $REGISTRY_USERNAME \
    --registry-password $REGISTRY_PASSWORD \
    --dns-name-label $ACI_UNIQUENAME \
    --ports 8080
```

Etter en stund vil du få endel JSON output. Se etter "ipAddress / fqdn". Det er FQDN (fully qualified domain name) verdien vi skal bruke nå.

Du kan nå besøke trivia-api kjørende på ACI. Legg merke til at den kjører på port 8080:

1.  I nettleseren din, lim inn FQDN fra over: 
     *  `http://FQDN:8080/swagger/index.html`
1.  Det bør gå an å invokere api'et.
1.  Du kan evt slette ressursen din i Azure når du er ferdig: 
     *  `az container delete -g $RESOURCE_GROUP -n $ACI_UNIQUENAME`
1.  Logg gjerne ut av AZ CLI nå: 
     *  `az logout`

----

Sidenotat: ACI hosting på port 80 fungerer ikke i dette tilfellet, f.eks: 

```shell
# Fungerer ikke i dette tilfellet:
az container create \
    ...etc... \
    --ports 80 \
    --environment-variables HTTPLISTENPORT=80
```


## 3.6 Bygg og deploy med GitHub Actions

*Bare gjør denne om du får tid.*

Alle bygg, push, deployment stegene gjort for hånd kan automatiseres med f.eks. GitHub Actions workflows.

For å la GitHub deployere til ACI trengs en **Service Principal (SP)**, som har rettigheter til pull fra ACR. Men du får ikke opprettet Service Principal som vanlig "Contributor"-bruker i Azure, noe som gjelder de aller fleste brukerne i bedriftens Azure tenant.

For å løse dette er det definert en SP på forhånd, til bruk her. Den er er representert som et JSON objekt. Se presentasjonen om hvor du kan finne den. 

1.  Ta en kikk på den vedlagte YAML-filen (`.github/workflows/build_deploy_to_aci.yaml.template`).
     *  Legg merke til at den bruker endel "secrets", og disse eksisterer ikke ennå.
1.  Definér secrets som er brukt i YAML filen.
     *  Steg:
         1.  Gå til GitHub repository nettsiden
         1.  "Settings" fanen øverst på siden, ved siden av "Insights"
         1.  "Secrets and variables" og så "Actions" under den.
     *  Legg til hemmelighetene:
     *  `AZURE_CREDENTIALS`
         *  Hele JSON-representasjonen av SP, med clientId, clientSecret, tenantId osv.
     *  `REGISTRY_LOGIN_SERVER`
         *  Se presentasjonen om du bruker vår "standard", eller bytt til ditt eget ACR navn om du har en egen.
     *  `REGISTRY_USERNAME`
         *  `clientId` verdien fra SP (f.eks. fra JSON-repr). GUID alene er nok, du trenger ikke doble apostrofer.
     *  `REGISTRY_PASSWORD`
         *  `clientSecret` fra SP.
     *  `ACI_UNIQUENAME`
         *  Finn på et unikt navn til ACI resursen.
         *  Obs! Må være DNS-kompatibelt: maks 63 tegn. "Pro tip": hold deg til latinske bokstaver, tall og bindestrek.
     *  `RESOURCE_GROUP`
         *  Navn på ressursgruppen brukt over, som er der ACI(ene) skal tilhøre. Se også presentasjonen.
1.  På tide å teste GitHub Actions workflowen som skal bygge imaget, og sørge for å deployere til Azure ACI.
     1.  Dette kan løses enkelt i et "code space" rett på GitHub repository nettsiden.
          *  Bruk den store grønne "Code" knappen.
     1.  Lag en ny branch. Sjekk at du jobber på branchen.
          1.  Ctrl+j for å åpne terminal inni online code editoren (evt er terminalen allerede åpen).
          1.  `git checkout -b workflow`
     1.  Rename `.yaml.template` filen til `.yaml`, eller lag en tilsvarende kopi av filen som slutter med `.yaml`.
     1.  Commit.
          1.  I Ctrl+j terminalen:
          1.  `git status` for liste filer med endringer.
          1.  `git add .github/*` for å være sikker på å "stage" YAML filen.
          1.  `git commit -m "workflow setup"` for å committe til branch `workflow`
          1.  `git push --set-upstream origin workflow`
1.  Tilbake på repository hovedsiden.
     1.  Lag en pull request fra branchen til `main`.
     1.  Merge pull requesten inn i `main`.
1.  Se på GitHub Action som kjører.
1.  Når den er ferdig, og ikke hadde noen feil, ekspandér loggen på trinnet "Deploy to Azure Container Instances (ACI)".
     *   Du vil se en melding av typen `Your App has been deployed at: http://***.northeurope.azurecontainer.io:8080/`
     *   Stjernene er egentlig `ACI_UNIQUENAME` som du definerte istad.
     *   Lim inn URI i nettleseren din, og legg til `/swagger/index.html`
     *   F.eks. `http://test123.northeurope.azurecontainer.io:8080/swagger/index.html`
1.  Ta også en kikk i Azure Portalen om du har tilgang dit.
     *  `https://portal.azure.com` og søk etter ressursgruppenavnet brukt over. 
     *  Du finner en ACI ressurs med `ACI_UNIQUENAME` navnet ditt i ressursgruppen.


### Notat om å gjøre dette fra scratch selv

Dersom du har en privat Azure tenant, der du er Owner på subscription, kan du også opprette en Service Principal og gjøre alt selv der. Bare følg stegene i tutorialen som er linket under. Dette vil også fungere dersom du er Owner på ressursgruppen, men "bare" Contributor på subscription.

Det ligger en workflow (fra tutorialen) i filen: `.github/workflows/build_deploy_to_aci.yaml.template`. Bare bytt navn på den så den slutter på `.yaml`.

Se presentasjonen for navnet på **Ressursgruppen** nevnt under, samt **registry login server**.

#### Tutorialen oppsummert: 

1.  Ha en ACR opprettet på forhånd.
1.  Opprett ressursgruppe for ACI(er).
1.  Create Service Principal (SP), som er Contributor på ressursgruppen
1.  Assign AcrPush role til SP også, med scope til ACR-ressursen.
     *  Dette gir brukeren av SP rettigheter til push & pull images i ACR-ressursen.
1.  Definér secrets. Meningen med disse er beskrevet ovenfor.
     *  `AZURE_CREDENTIALS`
     *  `REGISTRY_LOGIN_SERVER`
     *  `REGISTRY_USERNAME`
     *  `REGISTRY_PASSWORD`
     *  `ACI_UNIQUENAME`
     *  `RESOURCE_GROUP`
1.  Branch koden.
1.  Aktiver GitHub actions workflow ved å rename YAML filen.
1.  Merge koden tilbake i "main" vha av Pull request f.eks.
1.  Se at Action kjører.

Tutorialen fra Microsoft om du er interessert i å gjøre dette: [Tutorial](https://learn.microsoft.com/en-us/azure/container-instances/container-instances-github-action)


# Ferdig!

Gratulerer, du kom deg gjennom!
