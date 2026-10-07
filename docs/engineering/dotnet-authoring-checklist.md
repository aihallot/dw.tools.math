# Préparation .NET et DWF

Le planning utilise un fichier C# exécutable .NET 10 et la BCL. Aucun Python, package NuGet ou shell Unix n'est nécessaire au validateur.
Le SDK 10.0.401 est observé localement, mais global.json, runner, versions MSTest et analyseurs produit restent à qualifier dans M0-W01-C01.

- Chemins de projets et commandes lowercase ; namespaces nouveaux PascalCase. Les namespaces dw.quantities hérités se conservent au transfert initial sauf ADR justifiée.
- Sorties compilées sous build/artifacts/, packages sous publish/. Ne pas ignorer tout build/ : des scripts sources pourraient y vivre.
- Gestion centrale des packages, verrouillage, nullable et analyseurs configurés au bootstrap.
- CancellationToken dernier argument public ; annulation contrôlée avant calcul puis aux points bornés.
- Vérifier signatures réelles du runner/framework, notamment assertions relationnelles ; ne pas transposer xUnit/NUnit vers MSTest.
- Si MSTest 4 retenu : TestMethod + DataRow, assertions de collections dédiées, helpers de bornes non ambigus.
- Éviter les doublons ContainsKey/indexer, allocations répétées de tableaux constants et exceptions portant le nom d'un paramètre absent.
- Restore/build/test partagent configuration et artifacts root. Ne pas lancer --no-build sur une sortie non construite.
- DWF : chemins générés exacts, dotnet run --file, arguments structurés ; stagedFiles sous le staged/ du run courant.
- Vérifications de préparation en lecture seule. Génération des projections dans le payload puis déclaration de tous les fichiers modifiés.
- Payload rejouable depuis baseline ou résultat attendu ; refuse les troisièmes états, ne masque pas la dérive.
- API publiques des providers vérifiées sur versions épinglées ; aucune hypothèse de disponibilité sur tous les RID.

