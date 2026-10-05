# TODO

Ce qu'il reste à faire, et seulement ça. Tout ce qui est déjà mesuré est dans [le README](README.md) et
ses chapitres sous [docs/](docs/), avec ses logs dans [perfs/](perfs/) et dans les dépôts des trois
Diags ; aucun résultat n'est consigné ici. **Les tests à jouer n'y sont pas non plus** : l'impact sur
stock est dans les tableaux de [Non-regression tests: the ground](docs/non-regression-ground.md) et
[the statics](docs/non-regression-statics.md), l'impact sur les autres mods dans les *Still to test* de
[Limits and solutions](docs/limits-and-solutions.md), le coût dans [Performance](docs/performance.md).
Un nouveau test s'ajoute là-bas (une ligne de tableau, et une page dans `docs/non-regression/` s'il en
faut une ; ou un fichier dans `docs/limits-and-solutions/` et son résumé dans la page d'index), pas ici.

## Dans cet ordre

D'abord les tests encore ouverts de [Real Solar System](docs/limits-and-solutions/rescaled-systems-real-solar-system.md#still-to-test),
puis les deux points ci-dessous (Lionel, 2026-09-25).

1. **Restructurer les autres cas** de `docs/limits-and-solutions/` et de `docs/non-regression/` sur le
   plan du cas RSS
   (introduction lue dans le code et sur GitHub, `## Checking the culprit` avec Diag LandedVessel et Diag TerrainHeight,
   `## What the results show`), après avoir décidé comment traiter un cas sans mesure.
2. **Mettre à jour le texte de #440** (la branche est fusionnée dans `main`). Son lien *so many cases to test* vise
   `limits-and-solutions.md`, qui ne parle plus que des autres mods : y ajouter les tests de
   non-régression ([the ground](docs/non-regression-ground.md), [the statics](docs/non-regression-statics.md)),
   l'impact sur stock. Décider si RSS y entre comme repro visible : l'issue ne le cite qu'en passant
   (« especially with RSS »), alors que la Terre donne un saut par série de six chargements sans le
   correctif, et jusqu'à 4,0 pas de correction avec.

## Sans ordre imposé

3. **Une release GitHub sur chaque dépôt vers lequel l'issue envoie le lecteur.** Aucun n'en a : ce
   dépôt, Diag LandedVessel, Diag TerrainHeight, Diag FloatingOrigin et Diag TerrainQuads (le cas
   [The seam between subdivision levels](docs/non-regression/the-seam-between-subdivision-levels.md)
   en dépend : liens, images et *Get it*). L'issue commence par faire installer Diag LandedVessel, et les trois README des
   Diags renvoient vers `releases/latest` dans *Get it*, qui donne une 404 aujourd'hui — sur la page
   même où arrive un mainteneur depuis la première consigne du repro. `build.bat` produit déjà le
   dossier `GameData` ; la release, c'est ce dossier zippé. PQS Bench et Stock Quad Cache ne sont cités
   qu'en appui du chiffre de performance et peuvent attendre, mais leur *Get it* ne doit pas non plus
   promettre un téléchargement qui n'existe pas. Vérifier tous les liens `releases/latest` de la famille.
4. **Confirmer la marge du garde-fou.** Il compte seize pas de float à la distance du quad (1 m sur
   Kerbin, 8 m sur la Terre de RSS). Plus grande correction vue : 4,0 pas sur la Terre (3,6 sur
   Vénus, 3,5 sur la Lune, environ un sur Mars et Mercure), aucun refus sur ces cinq corps. Reste à
   relever la plus grande correction (`origin moved by … mm`) en `logLevel = Debug` sur Kerbin, la
   Mun, Minmus et Gilly, et vérifier qu'elle reste loin de seize pas.
5. **L'écart diffère-t-il d'un point du sol à l'autre ?** C'est ce qui casse une structure posée sur
   plusieurs pieds (un pied enterré, un autre en l'air) : sans lui, le sol monte ou descend d'un bloc et
   la structure suit. Je le crois, puisque chaque quad arrondit sa propre position, mais rien ne le
   mesure. Diag TerrainHeight sur quelques points éloignés de plusieurs quads, six chargements, sans le correctif ;
   faisable sur Kerbin dans `ksp-dev\`, sur les deux PC. Si la mesure le confirme, une phrase à ajouter
   au texte de #440, là où il dit que les grosses bases explosent : un pied enterré, un autre en l'air.

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
- **Les sauvegardes existantes, ce qu'il reste à rédiger** (le cas lui-même est le chapitre
  [Existing saves](docs/non-regression/existing-saves.md)). Le seul essai sur une vraie sauvegarde
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
- **Rejouer toutes les campagnes en automatique, et reconstituer mesures et logs** (Lionel,
  2026-10-03). Chaque protocole publié (chargement par corps, approche, changement de vaisseau, piste,
  roulage, cas RSS, cas de *Limits and solutions*, tests de non-régression) reçoit un script KSP-MCPServer, **à côté** de sa
  procédure à la main, qui reste ; on rejoue sans le correctif et avec, puis on remplace les séries, les
  logs et les chiffres publiés par ceux des scripts. Modèles : `diag/automation/run-runway.py` et
  `run-approach.py` (campagne Deferred). À trancher au départ : où vit un script (le protocole appartient
  à l'instrument, donc plutôt dans le dépôt du Diag : y déplacer les deux d'ici) ; un Diag par session
  comme aujourd'hui, ou les deux ensemble ; les outils du serveur qui manqueront (Set Position, Set
  Orbit, Infinite Fuel : seulement ceux qu'un script utilise).
- **Diags : *Clear* remet la fenêtre à sa hauteur initiale** (Lionel, 2026-10-04). Diag TerrainHeight,
  Diag LandedVessel et Diag FloatingOrigin : leur fenêtre (`GUILayout.Window`) garde la hauteur qu'elle
  avait avant *Clear table*, et un vide reste sous le tableau, jusque dans les captures. Remettre la
  hauteur de `windowRect` à zéro au *Clear* (et sans doute au *Delete* d'une ligne), pour qu'elle se
  recale sur son contenu. Contourné en attendant par les scripts de capture (`set_member` de
  `windowRect`).
