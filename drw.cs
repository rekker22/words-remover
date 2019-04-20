using System.Drawing;

namespace micro_post
{
    class drw
    {
        public Bitmap DrawLineInt(Bitmap bmp,int x1,int x2,int y1,int y2)
        {
            Pen greenPen = new Pen(Color.Red, 3);
            // Draw line to screen.
            using (var graphics = Graphics.FromImage(bmp))
            {

                Rectangle rect = new Rectangle(x1, x2, y1, y2);
                SolidBrush sb = new SolidBrush(Color.White);
                graphics.FillRectangle(sb,rect);
                //graphics.DrawRectangle(greenPen, rect);
                var h = bmp.Width;
                var w = bmp.Height;
                
                //graphics.DrawLine(greenPen, x1, y1, x1, y2);
            }

            bmp.Save("Edited", System.Drawing.Imaging.ImageFormat.Bmp);

            return bmp;
        }
    }
}
