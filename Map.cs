public class Map
{
    public Tile[,] Tiles;

    public void Render()
    {
        Console.SetCursorPosition(0, 0);
        string map = "";
        for (int row = 0; row < Tiles.GetLength(1); row++)
        {
            for (int column = 0; column < Tiles.GetLength(0); column++)
            {
                if (Tiles[column, row].isPath == true) map += "#";
                else map += Tiles[column, row].Symbol;
            }
            map += '\n';         
        }
        Console.WriteLine(map);
    }

    public void RecalculateNeighbors()
    {
        for (int row = 0; row < Tiles.GetLength(1); row++)
        {
            for (int column = 0; column < Tiles.GetLength(0); column++)
            {
                Tiles[column, row].Neighbors = Tiles[column, row].GetNeighbors();
            }
        }
    }

    public Map(int width, int height)
    {
        Tiles = new Tile[width, height];
        for (int row = 0; row < Tiles.GetLength(1); row++)
        {
            for (int column = 0; column < Tiles.GetLength(0); column++)
            {
                Tiles[column, row] = new WallTile(this, column, row);
            }
        }

    }



    public void Smooth(Map map, int iterations)
    {

        while (iterations > 0)
        {
            Map mapCopy = GetMapCopy(map);
            
            for (int row = 0; row < Tiles.GetLength(1); row++)
            {
                for (int column = 0; column < Tiles.GetLength(0); column++)
                {
                    int wallCount = GetWallNeighborCount(mapCopy, column, row);

                    if (wallCount == 3 && map.Tiles[column, row] is WallTile) map.Tiles[column, row] = new EmptyTile(map, column, row);
                    if (wallCount <= 2 && map.Tiles[column, row] is WallTile) map.Tiles[column, row] = new EmptyTile(map, column, row);
                }
            }
            iterations--;
        }
    }

    public void GenerateWithCellularAutomata(Map map, int iterations)
    {
        while (iterations > 0)
        {
            Map mapCopy = GetMapCopy(map);


            for (int row = 0; row < Tiles.GetLength(1); row++)
            {
                for (int column = 0; column < Tiles.GetLength(0); column++)
                {
                    int wallCount = GetWallNeighborCount(mapCopy, column, row);

                    if (wallCount > 4) map.Tiles[column, row] = new WallTile(this, column, row);
                    else map.Tiles[column, row] = new EmptyTile(this, column, row);
                }

            }     
            iterations--;
        }
    }

    public bool IsTileInBounds(Map map, int column, int row)
    {
        int tilesPerColumn = map.Tiles.GetLength(0);
        int tilesPerRow = map.Tiles.GetLength(1);


        if (column < tilesPerColumn && row < tilesPerRow && column >= 0 && row >= 0) return true;
        else return false;
    }

    private int GetWallNeighborCount(Map map, int column, int row)
    {
        int wallNeighbors = 8;

        if (IsTileInBounds(map, column + 1, row))
        {
            if (map.Tiles[column + 1, row] is EmptyTile) wallNeighbors--;
        }       
        if (IsTileInBounds(map, column - 1, row))
        {
            if (map.Tiles[column - 1, row] is EmptyTile) wallNeighbors--;
        }
        if (IsTileInBounds(map, column, row + 1))
        {
            if (map.Tiles[column, row + 1] is EmptyTile) wallNeighbors--;
        }
        if (IsTileInBounds(map, column, row - 1))
        {
            if (map.Tiles[column, row - 1] is EmptyTile) wallNeighbors--;
        }
        if (IsTileInBounds(map, column + 1, row - 1))
        {
            if (map.Tiles[column + 1, row - 1] is EmptyTile) wallNeighbors--;
        }
        if (IsTileInBounds(map, column - 1, row + 1))
        {
            if (map.Tiles[column - 1, row + 1] is EmptyTile) wallNeighbors--;
        }
        if (IsTileInBounds(map, column + 1, row + 1))
        {
            if (map.Tiles[column + 1, row + 1] is EmptyTile) wallNeighbors--;
        }
        if (IsTileInBounds(map, column - 1, row - 1))
        {
            if (map.Tiles[column - 1, row - 1] is EmptyTile) wallNeighbors--;
        }
        return wallNeighbors;
    }


    public void AnimateWithCellularAutomata(Map map, int iterations, int speedInMilliseconds)
    {
        while (iterations > 0)
        {
            Map mapCopy = GetMapCopy(map);

            for (int row = 0; row < Tiles.GetLength(1); row++)
            {
                for (int column = 0; column < Tiles.GetLength(0); column++)
                {
                    int wallNeighbors = GetWallNeighborCount(mapCopy, column, row);

                    if (wallNeighbors > 4) map.Tiles[column, row] = new WallTile(this, column, row);
                    else map.Tiles[column, row] = new EmptyTile(this, column, row);


                }
            }

            Thread.Sleep(speedInMilliseconds);
            Render();
            iterations--;
        }
    }

    private static Map GetMapCopy(Map map)
    {
        Map mapCopy = new(map.Tiles.GetLength(0), map.Tiles.GetLength(1));
        for (int row = 0; row < map.Tiles.GetLength(1); row++)
        {
            for (int column = 0; column < map.Tiles.GetLength(0); column++)
            {
                if (map.Tiles[column, row] is WallTile) mapCopy.Tiles[column, row] = new WallTile(map, column, row);
                else mapCopy.Tiles[column, row] = new EmptyTile(map, column, row);
            }
        }
        return mapCopy;
    }

    public void GenerateNoise(Map map, double density)
    {
        Random r = new();

        for (int row = 0; row < Tiles.GetLength(1); row++)
        {
            for (int column = 0; column < Tiles.GetLength(0); column++)
            {
                double bias = r.NextDouble();

                Tiles[column, row] = bias >= density ? new EmptyTile(this, column, row) : new WallTile(this, column, row);
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
                Tiles[w.PositionX, w.PositionY] = new EmptyTile(this, w.PositionX, w.PositionY);
                remainingMoves--;
            }

            walkerCount--;
        }
    }
}
