using UnityEngine;
namespace OneMoreTime {
[DefaultExecutionOrder(200)]
public sealed class SceneryMotion:MonoBehaviour {
 public Camera view;public Transform[] layers;public float[] amounts;Vector3[] origins;float originX;
 void Start(){originX=view.transform.position.x;origins=new Vector3[layers.Length];for(int i=0;i<layers.Length;i++)origins[i]=layers[i].position;}
 void LateUpdate(){if(origins==null)return;for(int i=0;i<layers.Length;i++)layers[i].position=origins[i]+Vector3.right*(view.transform.position.x-originX)*amounts[i];}
}
}
