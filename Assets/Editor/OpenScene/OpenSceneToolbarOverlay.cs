using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

[Overlay(typeof(SceneView), "Open Scene")]
[Icon("Assets/PlacementToolsIcon.png")]
public class OpenSceneToolbarOverlay : ToolbarOverlay
{
    OpenSceneToolbarOverlay() : base(
        SelectSceneDropdown.id)
    { }
}
