using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor {
    public override void OnInspectorGUI() {
        // base.OnInspectorGUI();


        MapGenerator map = target as MapGenerator;
        
        if(DrawDefaultInspector()){ // 인스펙터 상의 값이 바뀔 때만 맵을 재생성.
            map.GenerateMap();
        }

        if(GUILayout.Button("새로고침")){
            map.GenerateMap();
        }

    }
}

