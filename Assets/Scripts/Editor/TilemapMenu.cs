using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.Tilemaps;

public class TilemapMenu : MonoBehaviour
{
    [MenuItem("Shortcuts/Tilemap Next &t")] // alt+t
    public static void OpenNextTilemap() => NextTilemap(move: 1);

    [MenuItem("Shortcuts/Tilemap Prev #&t")] // shift+alt+t
    public static void OpenPrevTilemap() => NextTilemap(move: -1);

    static void NextTilemap(int move)
    {
        EditorApplication.ExecuteMenuItem("Window/2D/Tile Palette");
        var targets = GridPaintingState.validTargets
            .OrderBy(go => go.transform.GetSiblingIndex()).ToArray();

        int now = System.Array.IndexOf(targets, GridPaintingState.scenePaintTarget);
        if (now != -1)
        {
            int next = now + move;
            if (next >= targets.Length) next = 0;
            else if (next < 0) next = targets.Length - 1;

            GridPaintingState.scenePaintTarget = targets[next];
            EditorGUIUtility.PingObject(GridPaintingState.scenePaintTarget);
        }
        else Debug.LogWarning($"No valid Tilemap Targets");
    }
}
