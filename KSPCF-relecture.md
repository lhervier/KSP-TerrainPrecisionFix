# Issue KSPCF — la relecture, anticipée

Ce fichier imagine **la réponse de Gotmachine** à `KSPCF-issue.md` et `KSPCF-comment.md` dans leur état
du 2026-09-24 (repro de chargement, puis d'approche et de changement de vaisseau ; commentaire avec les
trois séries sous correctif), puis ce qu'on lui répondra. Ce qu'il **reste à faire** n'est pas ici :
c'est [TODO.md](TODO.md), et les cas à tester sont les chapitres TBD de
[Limits and solutions](docs/limits-and-solutions.md). Revoir ce fichier à chaque modification de
l'issue ou du commentaire.

⚠️ **La réponse ci-dessous est une fiction** écrite par Claude dans sa voix, à partir de ce qu'il a
réellement écrit : `kspmod\claude-notes\archives\kspcf-forum\issues\` (#9, #271, #296, #324), la PR #435 et le fil du forum,
résumés dans [kspcf.md](../claude-notes/kspcf.md). Ce qu'on y lit de lui : précis, il connaît le moteur
à fond, il reprend chaque mot inexact (#9), il ne bouge que sur un cas reproductible, et il redoute les
effets de bord « sneaky, very situational » (#296) plus que les bugs francs. Ne jamais la citer comme
venant de lui.

**Qui, et quand.** D'abord le silence : #435 n'avait rien reçu en cinq jours. La première réponse peut
venir de steamroller ou de JonnyOThan. Ne pas relancer avant une ou deux semaines.

## Sa réponse, telle qu'il pourrait l'écrire

> Thanks for the detailed write-up, and for the instruments. Being able to check the claim on a stock
> install without running your patch is exactly what I want to see, and I appreciate the upfront note
> about the AI. I'll review the code the way I'd review anything else.
>
> On the diagnosis, I don't have much to argue with. Max level quads are parented to
> `LocalSpacePQStorage`, and anything else hanging off the sphere at 600 km is going to eat a
> `localPosition` rounding, that part is just arithmetic. Your approach and switching readings also
> match what I'd expect from the stock code: `FloatingOrigin` calls
> `CelestialBody.PreciseUpdateQuadPositions` on every shift, so every shift is a new draw, and a packed
> vessel is held at its saved altitude and only meets the collider when it's unpacked. So I'm fairly
> convinced the terrain moves. What I'm less convinced of is how much it matters: we're talking
> centimetres, and stock physics handles a few centimetres of penetration most of the time. The gif is
> nice, but what I'd really want is a save where this reliably destroys something, typically a large
> base on legs or anchors. Otherwise this stays in the "correct but marginal" category, and that
> category has a very different risk budget.
>
> Because the risk is not in the maths, it's in everything else that assumes where the terrain is:
>
> - You now place max level quads with a double rotation, while everything else parented to the body
>   still goes through the float transform: lower level quads, `PQSCity`, scatter, anything a mod
>   parents to the sphere. You say yourself the rock gap gets wider on Kerbin. Did you look at the
>   seams between a corrected quad and a stock neighbour one level down? I'd expect them to move, and
>   people do notice terrain cracks.
> - Principia drives the body rotations itself. Kopernicus you checked, good, but Principia is the one
>   I'd worry about for anything touching `body.rotation`.
> - Existing saves: every landed vessel in every save was recorded on a stock draw. After install, the
>   first load of each one will be a few cm off, one way or the other. Probably fine, but it's exactly
>   the kind of thing that gets reported as "KSPCF broke my base".
>
> On the implementation, a prefix returning false on `BuildVertexSurfaceRelative` means nobody else can
> patch that method after you, and it's called for every vertex. Your figures say it's not slower, fine,
> but a transpiler on the two `TransformPoint` / `InverseTransformPoint` calls would be much less
> invasive. The reflection stuff can go away, we compile against publicized assemblies. And the 1 m
> safeguard reads like it's there to hide cases you haven't understood yet; I'd rather know when it
> triggers than have it silently fall back.
>
> Given the `PQSOnlyStartOnce` history, which you noted yourself, I'm not going to ship something that
> moves the terrain for everyone without a lot more exposure. My suggestion: keep it as a standalone
> mod for a while, get it on CKAN, let the Parallax / Kopernicus / Deferred crowd run it and report. If
> it holds up, we can talk about bringing it in, most likely disabled by default at first.
>
> Unrelated but related: #435 and this touch the same frame from two sides. I haven't looked at the PR
> in depth yet, but if both end up in, we'll need to check they don't interact.

## Ce qu'on en tire, point par point

**1. « Ça compte vraiment ? Montre-moi un kraken reproductible. »** Toujours le point le plus faible
(#9 : *« Unless I find a reproducible case of that issue, I won't implement that more general fix »*).
On a un saut reproductible et des dispersions mesurées, pas une destruction à la demande. Réponse : le
défaut, c'est justement que le sol ne se reproduit pas, ce qui rend les krakens de chargement
irreproductibles ; #214 (l'ancre) en est l'exemple travaillé. **Nouveau depuis la campagne du
changement de vaisseau** : on peut dire *quand* le saut a lieu (le sol est en place à l'ouverture, le
vaisseau tenu à sa hauteur sauvegardée s'y pose au dépaquetage), et l'approche le montre sans aucun
rechargement. Ne jamais écrire que le correctif supprime les krakens. **Deux pistes de Lionel pour
un vrai kraken à la demande** : une capsule sur Gilly, où la poussée qui la sort du sol suffit à la
faire décoller dans une gravité si faible ; et la même, réservoir plein, sur la Terre de RSS (pas d'un
float : 0,5 m), où quelques chargements pourraient la faire exploser. Dans [TODO.md](TODO.md), avant
l'issue, RSS compris.

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

**5. « Un préfixe qui renvoie `false`, sur une méthode appelée par vertex. »** L'objection de #296 sous
une autre forme. La mesure répond pour le coût (plus rapide que le stock, *Performance*) ; pas pour
l'exclusivité. Réponse : aucun mod de `kspmod-ext` (Kopernicus et Parallax compris) ne patche ces
méthodes, vérifié le 2026-09-24 ; et on accepte volontiers un transpiler, c'est son code de référence.
La réflexion disparaît avec leurs assemblies publicisées.

**6. « Ton garde-fou d'un mètre cache des cas que tu ne comprends pas. »** Retournement probable du
garde-fou, qu'on présente comme une sécurité. Réponse : il n'est pas silencieux, chaque refus est
journalisé une fois par corps (un `Warning`) ; et le seuil passera en pas de float **avant** l'issue
([TODO.md](TODO.md)) : quelques lignes dans `IsRoundingCorrection`, plus une session en `Debug` pour
choisir le multiple.

**7. « Publie-le à part d'abord, on verra ensuite, désactivé par défaut. »** Le scénario le plus
probable, après `PQSOnlyStartOnce` — **et devenu notre stratégie** (Lionel, 2026-09-24) : le correctif
est un mod à nous, bâti sur KSPCF ; l'issue demande un avis et de l'aide, jamais une intégration, et on
le laisse la proposer s'il le veut. Le README et le commentaire le disent déjà (« I am not proposing it
for KSPCF »). Sa réponse devient alors un conseil, pas un refus : CKAN, s'il le recommande.

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

- **Un kraken à la demande** (point 1). Aucune mesure ne le donnera : c'est la nature même du défaut.
- **Les raccords entre niveaux** (point 2), tant que personne ne les a regardés.
- **La liste complète des mods touchés** : elle est ouverte, et l'issue le dit. C'est l'aide qu'on lui
  demande, pas une faiblesse à cacher.
