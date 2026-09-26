# TODO

Ce qu'il reste à faire, et seulement ça. Tout ce qui est déjà mesuré est dans [le README](README.md) et
ses chapitres sous [docs/](docs/), avec ses logs dans [perfs/](perfs/) et dans les dépôts des trois
Diags ; aucun résultat n'est consigné ici. La relecture anticipée de l'issue est dans
[KSPCF-relecture.md](KSPCF-relecture.md).

L'issue KSPCF demande une relecture et de l'aide, pas une publication : elle dit ce qui est vérifié et ce
qui ne l'est pas, et [Limits and solutions](docs/limits-and-solutions.md) liste en public les cas encore
ouverts. Seul ce qui casserait la lecture de l'issue elle-même doit donc passer avant.

## Avant d'ouvrir l'issue KSPCF

**Dans cet ordre** (Lionel, 2026-09-25) : finir RSS (point 1), puis restructurer les autres cas de
*Limits and solutions* (point 2), puis relire les `KSPCF-*.md` (point 3). Les points suivants viennent
ensuite, sans ordre imposé.

1. **Real Solar System : les mesures qui manquent**, dans `ksp-rss-dev\` (PC fixe seulement). La Lune et
   la Terre (près du KSC, en `PRELAUNCH`) sont mesurées et publiées dans
   [Rescaled systems: Real Solar System](docs/limits-and-solutions/rescaled-systems-real-solar-system.md) ;
   les points 1 et 2 ci-dessous sont son *Still to test*.
   1. **La Terre, rechargements à la chaîne avec le correctif**, sans instrument, sur
      `reload-earth-rss-resave.sfs` (partie : `diag1-reload-earth-resave`), comme les 24 chargements de
      la Lune. Compter les `Moving Vessel` et les sauts vus.
   2. **Un vaisseau posé hors du KSC**, en `LANDED`, pour voir la repose de RSS à l'œuvre sur la Terre
      (au KSC, en `PRELAUNCH`, elle ne tourne pas). Une nouvelle sauvegarde, à copier ensuite dans le
      `diag\` des trois Diags comme les trois autres.
   3. **Vénus, Mars, Mercure** : un atterrissage chacun en `logLevel = Debug`, pour la plus grande
      correction (pas de 500 / 250 / 250 mm). Alimente le point 7 (marge du garde-fou).
   4. **La piste du KSC terrestre : le correctif rend-il inutile le verrou d'origine de RSS ?** RSS
      intègre RSSRunwayFix, qui coupe les colliders des sections de la piste et, tant que le vaisseau
      roule dessus, bloque tout décalage d'origine (`SetSafeToEngage(false)`, seuil porté à 2 700 m).
      Lu dans le source, la piste est un `PQSCity`, que le correctif ne touche pas : il ne devrait donc
      **pas** rendre ce verrou inutile sur la piste (sur l'herbe, si). Le test : avec le correctif, un
      roulage et un décollage sur la piste avec le verrou de RSS, puis sans. Aucun réglage ne coupe le
      verrou : il faut un petit addon de test qui détruit `RSSRunwayFix.Instance` en vol. Sans verrou,
      si la piste saute encore à chaque décalage, c'est le static, et c'est l'argument pour étendre le
      correctif aux `PQSCity`. Voir aussi le cas
      [The KSC buildings, runway and launchpad](docs/limits-and-solutions/the-ksc-buildings-runway-and-launchpad.md),
      qui ne parle pas encore de RSS.
   5. **Pourquoi la repose de KSP a tourné sur la Terre** sans le correctif (3 chargements sur 6), en
      `PRELAUNCH`, alors que la sauvegarde porte des niveaux PQS non nuls (2/11) : comparer au
      `maxLevel` du `pqsController` de la Terre de RSS. La page publique donne le fait sans
      l'expliquer.
2. **Restructurer les 19 autres cas** de `docs/limits-and-solutions/` sur le plan du cas RSS
   (introduction lue dans le code et sur GitHub, `## Checking the culprit` avec Diag 1 et Diag 2,
   `## What the results show`), après avoir décidé comment traiter un cas sans mesure.
3. **Relire `KSPCF-issue.md`, `KSPCF-comment.md` et `KSPCF-relecture.md`**, pas revus depuis RSS.
   Au moins : la Terre (le saut de +88,8 mm sans le correctif, 2,8 pas avec le nouveau garde-fou) est
   absente de la relecture ; l'issue ne cite pas RSS, alors que la relecture en fait la seule repro
   visible : décider s'il y entre.
