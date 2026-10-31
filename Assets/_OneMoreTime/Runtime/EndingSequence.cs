using UnityEngine;
namespace OneMoreTime {
public sealed class EndingSequence {
 public const float Duration=11;
 public float Elapsed{get;private set;}
 public bool Complete=>Elapsed>=Duration;
 public void Tick(float delta){Elapsed=Mathf.Min(Duration,Elapsed+Mathf.Max(0,delta));}
 public void Skip(){Elapsed=Duration;}
 public float Walk=>Mathf.SmoothStep(0,1,Mathf.Clamp01(Elapsed/6));
 public float Enter=>Mathf.Clamp01((Elapsed-7)/1.5f);
 public float Reveal=>Mathf.Clamp01((Elapsed-9)/2);
}
}
