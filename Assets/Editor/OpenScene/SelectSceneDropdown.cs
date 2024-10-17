using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.SearchService;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

[EditorToolbarElement(id, typeof(SceneView))]
public class SelectSceneDropdown : EditorToolbarDropdown, IAccessContainerWindow
{
    public const string id = "OpenSceneToolbar/SelectSceneToggle";

    public EditorWindow containerWindow { get; set; }


    SelectSceneDropdown()
    {
        text = "Open Scene";
        tooltip = "Open selected scene.";

        clicked += ShowSelectSceneMenu;
    }

    // When the dropdown button is clicked, this method will create a popup menu at the mouse cursor position.
    void ShowSelectSceneMenu()
    {

        Dictionary<string, string>  sceneNamesAndPaths = GetAllSceneInProjectNamesAndPaths();
        sceneNamesAndPaths = SolveNameDuplications(sceneNamesAndPaths);

        var menu = CreateSelectSceneMenu(sceneNamesAndPaths);
        menu.ShowAsContext();
    }

    private Dictionary<string,string> GetAllSceneInProjectNamesAndPaths()
    {
        var scenePathsAndNames = new Dictionary<string, string>();

        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");

        foreach (var guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
            scenePathsAndNames.Add(scenePath, sceneName);
        }

        return scenePathsAndNames;
    }

    private  Dictionary<string, string> SolveNameDuplications(
        Dictionary<string, string> scenePathsAndNames)
    {
        var scenePathsAndNamesWithoutNameDuplications = new Dictionary<string, string>(scenePathsAndNames.Count);

        var namesCount = new Dictionary<string, int>();

        foreach (var (_, name) in scenePathsAndNames)
        {
            if (namesCount.TryGetValue(name, out int _))
            {
                namesCount[name]++;
                continue;
            }

            namesCount.Add(name, 0);
        }

        foreach (var (path, name) in scenePathsAndNames)
        {
            if (namesCount[name] <= 1)
            {
                scenePathsAndNamesWithoutNameDuplications.Add(path, name);
                continue;
            }

            string newName =
                $"{name}/{Path.GetFileNameWithoutExtension(path)} at {Path.GetDirectoryName(path)}";

            scenePathsAndNamesWithoutNameDuplications.Add(path, newName);
        }

        return scenePathsAndNamesWithoutNameDuplications;
    }

    private GenericMenu CreateSelectSceneMenu(Dictionary<string, string> scenePathsAndNames)
    {
        var menu = new GenericMenu();

        foreach (var (scenePath, sceneName) in scenePathsAndNames)
        {
            var menuItem = new GUIContent(sceneName);
            menu.AddItem(menuItem, IsSceneOpened(sceneName), 
                () => OnSelectSceneMenuItemCallbackHandler(scenePath));
        }

        return menu;
    }
    
    private void OnSelectSceneMenuItemCallbackHandler(string scenePath)
    {
        OpenScene(scenePath);
    }

    private bool IsSceneOpened(string sceneName)
    {
        return SceneManager.GetActiveScene().name == sceneName;
    }

    private void OpenScene(string scenePath)
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(scenePath);
    }
}
