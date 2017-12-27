using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;


namespace ImageQuantization
{
    /// <summary>
    /// Holds the pixel color in 3 byte values: red, green and blue
    /// </summary>
    public struct RGBPixel
    {
        public byte red, green, blue;
    }
    public struct RGBPixelD
    {
        public double red, green, blue;
    }


    /// <summary>
    /// Library of static functions that deal with images
    /// </summary>
    public class ImageOperations
    {
        /// <summary>
        /// Open an image and load it into 2D array of colors (size: Height x Width)
        /// </summary>
        /// <param name="ImagePath">Image file path</param>
        /// <returns>2D array of colors</returns>
        /// 

        public static RGBPixel[,] OpenImage(string ImagePath)
        {
            Bitmap original_bm = new Bitmap(ImagePath);
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            RGBPixel[,] Buffer = new RGBPixel[Height, Width];

            unsafe
            {
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x].red = Buffer[y, x].green = Buffer[y, x].blue = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x].red = p[2];
                            Buffer[y, x].green = p[1];
                            Buffer[y, x].blue = p[0];
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;
        }
        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Height</returns>
        public static int GetHeight(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }

        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }

        /// <summary>
        /// Display the given image on the given PictureBox object
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <param name="PicBox">PictureBox object to display the image on it</param>
        public static void DisplayImage(RGBPixel[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[2] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[0] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }



        public static ulong lfsr(ulong seed, int N, int tap)
        {
            ulong most_bit, tap_bit, list_bit;                   //Θ(1)
            for (int i = 0; i < 8; i++)                          //Θ(1)
            {                                                   
                tap_bit = seed & (ulong)(1 << tap);              //Θ(1)
                tap_bit >>= tap;                                 //Θ(1)
                most_bit = seed & (ulong)(1 << (N - 1));         //Θ(1)
                most_bit >>= (N - 1);                            //Θ(1)
                list_bit = most_bit ^ tap_bit;                   //Θ(1)
                seed <<= 1;                                      //Θ(1) 
                seed |= list_bit;                          
            }
            return seed;                                                    //Θ(1)
        }


        public static RGBPixel[,] encrypt(RGBPixel[,] mat, int tap,string seed)
        {
             ulong key2 = Convert.ToUInt64(seed, 2);            //Θ(1)
             ulong mask;                                        //Θ(1)
             int Height = GetHeight(mat);                       //Θ(1)
             int Width = GetWidth(mat);                         //Θ(1)

            for (int j = 0; j < Height; j++)                    //Θ(n^2)
            {
                for (int i = 0; i < Width; i++)                 //Θ(n^2)
                {
                    key2 = lfsr(key2, seed.Length, tap);        //Θ(n^2)
                    mask = key2 & 0xff;                         //Θ(n^2)
                    mat[j, i].red ^= Convert.ToByte(mask);      //Θ(n^2)

                    key2 = lfsr(key2, seed.Length, tap);        //Θ(n^2)
                    mask = key2 & 0xff;                         //Θ(n^2)
                    mat[j, i].green ^= Convert.ToByte(mask);    //Θ(n^2)

                    key2 = lfsr(key2, seed.Length, tap);        //Θ(n^2)
                    mask = key2 & 0xff;                         //Θ(n^2)
                    mat[j, i].blue ^= Convert.ToByte(mask);     //Θ(n^2)
                }
            }
            return mat;                                         //Θ(1)

        }

        public static void getFrequency(RGBPixel[,] ImageMatrix,
            PriorityQueue redQ,                     //Θ(1)
            PriorityQueue greenQ,                   //Θ(1)
            PriorityQueue blueQ)                    //Θ(1)
        {
            int Height = GetHeight(ImageMatrix);    //Θ(1)
            int Width = GetWidth(ImageMatrix);      //Θ(1)
            int[] redArr = new int[256];            //Θ(1)
            int[] greenArr = new int[256];          //Θ(1)
            int[] blueArr = new int[256];           //Θ(1)

            for (int i = 0; i < Height; i++)        //Θ(n)
            {
                for (int j = 0; j < Width; j++)     //Θ(n^2)
                {
                    redArr[ImageMatrix[i, j].red]++;    //Θ(n^2)
                    greenArr[ImageMatrix[i, j].green]++;    //Θ(n^2)
                    blueArr[ImageMatrix[i, j].blue]++;  //Θ(n^2)
                }
            }
            for (int i = 0; i < 256; i++)               //Θ(1)
            {
                if (redArr[i] != 0)                     //Θ(1)
                {
                    redQ.Enqueue(new Node((byte)i, redArr[i]));    //Θ(1)
                }
                if (greenArr[i] != 0)                   //Θ(1)
                {
                    greenQ.Enqueue(new Node((byte)i, greenArr[i])); //Θ(1)
                }
                if (blueArr[i] != 0)                    //Θ(1)
                {
                    blueQ.Enqueue(new Node((byte)i, blueArr[i]));  //Θ(1)
                }
            }
        }
    }
}
