using Organisation.ReportTool;
using System;
using System.Windows.Forms;

namespace ReportTool
{
    public partial class ReportToolForm : Form
    {
        private string Cvr;
        private string Uuid;

        public ReportToolForm()
        {
            InitializeComponent();

            cvrTextBox.Text = Organisation.IntegrationLayer.OrganisationRegistryProperties.GetInstance().Municipality;
            uuidTextBox.Text = Organisation.IntegrationLayer.OrganisationRegistryProperties.GetInstance().MunicipalityOrganisationUUID;
        }

        private void generateButtonClick(object sender, EventArgs e)
        {
            Cvr = cvrTextBox.Text;
            Uuid = uuidTextBox.Text;

            if (isValid(Cvr, Uuid))
            {
                Controller controller = new Controller(Cvr, Uuid, logWindow);
                Model model = controller.BuildModel();

                if (model != null)
                {
                    controller.ParseAndWrite(model, "output.html");
                    System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\output.html");
                }
            }
            else
            {
                logWindow.Text += "Ulovlige Cvr / UUID værdier!" + Environment.NewLine;
            }
        }

        private bool isValid(string cvr, string uuid)
        {
            if (string.IsNullOrEmpty(cvr) || string.IsNullOrEmpty(Uuid))
            {
                return false;
            }
            else if (cvr.Length != 8 || uuid.Length != 36)
            {
                return false;
            }

            return true;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
