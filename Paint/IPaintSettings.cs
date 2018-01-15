
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Paint
{
    public interface IPaintSettings
    {
        DrawMode DrawMode
        {
            get;
        }

        int Width
        {
            get;
        }


        Color PrimaryColor
        {
            get;
        }

        Color SecondaryColor
        {
            get;
        }

        BrushType BrushType
        {
            get;
        }



    }
}