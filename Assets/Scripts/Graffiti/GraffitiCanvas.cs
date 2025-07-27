using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GraffitiCanvas : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Texture2D texture;
    public Vector2 textureSize = new Vector2(2048, 2048);
    public Camera mainCamera;
    public int spraySize = 20; // Denotes the height and width of a spray
    public Color sprayColor = Color.black;
    private int sprayColorIdx = 0;
    private Color[] colors;
    //private bool updateMipMap = false;
    private bool sprayedLastFrame = false;
    private Vector2 lastSprayPos;
    private float performanceTimer = 0;
    public GameObject sprayCan;
    public bool canDraw = false;
    private GameObject sprayCanInstance;
    public float swayAmount = 2f;
    public float swaySpeed = 5f;
    private Quaternion initialSprayCanRotation;
    private Vector2 lastMousePosition;
    private ParticleSystem particleSprayCan;
    public Color[] possibleSprayColors = { Color.black, Color.white, Color.red, Color.orange, Color.yellow, Color.green, Color.blue, Color.indigo, Color.pink };
    private Transform sprayCanColorTransform;
    public AudioSource sprayCanSound;

    void Start()
    {
        var r = GetComponent<Renderer>();
        texture = new Texture2D((int)textureSize.x, (int)textureSize.y);
        //texture.alphaIsTransparency = true;

        Color[] initialPixels = Enumerable.Repeat(new Color(0, 0, 0, 0), (int)(textureSize.x * textureSize.y)).ToArray();
        texture.SetPixels(initialPixels);
        texture.Apply();

        r.material.mainTexture = texture;
        sprayCanInstance = Instantiate(sprayCan);
        sprayCanInstance.transform.rotation = transform.rotation;
        sprayCanInstance.transform.Rotate(90f, 0f, 0f, Space.Self);
        initialSprayCanRotation = sprayCanInstance.transform.rotation;
        particleSprayCan = sprayCanInstance.GetComponentInChildren<ParticleSystem>();
        sprayCanInstance.SetActive(false);
        sprayCanColorTransform = sprayCanInstance.transform.Find("Circle.003");
        updateSprayColor();
    }

    void updateSprayColor()
    {
        sprayColor = possibleSprayColors[sprayColorIdx];
        colors = Enumerable.Repeat(sprayColor, spraySize * spraySize).ToArray();
        var main = particleSprayCan.main;
        main.startColor = sprayColor;

        if (sprayCanColorTransform != null)
            sprayCanColorTransform.GetComponent<Renderer>().material.color = sprayColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canDraw)
        {
            sprayCanInstance.SetActive(false);
            return;
        }

        var scrollValue = Mouse.current.scroll.ReadValue().y;

        if (scrollValue > 0)
        {
            sprayColorIdx++;
            if (sprayColorIdx == possibleSprayColors.Length)
                sprayColorIdx = 0;

            updateSprayColor();
        }
        else if (scrollValue < 0)
        {
            sprayColorIdx--;
            if (sprayColorIdx < 0)
                sprayColorIdx = possibleSprayColors.Length - 1;

            updateSprayColor();
        }

        if (performanceTimer > 0)
        {
            performanceTimer -= Time.deltaTime;
            return;
        }

        var mouseScreenPosition = Mouse.current.position.ReadValue();
        var ray = mainCamera.ScreenPointToRay(mouseScreenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == this.gameObject)
        {
            sprayCanInstance.SetActive(true);
            Vector3 offset = mainCamera.transform.up * -1.7f + mainCamera.transform.forward * -2f;
            sprayCanInstance.transform.position = hit.point + offset;
        }
        else
        {
            sprayCanInstance.SetActive(false);
        }

        var mouseDelta = (mouseScreenPosition - lastMousePosition) * 0.1f;
        var targetRotation = initialSprayCanRotation * Quaternion.Euler(-mouseDelta.y * swayAmount, 0, -mouseDelta.x * swayAmount);
        sprayCanInstance.transform.rotation = Quaternion.Slerp(sprayCanInstance.transform.rotation, targetRotation, swaySpeed * Time.deltaTime);
        lastMousePosition = mouseScreenPosition;

        if (Mouse.current.leftButton.isPressed)
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (!sprayCanSound.isPlaying)
                    sprayCanSound.Play();

                if (hit.collider.gameObject != this.gameObject) return;

                if (!particleSprayCan.isPlaying)
                    particleSprayCan.Play();

                var touchPos = new Vector2(hit.textureCoord.x, hit.textureCoord.y);


                var x = (int)(touchPos.x * textureSize.x) - (spraySize / 2);
                var y = (int)(touchPos.y * textureSize.y) - (spraySize / 2);

                if (y < 0 || y > textureSize.y || x < 0 || x > textureSize.x) return;

                if (sprayedLastFrame)
                {
                    texture.SetPixels(x, y, spraySize, spraySize, colors);

                    for (float f = 0.01f; f < 1.00f; f += 0.01f)
                    {
                        var lerpX = (int)Mathf.Lerp(lastSprayPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(lastSprayPos.y, y, f);
                        texture.SetPixels(lerpX, lerpY, spraySize, spraySize, colors);
                    }

                    texture.Apply();

                    // updateMipMap = true;
                }

                lastSprayPos = new Vector2(x, y);
                sprayedLastFrame = true;
                performanceTimer += 0.02f;
                return;
            }
        }

        if (!Mouse.current.leftButton.isPressed && particleSprayCan.isPlaying)
        {
            sprayCanSound.Stop();
            sprayCanInstance.GetComponentInChildren<ParticleSystem>().Stop();
        }

        sprayedLastFrame = false;

        /* if (updateMipMap && !Mouse.current.leftButton.isPressed)
        {
            Debug.Log("GraffitiCanvas: Updating MipMap");

            texture.Apply(true);

            updateMipMap = false;
        } */
    }
}
