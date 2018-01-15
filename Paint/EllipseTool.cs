
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Paint
{
  public class EllipseTool : RectangleTool
  {
    public EllipseTool(ToolArgs args)
      : base(args) {
    }

    protected override void DrawRectangle(Pen outlinePen, Brush fillBrush) {
        g.DrawEllipse(outlinePen, rect);
    }
  }
}
