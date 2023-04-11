using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ExternalUI.Pagination
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
