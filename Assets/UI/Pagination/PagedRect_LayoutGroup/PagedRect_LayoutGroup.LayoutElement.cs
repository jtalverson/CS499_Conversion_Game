using UnityEngine.UI;

namespace UI.Pagination
{
    public partial class PagedRect_LayoutGroup : ILayoutElement
    {
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            CalcAlongAxis(0, IsVertical);
        }

        public override void CalculateLayoutInputVertical()
        {
            CalcAlongAxis(1, IsVertical);
        }
    }
}
