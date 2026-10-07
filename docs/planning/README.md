# Plan d'implémentation — dw.tools.math

**Objectif :** extraire le socle mathématique AURA puis construire une plateforme indépendante, typée et qualifiée.

Plan produit 0.1.0. Les états viennent du [backlog canonique](backlog.json). Aucun statut documentaire ne prouve une capacité produit.

Hiérarchie : **release -> work package -> chunk -> task -> subtask**. Mêmes niveaux que Decision ; DWF les associe à milestone/workPackage/phase/task/subtask.

| Jalon | Version cible | Résultat | Statut |
|---|---|---|---|
| [M0](versions/M0.md) | 0.1.0-preview | Établir une chaîne .NET reproductible et un transfert autorisé, sans modifier AURA. | planned |
| [M1](versions/M1.md) | 0.2.0-preview | Livrer les packages exacts autonomes et leur dossier d'adoption AURA. | planned |
| [M2](versions/M2.md) | 0.3.0-preview | Définir et vérifier un IR minimal compatible avec les quantités et les futurs providers. | planned |
| [M3](versions/M3.md) | 0.4.0-preview | Livrer les premières familles numériques derrière des contrats propres. | planned |
| [M4](versions/M4.md) | 0.5.0-preview | Préserver domaines et hypothèses dans les opérations symboliques bornées. | planned |
| [M5](versions/M5.md) | 1.0.0 | Qualifier la première plateforme cohérente numérique/symbolique, utilisable sans AURA. | planned |
| [M6](versions/M6.md) | 1.1.0 | Étendre les familles utiles par contrats et providers qualifiés, sans récupérer les domaines décisionnels. | planned |
| [M7](versions/M7.md) | 1.2.0-candidate | Qualifier les besoins avancés et livrer seulement ceux admis par ADR ; rendre les reports explicites. | planned |

M1 fournit tôt le socle exact ; 1.0 arrive après la verticale numérique/symbolique M5. Les spikes M7 peuvent conclure au report : leurs décisions ne sont pas des fonctions livrées. Versions cibles indicatives, pas dates ni autorisation d'exécuter tout le programme.

- [Architecture](architecture.md) et [inventaire](existing-code-inventory.md)
- [Couverture](coverage.md), [sources](sources.md) et [qualification](testing-and-quality.md)
- [Coordination](cross-repository-coordination.md) et [reprise DWF](implementation-handoff.md)
- [Politique de ressources](resource-cost-policy.md)

Portée : 8 releases, 17 lots, 53 chunks, 106 tâches, 318 sous-tâches, 106 exigences.

Vérifier : dotnet run --file docs/planning/ValidatePlan.cs. Régénérer après édition : même commande suivie de -- --write, exclusivement hors validation de préparation. Auto-tests : -- --self-test.

Dépendances fonctionnelles, pas de fan-out d'agents. Raffiner fichiers/commandes avant ready ; scinder L ; aucun package produit vide par anticipation.
