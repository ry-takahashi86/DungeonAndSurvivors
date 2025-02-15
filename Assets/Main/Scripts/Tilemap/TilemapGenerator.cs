using UnityEngine;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections;
using System.Text;

[ExecuteAlways]
public class TilemapGenerator : MMTilemapGenerator
{
    public enum SpawnCategory
    {
        Enemy,
        Item
    }

    [Serializable]
    public class SpawnData
    {
        public GameObject Prefab;
        public int Quantity = 1;
        public SpawnCategory Category;
    }

    [Header("Settings")]
    public bool GenerateOnAwake = false;        // Awake時に実行するか(Treu:実行, False:実行しない)

    [Header("Bindings")]
    public Grid TargetGrid;
    public Tilemap ObstaclesTilemap;
    public MMTilemapShadow WallsShadowTilemap;
    public MMTilemapTileReplacer GroundMinimapTilemap;
    public MMTilemapTileReplacer WallMinimapTilemap;
    public LevelManager TargetLevelManager;

    [Header("Spawn")]
    public Transform InitialSpawn;              // スタート地点
    public Transform Exit;                      // ゴール地点
    public List<SpawnData> PrefabsToSpawn;      // 生成するプレハブのリスト
    protected int _startX = 0;                  // スタート座標

    [Header("Tilemap Cleanup")]
    public Tilemap targetTilemap;                   // クリーン対象のタイルマップ
    public Sprite targetSprite;                     // クリーン対象のスプライト
    protected int _maxTileIterationsCount = 10;     // 最大タイルクリーン回数

    // ランダム生成されたマップを定義した二次元配列
    protected int[,] _mainCorridorGrid;         // MainCorridor
    protected int[,] _obstacleTilemapGrid;      // ObstacleTilemap

    // 隣接タイルのオフセット(5方向)
    private static readonly Vector3Int[] frontAdjacentOffsets = new Vector3Int[]
    {
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

        TilemapSmooth();
        HandleWallsShadow();
        HandleMinimap();
        ResizeLevelManager();

        Debug.Log("MainCorridorの結果");
        DebugTileArray(_mainCorridorGrid);
        Debug.Log("MainCorridorの反転結果");
        DebugTileArray(InvertArray(_mainCorridorGrid));
        Debug.Log("ObstacleTilemapのタイルチェック結果");
        DebugTileArray(GetObstacleGrid(ObstaclesTilemap));
        Debug.Log("MainCorridorの除外結果");
        DebugTileArray(MergeTileArrays(GetObstacleGrid(ObstaclesTilemap), InvertArray(_mainCorridorGrid)));

        PlaceStartAndExit();
        SpawnPrefabs();
        StartCoroutine(DelayedScan());
    }

    protected override void GenerateLayer(MMTilemapGeneratorLayer layer)
    {
        base.GenerateLayer(layer);

        switch (layer.Name)
        {
            case "MainCorridor":
                Debug.Log("MainCorridor を生成しました");

                // 整形した文字列をコンソールに出力
                _mainCorridorGrid = GetObstacleGrid(ObstaclesTilemap);
                break;
            case "Detour1":
                Debug.Log("Detour1 を生成しました");

                // 整形した文字列をコンソールに出力
                GetObstacleGrid(ObstaclesTilemap);
                break;
            case "Detour2":
                Debug.Log("Detour2 を生成しました");

                // 整形した文字列をコンソールに出力
                GetObstacleGrid(ObstaclesTilemap);
                break;
            case "Room":
                Debug.Log("Room を生成しました");

                // 整形した文字列をコンソールに出力
                GetObstacleGrid(ObstaclesTilemap);
                break;
        }
    }

