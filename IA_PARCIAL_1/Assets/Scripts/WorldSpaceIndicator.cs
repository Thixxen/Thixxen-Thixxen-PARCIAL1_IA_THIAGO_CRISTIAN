using UnityEngine;

public class WorldSpaceIndicator : MonoBehaviour
{
    [Header("Indicator")]
    [SerializeField] private float height = 2.5f;
    [SerializeField] private int fontSize = 64;
    [SerializeField] private float characterSize = 0.15f;

    private TextMesh text;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;

        GameObject indicatorObject =
            new GameObject("IndicatorText");

        indicatorObject.transform.SetParent(transform);

        indicatorObject.transform.localPosition =
            new Vector3(0f, height, 0f);

        indicatorObject.transform.localRotation =
            Quaternion.identity;

        text =
            indicatorObject.AddComponent<TextMesh>();

        text.fontSize = fontSize;
        text.characterSize = characterSize;

        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;

        text.fontStyle = FontStyle.Bold;

        text.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (text == null || !text.gameObject.activeSelf)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
        {
            text.transform.LookAt(mainCamera.transform);

            text.transform.Rotate(
                0f,
                180f,
                0f
            );
        }
    }

    public void Show(string symbol, Color color)
    {
        if (text == null)
            return;

        text.text = symbol;
        text.color = color;

        text.gameObject.SetActive(true);
    }

    // Compatible con el Boid anterior
    public void Show()
    {
        Show("!", Color.red);
    }

    public void Hide()
    {
        if (text != null)
        {
            text.gameObject.SetActive(false);
        }
    }
}