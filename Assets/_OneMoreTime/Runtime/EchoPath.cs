using UnityEngine;
namespace OneMoreTime {
[RequireComponent(typeof(LineRenderer))]
public sealed class EchoPath:MonoBehaviour {
 LineRenderer line;public void Show(MemorySnapshot memory){if(line==null)line=GetComponent<LineRenderer>();if(memory==null||memory.Kind!=MemoryKind.Trace){line.positionCount=0;return;}line.enabled=true;line.positionCount=memory.Count;for(int i=0;i<memory.Count;i++)line.SetPosition(i,memory.Frame(i).position+Vector2.up*.4f);}
 public void Visible(bool value){if(line!=null)line.enabled=value;}
}
}
