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
            int tap = Int16.Parse(txtGaussSigma.Text);
            string seed = textBox1.Text;
            // start stopwatch
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            TimeSpan ts;
            string elapsedTime;

            RGBPixel[,] image3 = new RGBPixel[ImageMatrix.GetLength(0), ImageMatrix.GetLength(1)];
            Array.Copy(ImageMatrix, image3, ImageMatrix.GetLength(0) * ImageMatrix.GetLength(1));
            ImageMatrix = ImageOperations.encrypt(ImageMatrix, tap, seed);

            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
             ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
             elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            label10.Text = elapsedTime;

            PriorityQueue redQ = new PriorityQueue();
            PriorityQueue greenQ = new PriorityQueue();
            PriorityQueue blueQ = new PriorityQueue();
            ImageOperations.getFrequency(ImageMatrix, redQ, greenQ, blueQ);
            Huffman huffmanRed = new Huffman(redQ);
            Huffman huffmanGreen = new Huffman(greenQ);
            Huffman huffmanBlue = new Huffman(blueQ);
            double originalSize = ImageMatrix.GetLength(0) * ImageMatrix.GetLength(1) * 8 * 3;
            double compressedSize = huffmanRed.getCompressedSize() + huffmanGreen.getCompressedSize() + huffmanBlue.getCompressedSize();
            double compressionRatio = (compressedSize / originalSize) * 100;
            Compression.compress(ImageMatrix, huffmanRed, huffmanGreen, huffmanBlue,seed,short.Parse(txtGaussSigma.Text));
            ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
               ts.Hours, ts.Minutes, ts.Seconds,
               ts.Milliseconds / 10);

            label12.Text = elapsedTime;
            
            RGBPixel[,] ImageMatrix2 = Compression.decompress("compressEncode.bin",ImageMatrix);
            ImageOperations.DisplayImage(ImageMatrix2, pictureBox3);
            ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
               ts.Hours, ts.Minutes, ts.Seconds,
               ts.Milliseconds / 10);

            label11.Text = elapsedTime;
            stopWatch.Stop();


        }
       
      
        }

}