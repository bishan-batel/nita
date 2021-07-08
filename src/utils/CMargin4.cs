using Godot;

namespace Parry2.utils
{
    public struct CMargin4
    {
        public float Top, Bottom, Left, Right;

        public CMargin4(Camera2D camera)
        {
            Bottom = camera.DragMarginBottom;
            Top = camera.DragMarginTop;
            Left = camera.DragMarginLeft;
            Right = camera.DragMarginRight;
        }

        public CMargin4(float top, float bottom, float left, float right)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        public void AssignToDragMargin(Camera2D camera)
        {
            camera.DragMarginTop = Top;
            camera.DragMarginBottom = Bottom;
            camera.DragMarginLeft = Left;
            camera.DragMarginRight = Right;
        }


        public CMargin4 Lerp(CMargin4 to, float weight)
        {
            return new(
                Mathf.Lerp(Top, to.Top, weight),
                Mathf.Lerp(Bottom, to.Bottom, weight),
                Mathf.Lerp(Left, to.Left, weight),
                Mathf.Lerp(Right, to.Right, weight)
            );
        }
    }
}