Installare Docker Desktop

Attenzione!! Su windows ho avuto problemi ad eseguire la nuova versione di Docker Desktop.
             Nel caso utilizziate Docker su sistemi Unix non ci sono problemi

Aprire un terminale e dirigersi su rethinkdb-cluster-docker.
E' possibile ora scegliere in base alle proprie esigenze scegliere di utilizzare un Cluster Rethinkdb a 1, 2 o 5 nodi andando sempre via terminale sulla cartella specifica.

Una volta scelto, eseguire i seguenti comandi, validi per tutti e 3 i casi:
-docker-compose -f docker-compose.yml build
-docker-compose -f docker-compose.yml up -d

Aprire un browser e digitare localhost:8081, su questa porta infatti in tutti e tre i casi c'è un nodo Rethink in ascolto.

A questo punto aprire cartella RethinkdbApp e cliccare su RethinkDbApp.sln.
Aprire "start.cs" e seguire le istruzioni per eseguire l'applicativo in .Net
