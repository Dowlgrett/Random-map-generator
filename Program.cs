Map map = new(45, 16);
Walker walker = new(map);
map.GenerateWithWalker(walker, 1000);
map.Render();

Console.ReadKey();

public class Map
{
    public Tile[,] Tiles;

    public Tile? GetTileAtPosition(int x, int y)
    {
        if (x <= Tiles.GetLength(0) && y <= Tiles.GetLength(1)) return Tiles[x, y];
        else return null;
    }

    public void Render()
    {
        for (int row = 0; row < Tiles.GetLength(1); row++)
        {
            for (int column = 0; column < Tiles.GetLength(0); column++)
            {
                Console.Write(Tiles[column, row].Symbol);
            }
            Console.WriteLine();
        }
    }

    public Map(int width, int height)
    {
        Tiles = new Tile[width, height];
        for (int row = 0; row < Tiles.GetLength(1); row++)
        {
            for (int column = 0; column < Tiles.GetLength(0); column++)
            {
                Tiles[column, row] = new WallTile();
            }
        }

    }

    public void GenerateWithWalker(Walker w, int moveCount)
    {
        int remainingMoves = moveCount;

        Random r = new();

        //spawning walker at random map location
        int startX, startY;
        startX = r.Next(0, Tiles.GetLength(0) - 1);
        startY = r.Next(0, Tiles.GetLength(1) - 1);
        w.SetPosition(startX, startY);

        

        while (remainingMoves > 0)
        {
            //moving walker at random position on the map
            var newPosition = w.GetRandomMovePosition();
            w.SetPosition(newPosition.x, newPosition.y);
            Tiles[w.PositionX, w.PositionY] = new EmptyTile();
            remainingMoves--;
        }


    }

}

public abstract class Tile
{
    abstract public char Symbol { get; init; }

}

public class EmptyTile : Tile
{
    public override char Symbol { get; init; } = '.';
}

public class WallTile : Tile
{
    public override char Symbol { get; init; } = '#';
}

public class Walker
{
    public Map Map { get; private set; }
    public Random Random { get; private set; }

    public int PositionX { get; private set; } = 0;
    public int PositionY { get; private set; } = 0;

    public void SetPosition(int x, int y)
    {
        PositionX = x;
        PositionY = y;
    }

    public Walker(Map map)
    {
        Map = map;
        Random = new();
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

        if (newPosition.x == Map.Tiles.GetLength(0)) newPosition.x = 0;
        if (newPosition.y == Map.Tiles.GetLength(1)) newPosition.y = 0;
        if (newPosition.x == -1) newPosition.x = Map.Tiles.GetLength(0) - 1;
        if (newPosition.y == -1) newPosition.y = Map.Tiles.GetLength(1) - 1;


        return newPosition;
    }

}