using UnityEngine.EventSystems;

namespace Checkers
{
    public class ChipComponent : BaseClickComponent
    {
        public int Id { get; set; }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            CallBackEvent((CellComponent)Pair, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            CallBackEvent((CellComponent)Pair, false);
        }
    }
}
