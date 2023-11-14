using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; } //game board reference
    public TetrominoData data { get; private set; } //data for the piece (tetromino)
    public Vector3Int[] cells { get; private set; } //cell positions for piece
    public Vector3Int position { get; private set; } //current position of piece
    public int rotationIndex { get; private set; } //current rotation index of piece

    public float stepDelay = 1f; //downward delay
    public float lockDelay = 0.5f; //delay before locking the piece

    private float stepTime; //time of the next step
    private float lockTime; //time since last locked piece

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board; //assigning the game board to piece
        this.position = position; //initial posiiton of the piece to be set
        this.data = data; //assigning the tetromino date to the piece
        this.rotationIndex = 0; //rotational index of piece 0
        this.stepTime = Time.time + this.stepDelay; //setting next automatic time based of current time and step delay 
        this.lockTime = 0f; //lock time is 0

        if (this.cells == null) {
            this.cells = new Vector3Int[data.cells.Length]; //if not initialized make a new array with the length of data cells
        }

        for (int i = 0; i < data.cells.Length; i++) { //increment value of i by 1 after iteration of the loop
            this.cells[i] = (Vector3Int)data.cells[i]; //copying the posiitons from the data to the cell array
        }
    }

    private void Update()
    {
        this.board.Clear(this); //clear the board 

        this.lockTime += Time.deltaTime; //increment the lock time based of the elapsed time since the last frame 

        if (Input.GetKeyDown(KeyCode.Q)) {
            Rotate(-1); //anticlockwise
        } else if (Input.GetKeyDown(KeyCode.E)) {
            Rotate(1); //clockwise
        }

        if (Input.GetKeyDown(KeyCode.A)) {
            Move(Vector2Int.left); //shift to left
        } else if (Input.GetKeyDown(KeyCode.D)) {
            Move(Vector2Int.right); //shift to right
        }

        if (Input.GetKeyDown(KeyCode.S)) {
            Move(Vector2Int.down);//shift down
        }
        
        if (Input.GetKeyDown(KeyCode.Space)) {
            HardDrop();//drops all the way down 
        }

        if (Time.time >= this.stepTime) {
            Step();//if the current time is greater than the step time then perform a step 
        }
        
        this.board.Set(this);//updated position of the piece 
    }

    private void Step()
    {
        this.stepTime = Time.time + this.stepDelay;//time updated with the step delay to current time 

        Move(Vector2Int.down);//moves it down if time exceeds

        if (this.lockTime >= this.lockDelay) {
            Lock(); //if it exceeds or equals the lock delay then lock the piece
        }
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down)) { 
            continue;
        }

        Lock(); //dwon to the bottom and lock 
    }

    private void Lock()
    {
        this.board.Set(this); //set the current piece on the game board
        this.board.ClearLines(); //clears completed lines
        this.board.SpawnPiece(); //new piece
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x; //new position
        newPosition.y += translation.y;

        bool valid = this.board.IsValidPosition(this, newPosition); 

        if (valid) 
        { 
            this.position = newPosition; //update position if valid
            this.lockTime = 0f; //reset lock time as move has been occured 
        }

        return valid; //valid or invalid movement 
    }

    private void Rotate(int direction)
    {
        int originalRotation = this.rotationIndex; //original rotation by current rotation index
        this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4); //wraps within a range of 0-4

        ApplyRotationMatrix(direction); //apply rotation matrix to the cell position

        if (!TestWallKicks(this.rotationIndex, direction)) //check if wall kicks are possible for the new rotation and direction
        {
            this.rotationIndex = originalRotation; //revert the rotation index to its original value
            ApplyRotationMatrix(-direction); //go back to applied rotation
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3 cell = this.cells[i];

            int x, y;

            switch (this.data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f; //0.5 on the x axis
                    cell.y -= 0.5f; //0/5 on the y axis 
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;

            }

            this.cells[i] = new Vector3Int(x, y, 0); //new cell position with calculated values 
        }
    }


    private bool TestWallKicks(int rotationIndex, int rotationDirection) 
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < this.data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = this.data.wallKicks[wallKickIndex, i];

            //attempt to move tetromino using the wall kick translation
            if (Move(translation)) { 
                return true; //if move is correct = successful move
            }
        }

        return false; //invalid move
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0) {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, this.data.wallKicks.GetLength(0)); //stays within the index of wall kick and the valid range
    }

    private int Wrap(int input, int min, int max) //allows movement against a wall or a obstacle 
    {
        if (input < min) {
            return max - (min - input) % (max - min);
        } else {
            return min + (input - min) % (max - min);
        }
    }
}

