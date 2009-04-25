using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using tessnet2;
using System.Diagnostics;

namespace LicensePlateRecognition
{
   public partial class LicensePlateRecognitionForm : Form
   {
      private LicensePlateDetector _licensePlateDetector;

      public LicensePlateRecognitionForm()
      {
         InitializeComponent();
         _licensePlateDetector = new LicensePlateDetector();

         ProcessImage(new Image<Bgr, byte>("license-plate.jpg"));
      }

      private void ProcessImage(Image<Bgr, byte> image)
      {
         List<Image<Gray, Byte>> licensePlateList = new List<Image<Gray, byte>>();
         List<Image<Gray, Byte>> filteredLicensePlateList = new List<Image<Gray, byte>>();
         List<MCvBox2D> licenseBoxList = new List<MCvBox2D>();
         List<List<Word>> words = _licensePlateDetector.DetectLicensePlate(
            image,
            licensePlateList,
            filteredLicensePlateList,
            licenseBoxList);

         panel1.Controls.Clear();

         Point startPoint = new Point(10, 10);
         ShowLicense(ref startPoint, words, licensePlateList, filteredLicensePlateList, licenseBoxList);
         foreach (MCvBox2D box in licenseBoxList)
            image.Draw(box, new Bgr(Color.Red), 2);

         imageBox1.Image = image;
      }

      private void AddLabelAndImage(ref Point startPoint, String labelText, IImage image)
      {
         Label label = new Label();
         panel1.Controls.Add(label);
         label.Text = labelText;
         label.Width = 100;
         label.Height = 30;
         label.Location = startPoint;
         startPoint.Y += label.Height;

         ImageBox box = new ImageBox();
         panel1.Controls.Add(box);
         box.ClientSize = image.Size;
         box.Image = image;
         box.Location = startPoint;
         startPoint.Y += box.Height + 10;
      }

      private void ShowLicense(ref Point startPoint, List<List<Word>> licenses, List<Image<Gray, Byte>> licensePlateList, List<Image<Gray, Byte>> filteredLicensePlateList, List<MCvBox2D> boxList)
      {
         for (int i = 0; i < licenses.Count; i++)
         {
            AddLabelAndImage(
               ref startPoint,
               "License: " + String.Join(" ", licenses[i].ConvertAll<String>(delegate(Word w) { return w.Text; }).ToArray()),
               licensePlateList[i].ConcateVertical(filteredLicensePlateList[i]));
         }
      }

      private void button1_Click(object sender, EventArgs e)
      {
         DialogResult result = openFileDialog1.ShowDialog();
         if (result == DialogResult.OK)
         {
            Image<Bgr, Byte> img;
            try
            {
               img = new Image<Bgr, byte>(openFileDialog1.FileName);
            }
            catch
            {
               MessageBox.Show("Invalide file format");
               return;
            }

            ProcessImage(img);
         }
      }
   }

}