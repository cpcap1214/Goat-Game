using System;using System.Collections.Generic;using UnityEngine;
namespace OneMoreTime {
public enum ChapterEra { Both,Present,Past }
[Serializable] public struct EraBlock {
 public Rect rect;public ChapterEra era;public bool bridge;public int requires;
 public EraBlock(float x,float top,float width,float height,ChapterEra era=ChapterEra.Both,bool bridge=false,int requires=-1){rect=new Rect(x,top-height,width,height);this.era=era;this.bridge=bridge;this.requires=requires;}
}
public static class ChapterLayout {
 public const string Scene="Chapter01_EchoFoothills";
 public static readonly string[] Zones={"山腳入口","定格庭院","沉睡水車","雙影水閘","鐘樓迴廊","搭上昨天"};
 public static readonly Vector2[] Checkpoints={new Vector2(3,0),new Vector2(65,0),new Vector2(115,0),new Vector2(132,0),new Vector2(187,0),new Vector2(195,0),new Vector2(248,0),new Vector2(258,0),new Vector2(291,7.2f),new Vector2(323,0),new Vector2(357,6.3f)};
 public static readonly Vector2[] Devices={new Vector2(165,0),new Vector2(290,7.2f)};
 public static readonly Vector2[] Photos={new Vector2(115,0),new Vector2(248,0),new Vector2(380,6.3f)};
 public static List<EraBlock> Blocks(){
  var b=new List<EraBlock>();
  Action<float,float,float,float,ChapterEra> a=(x,y,w,h,e)=>b.Add(new EraBlock(x,y,w,h,e));
  // Broken bridge and fallen rock: both states must be used in the first clearing.
  b.Add(new EraBlock(-3,0,17,5));b.Add(new EraBlock(14,0,16,.45f,ChapterEra.Past,true));b.Add(new EraBlock(30,0,46,5));
  b.Add(new EraBlock(40,5,3,5,ChapterEra.Present));b.Add(new EraBlock(55,11,1.2f,11,ChapterEra.Past));
  // Past pressure pad and a wall that forces a return to the present after saving.
  b.Add(new EraBlock(76,0,52,5));b.Add(new EraBlock(96,12,1.2f,12,ChapterEra.Past));
  b.Add(new EraBlock(128,0,12,5));b.Add(new EraBlock(140,0,13,.5f,ChapterEra.Past,true));b.Add(new EraBlock(153,0,16,5));
  b.Add(new EraBlock(169,0,14,.6f,ChapterEra.Present,true,0));b.Add(new EraBlock(183,0,9,5));
  // Recording puzzle: A then B in the echo, while the live player holds the remote console.
  b.Add(new EraBlock(192,0,64,5));
  // A vertical bell tower: ascend alternating steps to release the second mechanism.
  b.Add(new EraBlock(256,0,12,5));
  for(int i=0;i<4;i++)b.Add(new EraBlock(270+i*5,1.8f+i*1.8f,3.5f,.7f,i%2==0?ChapterEra.Past:ChapterEra.Present,true));
  b.Add(new EraBlock(289,7.2f,7,1));
  b.Add(new EraBlock(298,5,6,.6f,ChapterEra.Present,true,1));
  b.Add(new EraBlock(307,3,5,.6f,ChapterEra.Past,true));b.Add(new EraBlock(315,0,15,5));
  // Echo relay: record a jump to the middle island and a second vertical jump.
  // The final cliff is above a solo jump or a reachable stationary echo.
  b.Add(new EraBlock(330,0,2,5));b.Add(new EraBlock(336,0,6,5));b.Add(new EraBlock(343,6.3f,44,11.3f));
  // Land continues beneath the camera instead of floating as one-cell soil slabs.
  for(int i=0;i<b.Count;i++){var block=b[i];if(!block.bridge){var r=block.rect;r.yMin=-22;block.rect=r;b[i]=block;}}
  return b;
 }
 public static bool Active(EraBlock b,bool past,bool[] devices)=> (b.era==ChapterEra.Both||(b.era==ChapterEra.Past)==past)&&(b.requires<0||devices[b.requires]);
 public static int Zone(float x)=>Mathf.Clamp((int)(x/64),0,5);
}
}
