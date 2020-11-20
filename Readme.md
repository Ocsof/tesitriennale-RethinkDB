-Installare Docker Desktop
-Installare Visual Studio e nel suo marketplace, scaricare il pacchetto NuGet di Bchavez per interfacciarsi con RethinkDb in C#.

Attenzione!! Ora La parte Docker (Cluster rethinkdb) funziona anche su windows.
             Corretto il file .sh richiesto dai DockerFile.
             Ogni volta che si "passa" da Windows a Mac bisogna crearsi un nuovo file .sh e riscriverselo per non avere problemi.
             In questo modo l'esecuzione del comando "docker-compose -f docker-compose.yml up -d" va a buon fine e i container rimangono "up" senza terminare subito!!

Aprire un terminale e dirigersi sulla cartella Cluster_Docker_rethinkdb.
E' possibile ora scegliere, in base alle proprie esigenze, di utilizzare un Cluster Rethinkdb a 1, 2 o 5 nodi andando via terminale sulla cartella corrispondente:
Docker-RethinkdbCluster : 5 nodi,
Docker-RethinkDbDueNodi : 2 nodi,
Docker-RethinkDbUnNodo : 1 nodo.

Una volta scelto, eseguire i seguenti comandi, validi per tutti e 3 i casi:
- "docker-compose -f docker-compose.yml build" per costruire l'immagine --> è necessario usarlo solo la prima volta, una volta eseguito il comando l'immagine viene "salvata"
-"docker-compose -f docker-compose.yml up -d" per mettere in esecuzione il cluster. 

Aprire un browser e digitare "proprioIndirizzoDiRete:8081", su questa porta infatti in tutti e tre i casi c'è un nodo Rethink in ascolto.
Nel caso a due nodi è possibile digitare anche "proprioIndirizzoDiRete:8082".
Mentre nel caso a 5 nodi si possono digitare anche: "proprioIndirizzoDiRete:8083", "proprioIndirizzoDiRete:8084" "proprioIndirizzoDiRete:8085"

A questo punto aprire cartella RethinkdbApp e cliccare su RethinkDbApp.sln.
Aprire "start.cs" e seguire le istruzioni per eseguire l'applicativo in .Net
