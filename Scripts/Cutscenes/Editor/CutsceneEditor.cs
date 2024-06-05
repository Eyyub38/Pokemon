using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Cutscene))]
public class CutsceneEditor : Editor{
    public override void OnInspectorGUI(){
        var cutScene = target as Cutscene;

        using(var scope = new GUILayout.HorizontalScope()){
            if(GUILayout.Button("Dialog")){
                cutScene.AddAction(new DialogAction());
            } else if(GUILayout.Button("Move Actor")){
                cutScene.AddAction(new MoveActorAction());
            } else if(GUILayout.Button("Turn Actor")){
                cutScene.AddAction(new TurnActorAction());
            }
        }

        using(var scope = new GUILayout.HorizontalScope()){
            if(GUILayout.Button("Teleport Actor")){
                cutScene.AddAction(new TeleportObjectAction());
            } else if(GUILayout.Button("Enable Object")){
                cutScene.AddAction(new EnableObjectAction());
            } else if(GUILayout.Button("Diasble Object")){
                cutScene.AddAction(new DisableObjectAction());
            }
        }

        using(var scope = new GUILayout.HorizontalScope()){
            if(GUILayout.Button("Npc Interact")){
                cutScene.AddAction(new NPCInteractAction());
            } else if(GUILayout.Button("Fade In")){
                cutScene.AddAction(new FadeInAction());
            } else if(GUILayout.Button("Fade Out")){
                cutScene.AddAction(new FadeOutAction());
            }
        }

        base.OnInspectorGUI();
    }
}
