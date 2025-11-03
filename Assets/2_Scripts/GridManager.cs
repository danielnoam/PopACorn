using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DNExtensions;
using DNExtensions.Button;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private SOGridShape gridShape;
    [SerializeField] private Vector3 gridOffset = Vector3.zero;
    [SerializeField] private Vector2 tileSize = Vector2.one;
    [SerializeField] private Vector2 tileOffset = Vector2.zero;
    [SerializeField] private Tile tilePrefab;
    

    
    
    [Space(25f)][Separator("Debug")]
    [SerializeField] private bool drawGrid;
    [SerializeField] private bool drawOnlyInactive = true;
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.white;
    [SerializeField] private GUIStyle labelStyle = new GUIStyle();
    

    private readonly Dictionary<Vector2Int, Tile> _tiles = new Dictionary<Vector2Int, Tile>();
    
    


    private Tile CreateTile(Vector3 position, Transform parent)
    {
        var tile = Instantiate(tilePrefab, position, Quaternion.identity, parent);
        tile.transform.localScale = new Vector3(tileSize.x, tileSize.y, 1f);

        return tile;
    }
    
    [Button]
    private void UpdateGrid()
    {
        if (!gridShape || !tilePrefab) return;

        
        ClearGrid();
        
        var grid = gridShape.Grid;
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                var tileState = grid.IsTileActive(x, y);
                
                if (tileState)
                {
                    Vector3 tilePosition = gridOffset + new Vector3(
                        (grid.Width - 1 - x) * (tileSize.x + tileOffset.x), 
                        (grid.Height - 1 - y) * (tileSize.y + tileOffset.y), 
                        0
                    );

                    var tile = CreateTile(tilePosition, transform);
                    if (tile)
                    {

                        tile.SetTile($"{x},{y}", null);
                    }
                    _tiles.Add(new Vector2Int(x, y), tile);
                }
            }
        }
    }
    
    [Button]
    private void ClearGrid()
    {
        // Clear dictionary tiles first
        foreach (var tile in _tiles.Values)
        {
            if (tile)
            {
#if UNITY_EDITOR
                DestroyImmediate(tile.gameObject);
#else
            Destroy(tile.gameObject);
#endif
            }
        }
        _tiles.Clear();
    
        // Clear any remaining children in reverse order to avoid iteration issues
#if UNITY_EDITOR
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
#else
    foreach (Transform child in transform)
    {
        Destroy(child.gameObject);
    }
#endif
    }
    
    
#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (!gridShape || !drawGrid) return;
    
        var grid = gridShape.Grid;

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                var tileState = grid.IsTileActive(x, y);
                
                if (drawOnlyInactive && tileState) continue;
                
                Vector3 tilePosition = gridOffset + new Vector3(
                    (grid.Width - 1 - x) * (tileSize.x + tileOffset.x), 
                    (grid.Height - 1 - y) * (tileSize.y + tileOffset.y), 
                    0
                );
                

                Gizmos.color = tileState ? activeColor : inactiveColor;

                Gizmos.DrawWireCube(tilePosition, tileSize);
                Handles.Label(tilePosition, $"{x},{y}", labelStyle);
            }
        }
    }
#endif
}