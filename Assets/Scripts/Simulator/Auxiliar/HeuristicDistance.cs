using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeuristicFunc
{
    public abstract float Function(Vector2Int Start, Vector2Int End);
    public abstract float UnitaryDiagonal();
}

public class ManhattanHeuristic : HeuristicFunc
{
    public override float Function(Vector2Int Start, Vector2Int End)
    {
        return Mathf.Abs(End.x - Start.x) + Mathf.Abs(End.y - Start.y);
    }
    public override float UnitaryDiagonal()
    {
        return 2f;
    }
}

public class EuclideanHeuristic : HeuristicFunc
{
    public override float Function(Vector2Int Start, Vector2Int End)
    {
        return Mathf.Sqrt(Mathf.Pow((End.x - Start.x), 2) + Mathf.Pow((End.y - Start.y), 2));
    }
    public override float UnitaryDiagonal()
    {
        return Mathf.Sqrt(2);
    }
}

public class ChebyshovHeuristic : HeuristicFunc
{
    public override float Function(Vector2Int Start, Vector2Int End)
    {
        return Mathf.Max(Mathf.Abs(End.x - Start.x), Mathf.Abs(End.y - Start.y));
    }
    public override float UnitaryDiagonal()
    {
        return 1;
    }
}
