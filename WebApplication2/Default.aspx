<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication2._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <link href="Scripts/jquery.Jcrop.css" rel="stylesheet" />
    <script src="http://code.jquery.com/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery-jcrop/2.0.4/js/Jcrop.min.js"></script>
    <link rel="Stylesheet" type="text/css" href="https://cdnjs.cloudflare.com/ajax/libs/jquery-jcrop/2.0.4/css/Jcrop.css" />
    <script type="text/javascript">
        $(document).ready(function () {
            $('#<%=imgUpload.ClientID%>').Jcrop({
                onSelect: SelectCropArea
            });
        });
        function SelectCropArea(c) {
            $('#<%=X.ClientID%>').val(parseInt(c.x));
            $('#<%=Y.ClientID%>').val(parseInt(c.y));
            $('#<%=W.ClientID%>').val(parseInt(c.w));
            $('#<%=H.ClientID%>').val(parseInt(c.h));
        }
    </script>

    <style>
        .center-content {
            text-align: center;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            text-decoration-color: white;
            background-color: grey
        }

        .htitle {
            font-size: 24px;
            font-family: 'Times New Roman', Times, serif;
            font-weight: 700;
        }

        #SelectedTextTextBox {
            border-style: none;
            border-color: inherit;
            border-width: 0;
            max-width: 99%;
        }
    </style>

    <h3>Image Upload, Crop & Save using ASP.NET & Jquery</h3>
    <table style="width: 635px; height: 57px">
        <tr>
            <td style="width: 127px; height: 37px">Select Image File : </td>
            <td style="width: 277px; height: 37px">
                <asp:FileUpload ID="FU1" runat="server" Height="26px" Width="190px" MaximumFileSize="10240" CssClass="col-xs-offset-0" />
            </td>
            <td style="height: 37px; width: 122px;">
                <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click" Height="26px" Width="95px" />
            </td>
            <td>
                <asp:Button ID="btnCrop" runat="server" Text="Crop & Save" OnClick="btnCrop_Click" />

            </td>

        </tr>
        <tr>
            <td colspan="3">
                <asp:Label ID="lblMsg" runat="server" ForeColor="Red" />
            </td>
        </tr>
    </table>
    <div class="container">
        <div>
            <asp:Label ID="Label1" runat="server" Text="Document Viewer" ForeColor="Red" />
            <asp:Image ID="imgUpload" runat="server" Style="max-width: 50%; height: auto;" BorderColor="#3366FF" BorderStyle="Solid" Height="281px" Width="96%" />

            <asp:Label ID="Label2" runat="server" Text="Text Viewer" ForeColor="Red" />
            <asp:TextBox ID="SelectedTextTextBox" runat="server" TextMode="MultiLine" Rows="10" Width="96%" Height="281px" ReadOnly="True"></asp:TextBox>
        </div>
    </div>

    <asp:HiddenField ID="X" runat="server" />
    <asp:HiddenField ID="Y" runat="server" />
    <asp:HiddenField ID="W" runat="server" />
    <asp:HiddenField ID="H" runat="server" />

</asp:Content>
