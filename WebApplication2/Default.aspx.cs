using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IronOcr;

namespace WebApplication2
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Visible = false;
            Label2.Visible = false;
            SelectedTextTextBox.Visible = false;
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            Label1.Visible = true;
            // Upload Original Image Here  
            string uploadFileName = "";
            string uploadFilePath = "";
            if (FU1.HasFile)
            {
                Label1.Visible = true;
                string orgFileName = Path.GetFileName(FU1.FileName).ToLower();
                string ext = Path.GetExtension(FU1.FileName).ToLower();
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".png")
                {
                    //uploadFileName = Guid.NewGuid().ToString() + ext;

                    uploadFileName = orgFileName.ToString();

                    uploadFilePath = Path.Combine(Server.MapPath("~/UploadImages"), uploadFileName);
                    try
                    {
                        FU1.SaveAs(uploadFilePath);
                        imgUpload.ImageUrl = "~/UploadImages/" + uploadFileName;
                        //panCrop.Visible = true;
                    }
                    catch (Exception)
                    {
                        lblMsg.Text = "Error! Please try again.";
                    }
                }
                else
                {
                    lblMsg.Text = "Selected file type not allowed!";
                }
            }
            else
            {
                lblMsg.Text = "Please select file first!";
            }
        }
        protected void btnCrop_Click(object sender, EventArgs e)
        {
            // Crop Image Here & Save
            string fileName = Path.GetFileName(imgUpload.ImageUrl);
            string filePath = Path.Combine(Server.MapPath("~/UploadImages"), fileName);
            string cropFileName = "";
            string cropFilePath = "";

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
                    Bitmap bitMap = new Bitmap(CropArea.Width, CropArea.Height);
                    using (Graphics g = Graphics.FromImage(bitMap))
                    {
                        g.DrawImage(orgImg, new Rectangle(0, 0, bitMap.Width, bitMap.Height), CropArea, GraphicsUnit.Pixel);
                    }

                    cropFileName = "crop_" + fileName;
                    cropFilePath = Path.Combine(Server.MapPath("~/UploadImages"), cropFileName);
                    bitMap.Save(cropFilePath);
                    //Response.Redirect("~/UploadImages/" + cropFileName, false);
                    if (!String.IsNullOrEmpty(cropFilePath))
                    {
                        Label2.Visible = true;
                        lblMsg.Text = "Text Extracted Successfully ";
                        // Extract and display text using IronOcr
                        var ocr = new AutoOcr();
                        var result = ocr.Read(cropFilePath);
                        SelectedTextTextBox.Visible = true;
                        SelectedTextTextBox.Text = result.Text;
                    }
                    else
                    {
                        lblMsg.Text = "Please Crop file first!";
                    }
                    
                }
                catch (Exception ex)
                {
                    throw;
                }

            }
            else
            {
                lblMsg.Text = "Please select file first!";
            }
        }
    }
}