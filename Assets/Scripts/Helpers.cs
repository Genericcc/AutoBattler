using Unity.Mathematics;

public static class Helpers
{
    public static int CalculateIndex(int x, int y, int2 gridSize)
    {
        return x + y * gridSize.x;
    }
    
    public static float3 GetPosition(int2 gridPosition) => new float3(gridPosition.x, 0, gridPosition.y);

    public static int Hash(int2 gridPosition)
    {
        unchecked
        {
            return gridPosition.x * 73856093 ^ gridPosition.y * 19349663;
        }
    }    
}