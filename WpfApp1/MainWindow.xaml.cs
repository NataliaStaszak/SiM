using System;
using System.Windows;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }
        private string pictureSrc;
        private BitmapImage bitmapPicture;
        private Image<Gray, Single> imageFilt;

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Obrazy|*.jpg;*.jpeg;*.png;*.gif;*.bmp|Wszystkie pliki|*.*";
            openFileDialog.Title = "Wybierz obraz";

            if (openFileDialog.ShowDialog() == true)
            {
                pictureSrc = openFileDialog.FileName;
            }
            else
            {
                Console.WriteLine("Anulowano wybór pliku.");
            }
            bitmapPicture=new BitmapImage(new Uri(pictureSrc, UriKind.RelativeOrAbsolute));
            
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = bitmapPicture;

            plainPicture.Fill= imageBrush;
            srcLabel.Content= String.Format("src: {0}", pictureSrc);
        }
        

        private void SpringFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (pictureSrc != null)
            {
                Image<Bgr, byte> img1 = new Image<Bgr, byte>(pictureSrc);
                Image<Hsv, byte> hsvImg = img1.Convert<Hsv, byte>();
                for (int y = 0; y < hsvImg.Height; y++)
                {
                    for (int x = 0; x < hsvImg.Width; x++)
                    {
                        Hsv pixel = hsvImg[y, x];
                        if(pixel.Hue >= 20 && pixel.Hue <= 100 && pixel.Satuation >= 50)
                        {
                            pixel.Value +=50;  
                        }
                    hsvImg[y, x] = pixel;
                    }
                }

            ImageBrush imageBrush = new ImageBrush(Bitmap2BitmapImage(hsvImg.ToBitmap()));
            filtredPicture.Fill = imageBrush;
            CvInvoke.WaitKey(0);
            }
        }

        private void AutumnFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (pictureSrc != null)
        {
            Image<Bgr, byte> img1 = new Image<Bgr, byte>(pictureSrc);
            Image<Hsv, byte> hsvImg = img1.Convert<Hsv, byte>();
            for (int y = 0; y < hsvImg.Height; y++)
            {
                for (int x = 0; x < hsvImg.Width; x++)
                {
                    Hsv pixel = hsvImg[y, x];
                    if(pixel.Hue >= 20 && pixel.Hue <= 100 && pixel.Satuation >= 50)
                    {
                        pixel.Hue =20;  
                        pixel.Satuation = 250;
                    }
                    hsvImg[y, x] = pixel;
            }
        }

        ImageBrush imageBrush = new ImageBrush(Bitmap2BitmapImage(hsvImg.ToBitmap()));
        filtredPicture.Fill = imageBrush;
        CvInvoke.WaitKey(0);
        }
    }

    private void WinterFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (pictureSrc != null)
            {
                Image<Bgr, byte> img1 = new Image<Bgr, byte>(pictureSrc);
                Image<Hsv, byte> hsvImg = img1.Convert<Hsv, byte>();

                for (int y = 0; y < hsvImg.Height; y++)
                {
                    for (int x = 0; x < hsvImg.Width; x++)
                    {
                        Hsv pixel = hsvImg[y, x];
                        if(pixel.Hue >= 20 && pixel.Hue <= 100 && pixel.Satuation >= 50)
                        {
                            pixel.Hue /=20;  
                            pixel.Satuation = 0; 
                            pixel.Value +=150;  
                        }
                    hsvImg[y, x] = pixel;
                    }
                }

                AddWhiteNoise(hsvImg, amount: 0.07);
                ImageBrush imageBrush = new ImageBrush(Bitmap2BitmapImage(hsvImg.ToBitmap()));
                filtredPicture.Fill = imageBrush;
                CvInvoke.WaitKey(0);
            }
        }
        
        private void RainFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if(pictureSrc!=null)
            {
                Image<Bgr, byte> img1 = new Image<Bgr, byte>(pictureSrc);
                Image<Hsv, byte> hsvImg = img1.Convert<Hsv, byte>();
                for (int y = 0; y < hsvImg.Height; y++)
                {
                    for (int x = 0; x < hsvImg.Width; x++)
                    {
                        Hsv pixel = hsvImg[y, x];
                        pixel.Satuation = (byte)(pixel.Satuation * 0.2); 
                        pixel.Value = (byte)(pixel.Value * 0.8);
                        hsvImg[y, x] = pixel;
                    }
                }
                AddWhiteNoise(hsvImg, amount: 0.01); 
                ImageBrush imageBrush = new ImageBrush(Bitmap2BitmapImage(hsvImg.ToBitmap()));
                filtredPicture.Fill = imageBrush;
                CvInvoke.WaitKey(0);
            }
        }

        private void FogFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (pictureSrc != null)
            {
                Image<Bgr, byte> img1 = new Image<Bgr, byte>(pictureSrc);
                Image<Hsv, byte> hsvImg = img1.Convert<Hsv, byte>();

                // Apply fog effect by modifying HSV channels
                for (int y = 0; y < hsvImg.Height; y++)
                {
                    for (int x = 0; x < hsvImg.Width; x++)
                    {
                        Hsv pixel = hsvImg[y, x];
                        pixel.Satuation = (byte)(pixel.Satuation * 0.01);
                        pixel.Value = (byte)Math.Min(255, pixel.Value * 1.1);

                        hsvImg[y, x] = pixel;
                    }
                }
                ImageBrush imageBrush = new ImageBrush(Bitmap2BitmapImage(hsvImg.ToBitmap()));
                filtredPicture.Fill = imageBrush;
                CvInvoke.WaitKey(0);
            }
        }

        private void AddWhiteNoise(Image<Hsv, byte> image, double amount = 0.01)
        {
            Random rand = new Random();
            int totalPixels = image.Width * image.Height;
            int numWhiteDots = (int)(totalPixels * amount);

            for (int i = 0; i < numWhiteDots; i++)
            {
                int x = rand.Next(0, image.Width);
                int y = rand.Next(0, image.Height);

                image[y, x] = new Hsv(0, 0, 255);   
            }
        }
        private void SnowFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (pictureSrc != null)
            {
                Image<Bgr, byte> img1 = new Image<Bgr, byte>(pictureSrc);
                Image<Hsv, byte> hsvImg = img1.Convert<Hsv, byte>();

                for (int y = 0; y < hsvImg.Height; y++)
                {
                    for (int x = 0; x < hsvImg.Width; x++)
                    {
                        Hsv pixel = hsvImg[y, x];
                        if (pixel.Hue >= 20 && pixel.Hue <= 100 && pixel.Satuation >= 50)
                        {
                            pixel.Hue /= 20;
                            pixel.Satuation = 0;
                            pixel.Value += 50;
                        }
                        hsvImg[y, x] = pixel;
                    }
                }

                AddWhiteNoise(hsvImg, amount: 0.1);
                ImageBrush imageBrush = new ImageBrush(Bitmap2BitmapImage(hsvImg.ToBitmap()));
                filtredPicture.Fill = imageBrush;
                CvInvoke.WaitKey(0);
            }
        }

        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);
            bi.StreamSource = ms;
            bi.EndInit();
            return bi;
        }
    }        
}
