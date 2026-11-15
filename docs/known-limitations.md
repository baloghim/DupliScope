# Ismert korlátok és következő ellenőrzések

## A 0.1-es verzió határai

- Nincs fájlbejárás, duplikátumkeresés, SHA-256, eredménylista, automatikus
  kijelölés, törlés, áthelyezés vagy karantén. A keresésindító gomb letiltott.
- A fájltípusszűrés és kizárt mappák kapcsolójának állapota már megőrződik,
  de minták, kiterjesztések és kizárási listák szerkesztése még nincs.
- A kapcsolók és útvonalak csak egy futáson belül élnek. Újraindításkor
  az alapértékek térnek vissza. A két mód közös első útvonalat használ.
- Nincs útvonal-elérhetőségi vagy azonossági validáció a kiválasztás után.
  Két azonos/egymásba ágyazott hely is választható. Ezek kezelése a későbbi
  keresési terv feladata; az előkészítés nem olvassa a fájlokat.
- A hosszú/Unicode útvonalak veszteség nélkül tárolódnak, a manifest long-path-aware.
  Ez nem bizonyítja minden Windows-szolgáltató, UNC-megosztás, házirend vagy
  fájlrendszer jövőbeli műveleteinek hosszúútvonal-kompatibilitását.
- A korábban jóváhagyott vizuális terv nem volt a tárolóban/csatolmányban.
  A felület a szöveges arculati leírást követi; képpontos egyezést nem ellenőriztünk.
- A saját alkalmazásikon generált grafika, az Oxylite-ikonok eredeti forrásból
  exportált képek. A teljes alkalmazás kiadási licence és az upstream téma-/
  ikonszintű licencviszony tisztázása nyitott: [részletek](icon-manifest.md).

## Windowsos kézi ellenőrzés – még nem történt meg

A fejlesztési környezet Linux, a WPF és a natív Windows-dialógus itt nem futtatható.
A keresztfordítás, a ViewModel-próbák és a bináris erőforrás-ellenőrzés sikeres;
ezek nem helyettesítik az alábbi Windows 10/11 próbákat:

- [ ] Debug és Release indítása .NET 8 Windows Desktop Runtime-mal.
- [ ] A kezdőképernyő minden felirata, két kártyája és ikonja helyesen jelenik meg;
      alapból az első mód és az almappák jelöltek.
- [ ] Egész kártya kattintása, Tab, nyilak, Szóköz, fókuszkeret és Alt-gyorsbillentyűk;
      egyszerre csak egy mód választható. Narrátorral a vezérlőnevek ellenőrzése.
- [ ] Tovább mindkét módból, megfelelő számú útvonalmező és tallózógomb;
      Vissza/Tovább több körön át megőrzi a módot, kapcsolókat és helyeket.
- [ ] Natív mappaválasztás: meghajtógyökér, magyar/Unicode név, hosszú útvonal,
      UNC-hely; megszakítás megőrzi a korábbi értéket. Dialógus a főablakhoz kötött.
- [ ] 100%, 125%, 150%, 200% méretezés, monitorok közötti áthúzás, minimum méret,
      maximalizálás és görgetés; minden művelet elérhető és a hosszú út másolható.
- [ ] Internet nélkül, a build teljes könyvtárából indítva minden ikon megjelenik.
- [ ] EXE ikonja Intézőben, címsor, tálca, Alt+Tab, kis és nagy ikonméret;
      a Windows ikoncache esetleges régi tartalmát külön kell megkülönböztetni.
- [ ] Kilépés gomb, ablak X és Alt+F4 valóban leállítja a folyamatot;
      a keresésindító gomb mindkét módban letiltott marad.

A kipipálatlan tételek nem igazolt hibák: Windowsos futtatást igénylő, nyitott
ellenőrzések. A jelen változat semmilyen Windowsos futási tesztet nem állít sikeresnek.
