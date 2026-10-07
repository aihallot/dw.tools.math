# Architecture et contrats

## Choix directeur

Trois approches examinées : déplacement massif de core.math (mélange politique et calcul, risque transversal), réécriture sur provider (perte possible d'exactitude et de comportement), extraction progressive des bibliothèques existantes (retenue).
La troisième préserve l'investissement et fournit tôt des packages testables. Elle accepte une duplication temporaire déclarée jusqu'à la bascule des consommateurs, jamais deux propriétaires permanents.

## Couches

1. Primitives exactes et quantités : rationnels, conversions exactes, dimensions et températures.
2. Catalogues et grammaire bornée : unités et alias déterministes, interprétation sans exécution de code.
3. Math IR : objets et opérations typés, domaines, hypothèses, provenance.
4. Adaptateurs : familles numériques/symboliques optionnelles, aucun type provider dans les contrats consommateurs.
5. Composition et codecs : validation, exécution bornée, échange et rendu.
6. Consommateurs indépendants : API .NET, sample console ; AURA apporte sa propre exposition.

La couche 1 fonctionne sans IR symbolique, sans AURA et sans provider lourd. Les futures couches ne doivent pas rendre ce socle dépendant d'elles.

## Packages et transfert

Au premier transfert, proposer de conserver dw.quantities, dw.quantities.expression et dw.quantities.standard et leurs namespaces pour séparer changement de propriétaire et évolution de modèle. Ce sont des identités proposées, pas des paquets déjà publiés. Sources cibles sous src/projects/<package>/, tests sous tests/projects/<package>.tests/.
Les futures familles dw.tools.math.* sont créées seulement avec comportement et consommateur prouvés. Pas de dizaines de projets vides.

Le catalogue standard dépend actuellement de dw.localization. M0-W02-C02 compare consommation d'un package partagé autorisé et remplacement de la dépendance de présentation par des métadonnées linguistiques passives. Recommandation : séparer données unitaires et présentation AURA sans déplacer toute la localisation dans Math. L'exact choix bloque le transfert du catalogue.
Il n'existe pas de preuve de licence de redistribution AURA dans l'inventaire racine : l'attestation du propriétaire et les notices sources précèdent toute copie distribuable.

## Valeurs et sémantique

Rationnel canonique à dénominateur positif ; zéro valide ; politique explicite du default struct et des entrées non initialisées. BigInteger ne signifie pas budget illimité. Borner tailles, puissances, allocations et produits intermédiaires.
ExactBinaryNumber préserve le binaire IEEE 754 reçu : FromDouble(0.1) n'est pas le rationnel décimal 1/10. Ne jamais confondre transport décimal et valeur binaire.
DimensionVector hérité a sept dimensions SI plus Information. La politique de dépassement des exposants et l'extension aux dimensions supplémentaires sont explicites.
Température absolue et intervalle sont distincts. Une différence d'absolus est un intervalle ; addition de deux absolus et abs d'un absolu ne sont pas justifiés par une simple égalité de dimensions.
Comparaison exacte par défaut ; approximation inclusive symétrique |a-b| <= max(atol, rtol*max(|a|,|b|)). Tolérances non négatives, dimensions compatibles ; rtol sans dimension ; zéro dimensionless autorisé comme atol nulle. Les absolus se comparent sur Kelvin.
Politique d'arrondi explicite pour affichage uniquement ; aucune chaîne arrondie utilisée pour décider égalité, tri ou admissibilité.

## IR

Étudier OpenMath, Content MathML et les AST des providers avant l'ADR. IR minimal : scalaires, symboles liés, opérations, relations, fonctions, collections finies, unités, domaines et hypothèses. Prévoir identités et versions de schéma sans fabriquer un langage universel.
Le parsing mathématique est séparé de l'évaluation ; la grammaire exacte héritée n'est pas rétroactivement qualifiée comme CAS symbolique.
Préserver les exclusions : (x²-1)/(x-1) peut devenir x+1 seulement avec x != 1. Sur les réels sqrt(x²)=|x|, et pas x sans x >= 0. Substitution sans capture, nombres exacts distincts des approximations, principal branch complexe déclarée.
Codec JSON déterministe sur le sous-ensemble admis ; limites de profondeur/nœuds/texte et refus des champs/versions non admis. Hash sémantique séparé du hash de présentation ; pas de promesse d'équivalence mathématique générale.

## Résultats et providers

Statuts distinguant succès exact, approximation, conditionnel, entrée invalide, domaine incompatible, non supporté, sans solution, inconnu, annulation, budget et échec provider. Un résultat partiel porte sa portée ; il ne vaut pas succès complet.
Provenance : opération/version, provider/version, paramètres effectifs, domaine, hypothèses, précision/tolérance, garantie et données de reproductibilité utiles.
Ne pas imposer tous les champs à une addition exacte ; enveloppes communes légères et résultats spécifiques.
Capabilities par opération, types d'entrée/sortie, domaines admis, politiques numériques, limites, plateforme et preuve ; support découvert ne vaut pas permission AURA.
Aucun fallback silencieux exact -> double ou provider A -> B. L'utilisateur/client choisit sa politique ; toute approximation ou changement apparaît.
Un CancellationToken ne permet pas d'interrompre de force un moteur natif. Qualifier coopération ou hôte externe borné ; AURA reste responsable de sa supervision et ses autorisations.

## Composition

API directe et fluent utilisent les mêmes contrats. Validation mécanique avant exécution : types, dimensions, domaines, shape matrices et hypothèses. Une composition numérique valide n'est pas une preuve d'équivalence symbolique.
Première pipeline séquentielle finie. DAG local optionnel en M7 après besoin démontré : cache borné, absence de cycles, provenance et nœuds partiels. Pas de scheduler distribué, de secrets ou de ressources AURA dans Math.

## Frontières et compatibilité

Statistiques descriptives, décompositions, distributions et arrondis génériques : Math.
LP/MIP, CP-SAT, SMT décisionnel, EVPI, MDP, politiques de risque et optimisation d'alternatives : Decision.
Normalisation liée à l'orientation des critères, seuils de préférence, PROMETHEE et quantification des flux : MCDM.
Recettes nutritionnelles, bases USDA, agrégation de records et stockage SQLite : leurs propriétaires.
Math peut proposer des primitives ; aucun déplacement automatique motivé seulement par la présence d'une multiplication.
Pré-1.0 Math peut évoluer avec ADR et migration ciblée. Les API Decision 1.x et données persistées externes conservent leurs garanties jusqu'à une décision du propriétaire.

