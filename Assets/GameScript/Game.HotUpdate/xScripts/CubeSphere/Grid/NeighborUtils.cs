using UnityEngine;

public static class NeighborUtils
{
    public static int side = SettingsManager.GirdSide; // 初始化前请设置

    // Cube unfold layout:
    //         [5]
    //   [3] [0] [2] [1]
    //         [4]

    // Top, Right, Bottom, Left
    private static readonly NeighborMap[,] map = new NeighborMap[6, 4]
    {
        // face 0 (Front)
        {
            new NeighborMap(5, false, false),   // Top → Top
            new NeighborMap(2, false, false),   // Right → Right
            new NeighborMap(4, false, true),    // Bottom → Bottom (flip Y)
            new NeighborMap(3, false, false)  // Left → Left (不翻转)
        },
        // face 1 (Back)
        {
            new NeighborMap(5, false, true),    // Top → Top (flip Y)
            new NeighborMap(3, true, false),    // Right → Left (flip X)
            new NeighborMap(4, false, false),   // Bottom → Bottom
            new NeighborMap(2, true, false)     // Left → Right (flip X)
        },
        // face 2 (Right)
        {
            new NeighborMap(5, true, false),    // Top → Top (flip X)
            new NeighborMap(1, true, false),    // Right → Back (flip X)
            new NeighborMap(4, true, false),    // Bottom → Bottom (flip X)
            new NeighborMap(0, false, false)    // Left → Front
        },
        // face 3 (Left)
        {
            new NeighborMap(5, true, true),     // Top → Top (flip X + Y)
            new NeighborMap(0, false, false),   // Right → Front
            new NeighborMap(4, true, true),     // Bottom → Bottom (flip X + Y)
            new NeighborMap(1, true, false)     // Left → Back (flip X)
        },
        // face 4 (Bottom)
        {
            new NeighborMap(0, false, true),    // Top → Front (flip Y)
            new NeighborMap(2, true, false),    // Right → Right (flip X)
            new NeighborMap(1, false, false),   // Bottom → Back
            new NeighborMap(3, true, false)     // Left → Left (flip X)
        },
        // face 5 (Top)
        {
            new NeighborMap(1, false, true),    // Top → Back (flip Y)
            new NeighborMap(2, true, true),     // Right → Right (flip X+Y)
            new NeighborMap(0, false, false),   // Bottom → Front
            new NeighborMap(3, true, true)      // Left → Left (flip X+Y)
        }
    };

    public static bool TryGetNeighbor(int face, int x, int y, out int nFace, out int nX, out int nY)
    {
        nFace = face;
        nX = x;
        nY = y;

        if (x >= 0 && x < side && y >= 0 && y < side)
            return true;

        // 判断方向：Top(0), Right(1), Bottom(2), Left(3)
        int dir = -1;
        if (y >= side) dir = 0;
        else if (x >= side) dir = 1;
        else if (y < 0) dir = 2;
        else if (x < 0) dir = 3;
        else return false;

        NeighborMap jump = map[face, dir];
        nFace = jump.toFace;

        int offset;
        switch (dir)
        {
            case 0: // Top
                offset = x;
                nY = 0;
                nX = jump.flipX ? side - 1 - offset : offset;
                nY = jump.flipY ? side - 1 - nY : nY;
                break;

            case 1: // Right
                offset = y;
                nX = 0;
                nY = jump.flipY ? side - 1 - offset : offset;
                nX = jump.flipX ? side - 1 - nX : nX;
                break;

            case 2: // Bottom
                offset = x;
                nY = side - 1;
                nX = jump.flipX ? side - 1 - offset : offset;
                nY = jump.flipY ? side - 1 - nY : nY;
                break;

            case 3: // Left
                offset = y;
                nX = side - 1;
                nY = jump.flipY ? side - 1 - offset : offset;
                nX = jump.flipX ? side - 1 - nX : nX;
                break;
        }

        return Validate(nX, nY);
    }

    private static bool Validate(int x, int y)
    {
        return x >= 0 && x < side && y >= 0 && y < side;
    }

    private struct NeighborMap
    {
        public int toFace;
        public bool flipX;
        public bool flipY;

        public NeighborMap(int face, bool fx, bool fy)
        {
            toFace = face;
            flipX = fx;
            flipY = fy;
        }
    }
}
