#!/usr/bin/env python3
"""Project-local commands. Uses the existing Editor bridge when the project is open."""
import argparse,os,pathlib,subprocess,time,sys,zipfile,hashlib
ROOT=pathlib.Path(__file__).resolve().parents[1]
EDITOR=pathlib.Path(os.environ.get('UNITY_EDITOR','/Applications/Unity/Hub/Editor/6000.6.0f1/Unity.app/Contents/MacOS/Unity'))
p=argparse.ArgumentParser();p.add_argument('command',choices=['audit','challenge-layouts','generate','validate','test','build-web','package-web','open-playground','open-gallery','play','stop']);p.add_argument('--mode',choices=['edit','play'],default='play');p.add_argument('--phase',type=int);a=p.parse_args()
os.chdir(ROOT);logs=ROOT/'Artifacts/Logs';logs.mkdir(parents=True,exist_ok=True)
if a.command=='audit':
 print('Root:',ROOT,'\nEditor:',EDITOR,'\nExists:',EDITOR.is_file(),'\nWeb:',(EDITOR.parents[3]/'PlaybackEngines/WebGLSupport').is_dir());sys.exit(0 if EDITOR.is_file() else 1)
if a.command=='package-web':
 web=ROOT/'Artifacts/Web'
 if not (web/'index.html').is_file() or not list(web.rglob('*.wasm')):sys.exit('No real Web build')
 notices="One More Time - third-party notices\n\nGoat animation, dialogue portrait, title artwork, magic tower ending background and magical relic pickup icons: created for this project with built-in image generation, using a user-supplied character reference.\n\nDialogue frame: user-supplied OpeningDialogue.svg.\n\nSunny Land art: ansimuz, CC0 1.0 Universal\nhttps://ansimuz.itch.io/sunny-land-pixel-game-art\nhttps://creativecommons.org/publicdomain/zero/1.0/\n\nMusic: Pascal Belisle (under the rainbow)\nhttps://opengameart.org/content/sunny-land-2d-pixel-art-pack\nhttps://www.soundcloud.com/thetoadz\n\nNoto Sans TC: Google Fonts / Adobe, SIL OFL 1.1\nhttps://github.com/google/fonts/tree/main/ofl/notosanstc\nMedium 500 static instance generated from the variable font.\n\n"
 (web/'ThirdPartyNotices.txt').write_text(notices+(ROOT/'Assets/_OneMoreTime/Fonts/OFL.txt').read_text())
 with zipfile.ZipFile(ROOT/'Artifacts/OneMoreTime-Web.zip','w',zipfile.ZIP_DEFLATED) as z:
  for f in sorted(web.rglob('*')):
   if f.is_file() and f.relative_to(web).parts[0]!='TemplateData':z.write(f,f.relative_to(web))
 sys.exit(0)
cmd='test-'+a.mode if a.command=='test' else a.command
# Temp may be removed while the Editor is still running. Check the process too,
# without printing command lines (Unity can include account tokens in them).
def editor_running():
 if (ROOT/'Temp/UnityLockfile').exists():return True
 if sys.platform!='darwin':return False
 listing=subprocess.run(['ps','-axo','args='],capture_output=True,text=True,check=True).stdout
 return any(line.startswith(str(EDITOR)+' ') and ('-projectpath '+str(ROOT)).lower() in line.lower() and '-batchmode' not in line.lower() for line in listing.splitlines())
# Never open a second Editor on an already-open project.
if editor_running():
 expected=hashlib.sha256(''.join(f.read_text() for f in sorted((ROOT/'Assets/_OneMoreTime').rglob('*.cs'))).encode()).hexdigest()
 compiled=logs/'compiled.sha256';compile_deadline=time.monotonic()+90
 while not compiled.exists() or compiled.read_text()!=expected:
  if time.monotonic()>compile_deadline:sys.exit('Current C# sources have not compiled. Refresh Unity and inspect Logs/Editor.log.')
  time.sleep(.5)
 result=logs/'command-result.txt';old=result.stat().st_mtime_ns if result.exists() else 0
 request=ROOT/'Artifacts/command.txt'
 if request.exists():sys.exit('An Editor command is already pending')
 request.write_text(cmd)
 deadline=time.monotonic()+600;reported=False
 while time.monotonic()<deadline:
  if result.exists() and result.stat().st_mtime_ns!=old:
   message=result.read_text()
   if not reported:print(message);reported=True
   if not message.startswith(cmd+' '):sys.exit('Unexpected Editor command response')
   if 'FAIL' in message:sys.exit(1)
   if a.command!='test':sys.exit(0)
   summary=logs/(a.mode+'mode.xml.summary')
   if summary.exists() and summary.stat().st_mtime>=result.stat().st_mtime:
    text=summary.read_text();print(text);sys.exit(0 if 'failed=0' in text and 'total=0' not in text else 1)
  time.sleep(.5)
 sys.exit('Editor timeout; inspect Unity compile errors and Logs/Editor.log')
else:
 if not EDITOR.is_file():sys.exit('Unity Editor missing')
 method={'challenge-layouts':'Apply','generate':'Generate','validate':'Validate','audit':'Audit','build-web':'BuildWeb'}.get(a.command)
 args=[str(EDITOR),'-batchmode','-projectPath',str(ROOT),'-logFile',str(logs/'batch.log')]
 if a.command=='test':args+=['-runTests','-testPlatform','EditMode' if a.mode=='edit' else 'PlayMode','-testResults',str(logs/(a.mode+'mode.xml'))]
 elif method:args+=['-quit','-executeMethod',('OneMoreTime.EditorTools.WebBuilder.Build' if a.command=='build-web' else 'OneMoreTime.EditorTools.ChallengeLayouts.Apply' if a.command=='challenge-layouts' else 'OneMoreTime.EditorTools.DemoBuilder.'+method)]
 else:sys.exit('This command needs the open Editor')
 code=subprocess.call(args)
 if code==0 and a.command=='test':
  import xml.etree.ElementTree as ET
  report=ET.parse(logs/(a.mode+'mode.xml')).getroot()
  code=0 if int(report.get('total','0'))>0 and int(report.get('failed','0'))==0 else 1
 sys.exit(code)
