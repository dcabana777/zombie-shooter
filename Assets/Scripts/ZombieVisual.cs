
using UnityEngine;

public class ZombieVisual : MonoBehaviour
{
    public enum ZombieState
    {
        Idle,
        Walk,
        Attack,
        Death
    }

    [Header("Animation Settings")]
    public ZombieState state = ZombieState.Walk;
    public bool autoDemo = true;
    public float walkSpeed = 5f;

    private Transform leftArm;
    private Transform rightArm;
    private Transform leftLeg;
    private Transform rightLeg;
    private Transform body;

    private float timer;
    private ZombieState previousState;

    void Start()
    {
        BuildZombie();
        previousState = state;
    }

    GameObject CreatePart(
        string partName,
        PrimitiveType shape,
        Vector3 position,
        Vector3 scale,
        Color color,
        Transform parent)
    {
        GameObject part = GameObject.CreatePrimitive(shape);

        part.name = partName;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = position;
        part.transform.localScale = scale;

        Shader shader = Shader.Find(
            "Universal Render Pipeline/Lit"
        );

        if (shader == null)
            shader = Shader.Find("Standard");

        Material material = new Material(shader);
        material.color = color;

        part.GetComponent<Renderer>().material = material;

        // Remove unnecessary primitive colliders.
        Collider collider = part.GetComponent<Collider>();

        if (collider != null)
            Destroy(collider);

        return part;
    }

    void BuildZombie()
    {
        Color skin = new Color(0.28f, 0.65f, 0.20f);
        Color shirt = new Color(0.35f, 0.17f, 0.16f);
        Color pants = new Color(0.15f, 0.20f, 0.30f);

        body = new GameObject("ZombieBody").transform;
        body.SetParent(transform, false);

        // Torso
        CreatePart(
            "Torso",
            PrimitiveType.Cube,
            new Vector3(0, 1.55f, 0),
            new Vector3(0.75f, 0.9f, 0.4f),
            shirt,
            body
        );

        // Head
        CreatePart(
            "Head",
            PrimitiveType.Cube,
            new Vector3(0, 2.32f, 0),
            new Vector3(0.55f, 0.65f, 0.55f),
            skin,
            body
        );

        // Eyes
        CreatePart(
            "LeftEye",
            PrimitiveType.Sphere,
            new Vector3(-0.14f, 2.39f, 0.29f),
            Vector3.one * 0.12f,
            Color.red,
            body
        );

        CreatePart(
            "RightEye",
            PrimitiveType.Sphere,
            new Vector3(0.14f, 2.39f, 0.29f),
            Vector3.one * 0.12f,
            Color.red,
            body
        );

        // Mouth
        CreatePart(
            "Mouth",
            PrimitiveType.Cube,
            new Vector3(0, 2.16f, 0.29f),
            new Vector3(0.28f, 0.07f, 0.04f),
            Color.black,
            body
        );

        // Arms
        leftArm = CreateLimb(
            "LeftArm", -0.48f, 1.92f, skin, true
        );

        rightArm = CreateLimb(
            "RightArm", 0.48f, 1.92f, skin, true
        );

        // Legs
        leftLeg = CreateLimb(
            "LeftLeg", -0.22f, 1.05f, pants, false
        );

        rightLeg = CreateLimb(
            "RightLeg", 0.22f, 1.05f, pants, false
        );
    }

    Transform CreateLimb(
        string limbName,
        float x,
        float y,
        Color color,
        bool isArm)
    {
        Transform pivot = new GameObject(limbName).transform;

        pivot.SetParent(body, false);
        pivot.localPosition = new Vector3(x, y, 0);

        CreatePart(
            limbName + "Mesh",
            PrimitiveType.Cube,
            new Vector3(0, -0.42f, isArm ? 0.2f : 0),
            new Vector3(
                isArm ? 0.25f : 0.32f,
                0.8f,
                0.3f
            ),
            color,
            pivot
        );

        return pivot;
    }

    public void SetState(ZombieState newState)
    {
        state = newState;
        previousState = newState;
        timer = 0f;
    }

    void Update()
    {
        if (body == null)
            return;

        timer += Time.deltaTime;

        if (autoDemo)
        {
            float cycle = timer % 12f;

            if (cycle < 3f)
                state = ZombieState.Idle;
            else if (cycle < 7f)
                state = ZombieState.Walk;
            else if (cycle < 9f)
                state = ZombieState.Attack;
            else
                state = ZombieState.Death;
        }
        else if (state != previousState)
        {
            timer = 0f;
            previousState = state;
        }

        float t = Time.time;

        float leftArmAngle = 0f;
        float rightArmAngle = 0f;
        float leftLegAngle = 0f;
        float rightLegAngle = 0f;

        body.localPosition = Vector3.zero;
        body.localRotation = Quaternion.identity;

        switch (state)
        {
            case ZombieState.Idle:
                leftArmAngle =
                    -55f + Mathf.Sin(t * 2f) * 4f;

                rightArmAngle =
                    -55f - Mathf.Sin(t * 2f) * 4f;

                body.localPosition =
                    Vector3.up * Mathf.Sin(t * 2f) * 0.025f;
                break;

            case ZombieState.Walk:
                float swing = Mathf.Sin(t * walkSpeed);

                leftArmAngle = -65f + swing * 18f;
                rightArmAngle = -65f - swing * 18f;

                leftLegAngle = swing * 28f;
                rightLegAngle = -swing * 28f;

                body.localPosition =
                    Vector3.up * Mathf.Abs(swing) * 0.06f;
                break;

            case ZombieState.Attack:
                float hit = Mathf.Sin(t * 9f);

                leftArmAngle = -95f + hit * 32f;
                rightArmAngle = -95f + hit * 32f;
                break;

            case ZombieState.Death:
                float progress;

                if (autoDemo)
                {
                    progress = Mathf.Clamp01(
                        ((timer % 12f) - 9f) / 1.5f
                    );
                }
                else
                {
                    progress = Mathf.Clamp01(timer / 1.5f);
                }

                body.localRotation = Quaternion.Euler(
                    75f * progress, 0f, 0f
                );
                break;
        }

        leftArm.localRotation =
            Quaternion.Euler(leftArmAngle, 0, 0);

        rightArm.localRotation =
            Quaternion.Euler(rightArmAngle, 0, 0);

        leftLeg.localRotation =
            Quaternion.Euler(leftLegAngle, 0, 0);

        rightLeg.localRotation =
            Quaternion.Euler(rightLegAngle, 0, 0);
    }
}