4. **Une release GitHub sur chaque dépôt vers lequel l'issue envoie le lecteur.** Aucun n'en a : ce
   dépôt, Diag 1, Diag 2 et Diag 3. L'issue commence par faire installer Diag 1, et les trois README des
   Diags renvoient vers `releases/latest` dans *Get it*, qui donne une 404 aujourd'hui — sur la page
   même où arrive un mainteneur depuis la première consigne du repro. `build.bat` produit déjà le
   dossier `GameData` ; la release, c'est ce dossier zippé. PQS Bench et Stock Quad Cache ne sont cités
   qu'en appui du chiffre de performance et peuvent attendre, mais leur *Get it* ne doit pas non plus
   promettre un téléchargement qui n'existe pas. Vérifier tous les liens `releases/latest` de la famille
   avant d'ouvrir l'issue.
5. **Les raccords entre un quad corrigé et ses voisins d'un niveau inférieur.** Le correctif place les
   quads du niveau max par la rotation en double ; leurs voisins d'un niveau en dessous restent placés
   par la matrice en float de la sphère, et l'écart entre les deux est du même ordre que le défaut. Personne
   n'a regardé le raccord : une capture au bord d'un quad du niveau max, avec et sans le correctif, et un
   chapitre **Status** dans [Limits and solutions](docs/limits-and-solutions.md). Objection anticipée dans
   [KSPCF-relecture.md](KSPCF-relecture.md), point 2.
6. **Une sauvegarde refaite avec le correctif ne saute plus.** Sur `switch-kerbin` (faite sans), la
   capsule se pose à −31,8 mm, au même endroit à chaque fois. Le test : avec le correctif, charger,
   `]`, repasser au rover, sauvegarder sous un autre nom, puis six fois « charger → *Record* → `]` →
   *Record* » ; *Moved* doit rester à quelques centièmes de zéro. Il complète le chapitre
   [Existing saves](docs/limits-and-solutions/existing-saves.md) et la page du changement de vaisseau.
7. **Confirmer la marge du garde-fou.** Il compte seize pas de float à la distance du quad (1 m sur
   Kerbin, 8 m sur la Terre de RSS). Le multiple repose sur la Lune (3,5 pas au plus) ; la Terre a
   donné 2,8 pas. Relever la plus grande correction (`origin moved by … mm`) en `logLevel = Debug` sur
   Kerbin, la Mun, Minmus et Gilly (et Vénus, Mars, Mercure, point 1.3), et vérifier qu'elle reste loin
   de seize pas.
8. **Deferred : refaire les campagnes Diag 1 et Diag 2 avec le correctif et Deferred** (six chargements,
   et le protocole d'approche). C'est le mod qui a fait tomber `PQSOnlyStartOnce` : le résultat, bon ou
   mauvais, entre dans le commentaire, qui l'annonce aujourd'hui comme « next ». Chapitre *Deferred* de
   Limits and solutions à mettre à jour ensuite.
9. **Parallax : les mêmes campagnes, et plus loin à cause de son scatter.** Montrer que le défaut du
   scatter de Parallax n'existe déjà pas sans le correctif, et qu'avec le correctif rien ne change
   (aujourd'hui, c'est lu dans ses sources, pas mesuré : *Rocks, grass and trees* de Limits and
   solutions). Il faut un instrument ou un protocole pour ce scatter, qui n'est pas celui de KSP : à
   concevoir.
10. **Principia**, que la plupart des joueurs RSS installent (cas
   [Principia](docs/limits-and-solutions/principia.md) de Limits and solutions). Test à part, **sur le
   système stock**, sans RSS (Lionel, 2026-09-25).
11. **L'écart diffère-t-il d'un point du sol à l'autre ?** C'est ce qui casse une structure posée sur
   plusieurs pieds (un pied enterré, un autre en l'air) : sans lui, le sol monte ou descend d'un bloc et
   la structure suit. Je le crois, puisque chaque quad arrondit sa propre position, mais rien ne le
   mesure. Diag 2 sur quelques points éloignés de plusieurs quads, six chargements, sans le correctif ;
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
  et approche sous 200 m (le témoin, chargé, verrouille l'origine). Diag 1 et Diag 2 visent le témoin
  comme dans le protocole d'approche, Diag 3 doit afficher **Shifts** = 0 du début à la fin. Si le sol
  du témoin ne bouge pas, le déchargement seul n'y est pour rien : c'est le décalage. En complément,
  avec le correctif en `logLevel = Debug`, le décalage du protocole d'approche doit apparaître comme une
  salve de lignes `origin moved by … mm`.
- **L'ancre, avant de commenter #214** — pas avant l'issue du terrain. Le commentaire sur #214 cite ses
  relevés, donc ils doivent être reproductibles par quelqu'un d'autre : les refaire sur KSP + Harmony +
  ModuleManager + KSPCF + Diag 1, puis avec le correctif. Ce qui doit en sortir : les valeurs de
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
- **Poster sur #435** une fois sûr de ce que Diag 3 montre aux chargements, comme le commentaire
  l'annonce.
- **Le coût du correctif sur Kerbin.** La campagne de performance a été volée au-dessus de la Mun ;
  Kerbin, où les quads sont quatre fois plus grands, vaut d'être mesuré.
