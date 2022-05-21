using UnityEngine;


public static class GizmosExtended
{
    public static void DrawCubeWithLocalSpace( Vector3 center , float size , Transform transform )
    {
        var s = new Vector3( size , size , -1 );
        Gizmos.matrix = Matrix4x4.TRS( transform.TransformPoint( Vector3.zero ) , transform.rotation , transform.lossyScale );
        Gizmos.DrawCube( Vector3.zero , Vector3.one + s );


        // var pUR = (Vector2) center + new Vector2( size.x , size.y );
        // var pUL = (Vector2) center + new Vector2( -size.x , size.y );
        // var pDR = (Vector2) center + new Vector2( size.x , -size.y );
        // var pDL = (Vector2) center + new Vector2( -size.x , -size.y );
    }
}

public static class Vector2Extension
{
    public static Vector2 Rotate( this Vector2 v , float degrees )
    {
        return Quaternion.Euler( 0 , 0 , degrees ) * v;
    }
}
