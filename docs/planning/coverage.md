# Couverture du cadrage et de la demande

Projection du backlog. Couverture signifie responsabilité planifiée, pas qualification acquise.

| Source | Besoin | Chunks |
|---|---|---|
| SEED-EXACT | Arithmétique exacte, nombres, arrondis | M1-W01-C01, M1-W01-C02, M6-W02-C01 |
| SEED-UNITS | Quantités, unités et dimensions | M1-W01-C03, M1-W01-C04, M1-W02-C01 |
| SEED-EXPRESSION | Grammaire existante et sélection | M1-W02-C02, M1-W02-C03 |
| SEED-IR | IR, domaines, hypothèses, ensembles, équations, fonctions | M2-W01-C01, M2-W01-C02, M2-W01-C03, M4-W02-C02 |
| SEED-COMPOSE | Appels directs, fluent, pipelines et graphe | M2-W02-C01, M2-W02-C02, M2-W02-C03, M7-W01-C03 |
| SEED-NUMERIC | Algèbre linéaire, matrices, vecteurs, tenseurs | M3-W01-C01, M3-W01-C02, M3-W01-C03, M7-W01-C02 |
| SEED-STATS | Statistiques, probabilités, régression, distributions, aléatoire | M3-W02-C01, M3-W02-C02, M3-W02-C03, M6-W01-C01 |
| SEED-ANALYSIS | Calcul numérique, racines, interpolation et méthodes numériques | M6-W01-C01, M6-W01-C02, M6-W01-C03 |
| SEED-SYMBOLIC | Simplification, algèbre, factorisation, calcul différentiel/intégral, résolution | M4-W01-C01, M4-W01-C02, M4-W01-C03, M4-W02-C01, M4-W02-C02, M4-W02-C03 |
| SEED-FORMATS | LaTeX, MathML, Markdown, Unicode, Office, texte et JSON | M5-W01-C01, M5-W01-C02, M5-W01-C03, M5-W01-C04, M2-W01-C03 |
| SEED-SCIENTIFIC | Géométrie, trigonométrie, combinatoire, suites, signaux, séries temporelles, finance, fonctions spéciales | M6-W02-C01, M6-W02-C02, M6-W02-C03, M6-W02-C04 |
| SEED-PROOF | Dérivations, équivalence, vérification indépendante et preuve formelle | M4-W01-C03, M7-W02-C01 |
| SEED-ADVANCED | Précision arbitraire, intervalles et incertitude mathématique | M7-W01-C01 |
| SEED-PLOT | PlotModel et représentation graphique | M7-W01-C03 |
| SEED-PROVIDER | Réutilisation, licences, indépendance et portabilité | M0-W02-C01, M3-W01-C01, M4-W01-C01, M5-W02-C02 |
| SEED-V1 | Dix critères V1 du cadrage et consommation sans AURA | M5-W02-C01, M5-W02-C02, M5-W02-C03 |
| USER-EXTRACT | Extraire l'existant AURA avant extension | M0-W02-C01, M0-W02-C02, M1-W03-C01, M1-W03-C02 |
| USER-DWF | Hiérarchie complète, validation .NET et préparation observationnelle | M0-W01-C01, M0-W01-C02 |
| USER-BOUNDARY | Prompts entre agents, pas d'écriture cross-repository | M0-W02-C03, M1-W03-C02, M3-W02-C04, M6-W02-C05 |
| USER-CONSUMERS | Consommateurs transitifs nutrition/données/Decision/MCDM | M1-W03-C02, M3-W02-C04 |

## Critères V1 du cadrage

1. Parser une notation : M5-W01-C01/C02.
2. Valider domaines/hypothèses : M2-W01-C02/C03.
3. Calcul numérique et symbolique : M3 et M4.
4. Composer : M2-W02-C02.
5. Rejeter composition incompatible : M2-W02-C02.
6. Préserver exclusions : M4-W01-C02/C03.
7. Rendre notation : M5-W01-C01/C03/C04.
8. Résultats/erreurs/provenance : M2-W02-C01.
9. Replay qualifié : M2-W02-C03.
10. Client sans AURA : M1-W03-C01 et M5-W02-C01.

Gate M5-W02-C03 : confronter ces critères aux preuves réelles.

## Frontières et exclusions

- **DIS-AURA — Host resources, capabilities and supervision** : externally_owned, propriétaire AURA. Not mathematical primitives; integration via request. Réexamen : Math package ready.
- **DIS-DECISION — LP/MIP, CP-SAT, SMT decision problems, risk policies, EVPI, MDP and games** : externally_owned, propriétaire Decision. Retain decision semantics and stable API. Réexamen : Proven reusable primitive.
- **DIS-MCDM — Preferences, PROMETHEE, ELECTRE, ORESTE, AHP, TOPSIS, SMAA and ROR** : externally_owned, propriétaire MCDM. Retain multicriteria semantics; no automatic extraction. Réexamen : Proven numerical duplication.
- **DIS-FORMAL — Home-grown universal theorem prover** : excluded, propriétaire Math. Integrate qualified systems instead. Réexamen : Bounded formal interoperability need.
- **DIS-UNBOUNDED — Universal natural-language parser, arbitrary macros and unlimited engine** : excluded, propriétaire Math. Ambiguity and resource guarantees require declared subset. Réexamen : Explicit scope decision.
- **DIS-DOMAIN — Nutrition, record aggregation, storage, localization framework, market data** : externally_owned, propriétaire Respective domain maintainers. Only shared arithmetic primitive is a Math candidate. Réexamen : Consumer adoption.
