using UnityEngine;
using UnityEditor;
using System.Linq;

public static class ZombieRunnerAnimationSetup
{
    [MenuItem("Tools/ZombieRunner/Configure Imported Animations")]
    public static void Configure()
    {
        const string modelFolder =
            "Assets/Art/Characters/Zombies/Models/ZombieRunner";

        string originalModel = AssetDatabase.FindAssets("t:Model", new[] { modelFolder })
            .Select(AssetDatabase.GUIDToAssetPath)
            .FirstOrDefault(p =>
                !p.Contains("/MotionExports/") &&
                p.EndsWith(".fbx"));

        if (string.IsNullOrEmpty(originalModel))
        {
            Debug.LogError("Original ZombieRunner FBX not found.");
            return;
        }

        Avatar avatar = AssetDatabase.LoadAllAssetsAtPath(originalModel)
            .OfType<Avatar>()
            .FirstOrDefault(a => a.isHuman && a.isValid);

        if (avatar == null)
        {
            Debug.LogError("Valid Humanoid Avatar not found on original model.");
            return;
        }

        string[] names = { "Idle", "Walk", "Attack", "Death" };

        foreach (string name in names)
        {
            string path = modelFolder +
                "/MotionExports/ZombieRunner_" + name + ".fbx";

            ModelImporter importer =
                AssetImporter.GetAtPath(path) as ModelImporter;

            if (importer == null)
            {
                Debug.LogError("Missing FBX: " + path);
                continue;
            }

            importer.animationType = ModelImporterAnimationType.Human;
            importer.avatarSetup = ModelImporterAvatarSetup.CopyFromOther;
            importer.sourceAvatar = avatar;
            importer.importAnimation = true;

            var clips = importer.defaultClipAnimations;
            foreach (var clip in clips)
            {
                clip.loopTime = name == "Idle" || name == "Walk";
            }

            importer.clipAnimations = clips;
            importer.SaveAndReimport();

            Debug.Log("Configured ZombieRunner_" + name);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("ZombieRunner animation import setup finished.");
    }
}
