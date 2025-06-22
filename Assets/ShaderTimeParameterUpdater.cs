// How I like Unity!
// Shader's _Time parameter isn't changing if there is no camera on a scene.
// And to fix that, this class exists.

using UnityEngine;

[ExecuteAlways]
public class ShaderTimeParameterUpdater : MonoBehaviour
{
    private static readonly int TimeID = Shader.PropertyToID("_Time");

    [ExecuteAlways]
    private void Update()
    {
        float time = Time.time;
        Shader.SetGlobalVector(TimeID, new Vector4(time / 20, time, time * 2, time * 3));
    }
}
