Pourquoi est ce que ce fix est utile ? 

En l'état, je cherche des raisons pour les autres joueurs, et la réponse est facile : Une capsule qui fait un petit saut de quelques cm, c'est déjà géré par la physique, et ça va mettre le bordel à plein d'endroits... Ca ne vaut donc pas le coup. Rien que ça, c'est un argument pour ne pas le mettre dans KSPCF.

Je pourrais faire la démonstration d'explosions avec de grosses bases, sur Eve par exemple. Mais on va mélanger les problèmes de suspensions, de déformation de la base sous son propre poids, et de hauteur aléatoire du sol. C'est ingérable... Je compte m'attaquer à ce problème, mais plus tard.

Surtout que j'ai une solution pour contourner tous ces problèmes.

Quand je construis une grosse base, pour éviter les problèmes de déformation sous son propre poids, lui faisant ainsi épouser la forme du terrain, je travaille avec une ancre, et des ingénieurs qui posent des poutrelles et des ports de docking verticaux (qui regardent le ciel). Une grue assemble ensuite des éléments sur ces ports de docking. Des poutrelles horizontales pour soutenir la future struture, et des poutrelles verticales, qui se calent contre le sol, créant ainsi des pieds épousant la forme du relief.

Et quand la base se recharge, elle epouse la forme du sol et se stabilise sans risquer de se casser. Il va sans dire que je ne fais jamais reposer une base sur une suspension quelconque...

Mais il y a plein de bugs dans le mode construction en EVA (ex : le #214). Moi, celui qui me handicape, c'est qu'on ne peut pas poser une poutrelle exactement sur le sol. Soit on la met au dessus, et la structure va courber sous son propre poids, ce que je veux éviter, soit je la mets à l'intérieur du sol, et là, le comportement est encore plus bizarre : 
- Sans KSPCF, au rechargement, toute la base est remontée de la valeur dont la pièce a été enterrée (et ça fait voler l'ancre => #214)
- Avec KSPCF, au rechargement, l'ancre reste au sol, mais la structure est déformée pour que le "pied" soit en contact...

J'ai créé le mod EVACMGroundPlugin qui empêche de déplacer une pièce à l'intérieur du sol quand on est en mod construction en EVA. Et il marche à peu près. Je peux déplacer une poutrelle vers le bas jusqu'à ce qu'elle touche le sol. Ou presque... c'est grâce à lui que j'ai vu des problèmes apparaitre : 
- Parfois ma base se met quand même à léviter: Et oui, le sol ne ré-apparaissant pas au même niveau, si le pied était à l'intérieur, KSP (sans KSPCF) remonte la base pour que le pied touche le sol, surelevant en même temps tous les autres pieds, qui - eux - ne touchent plus...
- Et en rechargeant la base, parfois, en construction en EVA, je pouvais de nouveau descendre le pied car il n'était plus en contact avec le sol au rechargement : C'est le cas où le sol ré-apparait trop bas. LA poutrelle est au dessus du sol...

Et vu que de telles bases se construisent sur des temps très longs (il faut apporter chaque module avec une grue sur les ports de docking), il y a de nombreux rechargements, et donc de multiples changements de hauteur du sol.

Donc, avec cette conception, je réussi à construire des bases stock. Mais elles se mettent quand même à osciller parfois de manière qui les font se disloquer... Quand je le détecte, je re-charge, et ça passe (coup de chance, le sol revient à un niveau acceptable). Mais si ça se passe quand j'approche avec un vaisseau après une manoeuvre longue et pénible (comme atterrir sur Gilly où on ne pas faire de timewarp), c'est très pénible... Il faut revenir en arrière, et recommencer en espérant que ça ne pose pas problème.

Note : J'utilise beaucoup Parking Brake pour stabiliser mes bases. Vu qu'il annule toutes les vitesses angulaires et linéaires pendant la pose de la base au sol, ça fonctionne pas mal. Mais c'est une bidouille...

Autre cas où je suis moins sûr : Il y a un autre cas où la hauteur de sol aléatoire me pose problème : Les rovers du Eve. La gravité est importante (1.7x celle de Kerbin). Et si je les laisse sur des roues, elles se cassent à la recharge. Je suis obligé de bidouiller avec des charnieres qui levent les roues pour que le rover ne repose pas dessus au chargement (et ne pas oublier de les lever dès que j'arrête le rover)... Pas sûr que mon fix résolve ce problème. C'est plutôt lié à la compression des suspensions qui vont casser les roues. Mais peut être permettra t'il de détecter ce qui se passe avec les suspensions.

Voilà, tu connais tout de mon véritable but avec ce fix. Mais ça va être compliqué de l'expliquer...

Ce que je tiens à expliquer : Je ne cherche pas à être intégré dans KSPCF : Je veux l'avis du mainteneur, et surtout qu'il me signal des impacts potentiels auxquels je n'ai pas encore pensé. Il est le mieux placé pour ça de par la transversalité de son mod qui impact tous les autres. Il doit forcement les connaitre, ou au moins connaitre leurs mainteneurs.

Je me dis d'ailleurs que ce fait (le sol jamais reconstruit à la même hauteur) est tellement énorme que suis certain que c'est quelque chose qu'il connait, comme faisant partie de la conception normale du jeu, et que m'y attaquer est juste impossible... pour plein de raisons qu'il pourra me citer, et qui nous feront arrêter nos investigations. Mais si ce n'est pas le cas, j'aurais pu donner un sacré coup de main à la communauté (et à mois même, les basesq qui explosent étant une grande source de frustration)