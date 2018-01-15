
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace Paint
{
  public partial class PaintForm : Form, IPaintSettings
  {
    private ImageFile imageFile;
    private ToolArgs toolArgs;
    private Tool curTool;
    private IPaintSettings settings;

    public PaintForm() {
      InitializeComponent();
      toolsBar.ImageList = imageList;
      settings = (IPaintSettings)this;
    }

    private void toolsBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e) {
      curTool.UnloadTool();
      ToolBarButton curButton = e.Button;

      SetToolBarButtonsState(curButton);

      if (curButton == arrowBtn) {
        curTool = new PointerTool(toolArgs);
      }
      if (curButton == lineBtn) {
        curTool = new LineTool(toolArgs);
      } else if (curButton == rectangleBtn) {
        curTool = new RectangleTool(toolArgs);
      } else if (curButton == pencilBtn) {
        curTool = new PencilTool(toolArgs);
      } else if (curButton == brushBtn) {
        curTool = new BrushTool(toolArgs, BrushToolType.FreeBrush);
      } else if (curButton == ellipseBtn) {
        curTool = new EllipseTool(toolArgs);
      }  else if (curButton == fillBtn) {
        curTool = new FillTool(toolArgs);
      } else if (curButton == eraserBtn) {
        curTool = new BrushTool(toolArgs, BrushToolType.Eraser);
      }
    }

    private void SetToolBarButtonsState(ToolBarButton curButton) {
      curButton.Pushed = true;
      foreach (ToolBarButton btn in toolsBar.Buttons) {
        if (btn != curButton)
          btn.Pushed = false;
      }
    }

    private void imageBox_Paint(object sender, PaintEventArgs e) {
      
      Rectangle clipRect = e.ClipRectangle;
      Bitmap b = toolArgs.bitmap.Clone(clipRect, toolArgs.bitmap.PixelFormat);
      e.Graphics.DrawImageUnscaledAndClipped(b, clipRect);
      b.Dispose();
    }

    private void PaintForm_Load(object sender, EventArgs e) {
      // fill (fill style) list
      for (int i = 0; i < 1; i++) {
        BrushType bt = (BrushType)i;
        fillStyleCombo.Items.Add(bt);
      }
      fillStyleCombo.SelectedIndex = 0;

      // fill shape style list
      for (int i = 0; i < 1; i++) {
        DrawMode ss = (DrawMode)i;
        shapeStyleCombo.Items.Add(ss);
      }
      shapeStyleCombo.SelectedIndex = 0;

      // fill Width list
      for (int i = 1; i < 11; i++)
        widthCombo.Items.Add(i);
      for (int i = 15; i <= 60; i += 5)
        widthCombo.Items.Add(i);
      widthCombo.SelectedIndex = 5;

      for (int i = 0; i < 1; i++) {
        DashStyle ds = (DashStyle)i;
        lineStyleCombo.Items.Add(ds.ToString());
      }
      lineStyleCombo.SelectedIndex = 0;

      // default image
      imageFile = new ImageFile(new Size(500, 500), Color.White);
      ShowImage();
    }

    DrawMode IPaintSettings.DrawMode {
      get {
        return (DrawMode)shapeStyleCombo.SelectedIndex;
      }
    }

    int IPaintSettings.Width {
      get {
        return Int32.Parse(widthCombo.Text);
      }
    }



    Color IPaintSettings.PrimaryColor {
      get {
        return primColorBox.BackColor;
      }
    }

    Color IPaintSettings.SecondaryColor {
      get {
        return secColorBox.BackColor;
      }
    }

    BrushType IPaintSettings.BrushType
    {
        get
        {
            BrushType type;
            int selIndex = fillStyleCombo.SelectedIndex;
            switch (selIndex)
            {
                case 0:
                case 1:
                case 2:
                    type = (BrushType)selIndex;
                    break;
                default:
                    type = BrushType.SolidBrush;
                    break;
            }
            return type;
        }
    }



    private void ColorBox_Click(object sender, EventArgs e) {
      PictureBox picBox = (PictureBox)sender;
      ColorDialog colorDlg = new ColorDialog();
      colorDlg.FullOpen = true;

      colorDlg.Color = picBox.BackColor;
      if (colorDlg.ShowDialog() == DialogResult.OK) {
        picBox.BackColor = colorDlg.Color;
      }
    }



    private void imageClearMnu_Click(object sender, EventArgs e) {
      Graphics.FromImage(imageFile.Bitmap).Clear(settings.SecondaryColor);
      imageBox.Invalidate();
    }
    

    private void fileNewMnu_Click(object sender, EventArgs e) {
      NewDialog newDlg = new NewDialog();
      if (newDlg.ShowDialog() == DialogResult.OK) {
        imageFile = new ImageFile(newDlg.ImageSize, newDlg.imageBackColor);
        ShowImage();
      }
    }

    private void fileOpenMnu_Click(object sender, EventArgs e) {
      OpenFileDialog openDlg = new OpenFileDialog();
      openDlg.Filter = "Image Files .BMP .JPG .GIF .Png|*.BMP;*.JPG;*.GIF;*.PNG";
      if (openDlg.ShowDialog() == DialogResult.OK) {
        if (imageFile.Open(openDlg.FileName)) {
          ShowImage();
        } else {
          MessageBox.Show("Error");
        }
      }
    }

    private void fileSaveMnu_Click(object sender, EventArgs e) {
      if (imageFile.FileName != null) {
        if (!imageFile.Save(imageFile.FileName))
          MessageBox.Show("Error");
        else
          ShowImage();
      } else {
        fileSaveAsMnu_Click(sender, e);
      }
    }

    private void fileSaveAsMnu_Click(object sender, EventArgs e) {
      SaveFileDialog saveDlg = new SaveFileDialog();
      saveDlg.Filter = "Bitmap (*.BMP)|*.BMP";
      if (saveDlg.ShowDialog() == DialogResult.OK) {
        if (!imageFile.Save(saveDlg.FileName))
          MessageBox.Show("Error");
        else
          ShowImage();
      }
    }

    private void fileExitMnu_Click(object sender, EventArgs e) {
      Application.Exit();
    }

    private void ShowImage() {
      string t = imageFile.FileName;
      Text = String.Format("Paint - [{0}]", t == null ? "Untitled" : new FileInfo(t).Name);

      imageBox.ClientSize = imageFile.Bitmap.Size;
      imageBox.Invalidate();
      toolArgs = new ToolArgs(imageFile.Bitmap, imageBox, pointPanel1, pointPanel2, settings);

      if (curTool != null)
        curTool.UnloadTool();
      curTool = new PointerTool(toolArgs);
      SetToolBarButtonsState(arrowBtn);
    }
  }
}