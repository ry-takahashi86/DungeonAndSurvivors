using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using MoreMountains.Tools;

/// <summary>
/// ReferenceTilemapの各タイル位置をコピーし、全てのタイルを指定のReplacementTileに置き換えるクラスです。
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(Tilemap))]
public class MMTilemapTileReplacer : MonoBehaviour
{
    [Tooltip("コピー元のTilemap")]
    public Tilemap ReferenceTilemap;

    [Tooltip("置き換えに使用するTile")]
    public TileBase ReplacementTile;

    [MMInspectorButton("UpdateTilemap")]
    public bool UpdateShadowButton;

    protected Tilemap _tilemap;

    /// <summary>
    /// ReferenceTilemapを元に、現在のTilemapのタイルをReplacementTileに置き換えます
    /// </summary>
    public virtual void UpdateTilemap()
    {
        if (ReferenceTilemap == null || ReplacementTile == null)
        {
            Debug.LogWarning("ReferenceTilemapまたはReplacementTileが設定されていません。");
            return;
        }

        _tilemap = this.gameObject.GetComponent<Tilemap>();
        ReplaceTiles(ReferenceTilemap, _tilemap, ReplacementTile);
    }

    /// <summary>
    /// コピー元のTilemapの全タイル位置に対して、指定したTileに置き換えます
    /// </summary>
    /// <param name="source">コピー元のTilemap</param>
    /// <param name="destination">配置先のTilemap</param>
    /// <param name="replacement">置き換え用のTile</param>
    public static void ReplaceTiles(Tilemap source, Tilemap destination, TileBase replacement)
    {
        // 両Tilemapのタイルを更新
        source.RefreshAllTiles();
        destination.RefreshAllTiles();

        List<Vector3Int> positions = new List<Vector3Int>();

        // コピー元のTilemapからタイルが配置されている全位置を取得
        foreach (Vector3Int pos in source.cellBounds.allPositionsWithin)
        {
            if (source.HasTile(pos))
            {
                positions.Add(pos);
            }
        }

        // 位置リストを配列に変換
        Vector3Int[] positionsArray = positions.ToArray();
        TileBase[] tilesArray = new TileBase[positions.Count];

        // 全てのタイルをreplacementに設定
        for (int i = 0; i < positions.Count; i++)
        {
            tilesArray[i] = replacement;
        }

        // destinationを初期化し、コピー元のサイズ・原点を引き継ぐ
        destination.ClearAllTiles();
        destination.RefreshAllTiles();
        destination.size = source.size;
        destination.origin = source.origin;
        destination.ResizeBounds();

        // 指定位置に置き換えたタイルを配置
        destination.SetTiles(positionsArray, tilesArray);
    }
}