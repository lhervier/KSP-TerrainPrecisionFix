# Issue KSPCF — la relecture, anticipée

Ce fichier dit **ce qu'un mainteneur répondra** en lisant `KSPCF-issue.md` et `KSPCF-comment.md` dans
leur état actuel, et ce qu'on lui répondra à notre tour. Ce qu'il **reste à faire** n'est pas ici : c'est
[TODO.md](TODO.md) (avant / après l'ouverture de l'issue), et les cas à tester sont les chapitres TBD de
[Limits and solutions](docs/limits-and-solutions.md). Revoir ce fichier à chaque modification de l'issue
ou du commentaire.

Sources de l'anticipation : ses réponses archivées dans `kspmod\kspcf-forum\issues\` (#9, #271, #296,
#324), la PR #435 et le fil forum (`kspcf-forum\texte\`), résumés dans
[CLAUDE-kspcf.md](../CLAUDE-kspcf.md). On y lit un homme **précis, qui connaît le moteur à fond,
prudent sur tout ce qui touche un chemin chaud du jeu, et qui ne bouge que sur un cas reproductible**.

## Qui répondra, et quand

D'abord **le silence** : #435 n'avait rien reçu en cinq jours. La première réponse peut venir de
steamroller (qui sort les releases) ou de JonnyOThan (qui ouvre les issues à partir du forum) plutôt que
du mainteneur dont ce fichier anticipe la réponse. Ne pas relancer avant une ou deux semaines.

## Sa réponse la plus probable

Plausiblement, en substance :

> *Good find, and a well documented one. The float precision issue with PQS quads is real, KSP was
> never designed around it. My main concern is the scope: this changes where the terrain is for
> everyone, and the terrain is something a lot of mods touch or rely on. I'd like to see it running for
> a while as a standalone mod before considering it for KSPCF.*

Donc : **d'accord sur le diagnostic, réservé sur la livraison**. Ce n'est pas un refus, et ça rejoint
ce que l'issue demande (une relecture et de l'aide, pas une publication). Le « publie-le d'abord à part »
est le scénario le plus probable, compte tenu de `PQSOnlyStartOnce` (désactivé le jour de sa sortie) et
de sa prudence dans #296. Réponse prête : c'est déjà un mod à part, et l'issue ne demande rien d'autre
qu'une relecture ; le passer sur CKAN est possible si c'est ce qu'il conseille.

## Ce qui va lui plaire

- **Le repro sans le correctif**, avec des instruments inertes d'environ 200 lignes. Dans #9 comme dans
  #324, il ne bouge que sur un cas reproductible ; dans #324, il reproduit lui-même en stock avant de
  parler.
- **Le GIF avec `PartStartStability` actif** : son propre patch contre les krakens au chargement ne
  l'empêche pas. Il écarte d'avance le premier réflexe (« c'est #9 »).
- **L'arithmétique** : le pas d'un float à 600 km (62,5 mm), vérifiable de tête.
- **Le verrou de l'origine cité** (`Krakensbane.SafeToEngage`) et **« placed again », pas « built »** :
  il connaît `FloatingOrigin`, il aurait relevé l'omission ou le mot faux.
- **KSPProfiler, pire 1 % compris** : son outil, et ce qu'il surveille sur le PQS dans #271.
- **`PQSOnlyStartOnce` nommé par nous**, et le « tout ou rien » si un patch ne s'installe pas.
- **Le ton** : on cite le code stock, on dit ce qu'on n'a pas vérifié. Il a été très sec avec Lisias
  (#9), qui affirmait sans s'être documenté.

## Ce qu'il objectera, et ce qu'on répond

**1. « Ça déplace le sol de tout le monde : quels mods ça casse ? »** L'objection de #296 : *« side
effects likely to be sneaky, very situational or not game breaking enough for end users to notice »*.
C'est **la** question, et l'issue y répond d'avance en renvoyant à *Limits and solutions* : ce qui est
vérifié, affecté, ou à faire, cas par cas. Point fort à avoir en tête : **aucun mod de `kspmod-ext`
(Kopernicus et Parallax compris) ne patche les méthodes que le correctif touche** (`BuildVertexSurfaceRelative`,
`SetupQuad`, `PreciseUpdateSubQuadsPosition`, `BuildQuad`), vérifié le 2026-09-24 dans les sources
clonées. Le préfixe qui renvoie `false` sur `BuildVertexSurfaceRelative` court-circuite la méthode
stock : il demandera ce qui arrive à un mod qui la patcherait. Réponse : aucun connu, et le correctif ne
court-circuite que les quads du niveau max, le reste passe par le stock.

**2. « Montre-moi un kraken reproductible. »** Le point le plus faible, et le seul où on n'a pas de
réponse franche. Dans #9 : *« Unless I find a reproducible case of that issue, I won't implement that
more general fix »*. On a un saut reproductible (le GIF, « several times if needed » : pas à chaque
chargement) et des dispersions mesurées, **pas une explosion à la demande**. La réponse honnête : le
défaut, c'est justement que le sol ne se reproduit pas, ce qui rend les krakens de chargement
irreproductibles ; #214 en est l'exemple travaillé (l'ancre dont le bruit du sol masquait la cause).
Ne pas surpromettre : ne jamais écrire que le correctif supprime les krakens.

**3. « Ce n'est pas aléatoire, c'est déterministe. »** Le titre « float rounding, and randomness » (figé
par Lionel) touche un réflexe : dans #9, il reprend Lisias sur chaque terme (*« Nope. […] perfectly
deterministic »*). Le corps de la section y répond d'avance (l'arrondi est déterministe, c'est le repère
qui ne se répète pas). Si l'objection vient, répondre dans ce sens, sans défendre le mot.

