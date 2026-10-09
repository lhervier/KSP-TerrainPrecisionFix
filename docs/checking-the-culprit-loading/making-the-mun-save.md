# Making the save on the Mun

Part of [Terrain Precision Fix](../../README.md), for [the runway protocol](the-runway-protocol.md): how
[`runway-mun-kk.sfs`](../../diag/checking-the-culprit-loading/runway-mun-kk.sfs) was made, two
identical craft on the Mun, one on a runway placed by
[Kerbal Konstructs](https://github.com/KSP-RO/Kerbal-Konstructs) 1.12.3 and one on the ground beside
it. You only need it to make your own; the published save and the two files of the runway are enough
to play the protocol.

The captures were taken in a game set to French.

1. **Launch the craft from the Spaceplane Hangar** and turn SAS on, so that it keeps upright when it is
   moved.

   ![The craft on the runway of the KSC](../../imgs/checking-the-culprit-loading/making-the-mun-save/000-launch.png)

   ![SAS turned on](../../imgs/checking-the-culprit-loading/making-the-mun-save/010-SAS.png)

2. **Move it to the Mun** with `Alt+F12 → Cheats → Set Position`: the Mun, latitude −0.23, longitude
   −75, pitch 90, and tick the two boxes that let a middle click set the position and skip the safety
   checks. Once it has landed, turn SAS off.

   ![Set Position, to the Mun](../../imgs/checking-the-culprit-loading/making-the-mun-save/030-cheat.png)

   ![Landed on the Mun, SAS turned off](../../imgs/checking-the-culprit-loading/making-the-mun-save/040-stabilize.png)

3. **Create a group** where the craft is: open the statics editor of Kerbal Konstructs with `Ctrl+K`,
   then *Edit Groups*, *Spawn new Group*, and *Save&Close* in the *Group Editor* that opens. Make it the
   active group with *Set Active Group*: pick it in the list and press *OK*. That list may open at the
   bottom edge of the screen; drag it up by its title.

   ![Spawn new Group, then Save&Close](../../imgs/checking-the-culprit-loading/making-the-mun-save/050-spawn-group.png)

   ![Set Active Group](../../imgs/checking-the-culprit-loading/making-the-mun-save/060-set-active-group.png)

4. **Place the runway**: under *Spawn New*, click the title *KSC Runway lv 3* — the title, not the mesh
   name on the right, which opens the model's own settings. The runway appears on the craft: drag one of
   its arrows until the craft stands on the ground beside it, then *Save&Close*.

   ![Spawn New, KSC Runway lv 3](../../imgs/checking-the-culprit-loading/making-the-mun-save/070-spawn-runway.png)

   ![The runway moved away from the craft](../../imgs/checking-the-culprit-loading/making-the-mun-save/080-move-away-from-capsule.png)

   ![Save&Close](../../imgs/checking-the-culprit-loading/making-the-mun-save/090-save-moon-runway.png)

5. **Launch the same craft a second time**: go back to the Space Center from the pause menu, launch it
   from the Spaceplane Hangar again, SAS on, and move it to the Mun with the same latitude and
   longitude. It lands a few kilometres from the runway.

   ![Back to the Space Center](../../imgs/checking-the-culprit-loading/making-the-mun-save/100-back-to-ksc.png)

   ![The second craft on the runway of the KSC](../../imgs/checking-the-culprit-loading/making-the-mun-save/110-launch-identical-capsule.png)

   ![Set Position again](../../imgs/checking-the-culprit-loading/making-the-mun-save/120-cheat-again.png)

   ![On the Mun, the runway 2.7 km away](../../imgs/checking-the-culprit-loading/making-the-mun-save/130-on-the-moon-again.png)

6. **Put it on the runway**: open *Set Position* again and middle-click the runway: latitude and
   longitude now read the spot you clicked. Press *Set Position*.

   ![Middle-click on the runway](../../imgs/checking-the-culprit-loading/making-the-mun-save/140-middle-click.png)

   ![The position of the runway, read by the middle click](../../imgs/checking-the-culprit-loading/making-the-mun-save/150-middle-click-on-runway.png)

7. **Save once**, from the pause menu, with SAS off.

   ![Save](../../imgs/checking-the-culprit-loading/making-the-mun-save/160-save.png)
