using Unity.Mathematics;

public static class Helpers
{
    public static int CalculateIndex(int x, int y, int2 gridSize)
    {
        return x + y * gridSize.x;
    }
    
    public static float3 GetGridPosition(int2 gridPosition) => new (gridPosition.x, 0, gridPosition.y);  
}