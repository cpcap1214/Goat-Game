using UnityEngine;
namespace OneMoreTime {
public sealed class GameAudio:MonoBehaviour {
 public AudioClip music;AudioSource musicSource,effects;AudioClip[] tones;
 public static float MusicVolume=.22f,EffectsVolume=.55f;
 void Awake(){musicSource=gameObject.AddComponent<AudioSource>();musicSource.clip=music;musicSource.loop=true;musicSource.volume=MusicVolume;if(music!=null)musicSource.Play();effects=gameObject.AddComponent<AudioSource>();tones=new AudioClip[7];for(int k=0;k<tones.Length;k++){const int rate=22050;var data=new float[rate/6];for(int i=0;i<data.Length;i++){float t=(float)i/rate,envelope=Mathf.Sin(Mathf.PI*i/data.Length);data[i]=Mathf.Sin(2*Mathf.PI*(220+k*85)*t+Mathf.Sin(t*30)*.2f)*envelope*.22f;}tones[k]=AudioClip.Create("OMT synthesized "+k,data.Length,1,rate,false);tones[k].SetData(data,0);}}
 public void Cue(int index){if(effects!=null&&tones!=null)effects.PlayOneShot(tones[Mathf.Clamp(index,0,6)],EffectsVolume);}
 void Update(){if(musicSource!=null)musicSource.volume=MusicVolume;}
 void OnDestroy(){if(tones!=null)foreach(var t in tones)if(t!=null)Destroy(t);}
}
}
