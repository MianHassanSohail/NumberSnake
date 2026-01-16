using TMPro;
using UnityEngine;

public class ChainNumber : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;
    private int value;

    private void Awake()
    {
        if (textMesh == null)
            textMesh = GetComponentInChildren<TextMeshPro>();
    }

    public void SetValue(int val)
    {
        value = val;
        if (textMesh != null)
        {
            textMesh.text = value.ToString();
        }
    }

    public int GetValue() => value;
}
