public class TileInfo{

    private int x, y;
    private float durability;
    private bool flagEroded;

    public TileInfo(int x, int y){
        this.durability = 1.0f;
        this.flagEroded = false;
        this.x = x;
        this.y = y;
    }

    public int[] GetCoordinates()
    {
        return new int[2] {x, y};
    }

    public int[,] GetNeighboursCoordinates()
    {
        int[,] neighbours = new int [6,2];
        neighbours[0, 0] = this.x+1;
        neighbours[0, 1] = this.y-1;

        neighbours[1, 0] = this.x+1;
        neighbours[1, 1] = this.y;

        neighbours[2, 0] = this.x;
        neighbours[2, 1] = this.y+1;

        neighbours[3, 0] = this.x-1;
        neighbours[3, 1] = this.y+1;

        neighbours[4, 0] = this.x-1;
        neighbours[4, 1] = this.y;

        neighbours[5, 0] = this.x;
        neighbours[5, 1] = this.y-1;

        return neighbours;
    }

    public void decreaseDurability( float value )
    {
        this.durability -= value;
    }

}