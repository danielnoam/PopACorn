using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private TextMeshPro tileLabel;
    [SerializeField] private SpriteRenderer tileRenderer;
    
    
    
    public void SetTile(string text, Sprite sprite)
    {
        if (tileLabel) tileLabel.text = text;
         if (tileRenderer && sprite) tileRenderer.sprite = sprite;
         
         gameObject.name = $"Tile ({text})";
    }
    

}
