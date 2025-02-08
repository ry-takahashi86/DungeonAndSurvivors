using UnityEngine;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections;

[ExecuteAlways]
public class TilemapGenerator : MMTilemapGenerator
{
    public enum SpawnCategory
    {
        Default,    // 制限なし
        Enemy       // スタートやゴール周辺には生成しない
    }

    [Serializable]
    public class SpawnData
    {
        public GameObject Prefab;
        public int Quantity = 1;
        public SpawnCategory SpawnCategory = SpawnCategory.Default;
    }

    [Header("Settings")]
    public bool GenerateOnAwake = false;        // Awake時に実行するか(Treu:実行, False:実行しない)

    public float EnemySafeDistance = 3f;        // 敵生成時、スタートやゴールからの安全距離

    [Header("Bindings")]
    public Grid TargetGrid;
    public Tilemap ObstaclesTilemap;
    public MMTilemapShadow WallsShadowTilemap;
    public LevelManager TargetLevelManager;

    [Header("Spawn")]
    public Transform InitialSpawn;              // プレイヤーのスポーン位置
    public Transform Exit;                      // レベルの出口
    public List<SpawnData> PrefabsToSpawn;      // 生成するプレハブのリスト
    public float PrefabsSpawnMinDistance = 2f;  // 生成するプレハブの最小距離

    [Header("Tilemap Cleanup")]
    public Tilemap targetTilemap;               // クリーン対象のタイルマップ
    public Sprite targetSprite;                 // クリーン対象のスプライト]

    protected int _maxIterationsCount = 1000;   // 最大繰り返し回数
    protected List<Vector3> _filledPositions;   // 塗りつぶし済みの座標リスト

