

## User
_Timestamp: 2026-11-28T08:28:27.9570000+00:00_

en attendant j'aimerais que tu me fasses un audit complet de notre solution actuelle:

- que couvre t elle
- le code a t il été optimisé avec les derniers développements / recommendations de dotnet 11 et csharp 15
- la couverture de discovery pour aura est elle optimale
- que pourrait on ajouter pour la prochaine version majeure, quels domaines je pense personnellement a&#x20;
  - tous les calcul temporels (avec un petit supplément sur les années académiques qui ne correspondent pas en général aux années civiles) y compris en tenant compte des fuseaux et des différentes représentation internes
  - extentions physiques , chimies, électricité avec les domaines de valeurs typées pour faire des fonctions typées (U = R \* I ou U est typée volt R ohm et I ampères)
  - à toi de me proposer d'autres pistes

## Assistant
_Timestamp: 2026-11-28T08:28:35.0120000+00:00_

Je vais auditer le dépôt à trois niveaux : les capacités réellement livrées et leurs preuves, l'exploitation de .NET 11/C# 15 et la surface de découverte AURA. Ensuite, je proposerai un roadmap majeur qui s'appuie sur les briques existantes plutôt que de dupliquer leur logique. Je ne modifierai pas le dépôt pendant cet audit.

## Assistant
_Timestamp: 2026-11-28T08:28:38.4730000+00:00_

The output of this plugin was redacted.

## Assistant
_Timestamp: 2026-11-28T08:28:39.1820000+00:00_

The output of this plugin was redacted.

## Assistant
_Timestamp: 2026-11-28T08:34:12.7200000+00:00_

# Audit stratégique et technique de `dw.tools.math`

**État de référence : RS429 — commit `a9547114a0ff0fc6ef6d0667bcc19f4079188ed8`.**

Mon premier constat est que nous disposons désormais d'un socle mathématique substantiel, indépendant des applications qui le consomment. En revanche, la complétude du roadmap actuel ne signifie ni que tous les domaines mathématiques sont couverts, ni que toutes les possibilités de .NET 11 sont exploitées, ni qu'AURA peut découvrir l'ensemble des capacités existantes.

C'est précisément sur ces trois axes que devrait porter la prochaine version majeure.

## 1. État général de la solution

<grid columns={2} gap={3}>
  <grid-item>
    <card gap={1}>
      <caption>Bibliothèques de production</caption>
      <title size="2xl">13</title>
    </card>
  </grid-item>
  <grid-item>
    <card gap={1}>
      <caption>Projets de tests</caption>
      <title size="2xl">11</title>
    </card>
  </grid-item>
  <grid-item>
    <card gap={1}>
      <caption>Tests de régression réussis</caption>
      <title size="2xl" color="success">2 722</title>
    </card>
  </grid-item>
  <grid-item>
    <card gap={1}>
      <caption>Capacités Discovery sélectionnées</caption>
      <title size="2xl">4</title>
      <caption>0 BrokerReady</caption>
    </card>
  </grid-item>
</grid>

| Axe | Appréciation |
|---|---|
| Architecture et séparation des responsabilités | Solide |
| Exactitude et contrats numériques | Solide dans le périmètre qualifié |
| Reproductibilité et traçabilité | Solide |
| Migration .NET 11 / C# 15 | Terminée et certifiée |
| Optimisation des performances | À mesurer et approfondir |
| Couverture Discovery pour AURA | Très partielle |
| Calculs temporels et calendaires | À développer |
| Typage physique et scientifique avancé | Socle existant, extensions nécessaires |
| Distribution des nouveaux packages | Qualification acquise, publication restante |

Le dépôt n'a pas été modifié pour cet audit.

# 2. Que couvre réellement notre solution ?

## 2.1. Cartographie des bibliothèques existantes

J'ai recensé 88 fichiers C# de production, 151 fichiers C# de tests, 13 bibliothèques et 49 projets de consommateurs de qualification dans le dépôt. Ces derniers comprennent des consommateurs historiques : ils ne sont pas tous des applications actuellement maintenues.

| Bibliothèque | Responsabilités actuelles |
|---|---|
| `dw.quantities` | Nombres rationnels exacts, dimensions physiques, quantités, températures, conversions, comparaisons |
| `dw.quantities.expression` | Analyse et évaluation d'expressions mathématiques dimensionnées |
| `dw.quantities.standard` | Catalogues d'unités, conversions et profils spécialisés |
| `math.foundation` | Contrat fondamental et identité de la bibliothèque |
| `math.ir` | Représentation intermédiaire typée, domaines, hypothèses, matrices, JSON canonique |
| `math.composition` | Composition d'opérations, pipelines exacts, replay, cache sémantique |
| `math.numerics` | Vecteurs, matrices denses et creuses, LU, QR, résolution linéaire |
| `math.statistics` | Statistiques descriptives, corrélations, quantiles, distributions, génération pseudo-aléatoire |
| `math.symbolics` | Transformations symboliques, dérivation, résolution, intégration polynomiale |
| `math.analysis` | Interpolation, régression, recherche de racines, intégration numérique, EDO |
| `math.scientific` | Entiers, combinatoire, fonctions spéciales, géométrie, signaux, séries temporelles, finance |
| `math.codecs` | Texte canonique, LaTeX, MathML, Markdown, UnicodeMath, Office OMML |
| `math.distribution` | Composition verticale des moteurs, qualification et distribution |

Le préfixe `dw.tools.` est omis dans les dix dernières lignes pour faciliter la lecture.

## 2.2. Architecture actuelle

