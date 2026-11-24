using UnityEngine;
namespace OneMoreTime {
public sealed class BreakableBridge:MonoBehaviour {
 public Collider2D deck;public GameObject intact;public SpriteRenderer[] pieces;
 public float breakDelay=.25f,segmentImpulse=1.8f,segmentGravity=10,fadeDelay=.35f,fadeDuration=1.1f;
 public bool Broken{get;private set;}float time;Vector3[] homes;Vector2[] velocities;
 void Awake(){homes=new Vector3[pieces.Length];velocities=new Vector2[pieces.Length];for(int i=0;i<pieces.Length;i++){homes[i]=pieces[i].transform.localPosition;pieces[i].gameObject.SetActive(false);}}
 public void Break(){if(Broken)return;Broken=true;time=0;deck.enabled=false;intact.SetActive(false);for(int i=0;i<pieces.Length;i++){pieces[i].gameObject.SetActive(true);velocities[i]=new Vector2((i-(pieces.Length-1)*.5f)*segmentImpulse,.35f);}}
 public void SetPostAcquisition(){Broken=true;deck.enabled=false;intact.SetActive(false);foreach(var p in pieces)p.gameObject.SetActive(false);time=fadeDelay+fadeDuration;}
 public void Tick(float dt){if(!Broken)return;time+=dt;for(int i=0;i<pieces.Length;i++){var p=pieces[i];if(!p.gameObject.activeSelf)continue;velocities[i]+=Vector2.down*segmentGravity*dt;p.transform.localPosition+=(Vector3)(velocities[i]*dt);p.transform.Rotate(0,0,(i%2==0?-1:1)*55*dt);var color=p.color;color.a=1-Mathf.Clamp01((time-fadeDelay)/fadeDuration);p.color=color;if(color.a<=0)p.gameObject.SetActive(false);}}
}
}
