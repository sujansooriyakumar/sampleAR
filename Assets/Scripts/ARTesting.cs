using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ARTesting : MonoBehaviour
{
    public XRRayInteractor arInteractor;
    XRInputButtonReader m_SpawnObjectInput = new XRInputButtonReader("Spawn Object");

    public UnityEvent<Vector3, Vector3> objectSpawnTriggered
    {
        get => m_ObjectSpawnTriggered;
        set => m_ObjectSpawnTriggered = value;
    }

    UnityEvent<Vector3, Vector3> m_ObjectSpawnTriggered = new UnityEvent<Vector3, Vector3>();

    private void OnEnable()
    {
        m_SpawnObjectInput.EnableDirectActionIfModeUsed();

    }

    void Start()
    {
        if (arInteractor == null)
        {
            Debug.LogError("Missing AR Interactor reference, disabling component.", this);
            enabled = false;
        }
    }



}
