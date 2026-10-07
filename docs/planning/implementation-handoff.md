# Reprise par ChatGPT et DWF

## Situation

Le dépôt contient une planification, un outil documentaire .NET et des demandes externes draft. Pas de produit, pas de DWF initialisé, pas de publication. Tous les éléments produit sont planned.
Lire AGENTS.md, PROJECT-MANTRA.md, PROJECT-CONSTITUTION.md, architecture.md, existing-code-inventory.md, testing-and-quality.md et resource-cost-policy.md.

## Premier échange après init

1. Lire la version réelle de DWF et sa guidance active : bootstrap, constitution, agent-playbook, run-authoring et payload-authoring. Ne pas copier celle observée dans Decision comme contrat permanent.
2. Qualifier l'adoption d'après les faits : nouveau produit avec historique documentaire, aucune histoire de run inventée. Préserver project.id/title et schemaVersion créés par init.
3. Si le plan natif est vide et aucun run pending, écrire le premier vrai plan produit selon la procédure DWF d'initialisation. Ne pas écrire state.json à la main et ne pas créer un run de remplissage.
4. Utiliser [dwf-map.json](dwf-map.json) comme correspondance d'identifiants, pas comme fichier natif prêt à copier. Mapper release -> milestone, work_package -> workPackage, chunk -> phase, task -> task, subtask -> subtask. Ces phases sont fonctionnelles, pas une phase par RS.
5. Le premier résultat admissible est M0-W01-C01 : socle .NET, tests/pack et sample minimal. Raffiner ses fichiers/commandes avec la syntaxe du runner retenu, rendre ready le chemin natif nécessaire ; garder les suivants planned.
6. Le chunk M0-W01-C02 qualifie ensuite cette adoption et ses invariants, il ne retarde pas l'autorité native nécessaire avant le premier run.

La documentation produit expose 8 releases, mais le premier run ne doit pas implémenter tout M0. Conserver les IDs stables ; ne pas créer de nouvelle hiérarchie uniquement pour représenter chaque tentative.

## Autorités après adoption

Le backlog porte la portée et les recettes produit ; le plan natif porte l'autorité d'exécution DWF. Maintenir une correspondance vérifiée dans les mutations déclarées. Les statuts produit sont projetés depuis les preuves natives, jamais utilisés pour contourner une précondition DWF.
Au checkpoint : lire reçu réel, réconcilier statuts et preuves du backlog, générer les pages, vérifier. Une incohérence reste visible et interdit la préparation du scope concerné.
Le validateur local actuel ne lit pas les fichiers privés DWF et ne certifie pas la compatibilité native ; cette dernière appartient à M0-W01-C02 et à l'exécutable actif.

## Leçon Python et validations mutantes

La version de Decision observée utilise déjà dotnet run --file docs/planning/ValidatePlan.cs. Son ancien Python n'est pas à reprendre.
Dans Math, le C# vérifie par défaut et sort 0/1 sans modifier les fichiers versionnés. --write est une commande de génération explicitement mutante, interdite dans une validation preparation-safe.
Exemple d'intention de validation (à adapter au schéma réel installé) : fileName=dotnet ; arguments=[run,--file,docs/planning/ValidatePlan.cs] ; workingDirectory=. ; classification=structural ; packs=[].
La génération s'exécute dans le payload, puis la vérification dans la validation. Déclarer backlog, versions, README de planning, coverage, dwf-map et planning-validation parmi les mutations prévues.
Un helper C# staged s'invoque par son vrai chemin .workflow/runs/<id>/staged/... avec --file. Ne jamais référencer un helper d'un ancien run ou ajouter staged/ deux fois.
La sortie build temporaire du SDK n'est pas une permission d'écrire dans le produit ; les artefacts choisis restent ignorés et bornés.

## Avant ready

Dépendances internes et gates préalables done avec preuves ; dépendances externes satisfied attestées ; taille L décomposée sans perte d'exigences ; oracle indépendant ; fichiers exacts existants ou à créer identifiés ; commandes du runner effectif ; budget et retour arrière ; tests de limite ; aucun path extérieur.
Les noms de fichiers *Contract.cs du backlog sont des ancrages proposés. Les remplacer par responsabilités/types concrets lors du raffinement ; ne pas créer des classes nommées après les IDs pour cocher le plan.
Les commandes proposed_commands sont des intentions VSTest à adapter au runner retenu, pas des commandes validées à lancer sur des projets encore absents.

## Décomposition et clôture

Pour scinder L : garder le chunk comme agrégat, ajouter des chunks enfants avec parent_chunk, reporter ses exigences vers au moins un enfant, garder le parent dans la gate et ne le clôturer qu'après enfants. Régénérer le plan et mettre à jour la correspondance DWF sans perdre ses IDs historiques.
Un chunk done exige tâches/sous-tâches done et preuves locales existantes ; un spike done signifie décision rendue, pas feature livrée. La gate release réclame toutes ses recettes, pas seulement un compteur.
Remplir evidence/<id>.json avec les commandes réellement exécutées et artefacts, jamais avant exécution.
Exécution normale opérateur : dwf run next, selon guidance active ; en échec corriger le même run, ne pas effacer l'état ni en préparer un autre pour l'éviter.

## Sortie de session

Rapporter résultat et limites, prochain chunk admissible, validations réelles, aucune estimation de facturation inventée. Les prompts externes restent dans Math jusqu'à transmission autorisée. Ne jamais modifier les autres dépôts pour débloquer une gate.

