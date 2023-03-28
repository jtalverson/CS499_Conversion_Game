using UnityEngine;

namespace UI.Pagination
{
    public partial class PagedRect
    {
        void HandleKeyboardInput()
        {
            if (Input.GetKeyDown(NextPageKey))
            {
                NextPage();
            }
            else if (Input.GetKeyDown(PreviousPageKey))
            {
                PreviousPage();
            }
            else if (Input.GetKeyDown(FirstPageKey))
            {
                ShowFirstPage();
            }
            else if (Input.GetKeyDown(LastPageKey))
            {
                ShowLastPage();
            }
        }
    }
}
