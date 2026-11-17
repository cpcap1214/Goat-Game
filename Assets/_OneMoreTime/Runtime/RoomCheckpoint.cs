using UnityEngine;
namespace OneMoreTime {
public sealed class RoomCheckpoint:MonoBehaviour {
 public StoneCheckpointVisual stoneVisual;
 public SpriteRenderer[] eyes=new SpriteRenderer[0];public bool Saved{get;private set;}
 public void Save(){Saved=true;stoneVisual?.Activate();foreach(var eye in eyes)if(eye!=null)eye.color=new Color(.25f,1,.35f);}
}
}
