# dw.tools.math — remise opérationnelle compacte (2026-10-09)

> **But :** reprendre immédiatement la boucle DWF, sans reconstituer les anciens runs et sans saturer le contexte. Lire ce document **en entier une fois**, puis travailler exclusivement depuis les autorités récentes GitHub.

## 1. Autorité et environnement

- Dépôt GitHub : **`aihallot/dw.tools.math`**, branche **`main`**.
- Racine opérateur Windows : `D:\data.dev\github\ai@hallot.net\dw.tools.math`.
- Commande opérateur unique : **`dwf run next`**. Ne pas demander une exécution différente ou des modifications manuelles.
- Sources de vérité, dans l'ordre : **GitHub main actuel**, preuves/rapports DWF durables sur leurs refs, `docs/planning/backlog.json`, `.aura/workflow/plan/project.json`, SDK `docs/workflow/payload-sdk-reference.md`, puis cette remise. Cette remise n'écrase jamais une preuve plus récente.
- Ne pas modifier les dépôts frères (AURA, Decision, MCDM) ni `dw.tools.workflow`. Autorisation propriétaire pour les seules sources Math d'AURA à la révision `82a6b435a387a7e116b47a6b2c433ae9e067bf21`; `docs/planning/source-baseline.json` et attestation `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`. Demande AURA `MATH-XR-002` **draft / transmission none**, jamais déclarer l'adoption effectuée.

## 2. État prouvé

- **M0 et M1 : done.** Gate M1 `docs/planning/evidence/M1-W03-C02-gate.json` ; les trois packages exacts sont qualifiés en local, non publiés chez AURA.
- **M2** (`Semantic representation and composition`) : **in_progress**. **M2-W01** (Mathematical model) : **in_progress**.
- `M2-W01-C01` ADR minimal typé : **done** (RS027).
- `M2-W01-C02` modèle IR immuable, graphes bornés et matrices : **done** (RS028 + RS029).
- `M2-W01-C03-T1` codec canonique `math-ir/1` : **done** (RS030, `attempt-001` `succeeded`, 12 fichiers, 0 violation). `M2-W01-C03` demeure **in_progress** ; `T2` et `T2-R` sont **ready**.
- Backlog actuel avant RS031 : **`plan_version: 0.1.29`** ; `.aura/workflow/state.json` : phase active `M2-W01-C03`, `pendingRun: null`.

## 3. Échec RS031 et correctif déjà staged

- `RS031` = renforcer le codec JSON, rejeter inconnus/doublons, restrictions sans symbole référencé, distinguer hashes SHA-256 structurel sémantique et présentation ; fermer **T2, C03 et M2-W01**, sans clôturer M2.
- La **dernière preuve terminale connue** : invocation `run-next-20261009T110432Z-0b137c58c59e4407`, branche `workflow/evidence/preselection-rs031/run-next-20261009T110432Z-0b137c58c59e4407`, rapport `.aura/workflow/evidence/<inv>/report.json`. Échec **pre-attempt**, classification `payload`, **attemptAllocated=false**, **retry=0**, **aucune mutation du produit** ; SHA d'autorité avant correction `2e8dd6e6effabcbec22f50893641a9d4dedce96d`.
- Erreur exacte : `MSTEST0032` à `tests/projects/dw.tools.math.ir.tests/M2W01C03BoundaryTests.cs:105` : assertion toujours vraie sur constante `IrCanonicalHashes.Scheme`.
- **Correctif déjà appliqué dans le staged test :** supprimer cette seule assertion redondante. Restent **11 tests adversariaux** et **3 RED**. Ne pas annuler ce correctif, et ne pas prétendre que les tests ont déjà été réexécutés.
- **À la reprise :** lire `.workflow/next-run/request.json` et `.workflow/next-run/payload.cs` sur `main`, contrôler la réentrée, puis **réagir au nouveau verdict utilisateur**. Le retry corrigé est **RS031 · retry 1 · attempts alloués 0**, sauf preuve ultérieure.
- Commit résultant attendu : `feat: harden canonical IR JSON and qualify distinct structural hashes`.

### Fichiers staged du RS031

Les six sources sont sous `.workflow/next-run/files/` puis leur chemin cible :

1. `src/projects/dw.tools.math.ir/IrCanonicalJsonCodec.cs`
2. `src/projects/dw.tools.math.ir/IrCanonicalJsonGuard.cs`
3. `src/projects/dw.tools.math.ir/IrCanonicalHashes.cs`
4. `tests/projects/dw.tools.math.ir.tests/M2W01C03BoundaryRedTests.cs`
5. `tests/projects/dw.tools.math.ir.tests/M2W01C03BoundaryTests.cs`
6. `docs/distribution/ir-canonical-json.md`

Request : **14 mutationPaths**, **6 stagedFiles**, **6 achievements** : `M2-W01-C03-T2-R`, `-G`, `-V`, `T2`, `M2-W01-C03`, `M2-W01`. Target du backlog : **0.1.30**. Preflight structurel uniquement ; compilation et tests exécutés dans le payload.