    // 指定したタイルマップのタイル有無を判定して、二次元配列を作成する
    private int[,] GetObstacleGrid(Tilemap tilemap)
    {
        int[,] tileArray;

        // Tilemapの全体範囲を取得
        BoundsInt bounds = tilemap.cellBounds;

        // 二次元配列のサイズを決定
        // bounds.size.x, bounds.size.y はそれぞれ幅と高さ
        tileArray = new int[bounds.size.x, bounds.size.y];

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                // 現在のセル位置をVector3Intで指定
                Vector3Int pos = new Vector3Int(x, y, 0);

                // タイルの有無をチェック
                if (tilemap.HasTile(pos))
                {
                    // タイルが存在する場合、配列には 1 を格納
                    // 配列のインデックスは0から始まるので、bounds.xMinとbounds.yMin分をオフセットとして引く
                    tileArray[x - bounds.xMin, y - bounds.yMin] = 1;
                }
                else
                {
                    // タイルが存在しない場合は 0（初期値でもあるので明示的に代入してもよい）
                    tileArray[x - bounds.xMin, y - bounds.yMin] = 0;
                }
            }
        }

        return tileArray;
    }
    // 各要素を反転させた二次元配列を返す
    public int[,] InvertArray(int[,] array)
    {
        int rows = array.GetLength(0); // 行数
        int cols = array.GetLength(1); // 列数
        int[,] invertArray = new int[rows, cols];

        // 二重ループで全要素にアクセス
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                // 算術演算で反転：タイルの状態が0か1の場合、1 - 値で反転可能
                invertArray[i, j] = 1 - array[i, j];
            }
        }
        return invertArray;
    }
    // 各要素をマージして結果を返す
    public int[,] MergeTileArrays(int[,] array1, int[,] array2)
    {
        // 配列のサイズが同じであることを前提とする
        int rows = array1.GetLength(0);
        int cols = array1.GetLength(1);

        int[,] mergedArray = new int[rows, cols];

        // 各セルごとにマージ処理を実施
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                // どちらかの配列でタイルがある場合は、結果を1にする
                if (array1[i, j] == 1 || array2[i, j] == 1)
                {
                    mergedArray[i, j] = 1;
                }
                else
                {
                    mergedArray[i, j] = 0;
                }
            }
        }

        return mergedArray;
    }
    // 二次元配列の内容をコンソールに出力する
    public void DebugTileArray(int[,] array)
    {
        StringBuilder sb = new StringBuilder();
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);
        for (int y = cols - 1; y >= 0; y--)  // 上から下に表示するため、逆順でループ
        {
            for (int x = 0; x < rows; x++)
            {
                sb.Append(array[x, y] + " ");
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
    }


    /// <summary>
    /// 自動生成後に数秒遅延させてからA*パスをスキャンさせる
    /// </summary>
    /// <returns></returns>
    IEnumerator DelayedScan()
    {
        yield return new WaitForSeconds(1f);
        AstarPath.active.Scan();
    }

    /// <summary>
    /// 生成ロジックにランダム性を持たせる
    /// </summary>
    public void RandomChangeMap()
    {
        foreach (MMTilemapGeneratorLayer layer in Layers)
        {
            if (layer.Name == "MainCorridor")
            {
                layer.GridWidth = GridWidth.y - 5;
                layer.GridHeight = GridHeight.y;

                // スタート座標を左か右かランダムに決定する
                if (UnityEngine.Random.Range(0, layer.GridWidth) < layer.GridWidth / 2)
                {
                    _startX = 0;
                    layer.PathDirectionChangeDistance = -2;
                }
                else
                {
                    _startX = layer.GridWidth - 1;
                    layer.PathDirectionChangeDistance = 2;
                }
                layer.PathStartPosition.x = _startX;
            }

            if (layer.Name == "Detour1" || layer.Name == "Detour2")
            {
                layer.GridWidth = GridWidth.y - 5;
                layer.GridHeight = GridHeight.y - 5;

                if (_startX < layer.GridWidth / 2)
                {
                    layer.PathStartPosition.x = 0;
                    layer.PathDirection = MMGridGeneratorPath.Directions.LeftToRight;
                }
                else
                {
                    layer.PathStartPosition.x = layer.GridWidth;
                    layer.PathDirection = MMGridGeneratorPath.Directions.RightToLeft;
                }

                if (layer.Name == "Detour1")
                {
                    layer.PathStartPosition.y = UnityEngine.Random.Range(0, layer.GridHeight / 2);
                }
                else if (layer.Name == "Detour2")
                {
                    layer.PathStartPosition.y = UnityEngine.Random.Range(layer.GridHeight / 2, layer.GridHeight);
                }

                if (layer.PathStartPosition.y < layer.GridHeight / 2)
                {
                    layer.PathDirectionChangeDistance = UnityEngine.Random.Range(-3, 1);
                }
                else
                {
                    layer.PathDirectionChangeDistance = UnityEngine.Random.Range(1, 3);
                }
            }
        }
    }

    /// <summary>
    /// タイルマップをスムージングする
    /// </summary>
    public void TilemapSmooth()
    {
        // タイルマップのクリーンアップ
        for (int i = 0; i<_maxTileIterationsCount; i++)
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
                int adjacentCount = GetAdjacentTileCount(position, frontAdjacentOffsets);

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
                int adjacentCount = GetAdjacentTileCount(position, frontAdjacentOffsets);

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
    /// 指定位置の隣接タイルを取得する
    /// </summary>
    private int GetAdjacentTileCount(Vector3Int position, Vector3Int[] AdjacentOffsets)
    {
        int count = 0;
        foreach (Vector3Int offset in AdjacentOffsets)
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
    /// ミニマップ用のタイルマップを生成する
    /// </summary>
    public void HandleMinimap()
    {
        if (GroundMinimapTilemap != null)
        {
            GroundMinimapTilemap.UpdateTilemap();
        }
        if (WallMinimapTilemap != null)
        {
            WallMinimapTilemap.UpdateTilemap();
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
        boxCollider.size = new Vector2(bounds.size.x, bounds.size.y + 5);
    }

    /// <summary>
    /// スタートとゴールの座標を設定する
    /// </summary>
    public void PlaceStartAndExit()
    {
        if (InitialSpawn != null && Exit != null)
        {
            // ObstacleTilemapのグリッド配列を取得
            _obstacleTilemapGrid = GetObstacleGrid(ObstaclesTilemap);

            int width = _obstacleTilemapGrid.GetLength(0);      // 横方向のセル数
            int height = _obstacleTilemapGrid.GetLength(1);     // 縦方向のセル数

            int topRowStartZero = -1;
            int topRowEndZero = -1;
            int bottomRowStartZero = -1;
            int bottomRowEndZero = -1;

            // 一番上の行の0範囲を取得
            for (int x = 0; x < width; x++)
            {
                if (topRowStartZero < 0)
                {
                    if (_obstacleTilemapGrid[x, height - 1] == 0)
                    {
                        topRowStartZero = x;
                    }
                }
                else
                {
                    if (_obstacleTilemapGrid[x, height - 1] == 0)
                    {
                        topRowEndZero = x;
                    }
                }
            }
            // 一番下の行の0範囲を取得
            for (int x = 0; x < width; x++)
            {
                if (bottomRowStartZero < 0)
                {
                    if (_obstacleTilemapGrid[x, 0] == 0)
                    {
                        bottomRowStartZero = x;
                    }
                }
                else
                {
                    if (_obstacleTilemapGrid[x, 0] == 0)
                    {
                        bottomRowEndZero = x;
                    }
                }
            }

            // スタート座標を設定
            int bottomCenter = bottomRowStartZero + (bottomRowEndZero - bottomRowStartZero) / 2;
            Vector3Int startCell = new Vector3Int(bottomCenter - GridWidth.y / 2, 0 - GridHeight.y / 2, 0);
            InitialSpawn.position = ObstaclesTilemap.CellToWorld(startCell) - new Vector3(0.5f, 0.5f, 0f);
            InitialSpawn.localScale = new Vector3Int(1 + bottomRowEndZero - bottomRowStartZero, 1, 1);

            // ゴール座標を設定
            int topCenter = topRowStartZero + (topRowEndZero - topRowStartZero) / 2;
            Vector3Int exitCell = new Vector3Int(topCenter - GridWidth.y / 2, -1 + GridHeight.y / 2, 0);
            Exit.position = ObstaclesTilemap.CellToWorld(exitCell) - new Vector3(0.5f, 0.5f, 0f);
            Exit.localScale = new Vector3Int(1 + topRowEndZero - topRowStartZero, 1, 1);
        }
    }

    /// <summary>
    /// ランダムな座標にプレハブを生成する
    /// </summary>
    public void SpawnPrefabs()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        // スポーン座標の探索をする二次元配列
        int[,] spawnGrid = MergeTileArrays(_obstacleTilemapGrid, InvertArray(_mainCorridorGrid));

        // 有効セル座標のリスト
        List<Vector3Int> largevalidCells = new List<Vector3Int>();      // 周囲24マスが、0のセル
        List<Vector3Int> smallvalidCells = new List<Vector3Int>();      // 周囲8マスが、0のセル

        // Gridの行数・列数を取得する
        int width = spawnGrid.GetLength(0);
        int height = spawnGrid.GetLength(1);

        // 周囲24マスの隣接するセルが0のセル座標を取得する
        for (int i = 2; i < width - 2; i++)
        {
            for (int j = height - 3; 1 < j; j--)
            {
                bool allNeighborsZero = true;

                // 5×5の領域をチェックする
                for (int di = -2; di <= 2; di++)
                {
                    for (int dj = -2; dj <= 2; dj++)
                    {
                        if (spawnGrid[i+di, j+dj] != 0)
                        {
                            allNeighborsZero = false;
                            break;
                        }
                    }
                    if (!allNeighborsZero)
                    {
                        break;
                    }
                }

                // 周囲24セルがすべて0であれば、結果リストに追加
                if (allNeighborsZero)
                {
                    // Debug.Log($"有効な大きいセル座標: [{i}, {j}]");
                    largevalidCells.Add(new Vector3Int(i, j, 0));

                    // このセルと隣接セルを除外対象に設定
                    for (int di = -2; di <= 2; di++)
                    {
                        for (int dj = -2; dj <= 2; dj++)
                        {
                            // ※インデックスが配列範囲内であることを必要に応じて確認してください
                            spawnGrid[i + di, j + dj] = 1;
                        }
                    }
                }
            }
        }

        Debug.Log("大きい有効座標を減らした状態");
        DebugTileArray(spawnGrid);

        // 周囲8マスの隣接するセルが0のセル座標を取得する
        for (int i = 2; i < width - 2; i++)
        {
            for (int j = height - 3; 1 < j; j--)
            {
                bool allNeighborsZero = true;

                // 3×3の領域をチェックする
                for (int di = -1; di <= 1; di++)
                {
                    for (int dj = -1; dj <= 1; dj++)
                    {
                        if (spawnGrid[i+di, j+dj] != 0)
                        {
                            allNeighborsZero = false;
                            break;
                        }
                    }
                    if (!allNeighborsZero)
                    {
                        break;
                    }
                }

                // 周囲8セルがすべて0であれば、結果リストに追加
                if (allNeighborsZero)
                {
                    // Debug.Log($"有効な小さいセル座標: [{i}, {j}]");
                    smallvalidCells.Add(new Vector3Int(i, j, 0));

                    // このセルと隣接セルを除外対象に設定
                    for (int di = -1; di <= 1; di++)
                    {
                        for (int dj = -1; dj <= 1; dj++)
                        {
                            // ※インデックスが配列範囲内であることを必要に応じて確認してください
                            spawnGrid[i + di, j + dj] = 1;
                        }
                    }
                }
            }
        }

        Debug.Log($"大きい有効座標数: {largevalidCells.Count}");
        Debug.Log($"小さい有効座標数: {smallvalidCells.Count}");

        UnityEngine.Random.InitState(GlobalSeed);

        foreach (SpawnData spawnData in PrefabsToSpawn)
        {
            for (int i = 0; i < spawnData.Quantity; i++)
            {
                if (SpawnCategory.Enemy == spawnData.Category)
                {
                    int index = UnityEngine.Random.Range(0, largevalidCells.Count);
                    Vector3 spawnPosition = ObstaclesTilemap.CellToWorld(largevalidCells[index]) - new Vector3(GridWidth.y / 2, GridHeight.y / 2, 0);
                    Instantiate(spawnData.Prefab, spawnPosition, Quaternion.identity);

                    // 一度選ばれた座標は削除する
                    largevalidCells.RemoveAt(index);

                    if (largevalidCells.Count <= 0)
                    {
                        Debug.LogWarning("有効なセル座標がなくなりました");
                        break;
                    }
                }
                else if (SpawnCategory.Item == spawnData.Category)
                {
                    int index = UnityEngine.Random.Range(0, smallvalidCells.Count);
                    Vector3 spawnPosition = ObstaclesTilemap.CellToWorld(smallvalidCells[index]) - new Vector3(GridWidth.y / 2, GridHeight.y / 2, 0);
                    Instantiate(spawnData.Prefab, spawnPosition, Quaternion.identity);

                    // 一度選ばれた座標は削除する
                    smallvalidCells.RemoveAt(index);

                    if (smallvalidCells.Count <= 0)
                    {
                        Debug.LogWarning("有効なセル座標がなくなりました");
                        break;
                    }
                }
            }
            if (largevalidCells.Count <= 0 && smallvalidCells.Count <= 0)
            {
                Debug.LogWarning("有効なセル座標がなくなりました");
                break;
            }
        }
    }
}