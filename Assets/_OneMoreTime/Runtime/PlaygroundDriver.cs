using UnityEngine;
namespace OneMoreTime {
public sealed class PlaygroundDriver : MonoBehaviour {
 public AnimationLibrary goat,fox;bool useFox;public PlayerMotor2D player;public PlayerVisual visual;public Camera view;KeyboardPlayerInput input=new KeyboardPlayerInput();float time;
 void Awake(){player.Init();player.ResetAt(new Vector2(2,0));}
 void Update(){input.Poll();var k=UnityEngine.InputSystem.Keyboard.current;if(k!=null&&k.fKey.wasPressedThisFrame){useFox=!useFox;visual.library=useFox?fox:goat;}}
 void FixedUpdate(){player.Step(input.Consume(),.02f);time+=.02f;if(player.Position.y< -6){player.ResetAt(new Vector2(2,0));input.Clear();}visual.Pose(player.Position,player.Facing,player.VisualState,player.VisualFrame,Color.white);}
 void LateUpdate(){view.transform.position=new Vector3(Mathf.Clamp(player.Position.x,9,20),3,-10);}
 void OnGUI(){GUI.Label(new Rect(20,20,600,40),"ONE MORE TIME / Playground    A D: Move    SPACE: Jump    F: Goat / Fox test character");}
}
}
