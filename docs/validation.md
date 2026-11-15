# Ellenőrzési eredmények

## DupliScope 0.2 – felületi finomítások

Ellenőrzési környezet: Linux x64, .NET SDK 8.0.428, Windows x64 keresztfordítás.

- Debug és Release: sikeres, egyenként 0 hiba és 0 figyelmeztetés.
- Meglévő navigációs, állapotmegőrzési és erőforráspróbák: 14/14 sikeres.
- XAML statikus ellenőrzése: maximalizált indulás, szokásos visszaállítható
  Windows-ablakkeret; a két elsődleges gombban nincs kép; a Tovább korábbi
  tartalommagassága megmaradt; a második út mezőfelirata pontosan „Elérési út”.
- A fejléc a ViewModel `ApplicationVersion` értékéhez kötött, amely a
  projekt `Version` mezőjéből generált assembly-verzió első két tagját mutatja.
  Mindkét build generált assembly-verziója `0.2.0.0`, így a kijelzés `0.2`.
- Mind a nyolc meglévő ikonállomány byte-ra változatlan a 0.1-es commithoz képest.
  A Tovább gomb ikonhivatkozása eltűnt; a többi ikonhivatkozás változatlan.
- A 10 megmaradt helyi XAML-erőforráshivatkozás és mindkét EXE hétméretű
  natív ikonja, DPI/long-path manifestje és licenc-/SVG-másolatai ellenőrizve.
- `git diff --check`: sikeres.

A felület tényleges megjelenése, a tálca láthatósága, a maximalizált indulás és
a normál méretre visszaállítás **Windows alatt még nincs futtatással ellenőrizve**.
A szabványos WPF `WindowState="Maximized"` beállítást használjuk; nincs keret
nélküli vagy exkluzív teljes képernyős mód. A kézi Windows-ellenőrzőlista továbbra
is a [korlátok dokumentumában](known-limitations.md) található.

## DupliScope 0.1 – korábbi ellenőrzések

Dátum: 2026-11-15. Környezet: Debian 13 Linux x64, .NET SDK 8.0.428,
Windows Desktop reference pack, Windows x64 keresztfordítás.
A felületet **nem futtattuk Windows alatt**.
A reprodukálható parancsok a [README](../README.md) ellenőrzési részében találhatók.

| Ellenőrzés | Eredmény | Mit bizonyít / mit nem |
|---|---|---|
| Debug build, `-r win-x64 --self-contained false -p:EnableWindowsTargeting=true` | Sikeres, 0 hiba, 0 figyelmeztetés | C# és XAML fordítható; nem UI-futtatás |
| Release build, ugyanezzel a célzással | Sikeres, 0 hiba, 0 figyelmeztetés | Windows GUI apphost és WPF assembly elkészül |
| ViewModel és parancsok | 12/12 sikeres | Valódi, megosztott forrásokon, hamisított OS-szolgáltatásokkal |
| Lefordított WPF-erőforrások | 2/2 sikeres (Debug és Release) | Mindkét assemblyben megtalálható és nem üres mind a 11 várt BAML/PNG/ICO erőforrás; nem WPF futásidejű betöltés |
| XAML képfájl-/szótárhivatkozások | 11 helyi hivatkozás sikeres | Létező helyi fájlok, nincs hálózati képforrás; `Window.Icon` a saját ICO-ra mutat |
| ICO | 7/7 méret | 16, 24, 32, 48, 64, 128, 256 pixel |
| Debug és Release EXE natív erőforrásai | Mindkettő sikeres | Valódi x64 Windows GUI PE, RT_GROUP_ICON és hét RT_ICON; képtartalmuk byte-ra megegyezik az ICO megfelelő képével |
| EXE manifest | Mindkettő sikeres | Beágyazott `PerMonitorV2` és `longPathAware=true` |
| Licenc- és forrásmásolatok | Mindkét buildben sikeres | GPL/LGPL szöveg, jegyzék, upstream értesítések, szerzőlisták, mind az öt eredeti SVG jelen van |
| Ikonkatalógus | Elkészült, vizuálisan ellenőrizve | Ténylegesen használt hat PNG, nagy és 16/24/32-es mintákkal; nem alkalmazás-képernyőkép |
| Git whitespace / kimenetek | `git diff --check` sikeres; buildfájlok ignoráltak | A változtatási lista nem tartalmaz bin/obj vagy más generált fordítási kimenetet |

### A 12 viselkedési próba

1. Kezdő mód, a három kapcsoló alapértéke, parancsok kezdeti állapota.
2. Kizáró módválasztás és a rádiógombokhoz szükséges property értesítések.
3. Navigáció oda-vissza, mindhárom beállítás és a mód megőrzése.
4. Nem elérhető parancsok nem nyitnak mappaválasztót.
5. Meghajtógyökér kiválasztása, megszakítás, előző mappa továbbadása a dialógusnak.
6. 260 karakternél hosszabb, ékezetes/japán karakteres UNC út változatlan tárolása.
7. Mindkét út megőrzése navigáció és keresésimód-váltás után.
8. Mappaválasztási hiba jelzése, meglévő út megőrzése, sikeres újrapróbálás.
9. Változatlan keresési pillanatkép; inaktív második út kimarad a keresési opciókból.
10. Navigáció/módváltás parancsállapot-változási eseményei.
11. Kilépési parancs meghívja az alkalmazás-élettartam szolgáltatását.
12. A parancs nem hajt végre műveletet, ha a `CanExecute` hamis.

A Windowsos mappaválasztót, a WPF bindingok tényleges futását, a képernyőolvasót,
a méretezést és a shell ikonmegjelenítését ezek nem helyettesítik.
A még elvégzendő próbák a [korlátok és Windows-ellenőrzőlista](known-limitations.md)
fájlban szerepelnek. A licenc technikai megőrzése ellenőrzött, a teljes alkalmazás
nyilvános terjesztési megfelelősége továbbra is nyitott kérdés.
