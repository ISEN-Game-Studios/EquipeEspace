using System.Collections;
using UnityEngine;

public class TextMeshTyper : MonoBehaviour
{
    [SerializeField]
    private TextMesh textMesh;

    public float Duration = 0f;

    private string text;

    private void Start()
    {
        SetText(textMesh.text);
    }

    public void SetText(string text)
    {
        StopAllCoroutines();

        this.text = text;
        textMesh.text = "";
    }

    public void Type()
    {
        StopAllCoroutines();

        StartCoroutine(_Type());
    }

    private IEnumerator _Type()
    {
        if (Duration > 0f)
        {
            textMesh.text = "";

            for (int i = 0; i < text.Length; ++i)
            {
                textMesh.text += text[i];

                yield return new WaitForSeconds(Duration / text.Length);
            }
        }
        else
            textMesh.text = text;
    }
}
