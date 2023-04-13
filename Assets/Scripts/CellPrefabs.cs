using System;

namespace Checkers
{
    [Serializable]
    public struct CellPrefabs
    {
        public ColorType Color;

        public CellComponent CellPrefab;
    }
}
