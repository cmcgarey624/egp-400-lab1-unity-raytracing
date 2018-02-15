using UnityEngine;

public class RayTracerLab : MonoBehaviour
{
    [SerializeField] private float _downsampling = 1.0f;
    [SerializeField] private int _recursionDepth;
    [SerializeField] private float _ambience;

    private Texture2D _canvasTex;

    // Use this for initialization
    void Awake()
    {
        // Create canvas to draw on (with downsampling)
        _canvasTex = new Texture2D((int)(Screen.width / _downsampling), (int)(Screen.height / _downsampling));
    }

    void Start()
    {
        UpdateRaytrace();
    }

    private void UpdateRaytrace()
    {
        // make sure the canvas is correct
        int newWidth = (int) (Screen.width / _downsampling);
        if (_canvasTex == null || _canvasTex.width != newWidth)
        {
            _canvasTex = new Texture2D(newWidth, (int)(Screen.height / _downsampling));
            _canvasTex.filterMode = FilterMode.Point;
        }

        // get main camera
        Camera cam = Camera.main;

        // get width and height
        int width = _canvasTex.width,
            height = _canvasTex.height;

        // get z pos
        float camZ = cam.transform.position.z;
        Color black = Color.black;

        // calculate color for each pixel on the canvas
        for (float i = 0; i < width; ++i)
        {
            for (float j = 0; j < height; ++j)
            {

                // Get Ray from pixel and zpos
                Vector3 pos = new Vector3(i / width, j / height, camZ);
                Ray ray = cam.ViewportPointToRay(pos);


                RaycastHit raycastHitInfo;
                bool didHit = Physics.Raycast(ray, out raycastHitInfo);
                if (didHit)
                {
                    // get the color from the RayTraceRenderer
                    RayTraceRenderer comp = raycastHitInfo.collider.GetComponent<RayTraceRenderer>();
                    if (comp != null)
                        _canvasTex.SetPixel((int)i, (int)j, comp.CalculateColor(raycastHitInfo, _recursionDepth, _ambience, ray));
                    else
                        _canvasTex.SetPixel((int)i, (int)j, black);
                }
                else
                {
                    // otherwise just set the color to black
                    _canvasTex.SetPixel((int)i, (int)j, black);
                }
            }
        }

        _canvasTex.Apply();
    }

    private void Update()
    {
        // Update Raytrace
        if (Input.GetKeyDown(KeyCode.Space))
            UpdateRaytrace();
    }

    private void OnGUI()
    {
        // Draw texture to screen
        GUI.DrawTexture(new Rect(0.0f, 0.0f, Screen.width, Screen.height), _canvasTex);
    }
}
