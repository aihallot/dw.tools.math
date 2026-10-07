# Premier prompt après initialisation DWF

Le propriétaire peut transmettre ce prompt à ChatGPT dans le contexte du dépôt Math après avoir initialisé la version de DWF retenue. Ce fichier ne prépare aucun run et ne constitue pas une commande de bootstrap.

> Tu travailles uniquement dans aihallot/dw.tools.math. Lis AGENTS.md, PROJECT-MANTRA.md, PROJECT-CONSTITUTION.md puis docs/planning/implementation-handoff.md et la guidance DWF active. Le backlog docs/planning/backlog.json est le plan produit ; docs/planning/dwf-map.json donne la correspondance des IDs, pas un plan natif à recopier.
>
> Observe l'état Git, l'identité DWF et l'éventuel run pending avant toute action. Si le plan natif est vide sans run pending, suis la procédure d'initialisation du plan produit prévue par DWF, en préservant schemaVersion et project.id/title. Ne fabrique ni historique de succès ni état d'activation.
>
> Prépare le premier résultat borné M0-W01-C01 : socle .NET, runner, scripts reproductibles, pack et consommateur minimal isolé. Raffine ses fichiers exacts et toute la chaîne restore/build/test/pack/consumer ; sa commande de test proposée seule ne suffit pas à sa recette. N'entreprends pas l'extraction AURA avant ses gates de provenance et de frontière.
>
> Vérifie le planning par dotnet run --file docs/planning/ValidatePlan.cs. Si tu modifies le backlog, régénère ses projections avec -- --write dans le payload et déclare tous les fichiers modifiés. Ne place jamais cette génération dans une validation preparation-safe.
>
> Aucun accès en écriture aux autres dépôts. Pour tout besoin externe, prépare une demande dans docs/coordination/requests/ pour transmission par le propriétaire. N'annonce pas une requête comme envoyée ni une adoption comme effectuée.
>
> Arrête ce premier lot au socle vérifié. Rapporte preuves, limites et prochain chunk. L'opérateur garde la boucle normale prévue par sa version DWF, en général dwf run next ; ne lui transfère pas les corrections internes du workflow.