    // 隣接タイルのオフセット
    private static readonly Vector3Int[] adjacentOffsets = new Vector3Int[]
    {
        // 8方向
        // new Vector3Int(-1, 1, 0), new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 0),
        // new Vector3Int(-1, 0, 0),                       new Vector3Int(1, 0, 0),
        // new Vector3Int(-1, -1, 0), new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 0)

        // 5方向
        new Vector3Int(-1, 1, 0), new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 0),
        new Vector3Int(-1, 0, 0),                       new Vector3Int(1, 0, 0),
    };

    private void Awake()
    {
        if (GenerateOnAwake)
        {
            // タイルマップの生成
            Generate();
        }
    }

    public override void Generate()
    {
        // 生成マップのルールにランダム性を持たせる
        RandomChangeMap();

        base.Generate();

        _filledPositions = new List<Vector3>();
        TilemapSmooth();
        HandleWallsShadow();
        ResizeLevelManager();
        PlaceStartAndExit();
        SpawnPrefabs();

        StartCoroutine(DelayedScan());
    }

    IEnumerator DelayedScan()
    {
        yield return new WaitForSeconds(1f);
        AstarPath.active.Scan();
    }

    /// <summary>
    /// メイン通路の開始位置と方向変更距離をランダムに変更する
    /// </summary>
    public void RandomChangeMap()
    {
        int startX = 0;
        int changeDistance = 0;

        foreach (MMTilemapGeneratorLayer layer in Layers)
        {
            if (layer.Name == "MainCorridor")
            {
                startX = UnityEngine.Random.Range(0, layer.GridWidth);
                layer.PathStartPosition.x = startX;

                changeDistance = UnityEngine.Random.Range(-2, 2);
                layer.PathDirectionChangeDistance = changeDistance;
            }

            if (layer.Name == "RandomWalk")
            {
                layer.RandomWalkStartingPoint.x = startX;
            }
        }
    }

    /// <summary>
    /// タイルマップをスムージングする
    /// </summary>
    public void TilemapSmooth()
    {
        // タイルマップのクリーンアップ
        for (int i = 0; i<_maxIterationsCount; i++)
        {
            CleanupTiles();

            if (CheckTiles())
            {
                return;
            }
        }
    }

    /// <summary>
    /// タイルをクリーンアップする
    /// </summary>
    public void CleanupTiles()
    {
        if (targetTilemap == null || targetSprite == null)
        {
            Debug.LogError("Tilemapまたはターゲットスプライトが指定されていません");
            return;
        }

        // 対象タイルマップの範囲を取得
        BoundsInt bounds = targetTilemap.cellBounds;
        TileBase[] allTiles = targetTilemap.GetTilesBlock(bounds);

        // 対象タイルをチェックし、削除する
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                TileBase tile = targetTilemap.GetTile(position);

                if (tile == null) continue;

                // タイルのスプライトを取得
                Sprite sprite = GetTileSprite(position);

                // 隣接タイル数を取得
                int adjacentCount = GetAdjacentTileCount(position);

                // 削除条件: 隣接タイルが3マス未満 && タイルのスプライトが一致
                if (adjacentCount < 4 && sprite == targetSprite)
                {
                    targetTilemap.SetTile(position, null);
                }
            }
        }
    }

    /// <summary>
    /// 不要なタイルが存在していたらfalseを返す
    /// </summary>
    /// <returns></returns>
    public bool CheckTiles()
    {
        // 対象タイルマップの範囲を取得
        BoundsInt bounds = targetTilemap.cellBounds;
        TileBase[] allTiles = targetTilemap.GetTilesBlock(bounds);

        // 対象タイルをチェックし、削除する
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                TileBase tile = targetTilemap.GetTile(position);

                if (tile == null) continue;

                // タイルのスプライトを取得
                Sprite sprite = GetTileSprite(position);

                // 隣接タイル数を取得
                int adjacentCount = GetAdjacentTileCount(position);

                // 削除条件: 隣接タイルが3マス未満 && タイルのスプライトが一致
                if (adjacentCount < 4 && sprite == targetSprite)
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 指定位置のタイルのスプライトを取得する
    /// </summary>
    private Sprite GetTileSprite(Vector3Int position)
    {
        TileBase tile = targetTilemap.GetTile(position);

        if (tile is Tile tileWithSprite)
        {
            // Tile の場合、スプライトを取得
            return tileWithSprite.sprite;
        }
        else if (tile is RuleTile ruleTile)
        {
            // RuleTile の場合、タイルデータを取得する
            TileData TileData = new TileData();
            ITilemap tilemap = targetTilemap.GetComponent<Tilemap>();
            ruleTile.GetTileData(position, tilemap, ref TileData);
            return TileData.sprite;
        }

        return null; // スプライトが取得できなかった場合
    }

    /// <summary>
    /// 指定位置の隣接タイル数を取得する
    /// </summary>
    private int GetAdjacentTileCount(Vector3Int position)
    {
        int count = 0;
        foreach (Vector3Int offset in adjacentOffsets)
        {
            Vector3Int adjacentPosition = position + offset;
            if (targetTilemap.GetTile(adjacentPosition) != null)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// 壁タイルマップを参照して影を生成する
    /// </summary>
    public void HandleWallsShadow()
    {
        if (WallsShadowTilemap != null)
        {
            WallsShadowTilemap.UpdateShadows();
        }
    }

    /// <summary>
    /// レベルマネージャーのカメラ範囲判定をリサイズする
    /// </summary>
    public void ResizeLevelManager()
    {
        BoxCollider2D boxCollider = TargetLevelManager.GetComponent<BoxCollider2D>();

        Bounds bounds = ObstaclesTilemap.localBounds;
        boxCollider.offset = bounds.center;
        boxCollider.size = new Vector2(bounds.size.x, bounds.size.y);
    }

    public void PlaceStartAndExit()
    {
        if (InitialSpawn != null && Exit != null)
        {
            int width = GridWidth.y / 2;
            int height = GridHeight.y / 2;
            Vector3Int startCell = new Vector3Int(-width + 5, -height + 5, 0);
            Vector3Int exitCell = new Vector3Int(width - 5, height - 5, 0);

            while (startCell.x < width)
            {
                if (!ObstaclesTilemap.HasTile(startCell))
                {
                    if (GetAdjacentTileCount(startCell) == 0)
                    {
                        break;
                    }
                }
                startCell.x++;
            }

            while (exitCell.x > -width)
            {
                if (!ObstaclesTilemap.HasTile(exitCell))
                {
                    if (GetAdjacentTileCount(exitCell) == 0)
                    {
                        break;
                    }
                }
                exitCell.x--;
            }

            InitialSpawn.position = ObstaclesTilemap.CellToWorld(startCell) + new Vector3(0.5f, 0.5f, 0);
            Exit.position = ObstaclesTilemap.CellToWorld(exitCell) + new Vector3(0.5f, 0.5f, 0);
        }
    }

    /// <summary>
    /// ランダムな座標にプレハブを生成する
    /// </summary>
    public void SpawnPrefabs()
    {
        // ゲーム中でなければ処理しない
        if (!Application.isPlaying)
        {
            return;
        }

        UnityEngine.Random.InitState(GlobalSeed);

        // Obstacles Tilemap のセル範囲を取得
        BoundsInt tileBonuds = ObstaclesTilemap.cellBounds;
        int cellXMin = tileBonuds.xMin;
        int cellXMax = tileBonuds.xMax;
        int cellYMin = tileBonuds.yMin;
        int cellYMax = tileBonuds.yMax;

        int width = UnityEngine.Random.Range(GridWidth.x, GridWidth.y);
        int height = UnityEngine.Random.Range(GridHeight.x, GridHeight.y);

        foreach (SpawnData data in PrefabsToSpawn)
        {
            for (int i = 0; i < data.Quantity; i++)
            {
                Vector3 spawnPosition = Vector3.zero;
                bool validPosition = false;
                int iterationsCount = 0;

                while (!validPosition && (iterationsCount < _maxIterationsCount))
                {
                    switch (data.SpawnCategory)
                    {
                        case SpawnCategory.Enemy:
                            // まずは全体のランダム位置を取得（ここでは既存の GetRandomPosition を利用）
                            spawnPosition = MMTilemap.GetRandomPosition(ObstaclesTilemap, TargetGrid, width, height, false, width * height * 2);
                            break;

                        case SpawnCategory.Default:
                        default:
                            // 制限なしの場合は、全体のランダム位置
                            spawnPosition = MMTilemap.GetRandomPosition(ObstaclesTilemap, TargetGrid, width, height, false, width * height * 2);
                            break;
                    }

                    // Enemy 用の場合、スタート地点やゴール地点の周辺は避ける
                    if (data.SpawnCategory == SpawnCategory.Enemy)
                    {
                        if ((InitialSpawn != null && Vector3.Distance(spawnPosition, InitialSpawn.position) < EnemySafeDistance) ||
                            (Exit != null && Vector3.Distance(spawnPosition, Exit.position) < EnemySafeDistance))
                        {
                            iterationsCount++;
                            continue; // 安全距離内なら再試行
                        }
                    }

                    // 既存処理: 既に配置済みオブジェクトとの最小距離チェック
                    bool tooClose = false;
                    foreach (Vector3 filledPosition in _filledPositions)
                    {
                        if (Vector3.Distance(spawnPosition, filledPosition) < PrefabsSpawnMinDistance)
                        {
                            tooClose = true;
                            break;
                        }
                    }
                    if (tooClose)
                    {
                        iterationsCount++;
                        continue;
                    }

                    validPosition = true;
                }

                // 有効な位置が見つかったら生成し、配置済みリストに追加
                Instantiate(data.Prefab, spawnPosition, Quaternion.identity);
                _filledPositions.Add(spawnPosition);
            }
        }
    }
}