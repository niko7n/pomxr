using UnityEngine;

[CreateAssetMenu(fileName = "New Space Time Cube", menuName = "Space Time Cube Properties")]
public class STCProperties : ScriptableObject
{
    [Header("General Settings")]
    public float Height;
    public float Width;

    [Header("Trajectory Settings")]
    public Material[] AgentMaterials = new Material[4];
    public float Thickness;
    public int Roundness;

    [Header("Board Settings")]
    public GameObject RigidBlockModel;
    public GameObject WoodenBlockModel;
    public Material BoardFloorMaterial;

    [Header("Bomb Settings")]
    public GameObject BombModel;
    public GameObject FlameModel;
    public Material[] BombMaterials = new Material[4];
    public Material FlameMaterial;

    [Header("Selection Settings")]
    public GameObject StepSelectorObject;
    public Material RangeSelectorMaterial;
}
