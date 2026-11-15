# DupliScope 0.2

Magyar nyelvű, .NET 8 / WPF Windows-alkalmazás duplikált fájlok későbbi
kereséséhez és kezeléséhez. A 0.2-es változat a kezdőképernyőt, a keresési
beállításokat, a helyválasztást és az egy ablakon belüli navigációt valósítja meg.
**Fájlokat még nem keres, nem hasonlít össze és nem módosít.**

A 0.2-es verzió maximalizált, natív Windows-ablakban indul, amely normál méretre
visszaállítható. Az elsődleges kék gombok csak szöveget tartalmaznak; a második
hely mezőfelirata „Elérési út”. A fejléc verziószáma a projektfájl `Version`
értékéből származik, így a következő verzióváltásnak egyetlen forrása van.

## Használat

1. Válassz egy helyen belüli vagy két hely közötti keresést. A teljes kártya
   kattintható; billentyűzettel Tab, nyílbillentyűk és Szóköz használható.
2. Állítsd be az almappák vizsgálatát és a két előkészített szűrési kapcsolót.
   Alapértékek: almappák be, a másik kettő ki. A szűrők részletes megadása
   még nem része ennek a verziónak.
3. A **Tovább** gomb után válassz egy vagy két mappát/meghajtógyökeret a natív
   Windows-mappaválasztóval. Az útvonal kijelölhető és másolható.
4. A **Vissza** megőrzi a módot, a kapcsolókat és mindkét útvonalat az adott
   alkalmazásfutáson belül. A keresésindító gomb szándékosan letiltott.
5. A **Kilépés** vagy a szokásos ablakbezárás szabályosan befejezi az alkalmazást.

## Fordítás és indítás Windowson

Windows 10/11, .NET 8 SDK, illetve Visual Studio 2022 17.8+ a
„.NET asztali fejlesztés” munkaterheléssel. Külső NuGet-csomagot nem használunk.

Nyisd meg a meglévő `DupliScope.sln` fájlt, válaszd a DupliScope projektet,
és indítsd F5-tel. Parancssorból a tároló gyökerében:

```powershell
dotnet build DupliScope.sln -c Debug
dotnet build DupliScope.sln -c Release
dotnet run --project DupliScope/DupliScope.csproj -c Debug
```

Futtatáshoz a célgépen .NET 8 **Windows Desktop Runtime** kell, ha nincs SDK.
A szokásos Windows-build az adott gép szerinti apphostot készíti. Ellenőrzött,
kifejezetten Windows x64-es, framework-dependent kimenet készítése:

```powershell
dotnet build DupliScope/DupliScope.csproj -c Release -r win-x64 --self-contained false
```

Kimenet: `DupliScope/bin/Release/net8.0-windows/win-x64/`. A futtatáshoz a
**teljes kimeneti könyvtár** kell, nem csak az EXE; a `licenses/` könyvtárat is
meg kell őrizni. Nyilvános kiadás előtt a [licencnyilvántartás](docs/icon-manifest.md)
nyitott kérdéseit rendezni kell.

## Ellenőrzés Linuxon / a felhőkörnyezetben

A WPF Windows-függő; Linuxon a felület és a natív mappaválasztó nem fut.
A .NET 8 SDK-val keresztfordítás és a UI-tól független logika ellenőrzése lehetséges.
A felhőkörnyezetben a telepített SDK aktiválása:

```bash
export DOTNET_ROOT=/workspace/.dotnet
export PATH="$DOTNET_ROOT:$PATH"
export DOTNET_CLI_HOME=/workspace/.dupliscope-setup
export NUGET_PACKAGES=/workspace/.nuget/packages
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
cd /workspace/DupliScope
```

A Windows RID-t a **projekten**, ne a solutionön add meg:

```bash
dotnet build DupliScope/DupliScope.csproj -p:EnableWindowsTargeting=true -r win-x64 --self-contained false -c Debug
dotnet build DupliScope/DupliScope.csproj -p:EnableWindowsTargeting=true -r win-x64 --self-contained false -c Release
dotnet run --project tests/DupliScope.Checks -c Release -- \
  DupliScope/bin/Debug/net8.0-windows/win-x64/DupliScope.dll \
  DupliScope/bin/Release/net8.0-windows/win-x64/DupliScope.dll
python tools/verify-assets.py \
  DupliScope/bin/Debug/net8.0-windows/win-x64/DupliScope.exe \
  DupliScope/bin/Release/net8.0-windows/win-x64/DupliScope.exe
```

Ha a felhő sandboxa `NuGet-Migrations`/named mutex hibát okoz, a támogatott
jogosultságbővítési útvonalon kell futtatni a .NET-parancsot. Ne kapcsold ki a
TLS-, csomag- vagy aláírás-ellenőrzéseket.

A `tests/DupliScope.Checks` egy külső csomag nélküli, konzolos ellenőrzőprogram,
nem második WPF-alkalmazás. A meglévő modellek/ViewModel-forrásokkal fordul;
12 viselkedési próbát és a megadott buildenként egy erőforrásvizsgálatot futtat.
Nem `dotnet test` tesztadapter: a fenti **dotnet run** parancsot használd.
Az ikonellenőrző Python-szkript csak a standard könyvtárat igényli.

## Dokumentáció

- [Architektúra és projektstruktúra](docs/architecture.md)
- [Ikonok, licencek és terjesztés](docs/icon-manifest.md)
- [Tényleges ikonok összehasonlító képe](docs/icon-catalog.png)
- [Korlátok és Windowsos ellenőrzőlista](docs/known-limitations.md)
- [Az elvégzett ellenőrzések eredménye](docs/validation.md)

A fordítás a kész, tárolt PNG/ICO fájlokat használja, nincs internetes ikonbetöltés.
A grafikai állományok újraelőállításához Inkscape és Python/Pillow szükséges;
ezek az alkalmazásnak nem futásidejű függőségei.
