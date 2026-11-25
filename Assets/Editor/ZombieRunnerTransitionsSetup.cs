using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Linq;

public static class ZombieRunnerTransitionsSetup
{
    static void Param(AnimatorController c, string name,
                      AnimatorControllerParameterType type)
    {
        if (!c.parameters.Any(p => p.name == name))
            c.AddParameter(name, type);
    }

    static AnimatorStateTransition Link(
        AnimatorState from, AnimatorState to, bool exitTime = false)
    {
        var existing = from.transitions
            .FirstOrDefault(t => t.destinationState == to);
        if (existing != null) return existing;

        var t = from.AddTransition(to);
        t.hasExitTime = exitTime;
        t.exitTime = 0.95f;
        t.duration = exitTime ? 0.1f : 0.15f;
        return t;
    }

    [MenuItem("Tools/ZombieRunner/Configure Gameplay Transitions")]
    public static void Configure()
    {
        var c = AssetDatabase.LoadAssetAtPath<AnimatorController>(
            "Assets/Animations/ZombieRunner_Controller.controller");

        if (c == null || c.layers.Length == 0)
        {
            Debug.LogError("Animator Controller not found.");
            return;
        }

        var sm = c.layers[0].stateMachine;

        AnimatorState Find(string name) =>
            sm.states.Select(s => s.state)
                .FirstOrDefault(s => s.name == name);

        var idle = Find("Idle");
        var walk = Find("Walk");
        var attack = Find("Attack");
        var death = Find("Death");

        // The original orange default state contains Running.
        var run = sm.defaultState;

        if (idle == null || walk == null || attack == null ||
            death == null || run == null ||
            new[] { idle, walk, attack, death }.Contains(run))
        {
            Debug.LogError("Expected animation states not found. No changes made.");
            return;
        }

        Param(c, "HasTarget", AnimatorControllerParameterType.Bool);
        Param(c, "Speed", AnimatorControllerParameterType.Float);
        Param(c, "Attack", AnimatorControllerParameterType.Trigger);
        Param(c, "Die", AnimatorControllerParameterType.Trigger);

        // Idle -> Walk when a player is detected.
        var t = Link(idle, walk);
        t.AddCondition(AnimatorConditionMode.If, 0, "HasTarget");
        t.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");

        // Walk <-> Run based on movement speed.
        t = Link(walk, run);
        t.AddCondition(AnimatorConditionMode.Greater, 1.5f, "Speed");

        t = Link(run, walk);
        t.AddCondition(AnimatorConditionMode.Less, 1.5f, "Speed");

        // Return to Idle when the player is lost.
        t = Link(walk, idle);
        t.AddCondition(AnimatorConditionMode.IfNot, 0, "HasTarget");

        t = Link(run, idle);
        t.AddCondition(AnimatorConditionMode.IfNot, 0, "HasTarget");

        // Attack can interrupt normal movement.
        var atk = sm.anyStateTransitions
            .FirstOrDefault(x => x.destinationState == attack);

        if (atk == null)
        {
            atk = sm.AddAnyStateTransition(attack);
            atk.hasExitTime = false;
            atk.duration = 0.1f;
            atk.canTransitionToSelf = false;
            atk.AddCondition(AnimatorConditionMode.If, 0, "Attack");
        }

        // After an attack, resume an appropriate state.
        t = Link(attack, idle, true);
        t.AddCondition(AnimatorConditionMode.IfNot, 0, "HasTarget");

        t = Link(attack, walk, true);
        t.AddCondition(AnimatorConditionMode.If, 0, "HasTarget");
        t.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");
        t.AddCondition(AnimatorConditionMode.Less, 1.5f, "Speed");

        t = Link(attack, run, true);
        t.AddCondition(AnimatorConditionMode.If, 0, "HasTarget");
        t.AddCondition(AnimatorConditionMode.Greater, 1.5f, "Speed");

        // Death can interrupt any state.
        var die = sm.anyStateTransitions
            .FirstOrDefault(x => x.destinationState == death);

        if (die == null)
        {
            die = sm.AddAnyStateTransition(death);
            die.hasExitTime = false;
            die.duration = 0.05f;
            die.canTransitionToSelf = false;
            die.AddCondition(AnimatorConditionMode.If, 0, "Die");
        }

        // Zombie begins standing still.
        sm.defaultState = idle;

        EditorUtility.SetDirty(sm);
        EditorUtility.SetDirty(c);
        AssetDatabase.SaveAssets();

        Debug.Log("ZombieRunner gameplay transitions configured.");
    }
}
