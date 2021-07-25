using UnityEngine;
using System.Collections;

public class SetPhysicsLayersMatrix : MonoBehaviour
{
    void Awake()
    {
        // Make the IgnoreRaycast layer ignore itself, all trigger sensor volumes are put on this layer in the examples.
        Physics.IgnoreLayerCollision(2, 2, true);
    }
}