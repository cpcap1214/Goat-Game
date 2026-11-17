using UnityEngine;
namespace OneMoreTime {
// Fixed cell pivots preserve alignment throughout splashes and sinking animations.
public sealed class WorldOneSequence:MonoBehaviour {
 public SpriteRenderer target;public Sprite[] frames;public float framesPerSecond=8;public bool loop=true,autoPlay=true;
 public bool Finished{get;private set;}public float TimeInSequence{get;private set;}
 public void Play(){TimeInSequence=0;Finished=false;target.enabled=true;Show();}
 void OnEnable(){if(target!=null)Play();}
 void Update(){if(autoPlay)Tick(Time.deltaTime);}
 public void Tick(float dt){if(Finished||frames==null||frames.Length==0)return;TimeInSequence+=Mathf.Max(0,dt);if(!loop&&TimeInSequence>=frames.Length/framesPerSecond)Finished=true;Show();}
 void Show(){if(frames==null||frames.Length==0||target==null)return;int frame=Mathf.FloorToInt(TimeInSequence*framesPerSecond);target.sprite=frames[loop?frame%frames.Length:Mathf.Min(frame,frames.Length-1)];}
}
}
