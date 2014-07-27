using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Supersuperproject
{


    public partial class HearthDecker : Form
    {
        #region variables
        int cropX;
        int cropY;
        int ImageWidth;
        int ImageHeight;
        int cardHeight;
        int logoHeight;
        int secondImageStartY;
        int buttonCount;
        int currentScale = 100;
        int colorDif = 1000000;
        int pixleDif = 1500;
        List<imageContainer> imageList = new List<imageContainer>();
        List<resolutionContainer> resolutionList = new List<resolutionContainer>();
        
        #endregion
        public HearthDecker()
        {
            InitializeComponent();
            resolutionList.Add((new resolutionContainer { width = 1360, height = 768 }));
            resolutionList.Add((new resolutionContainer { width = 1366, height = 768 }));
            resolutionList.Add((new resolutionContainer { width = 1440, height = 900 }));
            resolutionList.Add((new resolutionContainer { width = 1680, height = 1000 }));
            resolutionList.Add((new resolutionContainer { width = 1680, height = 1050 }));
            resolutionList.Add((new resolutionContainer { width = 1840, height = 1000 }));
            resolutionList.Add((new resolutionContainer { width = 1920, height = 1080 }));

        }
        
        const int WM_NCHITTEST = 0x0084;
        const int HTCAPTION = 2;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCHITTEST)
            {
                Point pt = this.PointToClient(new Point(m.LParam.ToInt32()));
                if (ClientRectangle.Contains(pt))
                {
                    m.Result = new IntPtr(HTCAPTION);
                    return;
                }
            }

            base.WndProc(ref m);
        }

        #region Resolutions
        private void setImageCoordiantes(int imageWidth, int imageHeight)
        {
            if (imageWidth == 1360 && imageHeight == 768)
            {
                cropX = 966;
                cropY = 0;
                ImageWidth = 172;
                ImageHeight = 688;
                cardHeight = 29;
                logoHeight = 80;
                secondImageStartY = 81;

            }
            if (imageWidth == 1440 && imageHeight == 900)
            {
                cropX = 1051;
                cropY = 0;
                ImageWidth = 203;
                ImageHeight = 805;
                cardHeight = 34;
                logoHeight = 93;
                secondImageStartY = 95;

            }
            if (imageWidth == 1680 && imageHeight == 1050)
            {
                cropX = 1226;
                cropY = 0;
                ImageWidth = 237;
                ImageHeight = 943;
                cardHeight = 40;
                logoHeight = 107;
                secondImageStartY = 111;

            }
            if (imageWidth == 1680 && imageHeight == 1000)
            {
                cropX = 1208;
                cropY = 0;
                ImageWidth = 225;
                ImageHeight = 900;
                cardHeight = 38;
                logoHeight = 102;
                secondImageStartY = 105;

            }
            if (imageWidth == 1840 && imageHeight == 1000)
            {
                cropX = 1288;
                cropY = 0;
                ImageWidth = 225;
                ImageHeight = 900;
                cardHeight = 38;
                logoHeight = 102;
                secondImageStartY = 105;

            }

            if (imageWidth == 1920 && imageHeight == 1080)
            {
                cropX = 1358;
                cropY = 0;
                ImageWidth = 244;
                ImageHeight = 970;
                cardHeight = 41;
                logoHeight = 110;
                secondImageStartY = 113;

            }

            if (imageWidth == 1366 && imageHeight == 768)
            {
                cropX = 966;
                cropY = 0;
                ImageWidth = 172;
                ImageHeight = 688;
                cardHeight = 29;
                logoHeight = 80;
                secondImageStartY = 81;

            }
        }

        private string resolutionSupport(int width, int height)
        {
            String res = "";
            foreach (var resolutions in resolutionList)
            {
                res += resolutions.width + "x" + resolutions.height + Environment.NewLine;
            }
            return "Unsupported screenshot size (" + width + "x" + height + ")" + Environment.NewLine +
                "Supported sizes:" + Environment.NewLine + res;
        }

        private Boolean isKnownSize(int imageWidth, int imageHeight)
        {
            foreach (var resolutions in resolutionList)
            {
                if (resolutions.width == imageWidth && resolutions.height == imageHeight)
                {
                    return true;
                }
            }
                return false;
        }

        #endregion

        #region ImageManager
        private void breakDownImage(Image image, object sender)
        {

            int currentY = 0;
            int topHeight = logoHeight;
            int pictureboxcount = 0;

            OpenFileDialog dialog = (OpenFileDialog)sender;

            if (dialog == loadBottomImage)
            {
                topHeight = secondImageStartY;
                foreach (var images in imageList)
                {
                    pictureboxcount = Convert.ToInt16(images.picturebox) + 1;
                }
            }
            Bitmap outputImage = new Bitmap(image.Width, topHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(outputImage))
            {
                graphics.DrawImage(image, new Rectangle(new Point(), new Size(image.Width, topHeight)),
                    new Rectangle(new Point(), new Size(image.Width, topHeight)), GraphicsUnit.Pixel);
            }
            if (dialog == loadTopImage)
            {
                imageList.Add(new imageContainer { image = global::HearthDecker.Properties.Resources.logo, show = true, picturebox = pictureboxcount.ToString() });
                pictureboxcount++;
            }
            currentY = topHeight;

            for (; currentY < image.Height; currentY += cardHeight)
            {
                outputImage = new Bitmap(image.Width, cardHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics graphics = Graphics.FromImage(outputImage))
                {
                    graphics.DrawImage(image, new Rectangle(new Point(), new Size(image.Width, cardHeight)),
                        new Rectangle(new Point(0, currentY), new Size(image.Width, cardHeight)), GraphicsUnit.Pixel);
                }
                if (image.Height - currentY > cardHeight || dialog == loadBottomImage)
                {
                    if (dialog == loadBottomImage && showImage((Image)outputImage))
                    {
                        imageList.Add(new imageContainer { image = (Image)outputImage, show = true, picturebox = pictureboxcount.ToString() });
                        pictureboxcount++;
                    }
                    else if (dialog == loadTopImage)
                    {
                        imageList.Add(new imageContainer { image = (Image)outputImage, show = true, picturebox = pictureboxcount.ToString() });
                        pictureboxcount++;
                    }
                }
            }
        }

        private Boolean isSameImage(Image listImage, Image newImage)
        {
            int correctPixels = 0;
            int wrongPixels = 0;
            int img1_ref, img2_ref;
            Bitmap img1 = new Bitmap(listImage);
            Bitmap img2 = new Bitmap(newImage);

            if (img1.Width == img2.Width && img1.Height == img2.Height)
            {
                for (int i = 0; i < img1.Width; i++)
                {
                    for (int j = 0; j < img1.Height; j++)
                    {
                        img1_ref = img1.GetPixel(i, j).ToArgb();
                        img2_ref = img2.GetPixel(i, j).ToArgb();
                        if (img1_ref >= img2_ref - colorDif && img1_ref <= img2_ref + colorDif)
                        {
                            correctPixels++;

                        }
                        else
                        {
                            wrongPixels++;
                            if (wrongPixels > pixleDif)
                            {
                                //   MessageBox.Show("wrongPixels: " + wrongPixels);
                                return false;
                            }

                        }
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        private Boolean showImage(Image newImage)
        {
            int truths = 0;
            foreach (var images in imageList)

                if (isSameImage(images.image, newImage))
                {
                    truths++;
                }

            if (truths > 0)
            {
                return false;
            }
            return true;
        }

        private Image changeAlphaOnImage(Image image, int alphaValue, int yStart, int yHeight)
        {
            if (image != null)
            {
                Bitmap bitMap = new Bitmap(image);

                for (int y = yStart; y < yStart + yHeight; y++)
                {
                    for (int x = 0; x < bitMap.Width; x++)
                    {
                        Color oColor = bitMap.GetPixel(x, y);
                        Color nColor = Color.FromArgb(alphaValue, oColor);
                        bitMap.SetPixel(x, y, nColor);
                    }
                }

                Image myAlphaImage = (Image)(bitMap);
                return myAlphaImage;
            }
            return image;

        }

        private int getAlphaFromImage(Image image)
        {
            if (image != null)
            {
                int alpha;
                Bitmap bitmap = new Bitmap(image);
                alpha = bitmap.GetPixel(1, 1).A;
                return alpha;
            }
            return 255;
        }

        private void mergeImageFromList(int scale, Object sender)
        {
            int newImageHeight = 0;
            buttonCount = 0;
            foreach (var images in imageList)
            {
                if (images.show)
                {
                    newImageHeight = newImageHeight + images.image.Height;
                    buttonCount++;
                }
            }

            int offset = 0;
            foreach (var images in imageList)
            {
                if (images.show)
                {
                    foreach (Control control in Controls)
                    {
                        if (control.Name == "pictureBox" + images.picturebox)
                        {
                            PictureBox drawImage = (PictureBox)control;
                            drawImage.Location = new Point(0, 0 + offset);
                            drawImage.Visible = true;
                            drawImage.Size = new Size((images.image.Width * scale) / 100, (images.image.Height * scale) / 100);
                            int drawImageAlpha = getAlphaFromImage(drawImage.Image);
                            if (drawImageAlpha.Equals(50) && sender == null)
                            {
                                drawImage.Image = changeAlphaOnImage(images.image, 50, 0, images.image.Height);
                            }
                            else
                            {
                                drawImage.Image = images.image;
                            }
                            drawImage.SizeMode = PictureBoxSizeMode.Zoom;
                            offset = offset + drawImage.Height;
                            setButtonLocation(drawImage, images);
                        }
                    }
                }
            }
        }

        private Image cropImage(Image image)
        {
            Bitmap bmpImage = new Bitmap(image);
            Bitmap bmpCrop = bmpImage.Clone(new Rectangle(cropX, cropY, ImageWidth, ImageHeight), bmpImage.PixelFormat);
            Image croppedImage = (Image)(bmpCrop);
            return croppedImage;
        }
        #endregion

        private void setButtonLocation(PictureBox picbox, imageContainer image)
        {

            int rowint = Convert.ToInt16(image.picturebox);
            if (rowint > 0)
            {
                foreach (Control control in Controls)
                {
                    if (control.Name == ("minus" + image.picturebox))
                    {
                        Button difButton = (Button)control;
                        difButton.Size = new Size(picbox.Height - 10, picbox.Height - 10);
                        difButton.Location = new Point(picbox.Right, picbox.Top + ((picbox.Height - difButton.Height) / 2));
                        difButton.Visible = true;
                    }
                    else if (control.Name == ("plus" + image.picturebox))
                    {
                        Button difButton = (Button)control;
                        difButton.Size = new Size(picbox.Height - 10, picbox.Height - 10);
                        difButton.Location = new Point(picbox.Right + difButton.Width, picbox.Top + ((picbox.Height - difButton.Height) / 2));
                        difButton.Visible = true;
                    }
                    else if (control.Name == ("label" + image.picturebox))
                    {
                        Label difLabel = (Label)control;
                        difLabel.Size = new Size(picbox.Height - 20, (picbox.Height - 20));
                        difLabel.Font = new Font(label1.Font.FontFamily,picbox.Height * 100 / 250,FontStyle.Bold);
                        difLabel.Location = new Point(picbox.Right - difLabel.Width - 3, picbox.Top + ((picbox.Height - difLabel.Height) / 2));
                    }

                }
            }

        }

        private void resetLabelNumbers()
        {
            foreach (Control control in Controls)
            {
                if (control.Name.StartsWith("label"))
                {
                    Label label = (Label)control;
                    label.Text = "1";
                    label.Visible = false;
                }
            }
        }
               
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            Image loadedImage = Image.FromFile(loadTopImage.FileName);

            if (isKnownSize(loadedImage.Width, loadedImage.Height))
            {
                setImageCoordiantes(loadedImage.Width, loadedImage.Height);
                Image croppedImage = cropImage(loadedImage);
                breakDownImage(croppedImage, sender);
                mergeImageFromList(currentScale, null);
                movePictureBoxButtons();
                setCardCounter();
            }
            else
            {
                MessageBox.Show(resolutionSupport(loadedImage.Width, loadedImage.Height), "Unsupported screenshot size", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            Image loadedImage = Image.FromFile(loadBottomImage.FileName);

            if (isKnownSize(loadedImage.Width, loadedImage.Height))
            {
                setImageCoordiantes(loadedImage.Width, loadedImage.Height);
                Image croppedImage = cropImage(loadedImage);
                breakDownImage(croppedImage, sender);
                mergeImageFromList(currentScale, null);
                movePictureBoxButtons();
                setCardCounter();
            }
            else
            {
                MessageBox.Show(resolutionSupport(loadedImage.Width, loadedImage.Height), "Unsupported screenshot size", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void movePictureBoxButtons()
        {
            resetShadePictureBox.Location = new Point(pictureBox0.Right + 10, 5);
            resetShadePictureBox.Visible = true;

            showButtonsPictureBox.Location = new Point(pictureBox0.Right + 10, resetShadePictureBox.Bottom + 5);
            hideButtonsPictureBox.Location = new Point(pictureBox0.Right + 10, resetShadePictureBox.Bottom + 5);
            hideButtonsPictureBox.Visible = true;
            cardCountLabel.Location = new Point(0,0);
            cardCountLabel.Visible = true;

        }

        private void setCardCounter ()
        {
            int cardCount = 0;
            foreach (Control control in Controls)
            {
                if (control.Name.StartsWith("label"))
                {
                    Label label = (Label)control;
                    if (Convert.ToInt16(label.Name.Substring(5)) < buttonCount)
                    {
                        cardCount = cardCount + Convert.ToInt16(label.Text);
                    }
                }
                Double cardPercent;
                cardPercent = (1 / (Double)cardCount) * 100;
                cardCountLabel.Text = cardCount + "/30  " + (int)cardPercent + "%";
            }
        }

        #region StateChange
        private void hideBorders_CheckedChanged(object sender, EventArgs e)
        {
            if (hideBorders.Checked) 
            {
                FormBorderStyle = FormBorderStyle.None;
            }
            else
            {
                FormBorderStyle = FormBorderStyle.Sizable;
            }
        }

        private void pixelNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            pixleDif = (int)pixelNumericUpDown.Value * 50;
        }

        private void alwaysOnTop_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = alwaysOnTop.Checked;
        }

        #endregion

        #region MouseHover
        private void resetShadePictureBox_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this.resetShadePictureBox, "Reset marked cards");
        }

        private void hideButtonsPictureBox_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this.hideButtonsPictureBox, "Hide buttons");
        }

        private void showButtonsPictureBox_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this.showButtonsPictureBox, "Show buttons");
        }
        #endregion

        #region Click
        private void aboutButton_Click(object sender, EventArgs e)
        {

            MessageBox.Show(global::HearthDecker.Properties.Resources.about.ToString(), "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void setTo720P_Click(object sender, EventArgs e)
        {
            this.Height = 720;
        }

        private void setTo1080P_Click(object sender, EventArgs e)
        {
            this.Height = 1080;
        }

        private void bloggerIcon_Click(object sender, EventArgs e)
        {
            Process.Start("http://hearthdecker.blogspot.se/");
        }

        private void twitterIcon_Click(object sender, EventArgs e)
        {
            Process.Start("https://twitter.com/Heltidsluffare");
        }

        private void youtubeIcon_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.youtube.com/user/heltidsluffare");
        }

        private void payPalPictureBox_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=HZHDXWV22CPC8");
        }

        private void removeDeckButton_Click(object sender, EventArgs e)
        {
            currentScale = 100;
            imageList.Clear();
            foreach (Control control in Controls)
            {
                if (control.Name.StartsWith("pictureBox"))
                {
                    PictureBox picbox = (PictureBox)control;
                    picbox.Visible = false;
                }
                if (control.Name.StartsWith("label"))
                {
                    Label label = (Label)control;
                    label.Visible = false;
                }
                if (control.Name.StartsWith("minus") || control.Name.StartsWith("plus"))
                {
                    Button button = (Button)control;
                    button.Visible = false;
                }
                hideButtonsPictureBox.Visible = false;
                resetShadePictureBox.Visible = false;
                cardCountLabel.Visible = false;
            }

        }

        private void hideButtonsPictureBox_Click(object sender, EventArgs e)
        {
            showButtonsPictureBox.Visible = true;
            hideButtonsPictureBox.Visible = false;
            loadImage.Visible = false;
            loadImage2.Visible = false;
            alwaysOnTop.Visible = false;
            exitButton.Visible = false;
            scaleDown.Visible = false;
            scaleUp.Visible = false;
            hideBorders.Visible = false;
            setTo1080P.Visible = false;
            setTo720P.Visible = false;
            twitterIcon.Visible = false;
            youtubeIcon.Visible = false;
            payPalPictureBox.Visible = false;
            bloggerIcon.Visible = false;
            removeDeckButton.Visible = false;
            copyrightLabel.Visible = false;
            logoPictureBox.Visible = false;
            aboutButton.Visible = false;
            pixelNumericUpDown.Visible = false;
            pixelLabel.Visible = false;
            pixelLabel2.Visible = false;
            this.Width = showButtonsPictureBox.Location.X + 100;
        }

        private void showButtonsPictureBox_Click(object sender, EventArgs e)
        {
            this.Width = 620;
            showButtonsPictureBox.Visible = false;
            hideButtonsPictureBox.Visible = true;
            loadImage.Visible = true;
            loadImage2.Visible = true;
            alwaysOnTop.Visible = true;
            exitButton.Visible = true;
            scaleDown.Visible = true;
            scaleUp.Visible = true;
            hideBorders.Visible = true;
            setTo1080P.Visible = true;
            setTo720P.Visible = true;
            twitterIcon.Visible = true;
            youtubeIcon.Visible = true;
            payPalPictureBox.Visible = true;
            bloggerIcon.Visible = true;
            removeDeckButton.Visible = true;
            copyrightLabel.Visible = true;
            logoPictureBox.Visible = true;
            aboutButton.Visible = true;
            pixelNumericUpDown.Visible = true;
            pixelLabel.Visible = true;
            pixelLabel2.Visible = true;
        }

        private void resetShadePictureBox_Click(object sender, EventArgs e)
        {
            mergeImageFromList(currentScale, sender);
            resetLabelNumbers();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to Exit?", "HearthDecker", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {

            }
            else
            {
                Application.Exit();
            }
        }

        private void scaleUp_Click(object sender, EventArgs e)
        {
            if (currentScale >= 100)
            {
                mergeImageFromList(100, null);
                movePictureBoxButtons();
            }
            else
            {
                mergeImageFromList(currentScale + 10, null);
                movePictureBoxButtons();
                currentScale = currentScale + 10;
            }

        }

        private void scaleDown_Click(object sender, EventArgs e)
        {
            if (currentScale <= 20)
            {
                mergeImageFromList(20, null);
                movePictureBoxButtons();
            }
            else
            {
                mergeImageFromList(currentScale - 10, null);
                movePictureBoxButtons();
                currentScale = currentScale - 10;
            }

        }

        private void minus_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string picname = button.Name.Substring(5, 0);
            int labelValue = 1;
            foreach (Control control in Controls)
            {
                if (control.Name == "pictureBox" + button.Name.Substring(5))
                {
                    PictureBox picbox = (PictureBox)control;
                    if (labelValue == 0)
                    {
                        picbox.Image = changeAlphaOnImage(picbox.Image, 50, 0, picbox.Image.Height);

                    }
                }
                if (control.Name == "label" + button.Name.Substring(5))
                {
                    Label label = (Label)control;
                    int newText = Convert.ToInt16(label.Text) - 1;
                    if (newText <= 0)
                    {
                        newText = 0;
                        label.Visible = false;
                    }
                    labelValue = newText;
                    label.Text = newText.ToString();
                }
            }
            setCardCounter();
        }

        private void plus_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string picname = button.Name.Substring(5, 0);
            foreach (Control control in Controls)
            {
                if (control.Name == "pictureBox" + button.Name.Substring(4))
                {
                    PictureBox picbox = (PictureBox)control;
                    picbox.Image = changeAlphaOnImage(picbox.Image, 255, 0, picbox.Image.Height);
                }
                if (control.Name == "label" + button.Name.Substring(4))
                {
                    Label label = (Label)control;
                    int newText = Convert.ToInt16(label.Text) + 1;
                    if (newText > 9)
                    {
                        newText = 9;
                    }
                    label.Text = newText.ToString();
                    if (newText > 1)
                    {
                        label.Visible = true;
                    }
                }
            }
            setCardCounter();
        }

        private void loadImage_Click(object sender, EventArgs e)
        {
            loadTopImage.Filter = "Image Files(*.bmp;*.jpg;*.gif;*.png)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            loadTopImage.ShowDialog();
        }

        private void loadImage2_Click(object sender, EventArgs e)
        {
            loadBottomImage.Filter = "Image Files(*.bmp;*.jpg;*.gif;*.png)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            loadBottomImage.ShowDialog();
        }

        #endregion

    }

    public class imageContainer
    {
        public Image image { get; set; }
        public Boolean show { get; set; }
        public string picturebox { get; set; }
    }

    public class resolutionContainer
    {
        public int width { get; set; }
        public int height { get; set; }
    }
}
