using UnityEngine.UI;

public class ScrollRectFix : ScrollRect
{
    public float size = 0.2f;

    override protected void LateUpdate()
    {
        base.LateUpdate();

        if (this.verticalScrollbar)
        {
            this.verticalScrollbar.size = 0.2f;
        }
    }

    override public void Rebuild(CanvasUpdate executing)
    {
        base.Rebuild(executing);

        if (this.verticalScrollbar)
        {
            this.verticalScrollbar.size = 0.2f;
        }
    }
}
