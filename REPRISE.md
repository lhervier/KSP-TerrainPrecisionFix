# Reprise — l'ancre (session du 2026-10-07/08, PC fixe → portable)

Fichier de passage entre deux postes : à lire au début de la prochaine session, puis à **supprimer** une
fois la reprise faite (ce dépôt est public).

## Où on en est

Le sujet : la **Stamp-O-Tron ground anchor**, et l'issue KSPCF **#214** (« Ground anchor levitates after a
scene load »).

1. **Cause trouvée et mesurée** : `Vessel.CheckGroundCollision` place l'origine du vaisseau à
   `|getLowestPoint()|` au-dessus du collider du sol. Le collider de l'ancre s'arrête 20,8 mm au-dessus
   de son origine, donc `getLowestPoint()` vaut −0,0208, et `Mathf.Abs` en fait +0,0208. L'ancre posée a
   son origine 2,07 cm **sous** le sol ; au premier passage hors rails, elle passe 2,08 cm **au-dessus** :
   +4,15 cm. Détail : `claude-notes/vaisseau-pose.md`, « `CheckGroundCollision` : la repose au sol ».
2. **Pas de cliquet** pour une ancre seule : un vaisseau d'une pièce est recalé à chaque passage hors
   rails, et la passe est absolue. Avec le correctif du sol, l'ancre ne bouge plus après le premier
   chargement. En stock, elle suit le sol de chaque chargement, vers le haut comme vers le bas (Kerbin,
   avec et sans KSPCF).
3. **Nouveau réglage `fixGroundAnchor`** (actif par défaut), écrit et validé en jeu sur Minmus : le
   collider du prefab de l'ancre est descendu jusqu'à son origine (`Src/GroundAnchorFix.cs`). Avec lui,
   la pose et le rechargement donnent la même position (−8,744 contre −8,787 mm). Décisions de Lionel :
   ne pas toucher au `Mathf.Abs` de `CheckGroundCollision`, ne traiter que l'ancre stock (à revoir avec
   les pylônes de KAS), en faire un réglage à part.
4. **#214 n'est pas reproduit** au sens « monte à chaque sauvegarde » (ni par cycles sauvegarde-chargement
   sur Kerbin, ni par le protocole exact de l'issue sur la Mune, via la station de suivi). Seule la
   lévitation au premier chargement l'est.

Tout est consigné dans `claude-notes/mods/TerrainPrecisionFix-ancre.md` (réglage, mesures, tests à jouer),
le récit dans `claude-notes/journal/TerrainPrecisionFix.md` (2026-10-07/08), #214 dans
`claude-notes/kspcf.md`. Archives des mesures : `claude-notes/archives/ancre-minmus/` (séries A à F,
sauvegardes de départ `x-avant-pose.sfs`, `D-kerbin-stock/x2-avant-pose.sfs`, `F-mune-station/x3.sfs`).

## Ce qui est prêt à commiter

- **KSP-TerrainPrecisionFix** : `Src/GroundAnchorFix.cs` (nouveau), `Src/Settings.cs`,
  `Src/TerrainPrecisionFixMod.cs`, `TerrainPrecisionFixMod.csproj` (référence
  `UnityEngine.PhysicsModule`), `GameData/TerrainPrecisionFixMod/PluginData/settings.cfg`, et ce
  fichier. La DLL n'est pas suivie par git : la recompiler sur le portable.
- **kspmod** : `CLAUDE.md` (index), `claude-notes/` (nouvelle note de l'ancre, `vaisseau-pose.md`,
  `kspcf.md`, `a-creuser.md`, `collider-pqs/defaut-sol.md`, `mods/TerrainPrecisionFix.md`, journal),
  `claude-notes/archives/ancre-minmus/` (25 Mo, captures en JPEG 2560 × 1440).

## Remettre le banc en place sur le portable

1. `git pull` de `kspmod` et de `KSP-TerrainPrecisionFix`.
2. Compiler : `build.bat` (sur le portable, `KSPDIR` vise `ksp-dev\`, c'est permis).
3. KSP arrêté : `python outils\gamedata.py --ksp <ksp-dev> refresh KSP-TerrainPrecisionFix`, puis
   `use stock +KSPMCPServer +TerrainPrecisionFixMod`. Le `settings.cfg` de la réserve garde ses valeurs :
   `fixGroundAnchor` absent vaut `true` ; l'ajouter pour pouvoir le couper.
4. Copier les sauvegardes de départ dans `<ksp-dev>\saves\claude\` : `x-avant-pose.sfs` (Minmus),
   `x2-avant-pose.sfs` (Kerbin), `x3.sfs` (Mune). Elles contiennent Bill (l'ancre dans son inventaire) et
   le Diag3-Rover. À vérifier : la partie `claude` du portable doit les ouvrir.
5. `python outils\kspctl.py --ksp <ksp-dev> start`, puis `load_save`.

Au démarrage, le log doit dire `Ground anchor fix: the anchor's collider lowered by 20.8 mm to its origin`.

## Ce qu'on fait ensuite, dans l'ordre

Le partage des rôles qui a marché : **Lionel** pose l'ancre (construction EVA), fait les F1 et les F5 ;
**Claude** joue les cycles par le serveur (`load_save`, `wait` 15 s, `save_game`), relève `alt`, `hgt`,
`PQSMin`/`PQSMax` et les lignes `Moving Vessel`, et archive. Lire `alt`/`hgt` dans le `.sfs`, pas la
ligne `Moving Vessel` (le clamp de `Vessel.Load` passe avant elle). Pour voir le bruit du sol stock, aller
sur Kerbin (Minmus : 3,9 mm seulement).

1. **Une pièce attachée à l'ancre** (demande de Lionel), avec et sans `fixGroundAnchor`. À deux pièces,
   la passe ne tourne que tant que les niveaux PQS sont à 0/0, puis plus jamais. Prévu : en stock,
   +4,15 cm au premier chargement puis la base figée là ; avec le réglage, rien.
2. **La poutrelle enfoncée** (piste de Lionel pour #214, sur la Mune depuis `x3`) : (a) ancre et deux
   poutrelles, une sous le sol, F5/F9 ; (b) retirer la poutrelle enfoncée, F5/F9 ; (c) retirer la
   dernière, F5/F9. Prévu : (a) ancre soulevée ; (b) elle reste en l'air ; (c) elle redescend à 2 cm.
3. **La désinstallation** : `B-correctif-pose.sfs` (ancre posée avec le réglage) chargée sans le mod.
   Prévu : +2,08 cm une fois, plus le bruit du sol stock.
4. La page publique du réglage et sa ligne dans la non-régression (règles :
   `claude-notes/readme-publics.md`, plan des pages : `claude-notes/mods/TerrainPrecisionFix.md`).

## À ne pas oublier

- `KSP-EvaCMGroundPlugin/CLAUDE-rechargement.md` parle encore de « cliquet » et de « collider trop
  court » : pas corrigé (autre dépôt), `vaisseau-pose.md` fait foi.
- Sur le fixe, `ksp-dev\` est resté en configuration `stock -KSPCommunityFixes-1.41.1 +KSPMCPServer`, et
  le `settings.cfg` de la réserve du correctif a reçu une ligne `fixGroundAnchor = true`.
