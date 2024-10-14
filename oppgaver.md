
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

PID'ene er forskjellige inni container og host, men det er de samme prosessene.


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


# Oppgave 3: Bygg og deploy med Dockerfiles

## 3.1 Bygg lokalt

1.  Sjekk om du har .NET SDK installert på VM:
     *  `dotnet --version` 
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

## 3.2 Bygg enda litt mer

1.  Dockerfile brukt i 3.1 var multi-stage. Prøv nå å bygge samme app, men med en single stage Dockerfile.. 
     *  Husk å stå i directory der `.sln` filen er.
     *  `docker build . -f DockerTrivia.API/Dockerfile-singlestage -t trivia-singlestage:latest`
1.  Ta en kikk på størrelsen på dette image, i motsetning til det fra oppg 3.1:
     *  `docker image ls`
     *  Hva kan størrelsesforskjellen komme av?
1.  Imaget kan evt slettes (ikke viktig):
     *  `docker image remove trivia-singlestage:latest`

## 3.3 Push til repository

Du kan bruke din egen ACR i denne øvelsen, eller bruke en som er allerede satt opp (enklest):

1.  Lag ny "latest" tag til `trivia:0.2` eller `0.1` fra tidligere:
     *  `docker tag trivia:0.2 trivia:latest`
1.  Lag enda en tag med adressen til ACR fra trivia:
     *  `docker tag trivia:latest triviaacr.azurecr.io/trivia:UNIQUETAG`
     *  UNIQUETAG : finn på noe unikt, f.eks. en string og noen siffer.
     *  Notér det ned
1.  Login til Docker
     *  `docker login -u triviaToken -p PASSWORD triviaacr.azurecr.io`
     *  Se presentasjonen for hvor du finner passordet.
1.  Push til ACR repositoriet:
     *  `docker push triviaacr.azurecr.io/trivia:UNIQUETAG`
     *  Sjekk `docker image ls` om du glemte hva din UNIQUETAG var
    
----

Ikke del av oppgaven: For å bruke din egen ACR, istendenfor `triviaacr.azurecr.io`, må du lage en scopeMap og en Token som bruker dette scopeMap'et. I Portalen: 

1.  ACR
1.  Repos. permissions
1.  Scope Maps
     *  Gi READ og WRITE permission til scope map.
1.  Tokens
     *  Bruke scope map fra forrige steg til tokenet, med repos.name `trivia` f.eks.

## 3.4 Pull fra repository

1.  Prøv å hente ned imaget laget av en av de andre: 
     *  `docker pull triviaacr.azurecr.io/trivia:OTHERTAG`
     *  OTHERTAG er den tag'en de andre brukte
     *  Alternativt: 
         *  `docker image rm triviaacr.azurecr.io/trivia:UNIQUETAG`
         *  `docker image rm trivia:0.1`
         *  `docker image rm trivia:0.2`
         *  `docker image rm trivia:latest`    
         *  `docker pull triviaacr.azurecr.io/trivia:UNIQUETAG`
1.  Prøv eventuelt å kjøre image som ble pulled:
     *  Vær sikker på at webserveren fra tidligere er stoppet:
     *  `docker stop web`
     *  Start image som du pull-et fra ACR:
     *  `docker container run -e HTTPLISTENPORT=80 -d --name trivia-acr --rm -p 80:80 triviaacr.azurecr.io/trivia:OTHERTAG`
1.  Besøk `<IP>/swagger/index.html` i nettleseren på din fysiske maskin, eller `curl localhost/swagger/index.html` på VM'en
     *  Det skal gå an å invokere `/api/questions` fra Swagger eller `curl`.

## 3.5 Deploy til ACI

Denne går ut på å deployere trivia-api til en Azure container instance (ACI) ved å bruke AZ CLI. Din domene-bruker må ha tilgang til Sør Agder's subscription i Azure for at dette skal fungere: 

```shell
az login --use-device-code
```

Velg ".. Sør Agder .." subscription om du blir spurt om å velge av `az login`.

Definér noen variabler i shellet. ACINAME = VM-name ("vm-1", "vm-2" etc.) + "-aci". F.eks. "vm-10-aci":

```shell
# Dobbelsjekk at du er logget inn: 
az account show

# Definér variabler i shellet:
RG=sysutv20241016
ACINAME=vm-N-aci
ACRPASSWORD=se presentasjonen!
UNIQUETAG=din egen tag
UNIQUEDNS=finn på noe
```

UNIQUEDNS bør for enkelhets skyld bare være bokstaver (a-z) og tall.

Kommandoen er som følger:


```shell
az container create \
    --resource-group $RG \
    --name $ACINAME \
    --image triviaacr.azurecr.io/trivia:$UNIQUETAG \
    --cpu 1 \
    --memory 1 \
    --registry-login-server triviaacr.azurecr.io \
    --registry-username triviaacr \
    --registry-password $ACRPASSWORD \
    --dns-name-label trivia-$UNIQUEDNS \
    --ports 8080
```

Etter en stund vil du få endel JSON output. Se etter "ipAddress / fqdn". Det er FQDN (fully qualified domain name) verdien vi skal bruke nå.

Du kan nå besøke trivia-api kjørende på ACI. Legg merke til at den kjører på port 8080:

1.  I nettleseren din, lim inn FQDN fra over: 
     *  `http://FQDN:8080/swagger/index.html`
1.  Det bør gå an å invokere api'et.
1.  Du kan evt slette ressursen din når du er ferdig: 
     *  `az container delete -g $RG -n $ACINAME`
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


## 3.6 Bygg med GitHub Actions

Alle de foregående manuelle stegene kan automatiseres med f.eks. GitHub Actions workflows.

For å deployere til ACI trengs en "service principal", som har rettigheter til pull fra ACR. Men du får ikke opprettet "service principal" som vanlig "Contributor"-bruker i Azure, noe som gjelder de aller fleste brukerne i Bouvet's Azure tenant.

Dersom du har en privat Azure tenant, der du er Owner, kan du derimot gjøre dette. 

Jeg får bygget og deployert til min private subscription i Azure med filen `.github/workflows/build_deploy_to_aci.yaml`, men det krever ganske mye oppsett for å komme dit. Etter mye om og men fungerer det likevel bra nå.

Du må kjøre endel AZ CLI kommandoer, og opprette en håndfull Secrets i GitHub. Prosessen er litt vrien, så det tar fort mer tid enn vi har tilgjengelig i kveld.

Ta gjerne en kikk på tutorialen fra Microsoft om du er interessert i å gjøre dette selv: 

[Tutorial](https://learn.microsoft.com/en-us/azure/container-instances/container-instances-github-action)


