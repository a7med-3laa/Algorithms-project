using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
///Algorithms Project
///Intelligent Scissors
///

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
            bool most_bit, tap_bit , list_bit ;
            for (int i = 0; i < 8; i++)
            {
                tap_bit  = Convert.ToBoolean(seed & (ulong)(1 << tap));
                most_bit = Convert.ToBoolean(seed & (ulong)(1 << (N - 1)));
                list_bit = most_bit ^ tap_bit;
                seed<<= 1;
                seed |=Convert.ToUInt64(list_bit);
            }
            return seed;
        }

        public static RGBPixel[,] encrypt(RGBPixel[,] mat, int tap,string seed)
        {
            ulong key2 = Convert.ToUInt64(seed, 2);
            ulong mask;
            int Height = GetHeight(mat);
            int Width = GetWidth(mat);
         
            for (int j = 0; j < Height; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    key2 = lfsr(key2, seed.Length, tap);
                    mask = key2 & 0xff;
                    mat[j, i].red ^= Convert.ToByte(mask);

                    key2 = lfsr(key2, seed.Length, tap);
                    mask = key2 & 0xff;
                    mat[j, i].green ^= Convert.ToByte(mask);

                    key2 = lfsr(key2, seed.Length, tap);
                    mask = key2 & 0xff;
                    mat[j, i].blue ^= Convert.ToByte(mask);
                }
            }
            return mat;

        }

        public static void getFrequency(RGBPixel[,] ImageMatrix,
            PriorityQueue redQ,
            PriorityQueue greenQ,
            PriorityQueue blueQ)
        {
            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);
            int[] redArr = new int[256];
            int[] greenArr = new int[256];
            int[] blueArr = new int[256];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    redArr[ImageMatrix[i, j].red]++;
                    greenArr[ImageMatrix[i, j].green]++;
                    blueArr[ImageMatrix[i, j].blue]++;
                }
            }
            for (int i = 0; i < 256; i++)
            {
                if (redArr[i] != 0)
                {
                    redQ.Enqueue(new Node(i, redArr[i]));
                }
                if (greenArr[i] != 0)
                {
                    greenQ.Enqueue(new Node(i, greenArr[i]));
                }
                if (blueArr[i] != 0)
                {
                    blueQ.Enqueue(new Node(i, blueArr[i]));
                }
            }
        }
    }
}
