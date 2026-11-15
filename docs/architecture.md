# Architektúra

A meglévő .NET 8 WPF alkalmazás és `DupliScope.sln` maradt a belépési pont.
Nincs új alkalmazásprojekt és nincs külső MVVM- vagy ikonkönyvtár.

```text
DupliScope/
  App.xaml                       erőforrás-szótár, indulás, leállási mód
  MainWindow.xaml(.cs)            natív ablakkeret, közös fejléc/lábléc, összeállítás
  Views/                         WelcomeView és LocationsView
  ViewModels/                    MainViewModel, ObservableObject, RelayCommand
  Models/                        SearchMode, megváltoztathatatlan SearchOptions
  Services/                      mappaválasztás és alkalmazásleállítás interfészei,
                                 Windows/WPF implementációk
  Resources/Styles.xaml          színek, kártyák, gombok, fókuszjelzés, útvonalmezők
  Assets/Icons/Oxylite/           az öt eredeti ikon PNG-változata
  Assets/Icons/App/               saját alkalmazásikon forrása, PNG, többméretű ICO
  app.manifest                   jogosultság, Windows-kompatibilitás, DPI, hosszú utak
tests/DupliScope.Checks/          platformfüggetlen, csomagmentes ellenőrzőprogram
tools/                          ikon-előállítás és EXE/erőforrás-ellenőrzés
docs/                           dokumentáció és az ikonok katalógusa
licenses/Oxylite/                eredeti SVG-k, licencjegyzék és értesítések
```

## Adat- és eseményfolyam

A `MainWindow` code-behindja létrehozza a ViewModelt és a két konkrét
szolgáltatást, és az induló ablakméretet a rendelkezésre álló asztalterülethez
igazítja. A nézetek code-behindja csak `InitializeComponent()` hívást tartalmaz.
A két nézet ugyanazt a `MainViewModel` példányt örökli DataContextként;
a navigáció az `IsWelcomeScreen`/`IsLocationScreen` láthatóságot váltja.
Nem nyílik második főablak, nem épül fel külön navigációs keretrendszer.

A kártyák egy rádiógombcsoport tagjai. A ViewModelben egyetlen `SearchMode`
érték az állapot forrása; az `IsSingleLocation`/`IsTwoLocations` adapterek
kölcsönösen kizáró választást tesznek lehetővé. A ViewModel értesíti a kötéseket
és a parancsok `CanExecuteChanged` eseményét. A teljes kártya kattintható;
a fókuszkeret és a szokásos rádiógomb-billentyűkezelés megmarad.

A kapcsolók és útvonalak a MainViewModelben maradnak, ezért a Vissza/Tovább
nem állítja őket alaphelyzetbe. A két mód közös első útvonalat használ.
A második útvonal módváltáskor megmarad a ViewModelben, de egyhelyes módban
nem kerül a `CreateSearchOptions()` pillanatképébe. A beállításokat csak
az adott folyamat élettartamáig őrizzük, lemezre nem mentjük.

Az `IFolderPicker` az operációs rendszer határa. A `WindowsFolderPicker` a
.NET 8 `Microsoft.Win32.OpenFolderDialog` vezérlőjét használja, a főablakhoz
rendelt modális dialógusként. Egy mappa vagy meghajtógyökér választható;
megszakításkor az előző útvonal marad. A várható hozzáférési/dialógushibákról
magyar üzenet jelenik meg. Az útvonalakat nem daraboljuk, nem korlátozzuk 260
karakterre, nem alakítjuk kisbetűssé és nem járjuk be a fájlrendszert.

Az `IApplicationLifetime` WPF-implementációja `Application.Shutdown()`-t hív.
Az ablak szokásos bezárása is leállítja a folyamatot (`OnMainWindowClose`).
Az interfészek miatt mindkét viselkedés grafikus környezet nélkül tesztelhető.

## Későbbi aszinkron keresés

A `SearchOptions` változatlan pillanatkép átadható egy későbbi keresőszolgáltatásnak.
Nincs a ViewModelbe vagy a nézetbe tett keresési/fájlrendszer-logika. Az új keresést
lemondható, aszinkron szolgáltatásként kell bevezetni; a hosszú I/O és hash-számítás
nem futhat a UI-szálon. A jelenlegi szinkron parancsok kizárólag rövid
állapotváltást, a natív dialógus megnyitását és leállítást végeznek.
Nincs előre legyártott, üres keresőszolgáltatás vagy felesleges DI-konténer.

## Megjelenítés és erőforrások

Natív Windows-ablakkeret, legfeljebb 1040×820 logikai képpontos induló méret,
760×480 minimum (ennél kisebb asztalterületen a minimum is ahhoz igazodik),
képernyőközépre igazítás. A fejléc/lábléc rögzített, a középső tartalom függőlegesen
görgethető. A két kártya egyenlő szélességű, a tartalom 1120 DIP-nél nem nyúlik
tovább. A hosszú útvonalak vízszintesen görgethető, másolható mezőben jelennek meg.
A layout DIP-alapú, `UseLayoutRounding` és magas minőségű bitmap-méretezés aktív.
A manifest per-monitor V2 DPI- és hosszúútvonal-támogatást kér, emelt jogosultságot nem.

Az ikonok fordításkor WPF `Resource`-ként ágyazódnak be; a nézetek relatív vagy
assembly-relative pack URI-t használnak. A saját ICO `ApplicationIcon` és
`Window.Icon` is. Az eredeti ikonforrások és licencek a build/publish mappába
másolódnak, hogy a binárisok mellett megmaradjanak a tájékoztatók.
