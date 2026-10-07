# MATH-XR-002 — Primitives partagées avec Decision

Statut : draft. Destinataire : agent Decision. Non envoyé.
Déclencheur : M3-W02-C04 ; compléments M6-W02-C05.
Package/version/hash/feed et preuves : À FOURNIR avant transmission.

Dans Decision uniquement, examine si les primitives qualifiées de Math peuvent remplacer sans changement de sémantique une partie de ExactDecimalScaler.cs, Simulation.cs (PRNG/distributions) et RiskAndRobustness.cs (quantiles génériques).
Compare les signatures, exceptions, unités, conventions quantiles, séquences PRNG et garanties de replay. Tes API 1.x restent protégées : garde une façade, ou propose une majeure explicitement si nécessaire.
Conserve EVPI, VaR/CVaR comme politiques de risque, robustesse, Monte-Carlo décisionnel, MDP, graphes, LP/MIP, CP-SAT, SMT et jeux.
Ne remplace rien sans duplication concrète et preuve indépendante. Qualifie scaling sans perte/overflow, vecteurs golden du PRNG et quantiles ; préserve résultats historiques.
Réponds par décision, fichiers concernés, plan de bascule/rollback, commit et tests si réalisé. Les demandes de nouvelles primitives sont adressées à Math, jamais développées dans son dépôt par ce run.

