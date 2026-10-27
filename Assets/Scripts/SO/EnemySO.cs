using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "Scriptable Objects/EnemySO")]
public class EnemySO : ScriptableObject
{
    public float movementSpeed;
    public float rotationSpeed;

    public float hp;
    public float strength;

    public GameObject prefab;
}
