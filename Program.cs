
Map map = new(150, 50);
map.GenerateNoise(map, density: 0.57);
map.GenerateWithCellularAutomata(map, 4);
map.Render();



Console.ReadKey();

public class Map
{
    public Tile[,] Tiles;



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

    public void GenerateWithCellularAutomata(Map map, int iterations)
    {
        while (iterations > 0)
        {
            Map mapCopy = new(map.Tiles.GetLength(0), map.Tiles.GetLength(1));
            for (int row = 0; row < map.Tiles.GetLength(1) - 1; row++)
            {
                for (int column = 0; column < map.Tiles.GetLength(0) - 1; column++)
                {
                    if (map.Tiles[column, row] is WallTile) mapCopy.Tiles[column, row] = new WallTile();
                    else mapCopy.Tiles[column, row] = new EmptyTile();
                }
            }


            for (int row = 1; row < Tiles.GetLength(1) - 2; row++)
            {
                for (int column = 1; column < Tiles.GetLength(0) - 2; column++)
                {
                    int wallNeighbors = 0;

                    if (mapCopy.Tiles[column + 1, row] is WallTile) wallNeighbors++; 
                    if (mapCopy.Tiles[column - 1, row] is WallTile) wallNeighbors++;
                    if (mapCopy.Tiles[column, row + 1] is WallTile) wallNeighbors++;
                    if (mapCopy.Tiles[column, row - 1] is WallTile) wallNeighbors++;
                    if (mapCopy.Tiles[column + 1, row - 1] is WallTile) wallNeighbors++;
                    if (mapCopy.Tiles[column - 1, row + 1] is WallTile) wallNeighbors++;
                    if (mapCopy.Tiles[column + 1, row + 1] is WallTile) wallNeighbors++;
                    if (mapCopy.Tiles[column - 1, row - 1] is WallTile) wallNeighbors++;

                    if (wallNeighbors > 4) map.Tiles[column, row] = new WallTile();
                    else map.Tiles[column, row] = new EmptyTile();
                }
            }
            iterations--;
        }
    }


    public void GenerateNoise(Map map, double density)
    {
        Random r = new();

        for (int row = 1; row < Tiles.GetLength(1) - 1; row++)
        {
            for (int column = 1; column < Tiles.GetLength(0) - 1; column++)
            {
                double bias = r.NextDouble();

                Tiles[column, row] = bias >= density ? new EmptyTile() : new WallTile();
            }
        }



      


    }

    public void GenerateWithWalker(Map map, int moveCount, int walkerCount)
    {
        while (walkerCount > 0)
        {
            int remainingMoves = moveCount;
            
            Walker w = new Walker(map);
            //spawning walker at random map location
            int startX, startY;
            startX = Walker.Random.Next(0, Tiles.GetLength(0) - 1);
            startY = Walker.Random.Next(0, Tiles.GetLength(1) - 1);
            w.SetPosition(startX, startY);

        

            while (remainingMoves > 0)
            {
                //moving walker at random position on the map
                var newPosition = w.GetRandomMovePosition();
                w.SetPosition(newPosition.x, newPosition.y);
                Tiles[w.PositionX, w.PositionY] = new EmptyTile();
                remainingMoves--;
            }

            walkerCount--;
        }
    }
}

public abstract class Tile
{
    abstract public char Symbol { get; init; }

}

public class EmptyTile : Tile
{
    public override char Symbol { get; init; } = ' ';
}

public class WallTile : Tile
{
    public override char Symbol { get; init; } = '\u2593';
}

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

        if (newPosition.x == Map.Tiles.GetLength(0) - 1) newPosition.x = 1;
        if (newPosition.y == Map.Tiles.GetLength(1) - 1) newPosition.y = 1;
        if (newPosition.x == 0) newPosition.x = Map.Tiles.GetLength(0) - 2;
        if (newPosition.y == 0) newPosition.y = Map.Tiles.GetLength(1) - 2;


        return newPosition;
    }

}