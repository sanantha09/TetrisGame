using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino //it represents a set of named constants and provides a way to define a collection of related values that can be easily used in your code
{ //it can be used to represent a dinite set of options such as months, colours
    I,
    O,
    T,
    J,
    L,
    S,
    Z, 
}
[System.Serializable] //can convert data into serialized format and can be stored so can be sent
public struct TetrominoData
{
    public Tetromino tetromino; //the type of tetromino
    public Tile tile; //tile with the tetromino shape 
    public Vector2Int[] cells { get; private set; } //array of cell position for teronimo 
    public Vector2Int[,] wallKicks { get; private set; } //array of wall kick offsets for rotation

    public void Initialize()
    {
        this.cells = Data.Cells[this.tetromino]; //cell positions for the tetromino from a source
        this.wallKicks = Data.WallKicks[this.tetromino]; //wall kick offsets for the tetromino from a source
    }
}