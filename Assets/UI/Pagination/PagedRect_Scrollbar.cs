using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Pagination
{
    [ExecuteInEditMode]
    public class PagedRect_Scrollbar : Scrollbar
    {
        public PagedRect pagedRect;

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            pagedRect.ScrollBarValueChanged();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            pagedRect.ScrollBarValueChanged();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            pagedRect.ScrollBarValueChanged();
        }
    }
}
