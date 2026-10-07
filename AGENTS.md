# Contrat d'exécution — dw.tools.math

Lire PROJECT-MANTRA.md, PROJECT-CONSTITUTION.md, docs/planning/implementation-handoff.md, docs/planning/resource-cost-policy.md et le backlog avant tout travail.

- Périmètre d'écriture : ce dépôt uniquement, même si d'autres dossiers sont visibles. Ne pas modifier, committer, publier ou lancer une migration dans AURA, Decision, MCDM ou le brainstorming.
- Préparer les demandes externes dans docs/coordination/requests/ selon le modèle. Ne pas envoyer de message sans autorisation explicite ; une demande rédigée n'est pas une acceptation.
- Les décisions mathématiques sont vérifiées par des oracles indépendants et des contrats explicites. Un test historique vert ne prouve pas une spécification correcte.
- Le backlog produit définit portée, identifiants, dépendances et recettes. Après adoption DWF, son plan natif gouverne l'exécution ; la correspondance et les statuts produit sont réconciliés à partir des preuves, sans inventer une seconde autorité d'exécution.
- Ne pas écrire les fichiers gérés DWF avant son initialisation. Relire sa guidance active avant chaque préparation ; aucune copie figée de Decision n'est une autorité du runtime installé.
- Le validateur est .NET, sans dépendance Python. La vérification ne génère rien ; la génération appartient au payload.
- Zéro sous-agent par défaut ; travail et tests ciblés. Pas de boucle ouverte de revue.
- À chaque checkpoint : code, tests, contrats, backlog, projections et preuves cohérents. Ne pas appeler une extraction livrée tant que provenance, droits et consommateur isolé ne sont pas qualifiés.
- Pré-1.0 : pas de compatibilité artificielle des contrats abandonnés. Respecter néanmoins les engagements stables des consommateurs externes ; Decision a déjà des API 1.x.
- Lire docs/engineering/dotnet-authoring-checklist.md avant du C# non trivial.

