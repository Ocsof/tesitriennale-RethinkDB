-Installare Docker Desktop
-Installare Visual Studio o Visual Studio Code con le "estensioni" per l'utilizzo della libreria di Bchavez per interfacciarsi con RethinkDb  

Attenzione!! Ora La parte Docker (Cluster rethinkdb) funziona anche su windows.
             Corretto il file .sh richiesto dai DockerFile.
             Ogni volta che lo si "passa" da Windows a Mac bisogna crearsi un nuovo file .sh e riscriverselo per non avere problemi.
             In questo modo l'esecuzione del comando "docker-compose -f docker-compose.yml up -d" va a buon fine e i container rimangono "up" senza terminare subito!!


Aprire un terminale e dirigersi su rethinkdb-cluster-docker.
E' possibile ora scegliere in base alle proprie esigenze scegliere di utilizzare un Cluster Rethinkdb a 1, 2 o 5 nodi andando sempre via terminale sulla cartella specifica.

Una volta scelto, eseguire i seguenti comandi, validi per tutti e 3 i casi:
-docker-compose -f docker-compose.yml build
-docker-compose -f docker-compose.yml up -d

Aprire un browser e digitare localhost:8081, su questa porta infatti in tutti e tre i casi c'è un nodo Rethink in ascolto.

A questo punto aprire cartella RethinkdbApp e cliccare su RethinkDbApp.sln.
Aprire "start.cs" e seguire le istruzioni per eseguire l'applicativo in .Net
