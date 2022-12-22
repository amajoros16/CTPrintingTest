using GdPicture14;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;



namespace CTPrintingTest
{
    /// <summary>
    /// Interaction logic for PrintIt.xaml
    /// </summary>
    public partial class PrintIt : Page
    {
        private bool RegisteredGDPicture_14_1_24_ = false;

        public PrintIt()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtFile.Text))
            {
                MessageBox.Show("Select file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Create the print dialog object and set options
            PrintDialog pDialog = new PrintDialog();
            pDialog.PageRangeSelection = PageRangeSelection.AllPages;
            pDialog.UserPageRangeEnabled = true;

            // Display the dialog. This returns true if the user presses the Print button.
            Nullable<Boolean> print = pDialog.ShowDialog();
            if (print == true)
            {
                if (cmbPrintType.SelectedItem == null)
                {
                    MessageBox.Show("Select print type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if ("GD Picture Official".Equals(cmbPrintType.Text, StringComparison.InvariantCultureIgnoreCase))
                {
                    PrintGDPictureOfficial(txtFile.Text);
                }
                else if ("GD Picture CareTend".Equals(cmbPrintType.Text, StringComparison.InvariantCultureIgnoreCase))
                {
                    PrintGDPictureCareTend(txtFile.Text);
                }
                else
                {
                    txtOutput.Text = txtOutput.Text + "Not implemented yet." + System.Environment.NewLine;
                }
                //XpsDocument xpsDocument = new XpsDocument(fileSelected, FileAccess.ReadWrite);
                //FixedDocumentSequence fixedDocSeq = xpsDocument.GetFixedDocumentSequence();
                //pDialog.PrintDocument(fixedDocSeq.DocumentPaginator, "Test print job");
            }

        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog();
            var result = fileDialog.ShowDialog();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    var file = fileDialog.FileName;
                    txtFile.Text = file;
                    txtFile.ToolTip = file;
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    txtFile.Text = null;
                    txtFile.ToolTip = null;
                    break;
            }
        }

        private void PrintGDPictureOfficial(string fileSelected)
        {
            if (!RegisteredGDPicture_14_1_24_)
            {
                try
                {
                    //RegisterGdPictureKeys
                    LicenseManager licenseManager = new LicenseManager();
                    //old set hardcoded into the services
                    String gdPictureDocumentImaging = "13294798890617477172411429960842027316";
                    String gdPicturePDFPluginLicense = "72637474592727873112111497126291724196";
                    String gdPictureXMPAnnotationsPlugin = "73476099571711969151913499944328750100";
                    String gdPictureBarcodeRecognition = "72847377395707978112112498646373012148";

                    licenseManager.RegisterKEY(gdPictureDocumentImaging);
                    licenseManager.RegisterKEY(gdPicturePDFPluginLicense);
                    licenseManager.RegisterKEY(gdPictureXMPAnnotationsPlugin);
                    licenseManager.RegisterKEY(gdPictureBarcodeRecognition);

                    txtOutput.Text = "GD Picture 14.1.24 registered successfully." + System.Environment.NewLine;
                }
                catch (Exception ex)
                {
                    txtOutput.Text = "GD Picture 14.1.24 failed to register." + System.Environment.NewLine;
                    txtOutput.Text = txtOutput.Text + ex.Message + System.Environment.NewLine;
                    txtOutput.Text = txtOutput.Text + ex.StackTrace + System.Environment.NewLine;
                    return;
                }
            }

            //We assume that GdPicture has been correctly installed and unlocked.
            //Initializing the GdPicturePDF object.
            using (GdPicturePDF pdf = new GdPicturePDF())
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                if (pdf.LoadFromFile(fileSelected, false) == GdPictureStatus.OK)
                {
                    // the code that you want to measure comes here
                    watch.Stop();
                    var elapsedLoad = watch.ElapsedMilliseconds / 1000;

                    //Enabling the pre-rasterization option.
                    watch = System.Diagnostics.Stopwatch.StartNew();
                    pdf.PrintSetPreRasterization(true);
                    string dpiToUse = String.Join("", cmbDPI.Text.ToCharArray().Where(char.IsDigit));
                    pdf.PrintSetPreRasterizationDPI(float.Parse(dpiToUse));
                    pdf.PrintSetFromToPage(1, pdf.GetPageCount());
                    if (pdf.PrintDialog())
                    {
                        watch.Stop();
                        var elapsedPrint = watch.ElapsedMilliseconds / 1000;
                        MessageBox.Show("The file has been printed successfully. Loaded " + elapsedLoad + "s, Printed " + elapsedPrint + "s.", "Printing Example", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        string message = "The file can't be printed.\nStatus: " + pdf.PrintGetStat().ToString();
                        if (pdf.PrintGetStat() == GdPictureStatus.PrintingException)
                            message = message + "    Error: " + pdf.PrintGetLastError();
                        MessageBox.Show(message, "Printing Example", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("The file can't be loaded. Status: " + pdf.GetStat().ToString(), "Printing Example", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }


        private void PrintGDPictureCareTend(string fileSelected)
        {
            using (GdViewer gdViewer = new GdViewer())
            {
                using (GdPicturePDF pdf = new GdPicturePDF())
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    if (pdf.LoadFromFile(fileSelected, false) == GdPictureStatus.OK)
                    {
                        // the code that you want to measure comes here
                        watch.Stop();
                    }
                    gdViewer.DisplayFromGdPicturePDF(pdf);
                }

            }
        }
    }
        
}
