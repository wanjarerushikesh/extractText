using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.UI;
using IronOcr;
using IronPdf;

namespace WebApplication2
{
    public partial class _Default : Page
    {
        string pdfFolderPath = "";
        string pdfFolderName = "";

        private string[] pdfImagePaths;
        private int currentImageIndex = 0;
        private string convImage = "";
        private string filePath;

        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Visible = false;
            Label2.Visible = false;
            SelectedTextTextBox.Visible = false;

            // Always retrieve the value of convImage from ViewState
            if (ViewState["ConvImage"] != null)
            {
                convImage = ViewState["ConvImage"].ToString();
            }

            if (!IsPostBack)
            {
                // Load the PDF image paths when a PDF is uploaded
                if (pdfFolderName != "")
                {
                    pdfImagePaths = Directory.GetFiles(pdfFolderPath, "*.png");
                    ViewState["PdfImagePaths"] = pdfImagePaths; // Store in ViewState
                    ViewState["PDFFolderName"] = pdfFolderName; // Store pdfFolderName in ViewState
                    btnPrev.Visible = pdfImagePaths.Length > 0;
                    btnNext.Visible = pdfImagePaths.Length > 0;
                }
             
            }
            else
            {
                // Retrieve pdfFolderName and currentImageIndex from ViewState
                if (ViewState["PDFFolderName"] != null)
                {
                    pdfFolderName = ViewState["PDFFolderName"].ToString();
                }
                if (ViewState["CurrentImageIndex"] != null)
                {
                    currentImageIndex = (int)ViewState["CurrentImageIndex"];
                }
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            Label1.Visible = true;

            if (FU1.HasFile)
            {
                string orgFileName = Path.GetFileName(FU1.FileName).ToLower();
                string ext = Path.GetExtension(FU1.FileName).ToLower();

                if (IsImageFile(ext) || ext == ".pdf")
                {
                    string uploadFileName = orgFileName;
                    string uploadFilePath = Path.Combine(Server.MapPath("~/UploadImages"), uploadFileName);

                    FU1.SaveAs(uploadFilePath);

                    if (ext == ".pdf")
                    {
                        // Create a folder with the same name as the PDF file
                        pdfFolderName = Path.GetFileNameWithoutExtension(orgFileName);
                        ViewState["PDFFolderName"] = pdfFolderName;

                        pdfFolderPath = Path.Combine(Server.MapPath("~/UploadImages"), pdfFolderName);
                        Directory.CreateDirectory(pdfFolderPath);

                        PdfDocument pdf = PdfDocument.FromFile(uploadFilePath);
                        pdf.RasterizeToImageFiles(Path.Combine(pdfFolderPath, "*.png"));

                        pdfImagePaths = Directory.GetFiles(pdfFolderPath, "*.png");
                        Array.Sort(pdfImagePaths, new NumericFilenameComparer());
                        ViewState["PdfImagePaths"] = pdfImagePaths;

                        DisplayFirstImage();
                    }
                    else
                    {
                        imgUpload.ImageUrl = "~/UploadImages/" + uploadFileName;
                    }
                }
                else
                {
                    lblMsg.Text = "Selected file type not allowed!";
                }
            }
            else
            {
                lblMsg.Text = "Please select a file first!";
            }
        }

        protected void btnCrop_Click(object sender, EventArgs e)
        {
            // Crop Image Here & Save
            string fileName = Path.GetFileName(imgUpload.ImageUrl);
            if (!string.IsNullOrEmpty(convImage))
            {
                filePath = Server.MapPath(convImage);
            }
            else
            {
                filePath = Path.Combine(Server.MapPath("~/UploadImages"), fileName);
            }

            string cropFileName = "crop_" + fileName;
            string cropFilePath = Path.Combine(Server.MapPath("~/UploadImages"), cropFileName);

            if (File.Exists(filePath))
            {
                System.Drawing.Image orgImg = System.Drawing.Image.FromFile(filePath);
                Rectangle CropArea = new Rectangle(
                    Convert.ToInt32(X.Value),
                    Convert.ToInt32(Y.Value),
                    Convert.ToInt32(W.Value),
                    Convert.ToInt32(H.Value));

                try
                {
                    using (Bitmap bitMap = new Bitmap(CropArea.Width, CropArea.Height))
                    using (Graphics g = Graphics.FromImage(bitMap))
                    {
                        g.DrawImage(orgImg, new Rectangle(0, 0, bitMap.Width, bitMap.Height), CropArea, GraphicsUnit.Pixel);
                        bitMap.Save(cropFilePath);
                    }

                    Label2.Visible = true;
                    lblMsg.Text = "Text Extracted Successfully";
                    // Extract and display text using IronOcr
                    var ocr = new AutoOcr();
                    var result = ocr.Read(cropFilePath);
                    SelectedTextTextBox.Visible = true;
                    SelectedTextTextBox.Text = result.Text;
                }
                catch (Exception ex)
                {
                    lblMsg.Text = "Error during cropping: " + ex.Message;
                }
            }
            else
            {
                lblMsg.Text = "Please select a file first!";
            }
        }

        private bool IsImageFile(string extension)
        {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".tiff", ".bmp" };
            return Array.Exists(imageExtensions, ext => ext == extension);
        }

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            pdfImagePaths = ViewState["PdfImagePaths"] as string[];
            if (pdfImagePaths != null && currentImageIndex > 0)
            {
                currentImageIndex--;
                ViewState["CurrentImageIndex"] = currentImageIndex; // Store initial value in ViewState
                DisplayImage();
            }
        }
        private void DisplayFirstImage()
        {
            if (pdfImagePaths.Length > 0)
            {
                currentImageIndex = 0;
                DisplayImage();
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            pdfImagePaths = ViewState["PdfImagePaths"] as string[]; // Retrieve from ViewState
            if (pdfImagePaths != null && currentImageIndex < pdfImagePaths.Length - 1)
            {
                currentImageIndex++;
                ViewState["CurrentImageIndex"] = currentImageIndex; // Store updated value in ViewState
                DisplayImage();
            }
        }

        private void DisplayImage()
        {
            if (pdfImagePaths.Length > 0 && currentImageIndex >= 0 && currentImageIndex < pdfImagePaths.Length)
            {
                convImage = pdfImagePaths[currentImageIndex];

                string virtualImagePath = "~/UploadImages/" + pdfFolderName + "/" + Path.GetFileName(pdfImagePaths[currentImageIndex]);
                imgUpload.ImageUrl = virtualImagePath;

                ViewState["ConvImage"] = virtualImagePath;

                SelectedTextTextBox.Text = "";
                lblMsg.Text = "";

                SelectedTextTextBox.Visible = false;
                Label2.Visible = false;
            }
        }

        public class NumericFilenameComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                int numberX = ExtractNumber(x);
                int numberY = ExtractNumber(y);

                return numberX.CompareTo(numberY);
            }

            private int ExtractNumber(string s)
            {
                string filename = Path.GetFileNameWithoutExtension(s);
                string numberPart = new string(filename.SkipWhile(c => !char.IsDigit(c)).TakeWhile(char.IsDigit).ToArray());

                int number;
                if (int.TryParse(numberPart, out number))
                {
                    return number;
                }
                return -1;
            }
        }
    }
}
