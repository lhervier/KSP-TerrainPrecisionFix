# TODO

Ce qu'il reste à faire, et seulement ça. Tout ce qui est déjà mesuré est dans [le README](README.md) et
ses chapitres sous [docs/](docs/), avec ses logs dans [perfs/](perfs/) et dans les dépôts des trois
Diags ; aucun résultat n'est consigné ici. La relecture anticipée de l'issue est dans
[KSPCF-relecture.md](KSPCF-relecture.md).

L'issue KSPCF demande une relecture et de l'aide, pas une publication : elle dit ce qui est vérifié et ce
qui ne l'est pas, et [Limits and solutions](docs/limits-and-solutions.md) liste en public les cas encore
ouverts. Seul ce qui casserait la lecture de l'issue elle-même doit donc passer avant.

## Le correctif des statiques : à valider avant de commiter

Écrit le 2026-09-30 (`StaticsFix`, `KerbalKonstructsCompat`, `KopernicusCompat`), validé en partie. On
finit de valider, puis on met le README à jour (un second coupable, les statiques, et les autres
chapitres), puis on commite.

**Chaque test se joue à la main, en jeu, sans addon** (Lionel, 2026-10-01) : il doit montrer à un
mainteneur de KSPCF qu'un impact sur le jeu existe ou non, et se rejouer sans nous. La liste publique,
impact par impact, avec sa procédure, est le chapitre *Still to test* de
[The KSC buildings, runway and launchpad](docs/limits-and-solutions/the-ksc-buildings-runway-and-launchpad.md#still-to-test) ;
chaque test se joue avec le mod (`logLevel = Debug`) puis sans. Une fois joué, son résultat va dans la
page, son point quitte la liste, et il s'efface d'ici.

**1. Stock** (`ksp-dev\`, KSPCF, sans mod tiers)
- [ ] Bosse sous un avion qui roule seul sur toute la piste (décalages d'origine).
- [ ] Vaisseau qui apparaît au mauvais endroit : pas de tir (VAB) et piste (SPH).
- [ ] KSC perdu après un changement de scène : revert au lancement, F5/F9, KSC puis retour par la
      tracking station, revert au VAB, récupération.
- [ ] KSC absent ou flou après un passage à la Mune (`Set Orbit`, puis bascule depuis la carte sur un
      second vaisseau garé sur la piste).
- [ ] KSC qui dérive pendant une accélération du temps sur rails, vaisseau sur la piste.
- [ ] Bâtiment détruit en vol (château d'eau du pas de tir) : détruit au KSC, réparé, à sa place au vol
      suivant.
- [ ] Niveaux 1, 2 et 3 du pas de tir et de la piste en carrière : bâtiments de leur niveau, à leur place.
- [ ] Station CommNet du KSC : connexion d'une sonde sur le pas de tir, puis dans un scénario stock qui
      démarre en vol (là où `CommNetHome.Start` tourne hors de la sphère).
- [ ] Mission Making History `KSC flag fix` (déjà publiée dans `diag/kopernicus-flag-fix/Missions/`),
      sans aucun autre mod : le pod du pas de tir sur son point de départ, le pas de tir à son niveau.
- [ ] Autres statiques stock (KSC 2, Island Airfield, pyramides, anomalies) : **une sauvegarde à
      publier**, un vaisseau posé près de chacun ; chargée deux fois, puis on s'éloigne et on revient.
- [ ] Statique qui glisse sur un corps sans atmosphère : vaisseau à 25 km au-dessus d'une anomalie de la
      Mune. La portée du correctif est d'environ 27,5 km (`StaticsFix.ReachRange` : plus grand
      `unload`, 22,5 km, + 5 km) ; relever en jeu l'altitude où la Mune cesse la rotation inverse
      (15 km par défaut, `CelestialBody.inverseRotThresholdAltitude`). Si elle dépasse la portée,
      passer par Gilly et corriger la page.

**2. Hors liste publique**
- [ ] Coût par frame avec beaucoup de statiques enregistrés (centaines de groupes KK) : chapitre
      *Performance*, pas un impact jouable.

**3. KK (portable)**
- [ ] Pourquoi la section `Section3_Mesh` de la piste KK n'est active qu'au premier chargement d'une session
      (21,3 mm au-dessus du revêtement, en stock comme avec le correctif ; rien de tel sur la piste du KSC).
- [ ] Éditeur de groupe en vol : déplacer au gizmo, tourner, créer, copier, supprimer un groupe hors de la
      sphère, éditer un statique d'un groupe ; recharger, chaque groupe est où on l'a laissé.
- [ ] Lancement depuis un site KK.

**4. RSS** (`ksp-rss-dev\`)
- [x] La piste de RSS, sans `RSSRunwayFix` (diff dans `diag/rss-runway-fix/`), sans le mod et avec :
      publié dans [Real Solar System: the runway fix](docs/limits-and-solutions/rss/the-runway-fix.md).
- [x] Le verrou d'origine : protocole « piste et herbe pendant un décalage » de Diag TerrainHeight sur la Terre,
      RSS sans `RSSRunwayFix`, sans le mod (−375,9 / +45,7 mm, piste et herbe ensemble) et avec
      (≤ 0,15 mm) ; publié.
- [ ] Le dernier point de son *Still to test* : RSS tel que publié avec le mod.

**Ensuite** : README (second coupable, autres chapitres), pages *runway* des deux Diags refaites avec le
correctif, puis commit.

## Avant d'ouvrir l'issue KSPCF

**Dans cet ordre** (Lionel, 2026-09-25) : finir RSS (point 1), puis restructurer les autres cas de
*Limits and solutions* (point 2), puis relire les `KSPCF-*.md` (point 3). Les points suivants viennent
ensuite, sans ordre imposé.

1. **Real Solar System : les mesures qui manquent**, dans `ksp-rss-dev\` (PC fixe seulement). La Lune et
   la Terre (près du KSC, en `PRELAUNCH`) sont mesurées et publiées dans
   [Rescaled systems: Real Solar System](docs/limits-and-solutions/rescaled-systems-real-solar-system.md) ;
   la Terre à la chaîne (en `PRELAUNCH` et en `LANDED`, repose de RSS comprise) aussi, et la page
   publique suit (plus grande correction : 4,0 pas) ; Vénus, Mars et Mercure aussi, pour la plus grande
   correction (sauvegardes dans [diag/](diag/README.md)). Son *Still to test* liste les points 1.1 et
   1.2. Reste :
   1. **La piste du KSC terrestre : le correctif rend-il inutile le verrou d'origine de RSS ?** RSS
      intègre RSSRunwayFix, qui fait deux choses. Il coupe les colliders des sections de la piste pour
      ne garder que `runway_collider` : il vise les bosses aux jonctions entre sections, un défaut entre
      statiques auquel le correctif ne touche pas ; cette moitié reste utile quoi qu'il arrive. Et, tant
      que le vaisseau actif est posé (`LANDED` ou `PRELAUNCH`) et qu'un rayon vers le bas touche
      `runway_collider`, il bloque tout décalage d'origine (`SetSafeToEngage(false)` toutes les 25
      frames physiques, seuil porté à 2 700 m). Un décalage peut faire sauter deux surfaces sous la
      roue, les deux que la mesure stock de 117 mm sur la piste n'a pas départagées : **(a)** la piste,
      un `PQSCity` ré-arrondi, que le correctif ne touche pas ; **(b)** le terrain aplani sous la piste,
      ré-arrondi lui aussi, qui dépasse du revêtement par endroits, et que le correctif stabilise
      totalement. Le source ne tranche pas.
      - **D'abord en stock : fait, c'est la piste.** En stock, deux vaisseaux identiques sur l'herbe et
        sur la piste (publié dans les deux Diags, `docs/the-measurements-runway.md`) : la piste bouge
        (~130 mm) et pas d'un bloc avec l'herbe (marche sur ~82 mm). Avec le correctif, une capsule
        seule : le revêtement bouge de 185 mm (archivé dans `kspmod`, pas publié). Donc (a) sur Kerbin ;
        sous RSS, déduit, pas mesuré : le verrou protège au moins de la piste.
      - **Reste** : la même sauvegarde herbe/piste avec le correctif (la marche avec le correctif), puis
        réécrire le cas
        [The KSC buildings, runway and launchpad](docs/limits-and-solutions/the-ksc-buildings-runway-and-launchpad.md)
        (passe en *Checked, a real problem*, Kerbal Konstructs avec lui), et décider si un correctif des
        statiques se fait à part, comme RockPrecisionFix.
      - **Puis sous RSS** : avec le correctif, un roulage et un décollage sur la piste avec le verrou,
        puis sans. Aucun réglage ne coupe le verrou : sur le modèle du flag fix de Kopernicus, RSS
        recompilé sans lui, diff publié dans `diag/` (Lionel, 2026-10-01 ; plus d'addon). Sans verrou, si la piste saute encore à chaque décalage, c'est
        le static, et c'est l'argument pour étendre le correctif aux `PQSCity`.
      - Même dans le cas (b), écrire que, pour ce défaut, le verrou n'a plus rien à corriger ; jamais
        qu'il ne sert plus à rien.

      Le cas KSC en a la procédure publique, dernier point de son *Still to test*.
   2. **Un vol sous RSS, pas seulement des chargements.** Tout ce qui est mesuré sous RSS est un
      vaisseau posé qu'on recharge ; jamais des quads construits en continu pendant les décalages
      d'origine (`PQ.PreciseUpdateSubQuadsPosition`), ce que vit tout joueur RSS à chaque lancement.
      C'est un test de non-régression, pas une mesure par un Diag : ni exception
      `[TerrainPrecisionFix]` ni refus dans le log, aucun trou ni décalage visible entre quads, et la
      plus grande correction ; avec puis sans le correctif, pour comparer à l'œil.
      - Un décollage de Cape Canaveral jusqu'à l'orbite, et une descente sur les sites de Vénus, Mars
        et Mercure (ceux des sauvegardes de `diag\` ; celui de Mercure est très accidenté, cf. la page
        RSS), pilotés par MechJeb2 pour être rejouables, carburant infini (cheat stock, Alt+F12) pour garder
        des pièces stock. MechJeb s'ajoute à l'install : la série doit le déclarer (la page RSS dit
        « Nothing else »). Son source n'est pas dans `kspmod-ext\` : le récupérer s'il faut lire son
        comportement.
      - Le protocole d'approche au rover, celui des campagnes stock, couvre sans mod de plus les quads
        reconstruits et le décalage d'origine au sol : à refaire sur la Lune.
   3. **Hors RSS, soulevés par cette revue** : deux cas à ajouter à
      [Limits and solutions](docs/limits-and-solutions.md), à tester en stock (RSS en héritera) :
      - **les jonctions entre le terrain corrigé et les statiques du KSC** : le correctif déplace le
        terrain, pas la piste ni le pas de tir, donc l'écart entre les deux change (herbe qui traverse
        le bord de la piste, marche au pied du pas de tir). Un *To test* dans le cas KSC : captures des
        bords avec et sans le correctif, un rover qui sort de la piste sur l'herbe, une fusée en
        `PRELAUNCH` sur le pas de tir. Distinct du raccord entre deux quads (cas
        [The seam between subdivision levels](docs/limits-and-solutions/the-seam-between-subdivision-levels.md)) ;
      - **l'océan** : aucun cas aujourd'hui. Dans les logs RSS de la Terre, le correctif n'a corrigé que
        des quads de terrain, jamais l'océan, ce qui colle avec sa garde sur `surfaceRelativeQuads` ;
        la valeur de ce drapeau pour la sphère océan n'a pas été lue. Un amerrissage près d'une côte,
        log à l'appui.
2. **Restructurer les 20 autres cas** de `docs/limits-and-solutions/` sur le plan du cas RSS
   (introduction lue dans le code et sur GitHub, `## Checking the culprit` avec Diag LandedVessel et Diag TerrainHeight,
   `## What the results show`), après avoir décidé comment traiter un cas sans mesure.
3. **Relire `KSPCF-issue.md`, `KSPCF-comment.md` et `KSPCF-relecture.md`**, pas revus depuis RSS.
   Au moins : la Terre (un saut par série de six sans le correctif, jusqu’à 4,0 pas de correction avec le nouveau garde-fou) est
   absente de la relecture ; son point 6 justifie seize pas par les 3,5 pas de la Lune, alors que la
   Terre en a donné 4,0 et Vénus 3,6 ; l'issue ne cite pas RSS, alors que la relecture en fait la seule repro
   visible : décider s'il y entre.
4. **Une release GitHub sur chaque dépôt vers lequel l'issue envoie le lecteur.** Aucun n'en a : ce
   dépôt, Diag LandedVessel, Diag TerrainHeight, Diag FloatingOrigin et Diag QuadSeams (le cas
   [The seam between subdivision levels](docs/limits-and-solutions/the-seam-between-subdivision-levels.md)
   en dépend : liens, images et *Get it*). L'issue commence par faire installer Diag LandedVessel, et les trois README des
   Diags renvoient vers `releases/latest` dans *Get it*, qui donne une 404 aujourd'hui — sur la page
   même où arrive un mainteneur depuis la première consigne du repro. `build.bat` produit déjà le
   dossier `GameData` ; la release, c'est ce dossier zippé. PQS Bench et Stock Quad Cache ne sont cités
   qu'en appui du chiffre de performance et peuvent attendre, mais leur *Get it* ne doit pas non plus
   promettre un téléchargement qui n'existe pas. Vérifier tous les liens `releases/latest` de la famille
   avant d'ouvrir l'issue.
5. **Une sauvegarde refaite avec le correctif ne saute plus.** Sur `switch-kerbin` (faite sans), la
   capsule se pose à −31,8 mm, au même endroit à chaque fois. Le test : avec le correctif, charger,
   `]`, repasser au rover, sauvegarder sous un autre nom, puis six fois « charger → *Record* → `]` →
   *Record* » ; *Moved* doit rester à quelques centièmes de zéro. Il complète le chapitre
   [Existing saves](docs/limits-and-solutions/existing-saves.md) et la page du changement de vaisseau.
6. **Confirmer la marge du garde-fou.** Il compte seize pas de float à la distance du quad (1 m sur
   Kerbin, 8 m sur la Terre de RSS). Plus grande correction vue : 4,0 pas sur la Terre (3,6 sur
   Vénus, 3,5 sur la Lune, environ un sur Mars et Mercure), aucun refus sur ces cinq corps. Reste à
   relever la plus grande correction (`origin moved by … mm`) en `logLevel = Debug` sur Kerbin, la
   Mun, Minmus et Gilly, et vérifier qu'elle reste loin de seize pas.
7. **Deferred : refaire les campagnes Diag LandedVessel et Diag TerrainHeight avec le correctif et Deferred** (six chargements,
   et le protocole d'approche). C'est le mod qui a fait tomber `PQSOnlyStartOnce` : le résultat, bon ou
   mauvais, entre dans le commentaire, qui l'annonce aujourd'hui comme « next ». Chapitre *Deferred* de
   Limits and solutions à mettre à jour ensuite.
8. **Parallax : les mêmes campagnes, et plus loin à cause de son scatter.** Montrer que le défaut du
   scatter de Parallax n'existe déjà pas sans le correctif, et qu'avec le correctif rien ne change
   (aujourd'hui, c'est lu dans ses sources, pas mesuré : *Rocks, grass and trees* de Limits and
   solutions). Il faut un instrument ou un protocole pour ce scatter, qui n'est pas celui de KSP : à
   concevoir.
9. **Principia**, que la plupart des joueurs RSS installent (cas
   [Principia](docs/limits-and-solutions/principia.md) de Limits and solutions). Test à part, **sur le
   système stock**, sans RSS (Lionel, 2026-09-25).
10. **L'écart diffère-t-il d'un point du sol à l'autre ?** C'est ce qui casse une structure posée sur
   plusieurs pieds (un pied enterré, un autre en l'air) : sans lui, le sol monte ou descend d'un bloc et
   la structure suit. Je le crois, puisque chaque quad arrondit sa propre position, mais rien ne le
   mesure. Diag TerrainHeight sur quelques points éloignés de plusieurs quads, six chargements, sans le correctif ;
   faisable sur Kerbin dans `ksp-dev\`, sur les deux PC. Conditionne le paragraphe « Why a few
   centimetres matter » de l'issue.

## Après l'ouverture

Rien de ceci ne change ce que l'issue demande.

- **Les cas contre lesquels vérifier le correctif** sont les chapitres TBD de
  [Limits and solutions](docs/limits-and-solutions.md), le plan de test public vers lequel pointe
  l'issue : chacun dit ce qu'on sait et comment il sera testé. Un nouveau cas s'ajoute là-bas (un
  fichier dans `docs/limits-and-solutions/` et son résumé dans la page d'index), en **TBD**, pas ici.
- **Séparer le décalage d'origine du déchargement.** Dans le protocole d'approche, le vaisseau est
  déchargé et l'origine se décale à la même frame : aucune mesure ne dit encore lequel des deux fait
  bouger le sol ; le code stock désigne le décalage. ⚠️ Changer de vaisseau (`]`) **ne** décale **pas**
  l'origine (vérifié dans `FlightGlobals.setActiveVessel`, 2026-09-24), et un décalage sans
  déchargement est impossible tant que le témoin posé est chargé (verrou). Le test faisable est
  l'inverse : **un déchargement sans décalage**. Une sauvegarde avec le rover à ~2,15 km du témoin
  (chargé au départ, sous 2,25 km) : en s'éloignant, le témoin se décharge à 2,5 km alors que le rover
  n'est qu'à ~350 m de l'origine, donc sous le seuil de 500 m ; puis retour sous 2,25 km (rechargement)
  et approche sous 200 m (le témoin, chargé, verrouille l'origine). Diag LandedVessel et Diag TerrainHeight visent le témoin
  comme dans le protocole d'approche, Diag FloatingOrigin doit afficher **Shifts** = 0 du début à la fin. Si le sol
  du témoin ne bouge pas, le déchargement seul n'y est pour rien : c'est le décalage. En complément,
  avec le correctif en `logLevel = Debug`, le décalage du protocole d'approche doit apparaître comme une
  salve de lignes `origin moved by … mm`.
- **L'ancre, avant de commenter #214** — pas avant l'issue du terrain. Le commentaire sur #214 cite ses
  relevés, donc ils doivent être reproductibles par quelqu'un d'autre : les refaire sur KSP + Harmony +
  ModuleManager + KSPCF + Diag LandedVessel, puis avec le correctif. Ce qui doit en sortir : les valeurs de
  `Moving Vessel` sans le correctif (les deux signes, une différente à chaque chargement) et avec (la même
  à chaque chargement) ; un `.sfs` montrant `PQSMin`/`PQSMax` à `0/0` sur une ancre fraîchement posée ;
  l'écart de 2,08 cm du collider relu dans `groundAnchor.mu`. ⚠️ **Sauvegarder après chaque
  chargement** : c'est la re-sauvegarde qui arme le cliquet.
- **Les sauvegardes existantes, ce qu'il reste à rédiger** (le cas lui-même est le chapitre
  [Existing saves](docs/limits-and-solutions/existing-saves.md)). Le seul essai sur une vraie sauvegarde
  (la mienne, bases chargées une par une avec KSP, Harmony, KSPCF et le correctif) n'est pas rédigé :
  aucune n'a cassé sur la Mun, Minmus et Gilly ; la base d'Eve se pose sur des pieds construits sous la
  surface, un défaut de construction et pas le correctif. La sauvegarde n'échantillonne pas le pire cas
  (ses bases reposent sur des rails de poutrelles, alors que ce sont les modules amarrés sur des jambes
  d'atterrissage qui souffrent le plus). Reste à faire : une capture de l'ampleur de ce qui a été
  chargé ; décider où va la mise en garde sur les assemblages amarrés et les jambes (le *Disclaimer* est
  partagé mot pour mot avec les README des Diags) ; une phrase sous *How this was made* sur l'outil de
  construction en EVA avec lequel ces bases ont été montées ; vérifier dans le mécanisme de réglages de
  KSPCF qu'un patch se désactive, avant d'écrire qu'un joueur peut le couper, charger, relever un
  vaisseau et le remettre ; un outil de migration du `.sfs` seulement si un coût réel apparaît.
- **Poster sur #435** une fois sûr de ce que Diag FloatingOrigin montre aux chargements, comme le commentaire
  l'annonce.
- **Le coût du correctif sur Kerbin.** La campagne de performance a été volée au-dessus de la Mun ;
  Kerbin, où les quads sont quatre fois plus grands, vaut d'être mesuré.
