using UnityEngine;

[ExecuteAlways]
public class PortalCamera : MonoBehaviour
{
    [Header("Portal Pair")]
    public Transform thisPortal;
    public Transform otherPortal;

    private Camera mainCamera;
    private Camera portalCamera;
    [HideInInspector] public RenderTexture renderTexture;
    public Shader portalShader;

    void Start()
    {
//
//        mainCamera = Camera.current;
//else
        mainCamera = Camera.main;
 

        portalCamera = GetComponent<Camera>();

        // Auto create render texture
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        portalCamera.targetTexture = renderTexture;
        portalCamera.enabled = true;

        // Auto assign to this portal's quad material
        MeshRenderer quad = thisPortal.GetComponentInChildren<MeshRenderer>();
        quad.material = new Material(portalShader);
        quad.material.SetTexture("_MainTex", renderTexture);
    }

    void Update()
    {
        if (mainCamera == null || thisPortal == null || otherPortal == null) return;
        
        // Only Render when portal is visible to main camera
        Renderer portalRenderer = thisPortal.GetComponentInChildren<Renderer>();
        portalCamera.enabled = portalRenderer.isVisible;

        Matrix4x4 m = thisPortal.worldToLocalMatrix * mainCamera.transform.localToWorldMatrix;
        Matrix4x4 flip = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 180, 0), Vector3.one);
        Matrix4x4 result = otherPortal.localToWorldMatrix * flip * m;

        portalCamera.transform.position = result.MultiplyPoint(Vector3.zero);
        portalCamera.transform.rotation = result.rotation;
        portalCamera.projectionMatrix = mainCamera.projectionMatrix;
    }
}
