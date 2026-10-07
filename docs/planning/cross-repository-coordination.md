# Coordination Math / AURA / Decision / MCDM

Ce document est le protocole produit canonique. Une note de réception côté AURA renvoie ici ; les prompts complets restent dans Math pour permettre à DWF de travailler seul.

## Autorité

Le run DWF Math écrit uniquement sous la racine Math. Sont interdits : mutation d'un sibling, git -C vers un autre dépôt en écriture, ProjectReference traversant la racine, script d'installation qui patche AURA, symlink/junction utilisé pour sortir du scope.
Une lecture source n'est possible que si explicitement accessible et autorisée ; aucun chemin local de l'inventaire n'est requis à l'exécution.
L'agent prépare un fichier de demande, le propriétaire le transmet. Aucun message réseau, issue ou PR externe n'est émis implicitement.

## Chaîne de transfert sans verrou circulaire

1. **Baseline** : l'agent AURA exporte/liste les sources et tests avec commit, hashes et droits ; l'agent Math constate cette entrée.
2. **Disponibilité Math** : Math implémente et qualifie le package à partir d'un snapshot contrôlé, sans toucher AURA. Un sample sans AURA suffit pour la gate locale.
3. **Demande d'adoption** : package/version/hash/feed utilisable, changements proposés, preuves et rollback sont transmis à l'agent AURA.
4. **Adoption consommateur** : son agent modifie références/façades et exécute ses tests sur sa branche. Les données, contrats, autorisations et ressources restent sous son contrôle.
5. **Accusé attesté** : retour avec commit, version consommée, résultats et limites. Math conserve une copie durable de la réponse dans docs/coordination/responses/.
6. **Retrait de la duplication** : AURA retire les sources devenues redondantes seulement après validation de ses consommateurs et possibilité de rollback. Math ne le fait jamais lui-même.

Dès le snapshot pris, un correctif urgent dans AURA impose une notification et une réconciliation explicite. La fenêtre de double code n'autorise pas deux trajectoires divergentes silencieuses.
Un refus/besoin d'API bloque seulement la bascule concernée. Les travaux Math indépendants peuvent continuer selon leurs dépendances.

## États d'échange

draft -> ready_for_owner -> sent -> acknowledged -> accepted ou rejected -> implemented -> verified.
Les transitions sent/acknowledged/accepted réclament une preuve humaine ou une réponse externe ; un fichier Markdown ne les crée pas.
L'état de la fonctionnalité Math (planned/ready/in_progress/blocked/done) reste distinct. Une requête produite peut clôturer son chunk documentaire, jamais une intégration externe.

## Contenu d'une demande

Identité stable, auteur/destinataire, statut, contexte, baseline source, contrat et package proposé, liste indicative de fichiers chez le destinataire, actions demandées, non-objectifs, recettes mathématiques et tests hôte, effets versions/données/skill, rollback, preuves et format de réponse.
Ne pas remplir un SHA, une version ou une réussite par supposition. Les champs marqués À FOURNIR doivent être renseignés avant ready_for_owner.

## Particularités AURA

Traiter simultanément graphe de compilation et packages transitifs : core.math, kernel, nutrition, données structurées, SQLite et tests consommateurs.
Préserver erreurs publiques, configuration, fractions exactes, catalogue, culture, affine température, limites et worker policies.
Le changement de propriétaire n'accorde aucune nouvelle ressource. Si commandes/help/defaults/capabilities changent, modifier la source versionnée du skill et régénérer ses pages ; jamais éditer seulement le généré.
Mettre à jour conception canonique, programme/backlog, roadmap, exécution et guidance d'AURA dans son dépôt.
Validation : pure math + action/worker ciblé + scénario CLI et consommateurs impactés ; pas toute la solution par défaut.

## Particularités Decision / MCDM

Decision expose déjà des versions 1.x : préserver API, erreurs, conventions de quantile, seed/PRNG et garanties existantes ou proposer une évolution majeure motivée.
MCDM conserve rangs, ties, orientation, préférences et quantification des flux. Une variation d'accumulation flottante peut changer un classement : les oracles métier restent déterminants.
Ne pas introduire de dépendance Math pour une abstraction sans usage. Proposer une primitive seulement après comparaison de comportement et coût.

## Rollback

Le consommateur conserve baseline et dépendances précédentes dans son historique, verrouille version et hash adoptés, et peut revenir au package/source antérieur sans migration de données cachée.
Si le format persisté change, un plan autonome de migration/rollback validé par son agent est requis avant adoption.
Pas de double publication sous même PackageId/version avec bytes différents. Le transfert d'identité et propriétaire de feed est attesté avant publication.

