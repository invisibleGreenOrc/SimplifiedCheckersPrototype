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
        private BoardElementPrefab<CellComponent>[] _cellPrefabsForInput;

        private Dictionary<ColorType, CellComponent> _cellPrefabs = new();

        [SerializeField]
        private int _cellSideLength;

        [SerializeField]
        private BoardElementPrefab<ChipComponent>[] _chipPrefabsForInput;

        private Dictionary<ColorType, ChipComponent> _chipPrefabs = new();

        [SerializeField]
        private Material _highlightMaterial;

        [SerializeField]
        private Material _selectMaterial;

        private void Start()
        {
            foreach (BoardElementPrefab<CellComponent> prefab in _cellPrefabsForInput)
            {
                _cellPrefabs.Add(prefab.Color, prefab.Prefab);
            }

            foreach (BoardElementPrefab<ChipComponent> prefab in _chipPrefabsForInput)
            {
                _chipPrefabs.Add(prefab.Color, prefab.Prefab);
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
                    CellComponent newCell = CreateCell((ColorType)boardModel.Cells[x, y], new Vector2 (y * _cellSideLength, x * _cellSideLength));
                    newCell.OnClickEventHandler += OnClick;
                }
            }

            foreach (Chip chip in boardModel.Chips.Values)
            {
                ChipComponent newChip = CreateChip(chip.Color, new Vector2(chip.Position.Y * _cellSideLength, chip.Position.X * _cellSideLength));
                newChip.OnClickEventHandler += OnClick;
            }
        }

        private CellComponent CreateCell(ColorType color, Vector2 position)
        {
            return Instantiate(_cellPrefabs[color], new Vector3(position.x, 0f, position.y), Quaternion.identity, transform);
        }

        private ChipComponent CreateChip(ColorType color, Vector2 position)
        {
            return Instantiate(_chipPrefabs[color], new Vector3(position.x, 0.2f, position.y), Quaternion.identity, transform);
        }

        private void OnClick(BaseClickComponent clickedComponent)
        {
            clickedComponent.AddAdditionalMaterial(_highlightMaterial);
        }
    }
}
