"""Rebuild the ICO and catalog from the actual checked-in assets; no network access.

Requires Python 3 + Pillow. The app does not require either at runtime.
Oxylite PNGs are rendered with Inkscape from licenses/Oxylite/sources (see docs).
"""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parents[1]
APP = ROOT / "DupliScope/Assets/Icons/App"
OXY = ROOT / "DupliScope/Assets/Icons/Oxylite"
SIZES = (16, 24, 32, 48, 64, 128, 256)

source = Image.open(APP / "dupliscope-source.png").convert("RGBA")
source.thumbnail((256, 256), Image.Resampling.LANCZOS)
icon = Image.new("RGBA", (256, 256))
icon.alpha_composite(source, ((256 - source.width) // 2, (256 - source.height) // 2))
icon.save(APP / "dupliscope.png")
icon.save(APP / "dupliscope.ico", sizes=[(n, n) for n in SIZES])

items = [
    (APP / "dupliscope.png", "DupliScope", "Saját alkalmazásikon"),
    (OXY / "folder.png", "folder.svg", "places / LGPL-3.0"),
    (OXY / "kr_comparedirs.png", "kr_comparedirs.svg", "actions / LGPL-3.0"),
    (OXY / "application-exit.png", "application-exit.svg", "actions / LGPL-3.0-or-later"),
    (OXY / "go-previous.png", "go-previous.svg", "actions / LGPL-3.0-or-later"),
    (OXY / "go-next.png", "go-next.svg", "actions / LGPL-3.0-or-later"),
]

def font(size):
    # Also works on a standard Windows Python installation.
    for name in ("DejaVuSans.ttf", "C:/Windows/Fonts/segoeui.ttf"):
        try:
            return ImageFont.truetype(name, size)
        except OSError:
            pass
    return ImageFont.load_default(size=size)

catalog = Image.new("RGB", (1260, 710), "#f5f7fb")
draw = ImageDraw.Draw(catalog)
draw.text((32, 22), "DupliScope 0.1 | Beépített ikonok", fill="#233247", font=font(28))
draw.text((32, 64), "A nagy és kis minták az alkalmazás tényleges PNG-erőforrásaiból készültek.", fill="#52647a", font=font(16))
for index, (path, title, license_text) in enumerate(items):
    x, y = 24 + index % 3 * 414, 110 + index // 3 * 292
    draw.rounded_rectangle((x, y, x + 390, y + 270), radius=12, fill="white", outline="#dce3ec")
    image = Image.open(path).convert("RGBA")
    large = image.resize((128, 128), Image.Resampling.LANCZOS)
    catalog.paste(large, (x + 90, y + 16), large)
    for offset, size in enumerate((16, 24, 32)):
        small = image.resize((size, size), Image.Resampling.LANCZOS)
        catalog.paste(small, (x + 248 + offset * 42, y + 80 - size // 2), small)
    draw.text((x + 20, y + 166), title, fill="#233247", font=font(20))
    draw.text((x + 20, y + 203), license_text, fill="#52647a", font=font(15))
    draw.text((x + 20, y + 235), "128 px  /  16, 24, 32 px", fill="#52647a", font=font(13))
catalog.save(ROOT / "docs/icon-catalog.png")
print("ICO sizes:", sorted(Image.open(APP / "dupliscope.ico").ico.sizes()))