<card gap={2}>
  <svg viewBox="0 0 340 384" width="100%">
    <rect x="12" y="8" width="316" height="45" rx="8" fill="#64748b" fillOpacity=".15" stroke="#64748b"/>
    <text x="170" y="27" textAnchor="middle" fontSize="12" fontWeight="600" fill="currentColor">AURA · Decision · Applications</text>
    <text x="170" y="42" textAnchor="middle" fontSize="9.5" fill="currentColor">Consommateurs indépendants de Math</text>
    <path d="M170 53 V76" stroke="currentColor" strokeOpacity=".45" strokeWidth="1.5"/>
    <rect x="12" y="76" width="316" height="46" rx="8" fill="#8b5cf6" fillOpacity=".1" stroke="#8b5cf6"/>
    <text x="170" y="94" textAnchor="middle" fontSize="12" fontWeight="600" fill="currentColor">Discovery · Distribution</text>
    <text x="170" y="109" textAnchor="middle" fontSize="9.5" fill="currentColor">Exposition, qualification, consommation</text>
    <path d="M170 122 V145" stroke="currentColor" strokeOpacity=".45" strokeWidth="1.5"/>
    <rect x="12" y="145" width="316" height="79" rx="8" fill="#0ea5e9" fillOpacity=".09" stroke="#0ea5e9"/>
    <text x="170" y="162" fontSize="11" fontWeight="600" textAnchor="middle" fill="currentColor">Moteurs mathématiques</text>
    {#each ["Numerics","Statistics","Symbolics","Analysis","Scientific","Codecs"] as item,i}
      <rect x={22+(i%3)*102} y={171+Math.floor(i/3)*24} width="92" height="20" rx="4" fill="#0ea5e9" fillOpacity=".14"/>
      <text x={68+(i%3)*102} y={185+Math.floor(i/3)*24} fontSize="9.5" textAnchor="middle" fill="currentColor">{item}</text>
    {/each}
    <path d="M170 224 V245" stroke="currentColor" strokeOpacity=".45" strokeWidth="1.5"/>
    <rect x="12" y="245" width="316" height="44" rx="8" fill="#0d9488" fillOpacity=".12" stroke="#0d9488"/>
    <text x="170" y="263" textAnchor="middle" fontSize="12" fontWeight="600" fill="currentColor">IR · Composition</text>
    <text x="170" y="278" textAnchor="middle" fontSize="9.5" fill="currentColor">Représentation, sémantique, replay</text>
    <path d="M170 289 V310" stroke="currentColor" strokeOpacity=".45" strokeWidth="1.5"/>
    <rect x="12" y="310" width="316" height="46" rx="8" fill="#26976a" fillOpacity=".14" stroke="#26976a"/>
    <text x="170" y="328" textAnchor="middle" fontSize="12" fontWeight="600" fill="currentColor">Quantities · Expression · Standard</text>
    <text x="170" y="343" textAnchor="middle" fontSize="9.5" fill="currentColor">Rationnels exacts, dimensions, unités</text>
    <text x="170" y="376" textAnchor="middle" fontSize="9" fill="currentColor" opacity=".65">Vue fonctionnelle simplifiée, non graphe des références CLR</text>
  </svg>
</card>

Le principal avantage de cette architecture est de conserver **Math comme propriétaire des calculs génériques**, sans dépendance vers AURA, Decision ou un autre domaine métier.

Je conserverais absolument cette orientation.

## 2.3. Couverture mathématique détaillée

| Domaine | Couverture | Limites principales |
|---|---|---|
| Arithmétique exacte | Bonne | Budgets et représentations à généraliser |
| Fractions et rationnels | Bonne | Pas de moteur général de précision arbitraire pour les transcendantes |
| Unités et conversions | Bonne base | Catalogue scientifique encore incomplet |
| Analyse dimensionnelle | Implémentée | Pas de typage statique généralisé des grandeurs |
| Algèbre linéaire | Bornée | LU et QR, dimension maximale 16 pour certaines opérations |
| Matrices et vecteurs | Bornée | 4 096 éléments, budgets de produits scalaires |
| Statistiques descriptives | Bonne | Plusieurs API historiques limitées à 4 096 observations |
| Statistiques streaming | Implémentée | Moments jusqu'à un million d'observations |
| Quantiles | Bonne dans le périmètre admis | Un million d'observations binary64 ou 8 192 exactes avec budgets explicites |
| Probabilités | Partielle | Bernoulli et uniforme notamment ; nombreuses distributions absentes |
| Génération aléatoire | Bonne base déterministe | Pas de catalogue étendu de générateurs et distributions |
| Algèbre symbolique | Bornée | Pas de CAS universel |
| Dérivation | Bornée | Sous-ensemble exact qualifié |
| Intégration symbolique | Bornée | Polynômes jusqu'au degré 7 dans l'API concernée |
| Analyse numérique | Bornée | Racines, quadrature de Simpson, interpolation, EDO |
| Combinatoire | Partielle | Factorielle, binomiale, PGCD, PPCM, suites |
| Fonctions spéciales | Partielle | Sinus, cosinus, exponentielle, logarithme, Gamma |
| Géométrie | Partielle | Normes, distances, projections |
| Traitement du signal | Très borné | DFT directe et convolution, notamment sur 32 échantillons |
| Séries temporelles | Très bornée | Différences et moyennes mobiles sur séries uniformes |
| Mathématiques financières | Partielle | Actualisation et TRI, conventions temporelles limitées |
| Calculs calendaires | Très faible | Quelques calculs de dates financières |
| Tenseurs | Recherche uniquement | Pas de moteur public livré |
| Arithmétique d'intervalles | Recherche uniquement | Pas de moteur général qualifié |
| Calculs par graphes DAG | Recherche uniquement | Pas de runtime public |
| Visualisation mathématique | Recherche uniquement | Pas de PlotModel public |
| Démonstration formelle | Recherche limitée | Pas de moteur général de preuves |

Il existe donc une très bonne fondation, mais plusieurs familles scientifiques sont actuellement davantage des **premières implémentations bornées** que des moteurs destinés à des usages industriels étendus.

Sources : [architecture](https://github.com/aihallot/dw.tools.math/blob/main/docs/planning/architecture.md), [recherche M7](https://github.com/aihallot/dw.tools.math/blob/main/docs/distribution/m7-w01-advanced-research-matrix.md), [sources de production](https://github.com/aihallot/dw.tools.math/tree/main/src/projects).

## 2.4. Couverture des tests

<card gap={2}>
  <caption>Tests réussis par famille lors de RS419</caption>
  {#each [{n:"Scientific",v:995},{n:"Symbolics",v:370},{n:"Codecs",v:350},{n:"Analysis",v:328},{n:"Statistics",v:192},{n:"Distribution",v:155},{n:"Quantities",v:127},{n:"Numerics",v:81},{n:"Composition",v:74},{n:"IR",v:49},{n:"Foundation",v:1}] as item}
    <row align="center">
      <box width="88px">
        <text size="sm">{item.n}</text>
      </box>
      <box flex={1}>
        <box height={16} radius="sm" background="#3284ba" width={`${Math.max(.4,item.v/995*100)}%`}/>
      </box>
      <text size="sm" weight="medium" tabularNums={true} width="32px" textAlign="end">{item.v}</text>
    </row>
  {/each}
  <divider/>
  <row>
    **Total**
    <spacer/>
    **2 722**
  </row>
</card>

C'est une base de non-régression importante. Néanmoins, le nombre de tests ne constitue pas une mesure de couverture des lignes, des branches ou de la qualité des oracles mathématiques.

Je recommande de compléter cette métrique par la couverture de code, les tests de propriétés mathématiques, les tests différentiels et, pour les contrats critiques, les tests de mutation.

[Rapport de régression RS419](https://github.com/aihallot/dw.tools.math/blob/main/.aura/workflow/reports/RS419/attempt-001.json)

---

# 3. Exploitons-nous réellement .NET 11 et C# 15 ?

**Partiellement. La migration a été effectuée correctement, mais la modernisation du code n'est pas exhaustive.**

## 3.1. Ce qui est déjà bien fait

Le dépôt possède une configuration moderne et cohérente :

| Configuration | État |
|---|---|
| SDK 11.0.100 | Épinglé |
| .NET 11 | Actif |
| C# 15 | Actif |
| Nullable reference types | Activés |
| Analyseurs .NET | Activés |
| AnalysisLevel | Latest |
| Warnings as errors | Activé |
| Compilation déterministe | Activée |
| Gestion centralisée des packages | Activée |
| Sorties de compilation | Externalisées |
| Restauration verrouillée | Qualifiée |

Le code utilise déjà des types valeur immuables, des `record struct`, des collections immuables, des `FrozenDictionary`, des expressions de collection et, dans certaines opérations, des `Span` et `ReadOnlySpan`.

Une modernisation concrète a également été livrée dans RS343 : réduction des allocations lors du parsing des rationnels exacts grâce aux spans. Cette amélioration a fait l'objet d'une comparaison sur les deux runtimes avant son intégration.

[Décision RS343](https://github.com/aihallot/dw.tools.math/blob/main/docs/planning/decisions/m8-w02-c02-exact-rational-span-integration-rs343.md)

## 3.2. Ce que .NET 11 apporte automatiquement

Les améliorations du JIT, de l'élimination des contrôles redondants, de la désallocation de certains objets temporaires et de l'optimisation des boucles peuvent bénéficier au code existant sans réécriture. <Cite refs={["turn513277search6","turn513277search8","turn513277search10"]}/>

Mais nous n'avons pas mesuré systématiquement les gains propres à notre solution. Il serait donc incorrect d'affirmer que tous nos moteurs sont devenus significativement plus rapides.

## 3.3. Opportunités encore inexploitées ou insuffisamment qualifiées

| Technologie | Application dans Math | Priorité |
|---|---|---|
| `Decimal32`, `Decimal64`, `Decimal128` | Nouvelles représentations numériques | Haute |
| `Complex<T>` | Calcul complexe à plusieurs précisions | Haute |
| `TryParsePartial` | Analyse syntaxique et parsing numérique | Haute |
| `Span<T>` et `ReadOnlySpan<T>` | Réduction des allocations | Haute |
| SIMD / `Vector128` / `Vector256` | Matrices, vecteurs, statistiques | Haute |
| `TensorPrimitives` | Accélération de certains noyaux numériques | Haute |
| Generic Math | Mutualisation contrôlée des algorithmes | Haute |
| Union types C# 15 | Résultats exacts, approchés, refusés | Haute |
| Closed hierarchies C# 15 | Sécurisation des variantes de l'IR | Haute |
| JSON source generation | Transport AURA et compatibilité AOT | Haute |
| Nouvelles API Random génériques | Génération non déterministe optionnelle | Moyenne |
| Représentation hexadécimale IEEE 754 | Oracles binaires et replay | Moyenne |
| Native AOT | Consommateurs légers et exécutables autonomes | À qualifier |

Ces possibilités sont documentées dans les nouveautés officielles de .NET 11 et C# 15. <Cite refs={["turn739301search0","turn739301search2","turn513277search5"]}/>

Attention toutefois : Generic Math et les spans ne sont pas des nouveautés propres à .NET 11. Leur intérêt réside dans une exploitation plus systématique au sein de notre code.

## 3.4. Mes recommandations concrètes par moteur

### Rationnels exacts

Nous utilisons `BigInteger`, avec normalisation par PGCD et conservation exacte des valeurs. Je privilégierais l'amélioration des algorithmes et de leurs allocations plutôt que la création d'un moteur BigInteger concurrent.

Il faut mesurer les réductions de fractions avant multiplication, les comparaisons, le parsing de longues chaînes, les conversions et le coût des opérations intermédiaires.

Notre `RationalBinary64Conversion` calcule notamment l'erreur exacte de conversion. Il ne faut pas le remplacer par un simple cast sous prétexte que .NET 11 améliore certains arrondis.

### Matrices et statistiques

`QuantileStatistics64` matérialise et trie actuellement les observations. Pour un million de valeurs, il existe une possibilité d'optimisation algorithmique réelle :

| Opération | Évolution à étudier |
|---|---|
| Quantile isolé | Algorithme de sélection plutôt que tri complet |
| Plusieurs quantiles | Tri réutilisé ou sélection multiple |
| Moyenne et variance | Surcharge sur span, calcul streaming |
| Produits scalaires | SIMD avec référence scalaire |
| Multiplication matricielle | Stockage contigu et kernels spécialisés |
| Grandes matrices creuses | Formats CSR et CSC |
| Moments streaming | Fusion d'accumulateurs avec politique numérique explicite |

Toute modification doit préserver les conventions numériques et les comportements historiques. Un changement d'ordre des additions flottantes peut modifier les résultats.

Microsoft recommande justement de privilégier les API vectorisées existantes et de ne recourir aux intrinsics spécialisés qu'après identification d'un véritable point chaud. <Cite ref="turn513277search11"/>

### IR et codecs

Le modèle actuel utilise notamment :

```csharp
public abstract record IrNode;
public abstract record IrScalar : IrNode;
```

C'est un excellent candidat pour étudier les hiérarchies fermées de C# 15 et les types union.

L'objectif serait de rendre plus difficiles les états impossibles et de mieux contrôler l'exhaustivité des traitements, tout en conservant la compatibilité du format `math-ir/1` et de ses empreintes.

### Génération pseudo-aléatoire

Je conserverais impérativement notre `SplitMix64Random`.

Les nouvelles API génériques de `Random` peuvent compléter l'offre, mais ne doivent jamais remplacer silencieusement les algorithmes versionnés dont dépendent le replay, les distributions et les consommateurs historiques.

## 3.5. Lacune : un véritable laboratoire de performances

Je n'ai pas trouvé de projet BenchmarkDotNet dédié dans l'inventaire actuel.

Je proposerais d'ajouter un laboratoire de benchmarks indépendant de la production, par exemple :

`benchmarks/dw.tools.math.benchmarks`

Il mesurerait les opérations les plus utilisées, les allocations, la consommation mémoire, les performances de démarrage, les comparaisons entre implémentations scalaires et vectorisées ainsi que les résultats numériques.

Les optimisations seraient acceptées sur la base de mesures reproductibles, et non d'une préférence syntaxique pour les dernières nouveautés du langage.

---

# 4. Audit Discovery pour AURA

C'est, à mon avis, **la principale faiblesse stratégique de la solution actuelle**.

Nous avons investi énormément dans les capacités mathématiques, mais leur surface de découverte reste très réduite.

## 4.1. Ce qu'AURA peut actuellement découvrir via le manifeste Math

Le manifeste sélectionne exactement quatre opérations.

| Identifiant | Responsabilité |
|---|---|
| `math.quantity.exact-pipeline.evaluate` | Pipeline de calcul exact |
| `math.replay.try-evaluate` | Exécution reproductible |
| `math.ir.canonical.encode` | Encodage canonique de l'IR |
| `math.ir.hash.semantic` | Empreinte structurelle |

<card gap={3}>
  <caption>État du manifeste Discovery</caption>
  <grid columns={2}>
    <grid-item>
      <title size="2xl">4</title>
      <caption>AgentCallable</caption>
    </grid-item>
    <grid-item>
      <title size="2xl">0</title>
      <caption>BrokerReady</caption>
    </grid-item>
  </grid>
  <divider/>
  {#each [{n:"Quantités et composition",v:1},{n:"Replay",v:1},{n:"IR",v:2},{n:"Statistiques",v:0},{n:"Algèbre linéaire",v:0},{n:"Calcul symbolique",v:0},{n:"Analyse numérique",v:0},{n:"Physique et chimie",v:0},{n:"Finance",v:0},{n:"Temps et calendriers",v:0}] as item}
    <row align="center">
      <box flex={1}><text size="sm">{item.n}</text></box>
      <text color={item.v?"success":"secondary"} tabularNums={true}>{item.v}</text>
    </row>
  {/each}
</card>

Cela ne signifie pas qu'AURA ne possède aucune autre intégration Math propre à son dépôt. Cela signifie que **le mécanisme de Discovery fourni par Math lui-même n'expose que ces quatre opérations**.

[Manifeste Discovery actuel](https://github.com/aihallot/dw.tools.math/blob/main/.agents/skills/dw-tools-math/references/manifest.json)

## 4.2. Défauts et limitations identifiés

| Constat | Impact |
|---|---|
| Manifeste limité à quatre capacités | Une grande partie de Math reste invisible aux agents |
| `MathCapabilityCatalog` ne contient qu'une description d'inspection matricielle | Catalogue interne très incomplet |
| Métadonnées du manifeste encore liées à `net10.0` et à des versions source historiques | Décalage avec la production net11 |
| Binder principalement orienté méthodes statiques | Certaines API d'instance ne sont pas directement sélectionnables |
| Paramètres CLR complexes | Construction des appels difficile pour un LLM |
| Zéro BrokerReady | Pas de qualification d'invocation via broker |
| Pas de preuve d'installation effective dans AURA | Adoption externe non attestée côté Math |
| Protocole épinglé à `1.0.0-draft.1` | Alignement avec l'autorité AURA actuelle à confirmer |

Il existe cependant de bonnes fondations à préserver : manifestes déterministes, empreintes CLR, exemples exécutables, qualification des archives, vérification d'intégrité, navigation progressive et export portable Windows/Linux.

Je ne reconstruirais donc pas Discovery depuis zéro.

## 4.3. Architecture Discovery cible

<card gap={2}>
  <svg viewBox="0 0 340 360" width="100%">
    {#each [{y:8,t:"API Math existantes",s:"Exact · Scientific · Temporal · Statistics",c:"#64748b"},{y:78,t:"Inventaire automatique",s:"Types, signatures, versions, dépendances",c:"#3284ba"},{y:148,t:"Admission sémantique",s:"Contrats, budgets, unités, oracles, exposition",c:"#0d9488"},{y:218,t:"Génération des surfaces",s:"Skill · Manifest · Schémas · Exemples · Bindings",c:"#8b5cf6"},{y:288,t:"AURA",s:"Découverte · Autorisation · Invocation · Vérification",c:"#26976a"}] as item,i}
      <rect x="14" y={item.y} width="312" height="52" rx="8" fill={item.c} fillOpacity=".12" stroke={item.c}/>
      <text x="170" y={item.y+21} fontSize="12" textAnchor="middle" fontWeight="600" fill="currentColor">{item.t}</text>
      <text x="170" y={item.y+38} fontSize="9.5" textAnchor="middle" fill="currentColor">{item.s}</text>
      {#if i<4}
        <path d={`M170 ${item.y+52} V${item.y+66} l-4 -5 m4 5 l4 -5`} fill="none" stroke="currentColor" strokeOpacity=".5" strokeWidth="1.5"/>
      {/if}
    {/each}
  </svg>
</card>

Je voudrais arriver à un système dans lequel **une capacité est décrite une seule fois**, puis les artefacts Discovery, les exemples, les schémas d'entrée/sortie et les bindings sont générés à partir de cette autorité.

L'inventaire des API publiques peut être automatique. Leur admission comme outils d'agents doit en revanche rester explicite.

## 4.4. Catalogue cible des capacités

Chaque capacité devrait porter au minimum les informations suivantes :

| Métadonnée | Objectif |
|---|---|
| Capability ID stable | Identification indépendante du nom CLR |
| Description sémantique | Permettre au LLM de choisir la bonne fonction |
| Paramètres et résultats | Appel typé |
| Domaines de définition | Prévention des appels invalides |
| Unités et dimensions | Contrôle physique |
| Représentation numérique | Exact, binary64, décimal, intervalle |
| Hypothèses | Préconditions mathématiques |
| Budgets | Limites de taille, temps, mémoire et précision |
| Politique d'erreur | Refus, approximation, annulation |
| Versions de contrat | Compatibilité |
| Identités des packages | Reproductibilité binaire |
| Exemples positifs et négatifs | Apprentissage des usages corrects |
| Provenance et preuves | Vérification de la qualification |
| Effets et permissions | Autorité laissée à AURA |

Le but ne doit pas être d'exposer 100 % des méthodes publiques, mais 100 % des **capacités métier mathématiques pertinentes et effectivement admises pour les agents**.

Une première vague pourrait porter sur les conversions, les expressions, les statistiques, les distributions, l'algèbre linéaire, les fonctions scientifiques et les opérations symboliques les plus fréquemment utilisées.

---

# 5. Nouveau domaine majeur : le temps et les calendriers

Je suis entièrement favorable à cette extension. Je lui donnerais même une place structurante dans la prochaine version.

Aujourd'hui, nous avons une dimension physique `Time`, des unités de durée, quelques conventions financières basées sur `DateOnly` et des séries uniformes. Nous n'avons pas encore de bibliothèque temporelle générale.

Nom de package proposé : **`dw.tools.math.temporal`**.

## 5.1. Les différents concepts temporels

Le premier impératif consiste à ne pas confondre les notions suivantes.

<box border radius="lg" padding={3} gap={3}>
  <row align="center">
    <box background="surface-secondary" radius="md" size="48px" align="center" justify="center">
      <icon name="clock" size="xl"/>
    </box>
    <box flex={1}>
      **Instant**

      <caption>Un point précis sur une ligne du temps.</caption>
    </box>
  </row>
  <svg viewBox="0 0 320 50" width="100%">
    <path d="M12 26 H308" stroke="currentColor" strokeOpacity=".35" strokeWidth="1.5"/>
    {#each [25,70,115,160,205,250,295] as x}
      <path d={`M${x} 21 V31`} stroke="currentColor" strokeOpacity=".35"/>
    {/each}
    <circle cx="160" cy="26" r="6" fill="#3284ba"/>
    <text x="160" y="12" textAnchor="middle" fontSize="10" fill="currentColor">Instant t</text>
  </svg>
  <divider/>
  <row align="center">
    <box background="surface-secondary" radius="md" size="48px" align="center" justify="center">
      <icon name="timer" size="xl"/>
    </box>
    <box flex={1}>
      **Durée**

      <caption>Une quantité fixe de temps écoulé.</caption>
    </box>
  </row>
  <svg viewBox="0 0 320 42" width="100%">
    <path d="M12 22 H308" stroke="currentColor" strokeOpacity=".2"/>
    <path d="M55 22 H265 M55 15 V29 M265 15 V29" stroke="#0d9488" strokeWidth="2"/>
    <text x="160" y="12" textAnchor="middle" fontSize="11" fill="currentColor">24 heures</text>
  </svg>
  <divider/>
  <row align="center">
    <box background="surface-secondary" radius="md" size="48px" align="center" justify="center">
      <icon name="calendar-days" size="xl"/>
    </box>
    <box flex={1}>
      **Période calendaire**

      <caption>Une notion relative au calendrier, par exemple un mois.</caption>
    </box>
  </row>
  <grid columns={2}>
    <grid-item>
      <box background="surface" border radius="md" padding={2} gap={1}>
        <caption>Janvier</caption>
        <title size="lg">31 jours</title>
      </box>
    </grid-item>
    <grid-item>
      <box background="surface" border radius="md" padding={2} gap={1}>
        <caption>Février non bissextile</caption>
        <title size="lg">28 jours</title>
      </box>
    </grid-item>
  </grid>
</box>

Une journée calendaire locale peut également durer 23 ou 25 heures lors d'un changement d'heure. Un mois ne doit jamais être implicitement remplacé par 30 jours.

## 5.2. Types temporels fondamentaux à prévoir

| Type proposé | Sémantique |
|---|---|
| `Instant` | Position sur une échelle temporelle |
| `Duration` | Durée fixe |
| `CalendarPeriod` | Années, mois, semaines, jours |
| `LocalDate` | Date sans fuseau |
| `LocalTime` | Heure sans date |
| `LocalDateTime` | Date et heure civiles |
| `OffsetDateTime` | Date et heure avec décalage UTC |
| `ZonedDateTime` | Date, heure et zone identifiée |
| `DateInterval` | Intervalle de dates |
| `TimeInterval` | Intervalle d'instants |
| `AcademicYear` | Année académique |
| `FiscalYear` | Année fiscale paramétrable |
| `BusinessCalendar` | Calendrier de jours ouvrés |
| `RecurrenceRule` | Règle de répétition déterministe |

Il ne s'agit pas nécessairement de réimplémenter tous ces types nous-mêmes.

Je commencerais par comparer les types .NET (`DateOnly`, `TimeOnly`, `DateTimeOffset`, `TimeZoneInfo`, `TimeProvider`) et Noda Time, puis qualifierais un fournisseur adapté derrière nos contrats. Noda Time distingue déjà explicitement instant, durée, période et heure zonée. <Cite refs={["turn760789search0","turn760789search2","turn760789search6"]}/>

## 5.3. Représentations internes et interopérabilité

La bibliothèque devrait supporter des conversions explicites entre les principales représentations.

| Représentation | Utilisation |
|---|---|
| ISO 8601 / RFC 3339 | Transport JSON et API |
| Unix secondes | Systèmes et services |
| Unix millisecondes | JavaScript et API |
| Unix microsecondes | Bases de données |
| Unix nanosecondes | Mesures scientifiques |
| .NET ticks | Interopérabilité CLR |
| OLE Automation Date | Applications Office |
| Excel 1900 / 1904 | Importation de feuilles de calcul |
| Jour julien et jour julien modifié | Astronomie et sciences |
| NTP | Horodatages réseau |
| GPS / TAI / UTC | Échelles temporelles scientifiques |

Chaque conversion devra préciser son époque de référence, sa résolution, son domaine de validité, son échelle temporelle et sa politique d'arrondi.

Il faudra traiter séparément les secondes intercalaires, les limites de représentation et le comportement historique des dates Excel.

## 5.4. Fuseaux horaires

Je recommande l'utilisation d'une autorité IANA/TZDB versionnée plutôt que de dépendre implicitement du fuseau configuré sur la machine.

<card gap={2}>
  <title size="md">Un instant, plusieurs heures civiles</title>
  <caption>Exemple illustratif en hiver</caption>
  {#each [{n:"New York",h:"07:00",z:"UTC−5",v:7},{n:"Londres",h:"12:00",z:"UTC+0",v:12},{n:"Bruxelles",h:"13:00",z:"UTC+1",v:13},{n:"Tokyo",h:"21:00",z:"UTC+9",v:21}] as item}
    <row align="center">
      <svg viewBox="0 0 50 50" width="46px" height="46px">
        <circle cx="25" cy="25" r="21" stroke="currentColor" strokeOpacity=".5" fill="none"/>
        {#each Array.from({length:12}) as _,i}
          <line x1={25+17*Math.sin(i*Math.PI/6)} y1={25-17*Math.cos(i*Math.PI/6)} x2={25+20*Math.sin(i*Math.PI/6)} y2={25-20*Math.cos(i*Math.PI/6)} stroke="currentColor" strokeOpacity=".5"/>
        {/each}
        <line x1="25" y1="25" x2={25+12*Math.sin(item.v*Math.PI/6)} y2={25-12*Math.cos(item.v*Math.PI/6)} stroke="currentColor" strokeWidth="2.5" strokeLinecap="round"/>
        <line x1="25" y1="25" x2="25" y2="8" stroke="#3284ba" strokeWidth="1.5" strokeLinecap="round"/>
        <circle cx="25" cy="25" r="1.8" fill="currentColor"/>
      </svg>
      <box flex={1}>
        **{item.n}**
        <caption>{item.z}</caption>
      </box>
      <title size="lg" tabularNums={true}>{item.h}</title>
    </row>
  {/each}
</card>

Les règles horaires changent au fil des décisions politiques. Leur version fait donc partie du contexte nécessaire à la reproductibilité. <Cite ref="turn739301search4"/>

Le moteur devra refuser ou résoudre explicitement les heures locales inexistantes ou ambiguës, gérer les décalages fractionnaires, les changements historiques et les correspondances entre identifiants Windows et IANA.

## 5.5. Calculs calendaires

Je prévoirais une couverture étendue :

| Famille | Fonctions à fournir |
|---|---|
| Calendriers | Grégorien, julien, ISO et autres calendriers qualifiés |
| Dates | Addition, soustraction, comparaison, âge |
| Années | Bissextiles, longueur, début, fin |
| Semaines | ISO 8601, années à 53 semaines |
| Mois | Longueur variable, fin de mois |
| Intervalles | Intersection, union, différence, inclusion |
| Périodes | Trimestres, semestres, années fiscales |
| Jours ouvrés | Addition de jours ouvrés, exclusions |
| Récurrences | Quotidiennes, hebdomadaires, mensuelles, annuelles |
| Finance | ACT/ACT, ACT/360, ACT/365, 30/360 |
| Séries temporelles | Fenêtres calendaires, agrégation, rééchantillonnage |
| Comparaison | Chevauchements, distances et contraintes temporelles |

Les calendriers métier et institutionnels devraient être fournis en données par les applications propriétaires. Math ne doit pas posséder les règles administratives de chaque pays ou université.

## 5.6. Années académiques

Je développerais un système plus général qu'une simple classe `AcademicYear`.

Une année académique devrait être une période rattachée à une **politique de calendrier académique paramétrable et versionnée**.

{@body const [startMonth,setStartMonth]=DIL.useState("8")}
{@body const [startYear,setStartYear]=DIL.useState("2026")}
{@body const mnames=["Jan.","Fév.","Mars","Avr.","Mai","Juin","Juil.","Août","Sept.","Oct.","Nov.","Déc."]}
<card gap={3}>
  <title>Exemple de calendrier académique configurable</title>
  <caption>Simulation d'une politique débutant le premier jour du mois choisi, sans présumer du calendrier d'une institution réelle.</caption>
  <grid columns={2}>
    <grid-item>
      <label>Début de l'année</label>
      <select value={startMonth} onChange={setStartMonth} options={mnames.map((x,i)=>({label:x,value:String(i)}))}/>
    </grid-item>
    <grid-item>
      <label>Année de référence</label>
      <select value={startYear} onChange={setStartYear} options={Array.from({length:7},(_,i)=>({label:String(2023+i),value:String(2023+i)}))}/>
    </grid-item>
  </grid>
  <title size="2xl">{startMonth==="0"?startYear:startYear+"–"+(Number(startYear)+1)}</title>
  <grid columns={6} gap={1}>
    {#each Array.from({length:12},(_,i)=>i) as i}
      <grid-item>
        <box padding={1} radius="sm" align="center" background={i<6?"#3284ba20":"#0d948820"}>
          <text size="xs" color="secondary">{Number(startYear)+Math.floor((Number(startMonth)+i)/12)}</text>
          <text weight="medium" size="sm">{mnames[(Number(startMonth)+i)%12]}</text>
        </box>
      </grid-item>
    {/each}
  </grid>
  <row>
    <box size="10px" radius="sm" background="#3284ba"/>
    <caption>Première moitié</caption>
    <spacer/>
    <box size="10px" radius="sm" background="#0d9488"/>
    <caption>Seconde moitié</caption>
  </row>
  <divider/>
  <row>
    <text color="secondary" size="sm">Début inclus</text>
    <spacer/>
    **01/{String(Number(startMonth)+1).padStart(2,"0")}/{startYear}**
  </row>
  <row>
    <text color="secondary" size="sm">Fin exclue</text>
    <spacer/>
    **01/{String(Number(startMonth)+1).padStart(2,"0")}/{Number(startYear)+1}**
  </row>
</card>

Le modèle réel devrait aller plus loin et permettre des dates de début et de fin arbitraires, des périodes d'enseignement, des sessions d'examens, des congés, des périodes de transition et des découpages propres à chaque institution.

Exemples d'API proposées :

```csharp
AcademicYear year = calendar.GetAcademicYear(date);

AcademicPeriod period = calendar.GetPeriod(date);

int teachingWeeks = calendar.CountTeachingWeeks(year);

bool isTeachingDay = calendar.IsTeachingDay(date);
```

Les règles universitaires, les dates des vacances et les programmes scolaires restent externes à Math. Math fournit les opérations de calcul et les structures temporelles nécessaires.

---

# 6. Physique, chimie et électricité : une véritable algèbre typée

C'est probablement le domaine dans lequel nous pouvons le mieux valoriser nos investissements existants.

## 6.1. Ce que nous possédons déjà

Le type `DimensionVector` représente les sept dimensions du SI et une dimension supplémentaire d'information.

| Dimension | Symbole |
|---|---|
| Longueur | L |
| Masse | M |
| Temps | T |
| Intensité électrique | I |
| Température | Θ |
| Quantité de matière | N |
| Intensité lumineuse | J |
| Information | Extension Math |

Nous disposons également d'un profil électrochimique qualifié contenant six unités :

<grid columns={3} gap={2}>
  {#each [{s:"A",n:"Ampère"},{s:"V",n:"Volt"},{s:"Ω",n:"Ohm"},{s:"W",n:"Watt"},{s:"mol",n:"Mole"},{s:"mol/m³",n:"Concentration"}] as unit}
    <grid-item>
      <box border radius="md" padding={3} align="center" gap={1}>
        <title size="xl">{unit.s}</title>
        <caption>{unit.n}</caption>
      </box>
    </grid-item>
  {/each}
</grid>

Le catalogue hérité possède 58 entrées, le catalogue standard initial en possède 14 et le profil spécialisé contient ces six définitions. Ces nombres ne doivent pas être additionnés comme s'il s'agissait nécessairement d'unités distinctes.

[Profil électrochimique existant](https://github.com/aihallot/dw.tools.math/blob/main/docs/distribution/electrochemical-unit-profile.md)

## 6.2. La différence entre le modèle actuel et celui que tu proposes

Actuellement, notre type principal est :

```csharp
public readonly record struct Quantity(
    ExactRational Value,
    DimensionVector Dimension);
```

La multiplication additionne les vecteurs de dimensions. La division les soustrait. L'addition vérifie leur compatibilité.

C'est déjà un moteur d'analyse dimensionnelle, mais la vérification est principalement dynamique.

Ce que tu proposes est une couche supplémentaire permettant au compilateur C# de garantir la cohérence de certaines opérations scientifiques.

**Je recommande de conserver les deux modèles.**

<grid columns={2} gap={2}>
  <grid-item>
    <box border radius="lg" padding={3} gap={2}>
      **Modèle dynamique**

      <caption>Quantity + DimensionVector</caption>
      <divider/>
      Adapté aux expressions provenant d'AURA, aux données inconnues à la compilation et aux formules construites dynamiquement.
    </box>
  </grid-item>
  <grid-item>
    <box border radius="lg" padding={3} gap={2}>
      **Modèle statique**

      <caption>Voltage, Resistance, ElectricCurrent</caption>
      <divider/>
      Adapté aux applications fortement typées, aux bibliothèques scientifiques et aux erreurs détectables à la compilation.
    </box>
  </grid-item>
</grid>

Les deux doivent partager le même noyau de conversion et de calcul, sans duplication d'algorithmes.

## 6.3. Exemple : loi d'Ohm

{@body const [resistance,setResistance]=DIL.useState(6)}
{@body const [current,setCurrent]=DIL.useState(2)}
<card gap={3}>
  <title>Loi d'Ohm — exemple de grandeurs typées</title>
  <svg viewBox="0 0 320 148" width="100%">
    <path d="M74 120 H270 V32 H218 M104 32 H50 V62 M50 94 V120 H74" fill="none" stroke="currentColor" strokeWidth="2"/>
    <path d="M104 32 l8 -9 l9 18 l9 -18 l9 18 l9 -18 l9 18 l9 -18 l9 18 l9 -18 l9 18 l9 -9 h16" fill="none" stroke="#3284ba" strokeWidth="2" strokeLinejoin="round"/>
    <line x1="35" y1="68" x2="65" y2="68" stroke="#0d9488" strokeWidth="2.5"/>
    <line x1="42" y1="80" x2="58" y2="80" stroke="#0d9488" strokeWidth="2.5"/>
    <path d="M50 62 V68 M50 80 V94" stroke="currentColor" strokeWidth="2" fill="none"/>
    <text x="160" y="12" textAnchor="middle" fontSize="12" fill="#3284ba">R = {resistance} Ω</text>
    <text x="78" y="69" fontSize="13" fill="#0d9488">U = {resistance*current} V</text>
    <path d="M195 120 H145 l8 -5 M145 120 l8 5" fill="none" stroke="#b88735" strokeWidth="2" strokeLinecap="round"/>
    <text x="170" y="139" textAnchor="middle" fontSize="12" fill="#b88735">I = {current} A</text>
  </svg>
  <row>
    <label>Résistance R</label>
    <spacer/>
    **{resistance} Ω**
  </row>
  <slider min={1} max={30} step={1} value={resistance} onChange={setResistance}/>
  <row>
    <label>Intensité I</label>
    <spacer/>
    **{current} A**
  </row>
  <slider min={0} max={10} step={0.5} value={current} onChange={setCurrent}/>
  <divider/>
  <grid columns={2}>
    <grid-item>
      <caption>Tension — Voltage</caption>
      <title size="2xl" color="success">{resistance*current} V</title>
    </grid-item>
    <grid-item>
      <caption>Puissance — Power</caption>
      <title size="2xl">{Number((resistance*current*current).toFixed(3))} W</title>
    </grid-item>
  </grid>
</card>

Je souhaiterais pouvoir écrire à terme :

```csharp
Resistance r = Resistance.FromOhms(6);
ElectricCurrent i = ElectricCurrent.FromAmperes(2);

Voltage u = r * i;
Power p = u * i;
```

Avec une erreur de compilation pour une expression telle que :

```csharp
Voltage u = r + i;
```

Ces signatures sont des **propositions d'API**, pas du code déjà livré dans Math.

## 6.4. Généralisation du modèle des grandeurs

Je propose une architecture reposant sur cinq concepts séparés.

| Concept | Exemple |
|---|---|
| Valeur numérique | `12`, `1/3`, `12.5` |
| Dimension | Masse × longueur² / temps³ |
| Nature de grandeur | Puissance |
| Unité | Watt |
| Politique numérique | Exacte, approchée, incertitude |

La nature de grandeur est particulièrement importante.

Deux grandeurs peuvent avoir les mêmes dimensions sans être interchangeables : énergie et couple, fréquence et activité radioactive, angle et nombre sans dimension.

Une future architecture doit donc représenter davantage que les seuls exposants dimensionnels.

## 6.5. Un catalogue scientifique extensible

Je partirais des références internationales, notamment de la brochure SI du BIPM, actualisée en 2026, et étudierais UCUM pour les représentations machine des unités. <Cite refs={["turn337132search2","turn337132search5"]}/>

Le catalogue devrait permettre de définir les unités fondamentales et dérivées, les préfixes SI et binaires, les unités impériales, les transformations affines, les conversions logarithmiques et les grandeurs sans dimension portant une sémantique particulière.

Les identifiants canoniques des unités resteraient indépendants de leur traduction et de leur présentation locale.

## 6.6. Bibliothèques scientifiques envisageables

Je propose les familles suivantes comme domaines de développement, sans créer immédiatement un projet NuGet pour chacune.

| Domaine | Capacités proposées |
|---|---|
| Mécanique | Force, vitesse, accélération, énergie, quantité de mouvement |
| Électricité | Loi d'Ohm, puissance, résistances série/parallèle |
| Électronique | Capacité, inductance, impédance, circuits RC/RLC |
| Électromagnétisme | Charge, champ électrique, flux magnétique |
| Thermodynamique | Température, capacité thermique, énergie, entropie |
| Fluides | Pression, débit, viscosité, masse volumique |
| Optique | Longueur d'onde, fréquence, réfraction, lentilles |
| Acoustique | Fréquence, intensité, décibels |
| Chimie générale | Quantité de matière, masse molaire, concentration |
| Stœchiométrie | Conservation atomique, équations chimiques, réactifs limitants |
| Chimie des solutions | Dilution, concentration, pH sous hypothèses explicites |
| Électrochimie | Charge, loi de Faraday, équation de Nernst |
| Métrologie | Incertitudes, propagation d'erreur, chiffres significatifs |

Pour toutes les équations, il faudrait définir les paramètres typés, les domaines, les hypothèses physiques et les modalités de résolution.

Par exemple, pouvoir calculer U à partir de R et I, mais aussi R à partir de U et I, avec une politique explicite pour les divisions par zéro.

Cela donnerait à AURA un catalogue de véritables **lois scientifiques calculables et découvrables**.

---

# 7. Autres pistes que je recommande

## 7.1. Incertitudes et précision numérique

C'est une extension essentielle dès lors que Math entre dans les sciences expérimentales.

Un calcul exact effectué sur une mesure imprécise ne produit pas une connaissance exacte de la réalité.

<card gap={2}>
  <title size="md">Valeur nominale et incertitude</title>
  <svg viewBox="0 0 320 110" width="100%">
    <rect x="90" y="30" width="140" height="42" fill="#0d9488" fillOpacity=".14" rx="3"/>
    <path d="M20 80 H300" stroke="currentColor" strokeOpacity=".5"/>
    {#each [9.8,9.9,10,10.1,10.2] as v,i}
      <path d={`M${20+i*70} 76 V85`} stroke="currentColor" strokeOpacity=".5"/>
      <text x={20+i*70} y="101" textAnchor="middle" fontSize="10" fill="currentColor">{v}</text>
    {/each}
    <path d="M90 51 H230 M90 42 V60 M230 42 V60" stroke="#0d9488" strokeWidth="2.5"/>
    <circle cx="160" cy="51" r="6" fill="#0d9488"/>
    <text x="160" y="18" textAnchor="middle" fontSize="12" fontWeight="600" fill="currentColor">10,0 ± 0,1</text>
  </svg>
  <caption>Exemple : intervalle de mesure déclaré, distinct d'une erreur d'arrondi numérique.</caption>
</card>

Je proposerais de couvrir les intervalles, les erreurs absolues et relatives, les incertitudes de mesure, les propagations avec covariance, les arrondis dirigés et les représentations de précision variable.

Cette piste reprend directement un axe étudié puis différé dans M7.

## 7.2. Statistiques et probabilités avancées

| Famille | Extensions proposées |
|---|---|
| Distributions | Normale, binomiale, Poisson, exponentielle, Gamma, bêta |
| Estimation | Maximum de vraisemblance et intervalles de confiance |
| Tests statistiques | Comparaisons, indépendance, adéquation |
| Régression | Multiple, polynomiale, logistique |
| Statistiques robustes | Médiane, MAD, estimateurs robustes |
| Streaming | Quantiles approchés, histogrammes, fenêtres glissantes |
| Rééchantillonnage | Bootstrap et permutations reproductibles |
| Processus | Chaînes de Markov et processus stochastiques génériques |

Les politiques de risque, l'optimisation décisionnelle et les décisions métier resteraient dans Decision.

## 7.3. Séries temporelles et traitement du signal

Cette famille bénéficierait directement de la nouvelle bibliothèque temporelle.

Je recommande les FFT, les filtres numériques, les transformations spectrales, l'autocorrélation, les séries irrégulières, le rééchantillonnage temporel, la décomposition saisonnière et les mesures génériques de distance entre séries.

L'actuelle limite de 32 échantillons sur plusieurs opérations scientifiques est insuffisante pour des traitements réalistes de signaux.

## 7.4. Tenseurs et calcul matriciel avancé

M7 a déjà étudié ce sujet. Il est maintenant pertinent de le réexaminer avec les capacités numériques de .NET 11.

Les extensions pourraient couvrir matrices de grandes dimensions, systèmes linéaires creux, valeurs propres, SVD généralisée, normes matricielles, tenseurs multidimensionnels, broadcasting explicite et calculs vectorisés.

Je ne développerais pas immédiatement un moteur GPU ou un équivalent complet de NumPy. Je commencerais par qualifier les API numériques déjà disponibles et des fournisseurs adaptés aux besoins réels.

## 7.5. Calcul automatique et symbolique

Notre IR offre une base intéressante pour introduire la différentiation automatique, les gradients, les Jacobiennes, les Hessiennes et les fonctions scientifiques composées.

Je voudrais également réexaminer les DAG de calcul purs étudiés dans M7.

<card gap={2}>
  <svg viewBox="0 0 320 156" width="100%">
    <path d="M160 34 L86 74 M160 34 L234 74 M86 97 L160 127 M234 97 L160 127" stroke="currentColor" strokeOpacity=".4" strokeWidth="1.5" fill="none"/>
    <rect x="112" y="5" width="96" height="30" rx="6" fill="#3284ba" fillOpacity=".16" stroke="#3284ba"/>
    <text x="160" y="24" fontSize="11" textAnchor="middle" fill="currentColor">Valeur x</text>
    <rect x="38" y="65" width="96" height="33" rx="6" fill="#0d9488" fillOpacity=".12" stroke="#0d9488"/>
    <text x="86" y="85" textAnchor="middle" fontSize="11" fill="currentColor">f(x)</text>
    <rect x="186" y="65" width="96" height="33" rx="6" fill="#0d9488" fillOpacity=".12" stroke="#0d9488"/>
    <text x="234" y="85" textAnchor="middle" fontSize="11" fill="currentColor">g(x)</text>
    <rect x="104" y="118" width="112" height="32" rx="6" fill="#8b5cf6" fillOpacity=".13" stroke="#8b5cf6"/>
    <text x="160" y="138" fontSize="11" textAnchor="middle" fill="currentColor">h(f(x), g(x))</text>
  </svg>
  <caption>Calcul composé, partage des sous-expressions, traçabilité et dérivation automatique.</caption>
</card>

Le moteur de calcul resterait pur et local. AURA conserverait la responsabilité de l'orchestration, des tâches et des permissions.

## 7.6. Mathématiques éducatives et visualisation

Je vois également beaucoup de potentiel dans la production de résultats mathématiques exploitables pédagogiquement.

| Capacité | Utilité |
|---|---|
| Étapes de résolution structurées | Expliquer les calculs |
| Transformations justifiées | Enseigner l'algèbre |
| Domaine de définition | Identifier les restrictions |
| Génération d'exemples bornés | Construire des exercices |
| PlotModel | Représenter fonctions et courbes |
| Détection de discontinuités | Éviter les graphiques trompeurs |
| Géométrie paramétrique | Produire des figures calculables |
| Vérification d'équivalence | Évaluer certaines réponses symboliques |

Math fournirait les modèles, les calculs et les preuves disponibles. Les applications éducatives resteraient responsables de l'évaluation pédagogique, des parcours et de l'interface utilisateur.

---

# 8. Proposition de roadmap pour Math 2.0

Je propose de considérer la prochaine version majeure comme une évolution vers une **plateforme mathématique et scientifique typée, performante et entièrement découvrable par les agents**.

<card gap={2}>
  <svg viewBox="0 0 340 317" width="100%">
    <rect x="12" y="8" width="316" height="40" rx="7" fill="#64748b" fillOpacity=".13" stroke="#64748b"/>
    <text x="170" y="25" textAnchor="middle" fontSize="13" fontWeight="600" fill="currentColor">Applications et agents</text>
    <text x="170" y="38" textAnchor="middle" fontSize="10" fill="currentColor">AURA · Decision · Éducation · Sciences</text>
    <path d="M170 48 V64" stroke="currentColor" strokeOpacity=".4"/>
    <rect x="12" y="64" width="316" height="44" rx="7" fill="#8b5cf6" fillOpacity=".13" stroke="#8b5cf6"/>
    <text x="170" y="82" textAnchor="middle" fontSize="12" fontWeight="600" fill="currentColor">Universal Math Discovery</text>
    <text x="170" y="96" textAnchor="middle" fontSize="9.5" fill="currentColor">Contrats · Schémas · Bindings · Preuves</text>
    {#each [{n:"Temps",c:"#0d9488"},{n:"Physique",c:"#3284ba"},{n:"Chimie",c:"#3284ba"},{n:"Statistiques",c:"#0d9488"},{n:"Tenseurs",c:"#0d9488"},{n:"Symbolique",c:"#3284ba"},{n:"Signaux",c:"#3284ba"},{n:"Incertitudes",c:"#0d9488"},{n:"Géométrie",c:"#3284ba"}] as item,i}
      <rect x={12+(i%3)*107} y={128+Math.floor(i/3)*30} width="102" height="25" rx="4" fill={item.c} fillOpacity=".12" stroke={item.c} strokeOpacity=".65"/>
      <text x={63+(i%3)*107} y={144+Math.floor(i/3)*30} textAnchor="middle" fontSize="10" fill="currentColor">{item.n}</text>
    {/each}
    <rect x="12" y="228" width="316" height="37" rx="7" fill="#0d9488" fillOpacity=".12" stroke="#0d9488"/>
    <text x="170" y="244" textAnchor="middle" fontSize="11" fontWeight="600" fill="currentColor">Noyau mathématique typé et IR</text>
    <text x="170" y="257" textAnchor="middle" fontSize="9.5" fill="currentColor">Grandeurs · Dimensions · Représentations</text>
    <rect x="12" y="277" width="316" height="31" rx="7" fill="#26976a" fillOpacity=".14" stroke="#26976a"/>
    <text x="170" y="296" textAnchor="middle" fontSize="11" fontWeight="600" fill="currentColor">.NET 11+ · Performances · Qualification</text>
  </svg>
</card>

## Proposition de milestones

Les identifiants ci-dessous sont indicatifs et ne sont pas encore intégrés au backlog.

| Milestone | Objectif | Priorité |
|---|---|---|
| M12 | Modernisation, performances et consolidation technique | Critique |
| M13 | Discovery généralisée et qualification AURA | Critique |
| M14 | Temporal, calendriers et années académiques | Haute |
| M15 | Grandeurs fortement typées et catalogue universel d'unités | Haute |
| M16 | Physique, électricité, chimie et métrologie | Haute |
| M17 | Statistiques, probabilités et séries temporelles avancées | Haute |
| M18 | Calcul numérique avancé, tenseurs et précision | Moyenne |
| M19 | DAG, différentiation automatique et visualisation | Moyenne |
| M20 | Qualification et distribution Math 2.0 | Obligatoire |

Discovery devra être une exigence transversale de chaque nouvelle famille, pas un chantier réalisé seulement à la fin du développement.

## Ordre de développement recommandé

<box gap={1}>
  {#each [{n:"1",title:"Consolider l'existant",desc:"Audit des API, benchmarks, dette technique, distribution",c:"#3284ba"},{n:"2",title:"Rendre l'existant découvrable",desc:"Catalogue complet, contrats et bindings pour AURA",c:"#8b5cf6"},{n:"3",title:"Étendre les fondations",desc:"Temps, calendriers, types numériques et grandeurs",c:"#0d9488"},{n:"4",title:"Développer les moteurs scientifiques",desc:"Physique, chimie, électricité et incertitudes",c:"#0d9488"},{n:"5",title:"Étendre les capacités avancées",desc:"Statistiques, tenseurs, calcul automatique",c:"#64748b"},{n:"6",title:"Qualifier Math 2.0",desc:"Performances, compatibilité, consommateurs et publication",c:"#26976a"}] as item,i}
    <row align="start" gap={3}>
      <box background={item.c} radius="full" size="28px" align="center" justify="center">
        <text color="#ffffff" weight="medium">{item.n}</text>
      </box>
      <box flex={1} gap={1}>
        **{item.title}**
        <caption>{item.desc}</caption>
      </box>
    </row>
    {#if i<5}
      <box padding={{left:3}}>
        <box width="1px" height={16} background="border"/>
      </box>
    {/if}
  {/each}
</box>

---

# 9. Dette technique et améliorations de maintenance

L'audit fait également apparaître quelques sujets qui ne sont pas directement mathématiques, mais qui méritent d'être traités.

| Constat | Recommandation |
|---|---|
| README contenant encore une instruction de build sous .NET 10 | Actualiser les sections opérationnelles sans effacer l'historique |
| Classes publiques nommées `M6W02C04Contract`, `M6W02C04Boundary64` | Introduire des noms métier stables, préserver la compatibilité |
| Nombreux outils de qualification historiques | Consolider les mécanismes réutilisables avec l'infrastructure NuGet commune |
| Métadonnées de publication et de Discovery distinctes | Mettre en cohérence les identités sans les confondre |
| Budgets numériques hétérogènes | Formaliser un modèle commun adaptable par opération |
| Absence de campagne globale de benchmarks | Établir une baseline reproductible |
| Pas de mesure globale de couverture identifiée | Ajouter couverture, tests de propriétés et analyse des branches |
| Compatibilité Native AOT non certifiée | Introduire une qualification dédiée |
| Archives candidates ignorées par Git | Prévoir une conservation contrôlée et une acquisition vérifiable |

Je recommande d'éviter une réécriture générale. Il faut procéder par consolidation additive, mesures et preuves de compatibilité.

---

# 10. Synthèse et recommandation finale

<card gap={3}>
  <caption>Priorités proposées pour la prochaine génération — appréciation qualitative</caption>
  {#each [{n:"Discovery pour AURA",p:5},{n:"Temps et calendriers",p:5},{n:"Grandeurs physiques typées",p:5},{n:"Optimisation .NET 11",p:5},{n:"Physique et chimie",p:4},{n:"Incertitudes numériques",p:4},{n:"Statistiques avancées",p:4},{n:"Séries temporelles",p:4},{n:"Tenseurs",p:3},{n:"DAG et visualisation",p:3}] as item}
    <row align="center">
      <box flex={1}><text size="sm">{item.n}</text></box>
      <row gap={1}>
        {#each Array.from({length:5}) as _,i}
          <box size="12px" radius="sm" background={i<item.p?"#3284ba":"surface-tertiary"}/>
        {/each}
      </row>
    </row>
  {/each}
</card>

Ma recommandation est de donner à Math trois nouvelles ambitions complémentaires :

**Universal Math** — étendre la couverture mathématique, temporelle et scientifique.

**Typed Science** — permettre des calculs scientifiquement cohérents par construction, avec des grandeurs, unités, domaines et politiques de précision explicites.

**Agent-Native Mathematics** — rendre ces capacités réellement utilisables par AURA sans réimplémentation ni ambiguïté sur leurs contrats.

Nous avons déjà les fondations nécessaires pour y parvenir. Il ne s'agit donc pas de recommencer, mais de transformer un ensemble de bibliothèques mathématiques qualifiées en une véritable infrastructure scientifique réutilisable.

## Limites de cet audit

Cet audit repose sur l'inventaire intégral des projets et fichiers de production, l'inspection ciblée des implémentations principales, les preuves durables du dépôt et la documentation officielle .NET actuelle.

Je n'ai pas exécuté de nouveaux benchmarks, mesuré la couverture de code, effectué une revue sémantique ligne par ligne de chaque fichier ni vérifié l'installation effective dans le runtime AURA. Ces points doivent faire partie du premier chantier de la prochaine génération.

Les choix de packages, les identifiants M12–M20 et la cible Math 2.0 constituent mes propositions, pas des décisions déjà approuvées.

**Je n'ai modifié aucun dépôt et n'ai pas préparé RS430.** Je recommande de valider d'abord cette orientation générale, puis de construire le nouveau backlog canonique avec des objectifs, des dépendances et des critères d'acceptation précis.

---

Run terminé : RS429 — succeeded — attempt 001 — first-pass success — exécuteur 0.1.81 — commit a9547114a0ff0fc6ef6d0667bcc19f4079188ed8.

DIRECTORY: D:\data.dev\github\ai@hallot.net\dw.tools.math  
BRANCH: main  
EXPECTED RESULT: pushed/failed [aucun retry n/a] - none  
STATUS: aucun · retry n/a · attempts alloués n/a  
COMMAND: aucune

---

If you want, I can:

- Proposez un plan détaillé pour intégrer les calculs temporels avancés à la prochaine version majeure
- Suggérez des extensions concrètes dans les domaines physique et chimique pour .NET 11/C# 15
- Détaillez un plan pour améliorer la couverture Discovery AURA en exposant d’autres capacités mathématiques

## Unplaced archived assets

> These resources were found in the shared-conversation payload but could not be associated with a precise original position.

![Unplaced archived asset 1](assets/c8f26ea81a6b23939026.png)
