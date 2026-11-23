using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Linq;

public static class ZombieRunnerControllerSetup
{
    [MenuItem("Tools/ZombieRunner/Add Animation States")]
    public static void AddStates()
    {
        string controllerPath =
            "Assets/Animations/ZombieRunner_Controller.controller";

        var controller =
            AssetDatabase.LoadAssetAtPath<AnimatorController>(
                controllerPath);

        if (controller == null || controller.layers.Length == 0)
        {
            Debug.LogError("ZombieRunner controller not found.");
            return;
        }

        string folder =
            "Assets/Art/Characters/Zombies/Models/ZombieRunner/MotionExports/";

        string[] names = { "Idle", "Walk", "Attack", "Death" };
        AnimationClip[] clips = new AnimationClip[names.Length];

        // Validate every clip before changing the controller.
        for (int i = 0; i < names.Length; i++)
        {
            string fbxPath =
                folder + "ZombieRunner_" + names[i] + ".fbx";

            clips[i] = AssetDatabase.LoadAllAssetsAtPath(fbxPath)
                .OfType<AnimationClip>()
                .FirstOrDefault(c =>
                    !c.name.StartsWith("__preview__") &&
                    !c.name.StartsWith("Take 001 Preview"));

            if (clips[i] == null)
            {
                Debug.LogError("No animation clip found: " + fbxPath);
                return;
            }
        }

        var stateMachine = controller.layers[0].stateMachine;

        for (int i = 0; i < names.Length; i++)
        {
            string stateName = names[i];

            var existing = stateMachine.states
                .FirstOrDefault(s => s.state.name == stateName).state;

            if (existing != null)
            {
                Debug.Log("State already exists, skipping: " + stateName);
                continue;
            }

            var state = stateMachine.AddState(
                stateName,
                new Vector3(350 + (i % 2) * 250,
                            100 + (i / 2) * 120, 0));

            state.motion = clips[i];
            Debug.Log("Added state: " + stateName);
        }

        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();

        Debug.Log("ZombieRunner Animator states setup finished.");
    }
}
