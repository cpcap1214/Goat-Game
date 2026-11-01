using UnityEngine;
namespace OneMoreTime {
public sealed class EraTerrain:MonoBehaviour {
 public EraBlock block;public BoxCollider2D solid;public SpriteRenderer[] art;
 Color[] colors;
 public void Apply(bool past,bool[] devices){
  if(colors==null){colors=new Color[art.Length];for(int i=0;i<art.Length;i++)colors[i]=art[i].color;}
  bool active=ChapterLayout.Active(block,past,devices);solid.enabled=active;
  bool locked=block.requires>=0&&!devices[block.requires];
  for(int i=0;i<art.Length;i++){var c=colors[i];if(!active&&!locked)c=Color.Lerp(c,new Color(.65f,.94f,1),.55f);c.a=active?1:!block.bridge?0:locked?.06f:.42f;art[i].color=c;}
 }
}
}
