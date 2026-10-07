# Inventaire initial — 2026-10-07

Observation de fichiers de travail, pas audit exhaustif de correction ni preuve que les dépôts sont propres.
[source-baseline.json](source-baseline.json) fixe 54 fichiers sélectionnés et leurs SHA-256, ainsi que les HEAD observés. Une modification locale est caractérisée par son hash, pas attribuée automatiquement au HEAD.
Aucun code n'est copié à cette étape. M0 revalide les sources avant transfert et demande un export contrôlé à l'agent propriétaire si DWF n'a pas accès au dépôt source.

## AURA : transfert prioritaire

Racine observée : D:/data.dev/gitlab/AURA_projects/aura.

| Sources observées | Destination/traitement proposé | Chunks |
|---|---|---|
| lib/dw.quantities/ExactRational.cs | Package dw.quantities autonome, identité initiale conservée | M1-W01-C01 |
| ExactBinaryNumber.cs, ExactDecimalFormatter.cs du même dossier | Conversion binary64 exacte et formatage contrôlé | M1-W01-C02 |
| DimensionVector.cs, Quantity.cs, UnitDefinition.cs | Dimensions, quantités et transformations unitaires | M1-W01-C03 |
| ApproximateEquivalence.cs | Comparaison à tolérance explicite | M1-W01-C04 |
| lib/dw.quantities.standard/StandardUnitCatalog.cs, StandardExpressionUnitResolver.cs | Catalogue versionné et résolution indépendante | M1-W02-C01 |
| lib/dw.quantities.expression/ExpressionParser.cs | Grammaire exacte et fonctions de sélection bornées | M1-W02-C02/C03 |
| tests/dw.quantities.tests/ : ExtractedKernel, ExactFraction, ExpressionEngine, ApproximateEquivalence, StandardUnitCatalog, StandardExpressionUnitResolver | Tests Math importables après classification ; garder provenance | M1 et M1-W03-C01 |

Le noyau utilise BigInteger ; le parser dispose déjà de bornes (4096 caractères, 256 tokens, profondeur 32, 256 chiffres, puissance et racine 16). Ces bornes ne prouvent pas que chaque API primitive directe est bornée : c'est une recette à qualifier.

Les comportements observés incluent rationnels canoniques, fractions/nombres mixtes, conversions SI/impériales/US/culinaires, unités d'information, affine température, abs/min/max et comparaison. Aucun CAS général ou moteur flottant universel n'est inféré.

## AURA : séparation à décider

| Fichiers | Décision |
|---|---|
| src/aura.domains/core/aura.domains.core.math/UnitConverter.cs | Calcul source.ToBase puis target.FromBase candidat à une API pure ; adapter les erreurs côté AURA. Vérifier la température au-delà de l'égalité dimensionnelle. |
| ExpressionParser.cs du domaine | Façade vers dw.quantities.expression et resolver AURA : façade à conserver/adapter, moteur déjà séparé. |
| UnitCatalog.cs, MathCultureResolver.cs, UnitLocalization.cs, UnitProjection.cs | Classifier données/résolution génériques versus projection et présentation AURA ; ne pas déplacer en bloc. |
| lib/dw.quantities.standard/dw.quantities.standard.csproj | Dépend de dw.localization : ADR requise avant extraction, ne pas introduire une dépendance au checkout AURA. |
| lib/dw.quantities/ExactRational.cs et DimensionVector.cs | Caractériser default struct, valeurs invalides, taille intermédiaire et overflow exposants ; pas de correction silencieuse pendant transfert. |

## Ce qui reste AURA

CalculateAction/Contracts, ConvertAction/Contracts, CompareAction/Contracts, UnitListAction, UnitInspectAction, CoreMathDomainDeclaration, configuration et defaults hôte, policies sous config/worker-policies/core.math.*, documentation/skill embarquée, workers, autorisations et enveloppes.
Les tests tests/aura.tests.conformance/CoreMath* et tests/aura.tests.integration/CoreMath* sont des réservoirs de cas mathématiques ; les scénarios CLI/worker restent dans AURA.

## Consommateurs transitifs à ne pas oublier

- src/aura.kernel/aura.kernel.csproj référence expression et standard.
- domaine nutrition référence expression, standard et localisation.
- lib/dw.nutrition référence dw.quantities ; ses recettes, références alimentaires et calculs métier restent nutrition.
- lib/dw.data.transforms référence dw.quantities ; RecordTransformer et RecordAggregator portent les records/agrégations, pas Math.
- lib/dw.data.sqlite/SqliteTableReadContracts.cs construit des rationnels ; parcours INTEGER/REAL à préserver.
- lib/aura.json/JsonTransformProcessor.cs et JsonAggregationRecords.cs : parsing et projection des nombres exacts, garde des exposants et distinction décimal/binaire.
- dw.nutrition.sqlite/usda et autres clients transitifs : vérifier sérialisation, bases persistées et artefacts lors de l'adoption AURA.

Une recherche ciblée par références projets et symboles sera rejouée par l'agent AURA sur sa révision de bascule ; cet inventaire n'autorise pas à supprimer toutes les occurrences.

## Decision : candidats de mutualisation, pas de transfert immédiat

Racine : D:/data.dev/github/ai@hallot.net/dw.tools.decision.

- src/projects/dw.tools.decision.constraints/ExactDecimalScaler.cs : scaling decimal -> Int64 exact avec refus de perte/overflow. Primitive potentiellement générique, exceptions et contraintes CP-SAT restent Decision.
- src/projects/dw.tools.decision.simulation/Simulation.cs : PRNG déterministe, Bernoulli/uniforme, calculs de simulation. Séparer flux aléatoire/distribution des résultats et objectifs Monte-Carlo décisionnels ; conserver séquence versionnée.
- src/projects/dw.tools.decision.uncertainty/RiskAndRobustness.cs : quantiles/cumul génériques à étudier ; VaR/CVaR, robustesse et politiques restent propriétaires Decision.
- Pas de déplacement des graphes décisionnels, LP/MIP, CP-SAT, SMT, routage, EVPI, MDP et jeux. La présence de mathématiques ne suffit pas à changer le propriétaire.

## MCDM : frontières fines

Racine : D:/data.dev/github/ai@hallot.net/dw.tools.mcdm.

- lib/dw.tools.mcdm.core/Numerics/CompensatedSum.cs : candidat numérique concret.
- lib/dw.tools.mcdm.core/Numerics/FlowQuantization.cs : quantification liée aux flux ; conserver la convention multicritère.
- lib/dw.tools.mcdm.core/Relations/SquareMatrix.cs : structure liée aux relations ; comparer besoin d'une matrice générique avant proposition, pas remplacement de principe.
- lib/dw.tools.mcdm.application/Acquisition/Normalization.cs : orientation et transformation des critères restent MCDM même si des primitives peuvent devenir Math.
- PrometheeEngine, PreferenceFunction/Specification et les résultats de classement restent MCDM.

## Limites

Aucun test produit n'a été lancé par cet inventaire. Licences et anomalies sont des gates à instruire, pas des validations acquises. Les dépôts continuent d'évoluer : les hashes sont un point de départ daté, jamais une autorisation d'écraser du travail ultérieur.

