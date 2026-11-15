# Ikonok és licencek

## Forrás és változat

A funkcióikonok kizárólag az [Oxylite](https://github.com/mx-2/oxylite-icon-theme)
készletből származnak. Rögzített commit:
`59f49b202ad2db33c50dc8d67b7961f47786bc87` (beszerzés: 2026-11-15).
Az eredeti fájlneveket és a `licenses.yml` tényleges bejegyzéseit ellenőriztük.
A futó alkalmazás nem tölt le ikonokat.

| Felhasználás | Eredeti SVG az upstreamben | Beépített PNG | `licenses.yml` licence | Megadott előzmény |
|---|---|---|---|---|
| Egy hely, mappaválasztás | `places/folder.svg` | `folder.png` | `LGPL-3.0` | GNOME Adwaita `folders.svg`, LGPL-3.0 |
| Két hely | `actions/kr_comparedirs.svg` | `kr_comparedirs.png` | `LGPL-3.0` | Adwaita `folders.svg`, LGPL-3.0; Oxygen `text-plain.svgz`, LGPL-3.0-or-later |
| Kilépés | `actions/application-exit.svg` | `application-exit.png` | `LGPL-3.0-or-later` | Oxygen `application-exit.svgz`, LGPL-3.0-or-later |
| Vissza | `actions/go-previous.svg` | `go-previous.png` | `LGPL-3.0-or-later` | Oxygen `go-down.svgz`, LGPL-3.0-or-later |
| Tovább | `actions/go-next.svg` | `go-next.png` | `LGPL-3.0-or-later` | Oxygen `go-down.svgz`, LGPL-3.0-or-later |

A `LGPL-3.0` jelölést változatlanul idézzük: nem értelmezzük automatikusan
„vagy újabb” engedélyként. A 3. verzió szerinti teljesítés a közös alap.
A `kr_comparedirs.svg` sárga mappát, két dokumentumot és zöld/piros jelet ábrázol;
alkalmas a helyek összehasonlításának jelölésére, de önmagában nem egyértelmű.
Ezért mindig a keresési mód teljes magyar neve és leírása mellett jelenik meg.

## Megőrzött forrás és tájékoztatók

A `licenses/Oxylite/` tartalmazza:

- a teljes, változatlan upstream `licenses.yml` és `UPSTREAM-README.md` fájlt;
- az öt eredeti SVG-t a `sources/actions/` és `sources/places/` alatt;
- a GPLv3 és LGPLv3 teljes szövegét (SPDX license-list-data, `GPL-3.0-only.txt`, `LGPL-3.0-only.txt`);
- az Oxygen Team `AUTHORS` fájlját a KDE/oxygen-icons `master` ágáról;
- a GNOME/adwaita-icon-theme `master` ágának `AUTHORS` és `COPYING` fájlját,
  kiegészítő szerzői tájékoztatóként. Ezek nem a történeti `gnome-3-13`
  forrásváltozat szerzőinek teljes, ikonszintű bizonyítékai.

Köszönet és forrásmegjelölés: Oxylite projekt és közreműködői (mx-2/oxylite-icon-theme),
The Oxygen Team (https://github.com/KDE/oxygen-icons), GNOME Project
(https://www.gnome.org, https://github.com/GNOME/adwaita-icon-theme).
A kiválasztott SVG-k nem tartalmaznak további szerzői jogi metaadatot;
a rendelkezésre álló upstream értesítéseket és szerzőlistákat megőriztük,
nem állítunk ikonszinten pontosabb szerzőséget.

Átalakítás: Inkscape 1.4, átlátszó hátterű, 256×256 PNG-export,
2026-11-15. Az SVG-k rajzát nem módosítottuk. A PNG-k külön beépített WPF
Resource-ok; a WPF magas minőségű lekicsinyítést használ a nagy és kis nézetekhez.
A `docs/icon-catalog.png` ezekből a tényleges PNG-kből készül, a saját
alkalmazásikonnal együtt. Nem képernyőkép az alkalmazás futásáról.

Újragenerálás a tároló gyökeréből (fejlesztői eszközök, nem futásidejű függőségek):

```bash
for name in application-exit go-previous go-next kr_comparedirs; do
  inkscape "licenses/Oxylite/sources/actions/$name.svg" --export-type=png \
    --export-filename="DupliScope/Assets/Icons/Oxylite/$name.png" -w 256
done
inkscape licenses/Oxylite/sources/places/folder.svg --export-type=png \
  --export-filename=DupliScope/Assets/Icons/Oxylite/folder.png -w 256
python tools/build-icon-assets.py
```

## Saját DupliScope-ikon

`Assets/Icons/App/dupliscope-source.png`: ehhez a projekthez generált saját
alkalmazásgrafika. Két egymásra helyezett, kék szegélyű, behajtott sarkú fehér
lap, finom térhatás és árnyék. Nem másik funkcióikon-készletből származik,
és nem Oxylite-ikon átrajzolása. A generálás nem használt referenciaképet.
Az Oxylite AI-mentes eredetéről szóló upstream állítás erre a külön grafikára
nem vonatkozik. A változatlan generált forrás megmarad; újragenerálás nem kell
a fordításhoz. Az alkalmazásgrafika külön harmadik félre vonatkozó licencet
nem ad hozzá; a teljes DupliScope kiadási licence még nincs kijelölve.

Az ICO és a 256-os PNG a `tools/build-icon-assets.py` kimenete. Az ICO méretei:
16, 24, 32, 48, 64, 128, 256 pixel. A projekt `ApplicationIcon` tulajdonsága
az EXE ikonját állítja be, a főablak `Icon` tulajdonsága pedig ugyanazt az ICO-t
használja a címsorhoz, tálcához és Alt+Tabhoz. A Windows apphost EXE natív
RT_ICON és RT_GROUP_ICON erőforrásait a `tools/verify-assets.py` ellenőrzi;
a Windows shell tényleges megjelenése külön kézi ellenőrzést igényel.

## Terjesztési kötelezettségek és nyitott kérdés

Az upstream a teljes témát GPLv3-ként jelöli, miközben a kiválasztott fájlok
LGPLv3 licenceket kaptak. A teljes témát nem terjesztjük, csak a felsorolt ikonokat.
Ez **nem jelenti a teljes alkalmazás terjesztési licencének automatikus rendezését**.
Kiadás előtt tisztázandó az egyedi licencjelölések és a témaszintű GPLv3
viszonya az upstreammel vagy jogi felülvizsgálattal, és ki kell választani a
DupliScope ezekkel összeegyeztethető kiadási licencét. Nem feltételezünk
engedélyt zárt forrású terjesztésre.

A licenc hatálya alá eső anyagok terjesztésekor legalább:

1. Meg kell őrizni a szerzői/licencértesítéseket, a GPLv3/LGPLv3 szövegeket,
   és jelezni kell a változtatásokat (itt: SVG→PNG export).
2. A megfelelő, szerkeszthető forrást és az előállítás módját a licenc szerinti
   módon elérhetővé kell tenni. Itt az eredeti SVG-k és az exportparancsok
   a tárolóban vannak; az SVG-k és licencek a build/publish kimenetbe is bekerülnek.
3. Az LGPL anyagok módosítására, illetve adott esetben a beépített példányok
   cseréjére és a módosítások hibakereséséhez szükséges visszafejtésre vonatkozó
   jogokat nem szabad a kiadási feltételekkel elvonni. A beágyazás megfelelőségét
   a tényleges terjesztési konstrukcióval együtt kell felülvizsgálni.
4. Ha a GPL a terjesztett kombinációra is alkalmazandó, annak megfelelő
   forrását és jogosultságait is biztosítani kell; a konkrét hatókör nyitott.

A mellékelt értesítések technikai megőrzése ellenőrzött; a kereskedelmi vagy
nyilvános kiadás teljes jogi megfelelősége még nincs lezárva.
