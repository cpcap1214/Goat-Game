using UnityEngine;
namespace OneMoreTime {
[CreateAssetMenu(menuName="One More Time/Level")]
public sealed class LevelConfig : ScriptableObject {
 public int layoutRevision;public float cameraY=3.5f;public int[] doorSources=new int[0];
 public int DoorSource(int door)=>doorSources.Length==0?door:doorSources[door];
 public bool initialized;public int level=1;public string title="借我踩一下",hint="按 E，留下一個下一次踩得到的自己。";
 public Vector2 spawn=new Vector2(2,0);public Rect goal=new Rect(18,4.5f,1,2);public float killY=-5;
 public Vector2[] plates=new Vector2[0];public Rect[] doors=new Rect[0];
 public Rect[] crumbles=new Rect[0],hazards=new Rect[0];
 public Rect[] terrain;public float cameraSize=6,cameraMin=9,cameraMax=13;
}
}
