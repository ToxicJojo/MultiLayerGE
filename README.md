# Multilayer Graph Engine



## Setup

Zum kompilieren von Graph Engine müssen die Pakte \verb|libunwind8|, \verb|g++|,  \verb|cmake| und \verb|libssl-dev| installiert werden:

```
  $ sudo -P apt install libunwind8 g++ cmake libssl-dev
```

Anschließend muss das .NET.Core Framework von Microsoft installiert werden. Dafür muss das entsprechende Pakte zur Paketverwaltung hinzugefügt werden.

```
 $ wget -q https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb
 $ sudo -P dpkg -i packages-microsoft-prod.deb
```

Das Paket \verb|apt-transport-https| und das .NET.Core Framework können nun installiert werden.
```
 $ sudo -P apt-get -y install apt-transport-https
 $ sudo -P apt-get update
 $ sudo -P apt-get -y install dotnet-sdk-2.2
```

Nun muss noch das Graph Engine Repository heruntergeladen und das Skript zum kompilieren des Projektes ausgeführt werden.

```
  $ git clone https://github.com/Microsoft/GraphEngine.git
  $ sed -i 's/make -j/make -j `nproc`/g' ./GraphEngine/tools/build.sh
  $ ./GraphEngine/tools/build.sh
```

Dabei ist zu beachten, dass das kompilieren sehr viel Arbeitsspeicher benötigt und dies auf Maschinen mit geringem Arbeitsspeicher zu Probleme führen kann.
Der Speicherbedarf kann verringert werden, indem die Parallelität beim Kompilieren verringert wird. Dies ist möglich indem der \verb|make| Befehl in \verb|build.sh| zu \verb|make -j 'nproc' || exit -1| geändert wird.
Dies reduziert die Anzahl an parallelen Prozessen die beim kompilieren genutzt wird auf die Anzahl der vorhanden Prozessoren. 

Nun können Graph Engine Anwendungen auf der Maschine ausgeführt werden. 
Eine Reihe an Beispielanwendungen, die in Graph Engine 2 übersetzt wurden, findet sich unter [https://github.com/ToxicJojo/graph-engine-samples](https://github.com/ToxicJojo/graph-engine-samples). Es ist auch ein kleines Skript vorhanden, das ein neues Graph Engine Projekt mit einem Client und Server erstellt.
