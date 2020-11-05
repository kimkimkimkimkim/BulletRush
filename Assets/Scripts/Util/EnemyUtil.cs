using UnityEngine;

public static class EnemyUtil
{
    public static Vector3 GetPosition(int value)
    {
        float X = 2;
        float Z = 4;

        switch (value)
        {
            case 1:
                return new Vector3(-X, 0, Z);
            case 2:
                return new Vector3(0, 0, Z);
            case 3:
                return new Vector3(X, 0, Z);
            case 4:
                return new Vector3(-X, 0, 0);
            case 5:
                return new Vector3(0, 0, 0);
            case 6:
                return new Vector3(X, 0, 0);
            case 7:
                return new Vector3(-X, 0, -Z);
            case 8:
                return new Vector3(0, 0, -Z);
            case 9:
                return new Vector3(X, 0, -Z);
            default:
                return Vector3.zero;
        }
    }

    public static Vector3 GetDirection(int value) {
        switch(value)
        {
            case 1:
                return new Vector3(-1, 0, 1);
            case 2:
                return new Vector3(1, 0, 1);
            case 3:
                return new Vector3(-1, 0, -1);
            case 4:
                return new Vector3(1, 0, -1);
            default:
                return Vector3.zero;
        }
    }
}