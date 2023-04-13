using Checkers.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Checkers
{
    public class Game : MonoBehaviour
    {
        [SerializeField]
        private CellPrefabs[] _cellPrefabsForInput;

        private Dictionary<ColorType, CellComponent> _cellPrefabs = new();

        [SerializeField]
        private int _cellSideLength;

        [SerializeField]
        private ChipComponent _lightChipPrefab;

        [SerializeField]
        private ChipComponent _darkChipPrefab;

        private void Start()
        {
            foreach (CellPrefabs prefab in _cellPrefabsForInput)
            {
                _cellPrefabs.Add(prefab.Color, prefab.CellPrefab);
            }

            CreateBoard();
        }

        private void CreateBoard()
        {
            var boardModel = new Board();

            for (int x = 0; x < boardModel.Cells.GetLength(0); x++)
            {
                for (int y = 0; y < boardModel.Cells.GetLength(1); y++)
                {
                    CreateCell((ColorType)boardModel.Cells[x, y], new Vector2 (x * _cellSideLength, y * _cellSideLength));
                }
            }
        }

        private CellComponent CreateCell(ColorType color, Vector2 position)
        {
            return Instantiate(_cellPrefabs[color], new Vector3(position.x, 0f, position.y), Quaternion.identity, transform);
        }
    }
}
