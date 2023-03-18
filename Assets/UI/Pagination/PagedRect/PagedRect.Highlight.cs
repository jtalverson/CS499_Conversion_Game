namespace UI.Pagination
{
    public partial class PagedRect
    {
        protected void ShowHighlight()
        {
            imageComponent.color = HighlightColor;
        }

        protected void ClearHighlight()
        {
            imageComponent.color = NormalColor;
        }
    }
}
