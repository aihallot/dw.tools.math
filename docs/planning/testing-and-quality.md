# Qualification scientifique et livraison

## Familles de preuves

CONTRACT : types, statuts, sérialisation, provenance et identité.
EXACT : rationnels et résultats analytiques indépendants, dimensions et températures.
BOUNDARY : entrées invalides, limites, allocation, annulation, ambiguïté et dépassement.
NUMERIC : résidu, erreur absolue/relative, conditionnement, cas singuliers et non-finis.
SYMBOLIC : restrictions, domaines, branches, substitution sans capture ; tests de points seuls insuffisants comme preuve.
INTEROP : sous-ensemble publié, round-trip sémantique et pertes de présentation déclarées.
PACKAGE : consumer isolé sur nupkg, dépendances transitives, absence de sibling references.
REPLAY : versions/politiques/seed fixées ; bit-à-bit uniquement sur périmètre qualifié.
PLANNING : intégrité des IDs, graphe, couverture, états, projections et correspondance DWF.

## Corpus d'extraction

Importer avec provenance les tests utiles de dw.quantities.tests et isoler les attentes mathématiques de CoreMath*Tests. Garder dans AURA les tests de worker, policies, help, configuration et enveloppes.
Ajouter un petit corpus indépendant : 1/3+1/6=1/2 ; 1 inch=127/5000 m ; 0 degC=27315/100 K ; 32 degF=0 degC ; intervalle de 9 degF=5 K ; 1 KiB=1024 B.
Conserver une comparaison normalisée (résultat exact + sémantique + erreur), pas des messages localisés byte-identiques par défaut.
Inventorier les défauts éventuels au lieu de les figer aveuglément comme compatibilité : default rationnel, bornes primitives directes, exposants et température hors parser.

## Oracles

Une même implémentation appelée deux fois n'est pas un oracle. Priorité aux résultats exacts calculables à la main, invariants indépendants, résidus et jeux de référence publiés avec licence.
Provider distinct pour corroborer un calcul quand pertinent, sans le considérer comme vérité automatique.
Matrices : A*x-b et reconstruction ; valeurs propres comparées via invariants et résidus, pas ordre/signe arbitraires.
Statistiques : convention population/échantillon, quantile, valeurs manquantes, poids et normalisation figés.
Symbolique : portée des hypothèses vérifiée structurellement ; un CAS qui retourne une expression sans garanties ne reçoit pas une preuve inventée.
Aléatoire : vecteurs golden PRNG, comptage des tirages, tests déterministes ; tests statistiques seuls ne bloquent pas sur une fluctuation aléatoire.
Les performances ont des limites de ressources et mesures reproductibles, sans seuils absolus de temps fragiles sur CI.

## Gates

Chaque release possède la liste complète de ses chunks et des gates préalables. Done exige toutes les tâches/sous-tâches et preuves, plus résultats de gate propres.
M1 qualifie le transfert dans Math et le consumer isolé ; l'adoption réelle AURA est un état externe distinct.
M5 / 1.0 exige la verticale complète numérique + symbolique + parsing/rendu + restriction préservée + composition invalide rejetée + client sans AURA.
Une plateforme non exécutée reste non qualifiée. Les artefacts mentionnent SDK, OS/architecture, provider et scope. Windows/Linux/macOS sont des cibles, pas des réussites anticipées.
Les tests de release portent uniquement les capacités admises. Les recherches M6/M7 et intégrations externes non terminées ne sont pas maquillées comme promesses de 1.0.

## Preuves durables

Fichier docs/planning/evidence/<chunk-id>.json : id, commit source, commandes exactes, working directory, configuration/SDK, scopes, code sortie, nombres pass/fail/skip, artefacts et SHA-256, oracle, limites et décision.
Ne pas prédire le SHA du commit qui contiendra la preuve : référencer le baseline et les artefacts ; DWF apportera le commit résultat dans son reçu.
Les preuves utilisées par le backlog sont des chemins locaux existants. Un rapport peut renvoyer au reçu DWF et à un commit externe immuable.
Le validateur vérifie la présence et cohérence structurelle, pas la véracité scientifique. Le reviewer doit lire la preuve avant done.

