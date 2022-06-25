



//0.59 density with 6 iterations yields good results!








public class Walker
{
    public Map Map { get; private set; }
    public static Random Random { get; private set; } = new Random();

    public int PositionX { get; private set; }
    public int PositionY { get; private set; }

    public void SetPosition(int x, int y)
    {
        PositionX = x;
        PositionY = y;
    }

    public Walker(Map map)
    {
        Map = map;
    }

    public (int x, int y) GetRandomMovePosition()
    {     
        int direction = Random.Next(0, 4);


        (int x, int y) newPosition = direction switch
        {
            0 => (PositionX + 1, PositionY), 
            1 => (PositionX - 1, PositionY),
            2 => (PositionX, PositionY + 1),
            3 => (PositionX, PositionY - 1),
            _ => throw new ArgumentException(),
        };

        if (newPosition.x >= Map.Tiles.GetLength(0) - 2) newPosition.x = 1;
        if (newPosition.y >= Map.Tiles.GetLength(1) - 2) newPosition.y = 1;
        if (newPosition.x <= 0) newPosition.x = Map.Tiles.GetLength(0) - 2;
        if (newPosition.y <= 0) newPosition.y = Map.Tiles.GetLength(1) - 2;


        return newPosition;
    }

}