**4. « #435 remet le repère à zéro au chargement : ça ne suffit pas ? »** Il fera le lien, les deux
propositions touchent au même repère. Le commentaire y répond déjà, au
paragraphe *About #435* : remettre le repère en place au changement de scène ne suffit pas pour le sol,
puisque Diag 3 le montre bouger ensuite sans rien charger (angle au-dessus de 100 km, position à chaque
décalage d'origine, bond en quittant un vaisseau posé). Et **la mesure d'approche montre la
conséquence sans rechargement** (21,8 mm contre 0,094 mm). Elle n'est pas encore citée dans
l'issue : c'est un point *avant* de [TODO.md](TODO.md). Sans elle, cette réponse repose sur un
raisonnement, et il pourrait la ranger dans *« theoretical »* (#9).

**5. « C'est le déchargement de ton témoin qui le repose, pas le repère. »** Dans le protocole
d'approche, le décalage d'origine et le déchargement tombent à la même frame. Le code désigne le
décalage (`PreciseUpdateQuadPositions`), aucune mesure ne l'isole encore. **Ça ne fragilise pas le
correctif** — il supprime l'effet quelle qu'en soit la cause, c'est mesuré —, seulement l'explication.
Réponse : le code, et le test du déchargement sans décalage (dans [TODO.md](TODO.md), après).

**6. « Ton garde-fou d'un mètre ne voit que les grosses erreurs. »** Le revers de #296 : un repère
faux de 30 cm passerait, et le correctif déplacerait le sol de 30 cm en silence. Et sur RSS, où le pas
vaut 0,5 m, le garde-fou risque de couper le correctif. Réponse : c'est dit dans *Limits and solutions*
(chapitre RSS), et le seuil en pas de float est prévu ([TODO.md](TODO.md)).

**7. Le code.** Probables : l'accès aux champs privés par réflexion (KSPCF compile contre des
assemblies « publicisées » et a sa classe `BasePatch`) ; un préfixe appelé 225 fois par quad, avec un
cache (dans #271, il écrit que le surcoût de Harmony peut annuler un gain : notre mesure lui répond,
le placement est plus rapide que le stock) ; peut-être une suggestion de transpiler. Réponse : volontiers,
c'est son code de référence ; le mesh est envoyé à l'intérieur de `BuildQuad` (`PQS.cs:2516`), une passe
unique par quad demanderait un transpiler.

**8. « Et Deferred ? »** Le mod qui a fait tomber `PQSOnlyStartOnce`. Le commentaire l'annonce comme le
prochain test : il attendra ce résultat avant tout le reste. C'est le premier point *après* de
[TODO.md](TODO.md).

**9. « Et Parallax ? »** Le mod de terrain le plus installé. Ce qu'on sait : il déplace la géométrie dans
le shader et n'écrit aucun collider, donc la surface qui collisionne reste celle du PQS ; son scatter est
lu dans ses sources, pas mesuré avec le correctif (*Limits and solutions*, *Rocks, grass and trees*).
Les campagnes Parallax sont prévues ([TODO.md](TODO.md), après).

## Ce qu'on ne saura pas lui répondre

- **Un kraken à la demande** (objection 2). Aucune mesure ne le donnera : c'est la nature même du défaut.
- **La liste complète des mods touchés** : elle est ouverte, et l'issue le dit. C'est l'aide qu'on lui
  demande, pas une faiblesse à cacher.
