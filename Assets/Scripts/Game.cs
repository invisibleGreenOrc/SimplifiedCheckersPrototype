using Checkers.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class Game : MonoBehaviour
    {
        [SerializeField]
        private BoardElementPrefab<CellComponent>[] _cellPrefabsForInput;

        [SerializeField]
        private int _cellSideLength;
        
        private Dictionary<ColorType, CellComponent> _cellPrefabs = new();

        private List<CellComponent> _cells = new();
        private List<ChipComponent> _chips = new();

        [SerializeField]
        private BoardElementPrefab<ChipComponent>[] _chipPrefabsForInput;

        private Dictionary<ColorType, ChipComponent> _chipPrefabs = new();

        [SerializeField]
        private Material _highlightMaterial;

        [SerializeField]
        private Material _selectMaterial;

        private ChipComponent _activeChip;

        private CheckersGame _checkerGame;

        private List<CellComponent> _allowedToMoveCells =new();

        private PhysicsRaycaster _raycaster;

        private void Start()
        {
            _raycaster = FindObjectOfType<PhysicsRaycaster>();

            foreach (BoardElementPrefab<CellComponent> prefab in _cellPrefabsForInput)
            {
                _cellPrefabs.Add(prefab.Color, prefab.Prefab);
            }

            foreach (BoardElementPrefab<ChipComponent> prefab in _chipPrefabsForInput)
            {
                _chipPrefabs.Add(prefab.Color, prefab.Prefab);
            }

            StartGame();
        }

        private void OnDestroy()
        {
            _checkerGame.ChipRemoved -= RemoveChip;
            _checkerGame.PlayerWon -= CongratsPlayer;
        }

        private void StartGame()
        {
            _checkerGame = new CheckersGame();
            _checkerGame.StartNewGame();

            _checkerGame.ChipRemoved += RemoveChip;
            _checkerGame.PlayerWon += CongratsPlayer;

            CreateBoard();
            CreateChips();
        }

        private void CreateBoard()
        {
            for (int x = 0; x < _checkerGame.BoardCells.GetLength(0); x++)
            {
                for (int y = 0; y < _checkerGame.BoardCells.GetLength(1); y++)
                {
                    CreateCell((ColorType)_checkerGame.BoardCells[x, y], x, y);
                }
            }
        }

        private CellComponent CreateCell(ColorType color, int xCoordinate, int yCoordinate)
        {
            var position = new Vector3(yCoordinate * _cellSideLength, 0f,  xCoordinate * _cellSideLength);
            CellComponent newCell = Instantiate(_cellPrefabs[color], position, Quaternion.identity, transform);

            newCell.Coordinates = new Position() { X = xCoordinate, Y = yCoordinate };
            _cells.Add(newCell);

            newCell.OnClickEventHandler += OnClick;
            newCell.OnFocusEventHandler += Highlight;

            return newCell;
        }

        private void CreateChips()
        {
            foreach (Chip chip in _checkerGame.ChipsOnBoard)
            {
                var position = new Vector3(chip.Position.Y * _cellSideLength, 0.2f, chip.Position.X * _cellSideLength);
                ChipComponent newChip = Instantiate(_chipPrefabs[chip.Color], position, Quaternion.identity, transform);

                newChip.Id = chip.Id;
                newChip.Pair = _cells.Where(cell => cell.Coordinates.X == chip.Position.X && cell.Coordinates.Y == chip.Position.Y).FirstOrDefault();
                _chips.Add(newChip);

                newChip.OnClickEventHandler += OnClick;
                newChip.OnFocusEventHandler += Highlight;
            }
        }

        private void RemoveChip(int id)
        {
            ChipComponent chipToRemove = _chips.Where(chip => chip.Id == id).FirstOrDefault();
            _chips.Remove(chipToRemove);
            Destroy(chipToRemove.gameObject);
        }

        private void OnClick(BaseClickComponent clickedComponent)
        {
            if (clickedComponent is CellComponent cell && _activeChip != null)
            {
                if (_checkerGame.TryMakeMove(_activeChip.Id, new Position() { X = cell.Coordinates.X, Y = cell.Coordinates.Y }))
                {
                    StartCoroutine(MoveToTarget(_activeChip, new Vector3(cell.transform.position.x, 0.2f, cell.transform.position.z)));

                    _activeChip.Pair = cell;
                    _activeChip.RemoveAdditionalMaterial();
                    _activeChip = null;

                    foreach (var item in _allowedToMoveCells)
                    {
                        item.RemoveAdditionalMaterial(2);
                    }

                    _allowedToMoveCells.Clear();
                }
            }
            else if (clickedComponent is ChipComponent chip)
            {
                if (_activeChip != null)
                {
                    _activeChip.RemoveAdditionalMaterial();

                    foreach (var item in _allowedToMoveCells)
                    {
                        item.RemoveAdditionalMaterial(2);
                    }
                    _allowedToMoveCells = new();
                }

                _activeChip = chip;
                chip.AddAdditionalMaterial(_selectMaterial);

                foreach (var item in _checkerGame.GetAllowedPositionsToMoveChip(_activeChip.Id))
                {
                    var s = _cells.Where(cell => cell.Coordinates.X == item.X && cell.Coordinates.Y == item.Y).ToList();
                    _allowedToMoveCells.AddRange(s);
                }

                foreach (var item in _allowedToMoveCells)
                {
                    item.AddAdditionalMaterial(_selectMaterial, 2);
                }
            }
        }

        private void Highlight(CellComponent component, bool isSelect)
        {
            if (isSelect)
            {
                component.AddAdditionalMaterial(_highlightMaterial);
            }
            else
            {
                component.RemoveAdditionalMaterial();
            }
        }

        private IEnumerator MoveToTarget(ChipComponent chip,  Vector3 target)
        {
            _raycaster.enabled = false;

            while (chip.transform.position != target)
            {
                chip.transform.position = Vector3.MoveTowards(chip.transform.position, target, 5f * Time.deltaTime);

                yield return null;
            }

            _raycaster.enabled = true;
        }

        private void CongratsPlayer(ColorType color)
        {
            Debug.Log(color.ToString());
        }
    }
}
