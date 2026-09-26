# Issue KSPCF — la relecture, anticipée

Ce fichier imagine **la réponse d'un mainteneur** à `KSPCF-issue.md` et `KSPCF-comment.md` dans leur état
du 2026-09-24 (repro de chargement, puis d'approche et de changement de vaisseau ; commentaire avec les
trois séries sous correctif), puis ce qu'on lui répondra. Ce qu'il **reste à faire** n'est pas ici :
c'est [TODO.md](TODO.md), et les cas à tester sont les chapitres TBD de
[Limits and solutions](docs/limits-and-solutions.md). Revoir ce fichier à chaque modification de
l'issue ou du commentaire.

⚠️ **La réponse ci-dessous est une fiction** écrite par Claude dans sa voix, à partir de ce qu'il a
réellement écrit : `kspmod\claude-notes\archives\kspcf-forum\issues\` (#9, #271, #296, #324), ses posts
du fil (pages 30 à 35) et la PR #435, résumés dans [kspcf.md](../claude-notes/kspcf.md). Ce qu'on y lit
de lui : **aucune formule de politesse**, il entre directement dans le sujet ; il **reproduit d'abord
lui-même** (« Doesn't reproduce on a KSP 1.12.5 install with KSPCF 1.37.2 », « I've verified this
reproduce in a stock install with just KSPCF installed ») ; il connaît le moteur à fond et reprend chaque
mot inexact (#9) ; il dit sans détour quand il ne sait pas (« I'm frankly a bit lost », #324) ; il ne
bouge que sur un cas reproductible (#9) ; il redoute les effets de bord « sneaky, very situational »
qu'aucun joueur ne rattachera au patch, et ceux qu'on ne pourra pas corriger depuis KSPCF (#296) ; sur
le PQS, il pense en surcoût d'Harmony sur les petites méthodes appelées en masse et en compatibilité avec
Kopernicus et les PQSMods des autres plugins (#271) ; il propose volontiers une voie moins risquée
(« IMO it would safer and ultimately less work to… ») ; il reste indulgent envers Squad (« they did quite
a decent job »). Anglais d'un non-natif, avec de petites fautes. Ne jamais la citer comme venant de lui.

**Qui, et quand.** D'abord le silence : #435 n'avait rien reçu en cinq jours. La première réponse peut
venir d'un autre mainteneur. Ne pas relancer avant une ou deux semaines.

## Sa réponse, telle qu'il pourrait l'écrire

> Reproduces on a stock 1.12.5 install with just KSPCF and your Diag 1, I get the same order of
> magnitude on Kerbin.
>
> Nothing surprising in the rounding itself: max level quads are parented to `LocalSpacePQStorage`, so
> their position at 600 km goes through a float, and the precision at that range is what it is. The
> part I didn't have in mind is that the frame is never the same twice, but it makes sense.
> `directRotAngle` is derived from UT, and `CelestialBody.PreciseUpdateQuadPositions` is called on every
> floating origin shift, so every load and every shift mean a new draw. I don't see anything wrong in
> the diagnosis.
>
> What I'm less sure about is how much this actually matters in practice. A few cm of penetration is
> something the physics usually handle fine, the gif is a light craft on an empty tank. What would
> really make a difference is a save where this reliably destroys something, typically a large base on
> legs, or anchored stuff. #214 is a good candidate, but as you say yourself it's a different story.
>
> On the patch :
>
> - You only fix max level quads, everything else parented to the sphere is still placed through the
>   float transform: lower level quads, `PQSCity`, scatter, and whatever mods parent there. So you are
>   now mixing two placements that differ by the same amount as the bug. Did you look at the seams
>   between a corrected quad and a stock neighbour one level down ? Terrain cracks are something users
>   do notice.
> - A prefix returning false on `BuildVertexSurfaceRelative` means nobody can patch that method after
>   you. It's also called for every vertex, and as I noted in #271, the harmony overhead on such small
>   methods can easily eat the gains. Your figures say otherwise, fine, but IMO a transpiler on the
>   `TransformPoint` / `InverseTransformPoint` calls would be much less invasive, and would keep the
>   stock method in place for everyone else.
> - Why sixteen steps? You know what correction to expect, a couple of float steps at that distance.
>   Anything above means the frame isn't what you think it is, and I would want that to be loud, not a
>   log line.
> - Principia drives the body rotations itself. Kopernicus you checked, but Principia is the one I would
>   worry about for anything reading `body.rotation`.
> - Existing saves: every landed vessel was saved on a stock draw, so the first load after install will
>   be a few cm off for all of them. Probably harmless, but that's exactly what gets reported as "the mod
>   broke my base".
>
> As for your two questions. I only had a quick look at the code, but the maths look right to me:
> doing the subtraction while still in double is the correct way to do it, and I don't see a case where
> it would give something worse than stock, as long as the frame you read is the one the quad is
> actually rendered in. That last part is the one I would check more carefully.
>
> For the side effects, I can't give you a complete list, nobody can. What I would look for first is
> anything that assumes the quad `Transform` and its mesh agree with the stock arithmetic: code that
> reads quad vertices or positions to place things (scatter, statics, ground construction), code that
> caches terrain heights across a floating origin shift, and anything that raycasts the terrain then
> compares with `pqsController` heights. The problem with this kind of change is that the side effects
> will be sneaky, situational, and reported months later without anyone connecting them to the patch.
> So the more of it you can turn into a reproducible check like your Diags, the better.
>
> I haven't looked at #435 in depth yet. If both end up installed together, someone will have to check
> they don't interact.

## Ce qu'on en tire, point par point

**1. « Ça compte vraiment ? Montre-moi une sauvegarde qui casse quelque chose. »** Toujours le point le plus faible
(#9 : *« Unless I find a reproducible case of that issue, I won't implement that more general fix »*).
On a un saut reproductible et des dispersions mesurées, pas une destruction à la demande. Réponse : le
défaut, c'est justement que le sol ne se reproduit pas, ce qui rend les krakens de chargement
irreproductibles ; #214 (l'ancre) en est l'exemple travaillé. **Nouveau depuis la campagne du
changement de vaisseau** : on peut dire *quand* le saut a lieu (le sol est en place à l'ouverture, le
vaisseau tenu à sa hauteur sauvegardée s'y pose au dépaquetage), et l'approche le montre sans aucun
rechargement. Ne jamais écrire que le correctif supprime les krakens. **Il existe maintenant une
repro visible, sous RSS** ([Rescaled systems: Real Solar System](docs/limits-and-solutions/rescaled-systems-real-solar-system.md)) :
sur la Lune, une capsule sur réservoir se renverse dès le premier chargement sans le correctif, et
reste debout avec. **Mais il faut d'abord couper la repose au sol que RSS force par défaut**
(`VesselGroundPositionEnhancer`), sinon il verra un vaisseau téléporté de 11 à 17 cm, pas renversé ;
et RSS 20.1.3.0 n'offre pas de réglage pour ça (DLL vide nommée `WorldStabilizer`). Il demandera
pourquoi il faudrait désactiver un composant de RSS pour voir le défaut : réponse, ce composant existe
*à cause* du défaut (« mostly prevent vessels clipping into the ground »), et il déplace la base d'un
bloc au lieu de la laisser où elle était. Et le correctif ne la contrarie pas : RSS tel que publié plus
le correctif, la repose tourne à chaque chargement sans jamais rien déplacer (0,395 mm). Sur Kerbin,
toujours pas de destruction à la demande.

**2. « Ta rotation en double n'est plus cohérente avec le `Transform` en float. »** **Nouvelle objection,
et elle est juste.** Tout ce qui reste enfant de la sphère (quads des niveaux inférieurs, `PQSCity`,
scatter) est placé par la matrice en float, les quads corrigés par la rotation en double : l'écart
entre les deux est du même ordre que le défaut. La page des limites le dit pour les rochers (*Rocks,
grass and trees* : écart élargi sur Kerbin) et pour le KSC (*not covered*), **pas pour les raccords
entre un quad corrigé et son voisin d'un niveau inférieur**, jamais regardés. À ajouter en chapitre
**Status: TBD** dans *Limits and solutions* (une capture au raccord, avec et sans le correctif) :
dans [TODO.md](TODO.md), **avant** l'issue.

**3. « Et Principia ? »** Chapitre *Principia* ajouté à *Limits and solutions* (TBD, lu dans le
source). Principia n'a ni patch Harmony ni code de terrain ; il écrit `body.rotation` en double et le
recopie dans `bodyTransform.rotation` en float, comme le stock, donc le correctif lit ce que Principia a
posé ; et il déplace les corps par `celestial.position`, que le setter stock transmet à
`PQS.PrecisePosition` → `PQ.FastUpdateSubQuadsPosition`, qui ajoute le déplacement **en double** à la
position posée par le correctif. Reste ouvert : si les quads sont replacés assez souvent pour suivre une
rotation que Principia calcule.

**4. « Les sauvegardes existantes : le premier chargement déplacera chaque base. »** Déjà dans
*Existing saves* (TBD), et **la campagne du changement de vaisseau le mesure maintenant** : sur une
sauvegarde faite sans le correctif, la capsule se pose à −31,8 mm, mais au même endroit à chaque fois.
Réponse : un seul écart, du même ordre qu'un tirage stock, puis plus rien. Que l'écart disparaisse
après une sauvegarde refaite avec le correctif n'est **pas mesuré** : dans [TODO.md](TODO.md), avant
l'issue.

**5. « Un préfixe qui renvoie `false`, sur une méthode appelée par vertex. »** Sa propre mise en garde de
#271 (le surcoût d'Harmony sur les petites méthodes). La mesure répond pour le coût (plus rapide que le
stock, *Performance*) ; pas pour l'exclusivité. Réponse : aucun mod de `kspmod-ext` (Kopernicus et Parallax compris) ne patche ces
méthodes, vérifié le 2026-09-24 ; et on accepte volontiers un transpiler, c'est son code de référence.

**6. « Pourquoi seize pas, et un refus devrait se voir. »** Le seuil est maintenant en pas de float à la
distance du quad (seize : 1 m sur Kerbin, 8 m sur la Terre de RSS). Il demandera d'où vient seize.
Réponse : quatre fois le plus grand arrondi mesuré (3,5 pas sur la Lune de RSS), et très loin d'un
mauvais repère, qui se trompe de kilomètres ; la plus grande correction sur les corps stock reste à
relever ([TODO.md](TODO.md), point 7). Pour la visibilité : chaque refus est journalisé une fois par
corps (un `Warning`) ; s'il le veut plus visible, c'est une ligne.

**7. « Le calcul me semble juste ; pour les effets de bord, voilà où je chercherais. »** C'est la
réponse aux deux questions que pose l'issue : le patch est-il correct, où peut-il casser quelque chose.
Le mod n'est pas publié et ne le sera pas avant longtemps (il reste beaucoup de travail) ; l'issue ne
demande **ni intégration ni conseil de publication**, et le dit (« help, not a merge »). Ses pistes
(code qui lit les vertices ou positions de quads pour placer des objets, hauteurs mises en cache à
travers un décalage d'origine, raycast comparé à `pqsController`) se versent dans *Limits and
solutions*, chacune en chapitre **Status: TBD** avec son test. Son « vérifie que le repère lu est
bien celui du rendu » : c'est ce que garde le seuil du point 6.

**8. « #435 et ton correctif touchent le même repère. »** Le commentaire y répond déjà (*About #435*),
et la mesure d'approche lui donne maintenant un chiffre : remettre le repère en place au chargement ne
suffit pas, le sol bouge ensuite sans rien charger (21,8 mm contre 0,011 mm). S'il demande si les deux
interagissent : non vérifié, le correctif ne lit que `body.rotation` et `body.position`, quel que soit
celui qui les écrit.

**9. « Deferred, Parallax. »** Les deux campagnes passent **avant** l'issue ([TODO.md](TODO.md)) :
il aura les résultats en la lisant. Parallax : la surface qui collisionne
reste celle du PQS, son scatter est lu dans ses sources, pas mesuré (*Rocks, grass and trees*).

**10. Ce qu'il ne dira pas, mais pensera peut-être : « encore une issue écrite par un LLM ».** chambm
a ouvert #435 puis #436, visiblement avec une IA ; aucun des deux n'a de réponse au 2026-09-24. Notre
aveu en tête ne suffira pas si le texte a l'air d'un débutant qui ne mesure pas ce que son LLM propose.
Ce qui nous en sépare : chaque chiffre se refait sans nous, avec des instruments inertes ; la liste de
ce qui n'est pas vérifié est publique ; on ne demande pas d'intégration. Et l'issue doit rester courte.
Regarder l'accueil de #435/#436 avant d'ouvrir.

## Ce qu'on ne saura pas lui répondre

- **Un kraken à la demande sur KSP stock** (point 1). Sur Kerbin, aucune mesure ne le donnera : c'est
  la nature même du défaut. Sous RSS, on l'a, au prix d'un composant de RSS à couper.
- **Les raccords entre niveaux** (point 2), tant que personne ne les a regardés.
- **La liste complète des mods touchés** : elle est ouverte, et l'issue le dit. C'est l'aide qu'on lui
  demande, pas une faiblesse à cacher.
