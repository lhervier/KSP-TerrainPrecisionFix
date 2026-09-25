# TODO

Ce qu'il reste à faire, et seulement ça. Tout ce qui est déjà mesuré est dans [le README](README.md) et
ses chapitres sous [docs/](docs/), avec ses logs dans [perfs/](perfs/) et dans les dépôts des trois
Diags ; aucun résultat n'est consigné ici. La relecture anticipée de l'issue est dans
[KSPCF-relecture.md](KSPCF-relecture.md).

L'issue KSPCF demande une relecture et de l'aide, pas une publication : elle dit ce qui est vérifié et ce
qui ne l'est pas, et [Limits and solutions](docs/limits-and-solutions.md) liste en public les cas encore
ouverts. Seul ce qui casserait la lecture de l'issue elle-même doit donc passer avant.

## Avant d'ouvrir l'issue KSPCF

1. **Une release GitHub sur chaque dépôt vers lequel l'issue envoie le lecteur.** Aucun n'en a : ce
   dépôt, Diag 1, Diag 2 et Diag 3. L'issue commence par faire installer Diag 1, et les trois README des
   Diags renvoient vers `releases/latest` dans *Get it*, qui donne une 404 aujourd'hui — sur la page
   même où arrive un mainteneur depuis la première consigne du repro. `build.bat` produit déjà le
   dossier `GameData` ; la release, c'est ce dossier zippé. PQS Bench et Stock Quad Cache ne sont cités
   qu'en appui du chiffre de performance et peuvent attendre, mais leur *Get it* ne doit pas non plus
   promettre un téléchargement qui n'existe pas. Vérifier tous les liens `releases/latest` de la famille
   avant d'ouvrir l'issue.
2. **Les raccords entre un quad corrigé et ses voisins d'un niveau inférieur.** Le correctif place les
   quads du niveau max par la rotation en double ; leurs voisins d'un niveau en dessous restent placés
   par la matrice en float de la sphère, et l'écart entre les deux est du même ordre que le défaut. Personne
   n'a regardé le raccord : une capture au bord d'un quad du niveau max, avec et sans le correctif, et un
   chapitre **Status** dans [Limits and solutions](docs/limits-and-solutions.md). Objection anticipée dans
   [KSPCF-relecture.md](KSPCF-relecture.md), point 2.
3. **Une sauvegarde refaite avec le correctif ne saute plus.** Sur `switch-kerbin` (faite sans), la
   capsule se pose à −31,8 mm, au même endroit à chaque fois. Le test : avec le correctif, charger,
   `]`, repasser au rover, sauvegarder sous un autre nom, puis six fois « charger → *Record* → `]` →
   *Record* » ; *Moved* doit rester à quelques centièmes de zéro. Il complète le chapitre
   [Existing saves](docs/limits-and-solutions/existing-saves.md) et la page du changement de vaisseau.
4. **Le garde-fou en pas de float plutôt qu'en mètres.** Une modification du code, pas un cas : le
   chapitre RSS de Limits and solutions dit pourquoi le mètre fixe peut être faux. Un seuil de quelques pas
   de float au rayon du corps (quatre, par exemple) serait plus serré sur Kerbin et juste sur RSS ;
   mesurer d'abord, par corps, la plus grande correction réellement appliquée dans les logs de campagne
   (`origin moved by … mm`).
   Le code est léger (le seuil ne sert que dans `IsRoundingCorrection`) ; la mesure demande une session
   en `logLevel = Debug` sur Kerbin, la Mun, Minmus et Gilly, en relevant le plus grand `origin moved by`
   par corps. **Déjà relevé sous RSS** (chapitre « Sous RSS : la Lune » de
   `claude-notes\collider-pqs\correctif-sol.md`) : le mètre a refusé un quad de la Terre, et la Lune
   monte à 3,5 pas, donc « quatre pas » serait trop juste.
5. **Deferred : refaire les campagnes Diag 1 et Diag 2 avec le correctif et Deferred** (six chargements,
   et le protocole d'approche). C'est le mod qui a fait tomber `PQSOnlyStartOnce` : le résultat, bon ou
   mauvais, entre dans le commentaire, qui l'annonce aujourd'hui comme « next ». Chapitre *Deferred* de
   Limits and solutions à mettre à jour ensuite.
6. **Parallax : les mêmes campagnes, et plus loin à cause de son scatter.** Montrer que le défaut du
   scatter de Parallax n'existe déjà pas sans le correctif, et qu'avec le correctif rien ne change
   (aujourd'hui, c'est lu dans ses sources, pas mesuré : *Rocks, grass and trees* de Limits and
   solutions). Il faut un instrument ou un protocole pour ce scatter, qui n'est pas celui de KSP : à
   concevoir.
7. **Un kraken reproductible, sur la Lune de RSS d'abord** (objection 1 de
   [KSPCF-relecture.md](KSPCF-relecture.md)), avant l'issue. Gotmachine reproduit lui-même ce qu'on lui
   signale : sur Kerbin, une capsule qui saute de quelques centimètres ne le convaincra pas, la physique
   l'absorbe. RSS amplifie le défaut, car le pas du float double avec le rayon :
   - **La Lune** (1 737 km, entre 2²⁰ et 2²¹ m) : pas de 125 mm, soit ~25 cm d'écart attendu à ~2 pas.
     Un seul pas dépasse déjà la peau de ~25 mm que PhysX dépénètre, et la gravité (1,62 m/s²) laisse
     voir un saut. Le garde-fou de 1 m n'y est pas atteint : **le code actuel suffit**. C'est aussi là que
     les joueurs RO construisent leurs bases.
   - **La Terre** (0,5 m par pas) : seulement après le point 4, puisque le garde-fou y couperait le
     correctif.
   - Gilly reste une piste secondaire (capsule enfoncée d'un rien, qu'une gravité si faible laisse
     décoller).

   Fait le 2026-09-25 avec Diag 1 (install `ksp-rss-dev\`, PC fixe seulement) : la capsule sur réservoir
   se renverse sans le correctif, reste debout avec (chapitre « Sous RSS : la Lune » de
   `claude-notes\collider-pqs\correctif-sol.md`). Reste :
   1. **La repose au sol que RSS force par défaut** (`VesselGroundPositionEnhancer`) masque le
      renversement : la repro doit dire de la couper (DLL vide `WorldStabilizer.dll`, ou la case
      *Force off* de RSS `master`), sinon le mainteneur verra une capsule téléportée, pas renversée.
      Décider comment le présenter dans l'issue.
   2. La Terre, une fois le garde-fou passé en pas de float (point 4) : Diag 1 et Diag 2, terrain plat.

   Copies d'écran dans `imgs/Diag1/on-load/2parts/rss/`, mesures publiées dans
   [Rescaled systems: Real Solar System](docs/limits-and-solutions/rescaled-systems-real-solar-system.md).

   **Une fois RSS terminé** : restructurer les 19 autres cas de `docs/limits-and-solutions/` sur le plan
   du cas RSS (introduction lue dans le code et sur GitHub, `## Checking the culprit` avec Diag 1 et
   Diag 2, `## What the results show`). Reste à décider comment traiter un cas sans mesure.

   Ce qui existe déjà côté joueurs (WorldStabilizer, RSSRunwayFix) : chapitre « Les voisins » de
   `claude-notes\kspcf.md`.
8. **L'écart diffère-t-il d'un point du sol à l'autre ?** C'est ce qui casse une structure posée sur
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
