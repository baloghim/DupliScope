"""Check offline XAML assets and the *native EXE* icon resources. Python stdlib only."""
import struct
import sys
from pathlib import Path
from xml.etree import ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
PROJECT = ROOT / "DupliScope"
ICO = PROJECT / "Assets/Icons/App/dupliscope.ico"
SIZES = {16, 24, 32, 48, 64, 128, 256}


def require(condition, message):
    if not condition:
        raise ValueError(message)


def ico_images():
    data = ICO.read_bytes()
    reserved, kind, count = struct.unpack_from("<HHH", data)
    require((reserved, kind, count) == (0, 1, 7), "ICO header / image count")
    result = {}
    for index in range(count):
        width, height, _, _, _, _, size, offset = struct.unpack_from("<BBBBHHII", data, 6 + index * 16)
        width, height = width or 256, height or 256
        require(width == height, "Non-square ICO frame")
        frame = data[offset:offset + size]
        require(len(frame) == size, "Truncated ICO frame")
        result[width] = frame
    require(set(result) == SIZES, "Required ICO sizes missing")
    return result


def pe_resources(path):
    data = path.read_bytes()
    require(data[:2] == b"MZ", f"Not a Windows executable: {path}")
    pe = struct.unpack_from("<I", data, 60)[0]
    require(data[pe:pe + 4] == b"PE\0\0", "PE header")
    machine, sections = struct.unpack_from("<HH", data, pe + 4)
    require(machine == 0x8664, "Expected Windows x64 PE")
    optional_size = struct.unpack_from("<H", data, pe + 20)[0]
    optional = pe + 24
    require(struct.unpack_from("<H", data, optional)[0] == 0x20B, "Expected PE32+")
    require(struct.unpack_from("<H", data, optional + 68)[0] == 2, "Expected Windows GUI subsystem")
    resource_rva, _ = struct.unpack_from("<II", data, optional + 112 + 16)
    section_table = optional + optional_size

    def offset(rva):
        for index in range(sections):
            start = section_table + index * 40
            virtual_size, virtual_address, raw_size, raw_offset = struct.unpack_from("<IIII", data, start + 8)
            if virtual_address <= rva < virtual_address + max(virtual_size, raw_size):
                return raw_offset + rva - virtual_address
        raise ValueError(f"Unmapped RVA: {rva}")

    base = offset(resource_rva)
    result = {}

    def visit(relative, ids=()):
        require(len(ids) < 4, "Resource directory recursion")
        directory = base + relative
        named, numbered = struct.unpack_from("<HH", data, directory + 12)
        for index in range(named + numbered):
            identifier, target = struct.unpack_from("<II", data, directory + 16 + index * 8)
            if identifier & 0x80000000:
                continue  # Named resources are unrelated to the numeric Windows icon/manifest types.
            key = ids + (identifier,)
            if target & 0x80000000:
                visit(target & 0x7FFFFFFF, key)
            else:
                rva, size = struct.unpack_from("<II", data, base + target)
                start = offset(rva)
                payload = data[start:start + size]
                require(len(payload) == size, "Truncated PE resource")
                result[key] = payload

    visit(0)
    return result


def check_exe(path, frames):
    resources = pe_resources(path)
    groups = [(key, value) for key, value in resources.items() if key[0] == 14]
    require(len(groups) == 1, "Expected one RT_GROUP_ICON")
    group_key, group = groups[0]
    require(struct.unpack_from("<HHH", group) == (0, 1, 7), "EXE group icon frame count")
    found = set()
    for index in range(7):
        width, height, _, _, _, _, size, identifier = struct.unpack_from("<BBBBHHIH", group, 6 + index * 14)
        width, height = width or 256, height or 256
        require(width == height, "Non-square EXE icon")
        payload = resources.get((3, identifier, group_key[2]))
        require(payload is not None and len(payload) == size, "Missing RT_ICON")
        require(payload == frames[width], f"EXE icon differs from source ICO at {width}px")
        found.add(width)
    require(found == SIZES, "EXE icon sizes")
    manifests = [value for key, value in resources.items() if key[0] == 24]
    require(len(manifests) == 1, "Missing executable manifest")
    manifest = ET.fromstring(manifests[0].rstrip(b"\0"))
    text = {element.tag.split("}")[-1]: element.text for element in manifest.iter()}
    require(text.get("longPathAware") == "true", "Long path awareness missing from EXE")
    require(text.get("dpiAwareness") == "PerMonitorV2", "Per-monitor DPI awareness missing from EXE")
    output = path.parent / "licenses"
    for relative in ("icon-manifest.md", "Oxylite/GPL-3.0.txt", "Oxylite/LGPL-3.0.txt",
                     "Oxylite/licenses.yml", "Oxylite/OXYGEN-AUTHORS.txt", "Oxylite/ADWAITA-AUTHORS.txt",
                     "Oxylite/ADWAITA-COPYING.txt", "Oxylite/UPSTREAM-README.md"):
        require((output / relative).is_file(), f"Missing distributed notice: {relative}")
    for original in (ROOT / "licenses/Oxylite/sources").rglob("*.svg"):
        copied = output / "Oxylite/sources" / original.relative_to(ROOT / "licenses/Oxylite/sources")
        require(copied.read_bytes() == original.read_bytes(), "Missing/modified distributed icon source")
    print(f"PASS {path.relative_to(ROOT)}: native GUI EXE, 7 matching icon frames, DPI/long-path manifest, notices and SVG sources")


def main():
    require(len(sys.argv) > 1, "Pass the freshly built Debug and Release Windows EXE paths")
    frames = ico_images()
    references = 0
    for xaml in PROJECT.rglob("*.xaml"):
        if "obj" in xaml.parts or "bin" in xaml.parts:
            continue
        tree = ET.parse(xaml)
        for element in tree.iter():
            source = element.get("Source")
            if source is None or element.tag.split("}")[-1] not in ("Image", "ResourceDictionary"):
                continue
            require("://" not in source and not source.startswith("{"), f"Non-local source: {source}")
            target = PROJECT / source.split(";component/", 1)[1] if ";component/" in source else xaml.parent / source
            require(target.is_file(), f"Missing XAML resource: {target}")
            references += 1
        if xaml.name == "MainWindow.xaml":
            require(tree.getroot().get("Icon") == "Assets/Icons/App/dupliscope.ico", "Window ICO reference")
    require(references >= 10, "Unexpectedly few XAML resources inspected")
    print(f"PASS {references} offline XAML resource references; ICO includes all 7 required sizes")
    for argument in sys.argv[1:]:
        check_exe(Path(argument).resolve(), frames)


if __name__ == "__main__":
    try:
        main()
    except (ValueError, OSError, KeyError, struct.error, ET.ParseError) as error:
        print(f"FAIL {error}", file=sys.stderr)
        sys.exit(1)
