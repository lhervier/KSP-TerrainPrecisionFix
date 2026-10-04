On reprend l'automatisation des campagnes de mesure de mes mods KSP, pour les **performances** cette fois. Tout le travail d'automatisation de la conversation précédente est mergé sur `master`/`main` (ou master) de chaque dépôt, `kspmod` compris.

**Avant toute chose, lis** dans `claude-notes\` (en suivant la méthode du `CLAUDE.md` : sommaire, puis chapitres utiles) :
- `automatisation.md` : surtout « KSPProfiler », « Stock contre stock, pour les perfs », « La réserve de mods », « Démarrer et arrêter KSP », « L'orchestrateur et les scripts des protocoles » ;
- `mods/PQSBench.md` : « Protocole et pièges », « Lire les chiffres » ;
- `mods/TerrainPrecisionFix.md` : « Performances » ;
- `mods/RockPrecisionFix.md` : « Performances ».

**Où on en est :**
- **PQSBench** n'a plus de touches. Sa fenêtre a deux boutons, *Dump to KSP.log* et *Reset*, exposés en MCP (`pqsbench_dump`, `pqsbench_reset`, `pqsbench_show_window`). Alt+F6 masque sa fenêtre.
- **Le fork de KSPProfiler** (`kspmod\KSP-ExtMod-KSPProfiler`) expose `profiler_open`, `profiler_start_capture(frames)`, `profiler_stop_capture`, `profiler_export(directory, fileName)` et `profiler_state`. Ces outils appuient sur les boutons de sa fenêtre sans rien changer au comportement : le protocole doit rester rejouable à la main avec KSPProfiler 1.0.0. On le compile avec `outils\build-kspprofiler.py`.
- **KSP-MCPServer** offre `set_camera` (avec `aim_pitch`/`aim_heading`, le bouton du milieu de la souris, et `fov`), `set_ui`, `set_time` et `set_flight`. Il affiche un ScreenMessage par commande (réglage `screen_messages`).
- **`outils\`** contient `gamedata.py` (la réserve de mods en jonctions), `kspctl.py`, `campagne.py` et `nettoie-sfs.py`. Aucun protocole de perfs n'existe encore dans `campagne.py`.

**Ce que je veux :**
1. **Un script de campagne de perfs**, sur `ref-mune-5km.sfs` (orbite à 5 km au-dessus de la Mune), un **KSP neuf par run**, les configurations **entrelacées**. Attention à la caméra qui doit regarder le sol :
   - stock ;
   - StockQuadCache ;
   - TerrainPrecisionFix ;
   - pour RockPrecisionFix : TerrainPrecisionFix seul, contre TerrainPrecisionFix + RockPrecisionFix.

   Pour chaque configuration : un run PQSBench `calibrate`, et des runs **profiler sans PQSBench**. Le mode `counters` n'existe plus, donc la campagne de perfs de RockPrecisionFix est à refaire au profiler.

   Le script charge la sauvegarde, cadre la caméra **en prograde, la capsule entre deux gros cratères** (cadrage à reproduire et à me faire valider sur une capture), masque les fenêtres des mods, puis lance et arrête la mesure **à 30 s et 1 min 40 de temps de mission**. Il **ne fait aucun appel MCP pendant la fenêtre de mesure**. Il garde `KSP.log` et le CSV du profiler, puis quitte.
2. **Le test stock contre stock**, pour savoir si la présence du serveur MCP fausse le profiler : comparer les runs scriptés stock aux deux runs stock manuels du 2026-09-21 (`KSP-TerrainPrecisionFix\perfs\runs\profiler\`). Si l'écart dépasse leur dispersion, je referai les runs à la main, **sans le serveur installé**.

**Contraintes :**
- **Machine** : les perfs ne se comparent qu'entre runs d'une même machine. La référence a été faite sur le **PC fixe**, dont l'install `ksp-dev` est `D:\ksp-dev\`. **Sur le fixe, `KSPDIR` désigne l'install Steam, à laquelle on ne touche jamais.** Il faut d'abord y monter la réserve de mods (`gamedata.py`, depuis son cache CKAN) et y installer les builds.
- **Commits** : tu commites et tu pousses sur une branche `claude/perfs` de chaque dépôt touché.
- **Coups d'œil** : pour un emplacement, un cadrage ou une capture à valider, arrête-toi et attends ma réponse. Je regarde peut-être depuis mon téléphone.
- **Contexte** : délègue le gros travail (écriture de pages, publication) à des sous-agents.

Commence par me dire sur quel PC tu es, ce qu'il manque pour lancer la première campagne, et ton plan.
