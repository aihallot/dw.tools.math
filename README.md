# dw.tools.math

Socle de calcul et de représentation mathématiques indépendant d'AURA, destiné aux applications, aux bibliothèques scientifiques et aux agents.

**État au 2026-10-07 : planification initiale, aucun moteur implémenté dans ce dépôt.** Le code candidat à l'extraction existe dans AURA ; sa présence là-bas ne constitue pas une livraison Math.

Première priorité : reprendre et qualifier les rationnels exacts, quantités, unités et expressions déjà réutilisables dans AURA. Ensuite : représentation mathématique commune, providers numériques/symboliques, composition et formats d'échange. Les algorithmes décisionnels restent dans Decision ; les méthodes multicritères restent dans MCDM.

- [Plan complet et versions](docs/planning/README.md)
- [Architecture et contrats](docs/planning/architecture.md)
- [Inventaire de l'existant](docs/planning/existing-code-inventory.md)
- [Migration et coordination entre dépôts](docs/planning/cross-repository-coordination.md)
- [Reprise par ChatGPT et DWF](docs/planning/implementation-handoff.md)
- [Premier prompt après initialisation DWF](docs/planning/start-with-dwf.md)
- [Backlog canonique](docs/planning/backlog.json)
- [Constitution du projet](PROJECT-CONSTITUTION.md)

## Validation documentaire

Depuis la racine, avec SDK .NET 10 (10.0.401 observé lors de l'initialisation) :

```powershell
dotnet run --file docs/planning/ValidatePlan.cs
```

Cette commande vérifie sans réécrire les documents. Après une modification volontaire du backlog, régénérer dans le payload ou lors de l'édition, puis vérifier :

```powershell
dotnet run --file docs/planning/ValidatePlan.cs -- --write
dotnet run --file docs/planning/ValidatePlan.cs
```

Aucun Python requis pour le planning. Aucun SDK métier, provider, CI produit ni DWF n'est installé par cette préparation. Les futurs scripts build/test/publish sont des livrables M0, pas des commandes déjà disponibles.

DWF travaillera uniquement dans ce dépôt. Les demandes à AURA, Decision et MCDM seront préparées en Markdown et transmises par le propriétaire aux agents responsables. Aucun accès en écriture aux autres dépôts n'est implicite.

Les noms de packages, licences de distribution et versions exactes des providers doivent être confirmés aux gates prévues ; aucune publication NuGet n'est annoncée.
