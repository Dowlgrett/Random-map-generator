int width = 80;
int height = 40;

Map map = new(width, height);
Pathfinder pf = new(map);

//0.59 density with 6 iterations yields good results!

while (true)
{
    map.GenerateNoise(map, density: 0.59);


    while (true)
    {
        map.RecalculateNeighbors();
        var path = pf.FindPath(map.Tiles[10, 10], map.Tiles[width - 10, height - 10]);
        map.Render();
        ConsoleKeyInfo key = Console.ReadKey(true);
        if (key.KeyChar == 's')
        {
            map.Smooth(map, 1);
        }
        if (key.KeyChar == 'c')
        {
            map.GenerateWithCellularAutomata(map, 1);
        }
        if (key.KeyChar == 'r')
        {
            break;
        }


        
    }
}




public abstract class Tile
{
    public Map Map { get; set; }
    public int Column { get; set; }
    public int Row { get; set; }

    public bool isPath = false;
    abstract public char Symbol { get; init; }
    const int DIAGONAL_COST = 14;
    const int ORTHOGONAL_COST = 10;

    public int FValue { get; set; }
    public int GValue { get; set; }
    public int HValue { get; set; }

    public Tile? CameFrom { get; set; }
    public List<Tile> Neighbors { get; set; }

    public int CalculateG()
    {
        throw new NotImplementedException();
    }

    public int CalculateH(Tile target)
    {
        //diagonal distance heuristics http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html#S7
        
        int dx = Math.Abs(this.Column - target.Column);
        int dy = Math.Abs(this.Row - target.Row);
        return ORTHOGONAL_COST * (dx + dy) + (DIAGONAL_COST - 2 * ORTHOGONAL_COST) * Math.Min(dx, dy);
    }

    public Tile(Map map, int column, int row)
    {
        Map = map;
        Column = column;
        Row = row;  
        GValue = int.MaxValue;
        FValue = int.MaxValue;
    }

    public List<Tile>? GetNeighbors()
    {
        List<Tile> neighbors = new();

        if (Map.Tiles[Column, Row] is WallTile) return null;

        if (Map.IsTileInBounds(Map, Column + 1, Row) && Map.Tiles[Column + 1, Row] is not WallTile)
        {
            neighbors.Add(Map.Tiles[Column + 1, Row]);
        }
        if (Map.IsTileInBounds(Map, Column - 1, Row) && Map.Tiles[Column - 1, Row] is not WallTile)
        {
            neighbors.Add(Map.Tiles[Column - 1, Row]);
        }
        if (Map.IsTileInBounds(Map, Column, Row + 1) && Map.Tiles[Column, Row + 1] is not WallTile)
        {
            neighbors.Add(Map.Tiles[Column, Row + 1]);
        }
        if (Map.IsTileInBounds(Map, Column, Row - 1) && Map.Tiles[Column, Row - 1] is not WallTile)
        {
            neighbors.Add(Map.Tiles[Column, Row - 1]);
        }
        if (Map.IsTileInBounds(Map, Column + 1, Row - 1) && Map.Tiles[Column + 1, Row - 1] is not WallTile)
        {
            neighbors.Add(Map.Tiles[Column + 1, Row - 1]);
        }
        if (Map.IsTileInBounds(Map, Column - 1, Row + 1) && Map.Tiles[Column - 1, Row + 1] is not WallTile)
        {
            neighbors.Add(Map.Tiles[Column - 1, Row + 1]);
        }
        if (Map.IsTileInBounds(Map, Column + 1, Row + 1) && Map.Tiles[Column + 1, Row + 1] is not WallTile)
        {
            neighbors.Add(Map.Tiles[Column + 1, Row + 1]);
        }
        if (Map.IsTileInBounds(Map, Column - 1, Row - 1) && Map.Tiles[Column - 1, Row - 1] is not WallTile)
        {
            neighbors.Add(Map.Tiles[Column, Row]);
        }
        return neighbors;
    }

}
public class EmptyTile : Tile
{
    public override char Symbol { get; init; } = ' ';

    public EmptyTile(Map map, int column, int row) : base(map, column, row)
    {

    }
}
public class WallTile : Tile
{
    public override char Symbol { get; init; } = '\u2588';

    public WallTile(Map map, int column, int row) : base(map, column, row)
    {

    }
}



public class Pathfinder
{
    public Map Map { get; set; }
    public List<Tile> OpenSet { get; set; } = new();
    public List<Tile> ClosedSet { get; set; } = new();

 


    public Pathfinder(Map map)
    {
        Map = map;
    }

    public List<Tile>? FindPath(Tile start, Tile end)
    {
        OpenSet.Add(start);
        start.GValue = 0;
        start.FValue = start.CalculateH(end);


        while (OpenSet.Count > 0)
        {
            OpenSet.Sort((x, y) => x.FValue.CompareTo(y.FValue));
            Tile current = OpenSet.First();

            if (current == end)
            {
                return GetPath(current);
            }
            
            
            OpenSet.Remove(current);

            if (current is WallTile) return null;

            foreach (Tile neighbor in current.Neighbors)
            {
                int tentativeGValue = current.GValue + current.CalculateH(neighbor);
                if (tentativeGValue < neighbor.GValue)
                {
                    neighbor.CameFrom = current;
                    neighbor.GValue = tentativeGValue;
                    neighbor.FValue = tentativeGValue + neighbor.CalculateH(end);
                    if (!OpenSet.Contains(neighbor))
                    {
                        OpenSet.Add(neighbor);
                    }
                }

            }
        }

        return null;



        List<Tile> GetPath(Tile current)
        {
            List<Tile> path = new();
            while (current.CameFrom != null)
            {
                current.isPath = true;
                current = current.CameFrom;
                
                path.Add(current);
            }
            current.isPath = true;
            path.Reverse();
            return path;
        }
    }

    
}
