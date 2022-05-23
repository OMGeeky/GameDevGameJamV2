using System;

using Unity.Mathematics;


public static class MathHelpers
{
    /// <summary>
    /// Adds the area of the two boxes
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>The new box with the area of both boxes</returns>
    public static float3 AddBoxAreaToBox( float3 a , float3 b )
    {
        throw new NotImplementedException();
        /*
                float x;
                float y;
                float z;
        
                if ( a.y == 0 )
                    y = b.y;
                else if ( b.y == 0 )
                    y = a.y;
                else
                    y = ((b.x / a.x) * b.y) / a.y;
        
                if ( a.x == 0 )
                    x = b.x;
                else if ( b.x == 0 )
                    x = a.x;
                else
                    x = ((b.y / a.y) * b.x) / a.x;
        
                if ( a.z == 0 )
                    z = b.z;
                else if ( b.z == 0 )
                    z = a.z;
                else
                {
                    z = ((b.y / a.y) * b.x) / a.x;
                }
        
                return new float3( x , y , z );
        */
    }

    /// <summary>
    /// Adds the area of the two boxes
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>The new box with the area of both boxes</returns>
    public static float2 AddBoxAreaToBox( float2 a , float2 b )
    {
        float x;
        float y;

        if ( a.y == 0 )
            y = b.y;
        else if ( b.y == 0 )
            y = a.y;
        else
            y = (((b.x / a.x) * b.y) / a.y) + a.y;

        if ( a.x == 0 )
            x = b.x;
        else if ( b.x == 0 )
            x = a.x;
        else
            x = (((b.y / a.y) * b.x) / a.x) + a.x;

        return new float2( x , y );
    }

}
