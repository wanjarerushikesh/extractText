using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web.UI;
using IronOcr;
using IronPdf;

namespace WebApplication2
{
    public partial class _Default : Page
    {
        string pdfFolderPath = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Visible = false;
            Label2.Visible = false;
            SelectedTextTextBox.Visible = false;
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
                        string pdfFolderName = Path.GetFileNameWithoutExtension(orgFileName);
                        pdfFolderPath = Path.Combine(Server.MapPath("~/UploadImages"), pdfFolderName);
                        Directory.CreateDirectory(pdfFolderPath);

                        PdfDocument pdf = PdfDocument.FromFile(uploadFilePath);
                        pdf.RasterizeToImageFiles(Path.Combine(pdfFolderPath, "*.png"));
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
            string filePath = Path.Combine(Server.MapPath("~/UploadImages"), fileName);
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
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".pdf", ".tiff", ".bmp" };
            return Array.Exists(imageExtensions, ext => ext == extension);
        }
    }
}