Contrats : JSON `math-ir/1` exact (BigInteger string), bits IEEE-754 conservés, fermetures des propriétés JSON, doublons rejetés, restrictions uniquement sur symboles présents dans l'expression. `IrCanonicalHashes.SemanticStructuralSha256` exclut les seuls libellés passifs ; `PresentationSha256` les conserve. Ce sont des **hashes structurels**, non des preuves d'équivalence algébrique ou de liaison/exécution de scopes. Limites : IR 1024 occurrences développées, profondeur 32, 262144 caractères JSON, numéral exact 1024 caractères. Aucune modification externe.

## 4. Boucle DWF, non négociable

**Sur `pushed` :** vérifier `.aura/workflow/reports/RS###/attempt-00N.json` pour `outcome=succeeded` et violations vides ; lire le backlog et le plan natif réels ; préparer un **nouveau RS** substantiel, avec tests, preuves et fermeture exacte des nœuds. Ne jamais répondre seulement « reçu » en laissant la suite non préparée.

**Sur `failed` :** chercher la preuve terminale `workflow/evidence/preselection-rs###/run-next-...` (ou la ref de preuve du dernier run alloué), lire `.aura/workflow/evidence/<inv>/report.json` et `.aura/workflow/handoff/snapshots/<inv>.json` sur `workflow/handoff/invocations/<inv>`. Extraire **failureStage, failureClassification, erreur, attemptAllocated et run.retry**. Si pre-attempt, prochain retry = dernier retry + 1 et **attempts alloués 0**. Si attempt alloué, suivre les compteurs réels des reçus. Corriger **le même RS**, sans supprimer les validations ou camoufler les oracles.

**Préparation:** modifier/ajouter les fichiers `.workflow/next-run/files/...`, puis `.workflow/next-run/payload.cs`; respecter la SDK `PayloadContext` avec `p.Files.ReplaceFromStaged`, `p.Files.WriteComplete`, `p.Json.EditObject` et `p.ProjectPlan.*`. Le payload doit accepter baseline et target exactes (DWF fait une **target re-entry**). Exécuter les RED avant GREEN sur baseline, puis tests ciblés et régressions appropriées. Ne pas muter les fichiers de produit directement via `File.WriteAllText`. Aucun développement du moteur DWF ; au plus transmettre du feedback.

**Dernier commit de préparation :** toujours `.workflow/next-run/request.json` **en dernier**, sur `main`. Audit avant livraison : préconditions exactes, chemins de mutation, fichiers staged, SDK, tests nommés, échecs précédents, EOF newline, taille raisonnable du run et nœuds DWF. Ne jamais deviner SHA, issue « pushed » ou nombre de retries.

**Pièges rencontrés :**
- `CS0219` constantes C# inutilisées ; `MSTEST0032` assertions sur constantes connues.
- `ProjectPlan.ActivateReadyContinuation` exige une hiérarchie active. Après clôture complète d'un milestone, utiliser les transitions explicites légales `not-ready -> ready -> in-progress`.
- Un validateur historique de gate versionné (`ValidateM1Gate.cs` exige `0.1.25`) ne doit pas être rejoué aveuglément sur une nouvelle version cible.
- Comptage des nœuds dupliqués en DAG : 1023 admis, 2047 refusés (plafond 1024).
- Evidence et liste de fichiers doivent être **idempotentes** à la réentrée.

## 5. Format de fin obligatoire

**Terminer chaque réponse opérateur** exactement par ces six lignes, **sans texte ensuite** :

```
**DIRECTORY:** `D:\data.dev\github\ai@hallot.net\dw.tools.math`  
**BRANCH:** `main`  
**PREPARED TIP:** `<SHA exact du dernier commit main>`  
**EXPECTED COMMIT:** `<request.commitMessage>`  
**STATUS:** `RS### · retry N · attempts alloués N`  
**COMMAND:** `dwf run next`
```

Ne mettre ces lignes qu'après une préparation réellement publiée et vérifiée. Si blocage empêchant la préparation, signaler explicitement qu'aucune commande ne doit être lancée, plutôt que d'inventer un SHA.

## 6. Bootstrap compact pour la nouvelle conversation

```text
$hop @GitHub — Reprends aihallot/dw.tools.math/main depuis docs/handover/dw-tools-math-RS031-2026-10-09.md. Lis ce seul handover intégralement puis consulte UNIQUEMENT les autorités récentes listées : main (request.json, payload.cs, backlog/plan) et dernier rapport/preselection/handoff DWF, sans réinventer les anciens runs. On est au RS031 : échec pré-attempt MSTEST0032 sur l'assertion constante du test JSON, correction staged et retry 1 à vérifier. Vérifie main et le compteur exact, puis, à CHAQUE réponse « pushed » ou « failed », prépare le prochain run ou corrige le même, en respectant les transitions et les oracles. Priorité aux runs substantiels et fiables ; n'écris pas dans les dépôts frères ni dans dw.tools.workflow. Finis toujours par les SIX lignes DIRECTORY/BRANCH/PREPARED TIP/EXPECTED COMMIT/STATUS/COMMAND, sans rien après. Si j'ai déjà exécuté le dernier dwf, base-toi sur sa preuve la plus récente.
```
