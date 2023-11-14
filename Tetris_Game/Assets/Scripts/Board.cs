using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; } //tilemap
    public Piece activePiece { get; private set; } //active piece on the board
    public TetrominoData[] tetrominoes; //array of the tetromino data objects
    public Vector3Int spawnPosition; //position of where pieces will be spawned originally 
    public Vector2Int boardSize = new Vector2Int(10, 20); //size of the game board

    public RectInt Bounds //takes the position and size of vector2Int 
        //it is a structure in unity that represents a 2d axis alligned rectangle with integer coordinates
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(position, this.boardSize);
        }
    }


    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>(); //tilemap component attached to the object
        this.activePiece = GetComponentInChildren<Piece>(); //piece component attached to the object 

        for (int i = 0; i < this.tetrominoes.Length; i++) {
            this.tetrominoes[i].Initialize(); //initialize each data 
        }
    }  

    private void Start()
    {
        SpawnPiece(); //spwan a new piece 
    }

    public void SpawnPiece()
    {
        int random = Random.Range(0, this.tetrominoes.Length); //random spawn piece to the given length 
        TetrominoData data = this.tetrominoes[random]; //get a random teromino data from their arrayy

        this.activePiece.Initialize(this, this.spawnPosition, data); //active piece initialized with the selected data and set position to spawn position

        if (IsValidPosition(this.activePiece, this.spawnPosition))
        {
            Set(this.activePiece); //if valid set the piece
        }
        else
        {
            GameOver(); //if not valid game over
        }
    }

    private void GameOver()
    {
        this.tilemap.ClearAllTiles(); //clear all tiles  
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position; //tile posiiton based on piece's cell postions and overall position
            this.tilemap.SetTile(tilePosition, null); //set tile at calculated position on tilemap using the corresponding tile from the piece's data
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = this.Bounds; //rectangular bonds of game board

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition)) { //if tile position outside the bonds then false
                return false;
            }

            if (this.tilemap.HasTile(tilePosition)) {
                return false; //if tilemap has a tile a tile at the tile positions then true 
            }
        }

        return true; //if all tile positions are valid and no collisions then it is true
    }

    public void ClearLines()
    {
        RectInt bounds = this.Bounds;
        int row = bounds.yMin; //iteration from the bottom row of the game board

        while (row < bounds.yMax)
        {
            if (IsLineFull(row)) { //if current row is full, clear the line and shift the above line down
                LineCLear(row);
            } else {
                row++; //move to the next row if the existing one is not full
            }
        }
    }

    private bool IsLineFull(int row)
    {
        RectInt bounds = this.Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0); //position vector for each tile in the row

            if (!this.tilemap.HasTile(position)) {
                return false; //if any tile in the row is empty, return false, line is not full
            }
        }

        return true; //when line is full
    }

    private void LineCLear(int row)
    {
        RectInt bounds = this.Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            this.tilemap.SetTile(position, null); //clear each tile in the full row by setting it to null
        }

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = this.tilemap.GetTile(position); //tile aboce the current postion 

                position = new Vector3Int(col, row, 0); //creates a new object with specific column and row and 0 for the z value 
                this.tilemap.SetTile(position, above); //move the tile from the above position down to the current row 
            }

            row++; //move to the next row 
        }
    }

}