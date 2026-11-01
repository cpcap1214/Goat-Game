#!/usr/bin/env python3
"""Build a self-contained storyboard preview; does not build or validate Unity."""
from pathlib import Path
import base64
import json
import re
ROOT = Path(__file__).resolve().parents[1]
assets = ROOT / 'Assets/_OneMoreTime'
frames = []
for i in range(12):
    source = (assets / f'Generated/Goat_{i:02}.asset').read_text()
    rect = re.search(r'm_Rect:.*?x: (\d+)\s+y: (\d+)', source, re.S)
    pivot = re.search(r'm_Pivot: \{x: ([\d.]+), y: ([\d.]+)', source)
    frames.append(dict(x=int(rect[1]), y=int(rect[2]), pivot=dict(x=float(pivot[1]), y=float(pivot[2]))))
html = (ROOT / 'Tools/opening-preview.html').read_text().replace('__FRAMES__', json.dumps(frames))
for key, path in [('__PRIZE__', 'Resources/Opening/GTA7Prize.png'), ('__GOAT__', 'Art/Goat/GoatAnimations.png')]:
    html = html.replace(key, 'data:image/png;base64,' + base64.b64encode((assets / path).read_bytes()).decode())
output = ROOT / 'Artifacts/OpeningPreview/index.html'
output.parent.mkdir(parents=True, exist_ok=True)
output.write_text(html)
print(output)
