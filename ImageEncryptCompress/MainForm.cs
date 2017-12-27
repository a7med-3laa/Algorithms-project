using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Diagnostics;

namespace ImageQuantization
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            int tap = Int16.Parse(txtGaussSigma.Text);              //ϴ(1)
            string seed = textBox1.Text;                                    //ϴ(1)
            // start stopwatch
            Stopwatch stopWatch = new Stopwatch();                 // ϴ(1)
            stopWatch.Start();                                                     // ϴ(1)

            TimeSpan ts;                                                             // ϴ(1)
            string elapsedTime;                                                   // ϴ(1)

            RGBPixel[,] image3 = new RGBPixel[ImageMatrix.GetLength(0), ImageMatrix.GetLength(1)];          // ϴ(1)
            Array.Copy(ImageMatrix, image3, ImageMatrix.GetLength(0) * ImageMatrix.GetLength(1));           // ϴ(N^2) where n is the largest between width and height
            ImageMatrix = ImageOperations.encrypt(ImageMatrix, tap, seed);                                                 // ϴ(N^2) where n is the largest between width and height

            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);                                                            // ϴ(N^2) where n is the largest between width and height
            ts = stopWatch.Elapsed;                                                                                                                   // ϴ(1)
            // Format and display the TimeSpan value.
             elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",                                             
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);                                                                                                                  // ϴ(1)
            label10.Text = elapsedTime;                                                                                                             // ϴ(1)

            PriorityQueue redQ = new PriorityQueue();                                                                                       // ϴ(1)
            PriorityQueue greenQ = new PriorityQueue();                                                                                     // ϴ(1)
            PriorityQueue blueQ = new PriorityQueue();                                                                                        // ϴ(1)
            ImageOperations.getFrequency(ImageMatrix, redQ, greenQ, blueQ);                                                 // ϴ(N^2) where n is the largest between width and height
            Huffman huffmanRed = new Huffman(redQ);                                                                                     // ϴ(n log n) where n is the largest between width and height
            Huffman huffmanGreen = new Huffman(greenQ);                                                                             // ϴ(n log n) where n is the largest between width and height
            Huffman huffmanBlue = new Huffman(blueQ);                                                                                   // ϴ(n log n) where n is the largest between width and height
            double originalSize = ImageMatrix.GetLength(0) * ImageMatrix.GetLength(1) * 8 * 3;                          // ϴ(1)
            double compressedSize = huffmanRed.getCompressedSize() + huffmanGreen.getCompressedSize() + huffmanBlue.getCompressedSize();    // ϴ(1)
            double compressionRatio = (compressedSize / originalSize) * 100;                                                        // ϴ(1)
            Compression.compress(ImageMatrix, huffmanRed, huffmanGreen, huffmanBlue,seed,short.Parse(txtGaussSigma.Text));      // ϴ(N^2) where n is the largest between width and height
            ts = stopWatch.Elapsed;                                                                                                                         // ϴ(1)
            // Format and display the TimeSpan value.
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",      
               ts.Hours, ts.Minutes, ts.Seconds,
               ts.Milliseconds / 10);                                                                                                                           // ϴ(1)

            label12.Text = elapsedTime;                                                                                                                 // ϴ(1)

            RGBPixel[,] ImageMatrix2 = Compression.decompress("compressEncode.bin",ImageMatrix);                    // ϴ(N^2) where n is the largest between width and height
            ImageOperations.DisplayImage(ImageMatrix2, pictureBox3);                                                                // ϴ(N^2) where n is the largest between width and height
            ts = stopWatch.Elapsed;                                                                                                                         // ϴ(1)
            // Format and display the TimeSpan value.
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
               ts.Hours, ts.Minutes, ts.Seconds,
               ts.Milliseconds / 10);                                                                                                                           // ϴ(1)

            label11.Text = elapsedTime;                                                                                                                     // ϴ(1)
            stopWatch.Stop();                                                                                                                                   // ϴ(1)


        }
       
      
        }

}