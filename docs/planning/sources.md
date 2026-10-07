# Sources et qualification des providers

Consultation initiale : 2026-10-07. Les sources suivantes servent à construire des spikes, pas à annoncer des dépendances qualifiées.

| Source primaire | Utilité | Décision actuelle |
|---|---|---|
| [Math.NET Numerics](https://numerics.mathdotnet.com/) | Algèbre linéaire, probabilités, interpolation, intégration et autres calculs ; le projet annonce MIT et des implémentations managées/natives | Candidat numérique prioritaire ; version, RID et transitives à qualifier en M3 |
| [AngouriMath](https://github.com/asc-community/AngouriMath) | Bibliothèque symbolique C#/F#, dépôt annonçant MIT | Candidat CAS ; tester hypothèses, solutions, annulation et conversions |
| [Math.NET Symbolics](https://symbolics.mathdotnet.com/) | Autre candidat symbolique de l'écosystème .NET | Comparer la couverture utile, ne pas supposer une interchangeabilité CAS |
| [SymPy — gotchas](https://docs.sympy.org/latest/tutorials/intro-tutorial/gotchas.html) | Sémantique des expressions, égalité et pièges de conversion | Référence/oracle optionnel ; aucun Python dans le chemin obligatoire de DWF |
| [OpenMath](https://openmath.org/standard/) | Objets et dictionnaires sémantiques | Comparaison IR avant modèle propre |
| [MathML](https://www.w3.org/TR/mathml4/) | Distinction présentation/contenu et annotations sémantiques | Sous-ensemble et version de standard à figer ; ne pas confondre affichage et signification |
| [UnitsNet](https://github.com/angularsen/UnitsNet) | Catalogue et calculs de quantités à examiner | Évaluer couverture et représentation numérique ; pas de remplacement automatique du rationnel exact |
| [.NET file-based apps](https://learn.microsoft.com/en-us/dotnet/core/sdk/file-based-apps) | Exécution C# par SDK .NET | Support du validateur documentaire, sans dépendance NuGet |

Sources locales : source-baseline.json (hashes des fichiers lus), existing-code-inventory.md, cadrage brainstorming, planning et guidance active observée de Decision.
Les GUID/versions de guidance DWF ne sont pas des contrats éternels : l'exécutable utilisé lors de l'adoption tranche sa compatibilité.

## Dossier d'admission obligatoire

Pour chaque candidat sérieux : version immuable, URL source/commit, licence exacte et notices transitives/natives, droits de redistribution vérifiés, mainteneur/activité observée, TFM/RID, offline, installation, interfaces, erreurs, annulation, budgets, exactitude, précision, thread-safety et reproductibilité.
Un choix de licence juridique incertain est une question propriétaire, pas une conclusion automatique tirée d'un README.
Comparer au moins deux options plausibles par famille si disponibles ; sinon documenter pourquoi la seconde n'est pas pertinente.
Une option inadéquate se termine par refus motivé ou réduction de périmètre proposée ; elle ne justifie pas la réimplémentation d'un CAS.

## Séquence

M0 : provenance et droits du transfert ; M2 : IR/standards ; M3 : numérique ; M4 : symbolique ; M5 : codecs ; M6/M7 : nouvelles familles.
Aucune version exacte de provider ni capacité transitive n'est figée sans un smoke sur le package réel.

