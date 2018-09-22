
using Organisation.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OrganisationInspector
{
    /// <summary>
    /// This class serves as an entry point for all the form events.
    /// Only event handler code should be written here.
    /// </summary>
    public partial class MainForm : Form
    {
        private FormHelper formHelper;
        private ValidatorBroker validatorBroker;

        public MainForm()
        {
            InitializeComponent();
            Initializer.Init();
            formHelper = new FormHelper();
            validatorBroker = new ValidatorBroker();

            comboBox1.DataSource = formHelper.GetObjectTypeComboBox();
            cvr_box.Text = Organisation.IntegrationLayer.OrganisationRegistryProperties.GetMunicipality();
            uuid_box.Text = Organisation.IntegrationLayer.OrganisationRegistryProperties.GetInstance().MunicipalityOrganisationUUID[Organisation.IntegrationLayer.OrganisationRegistryProperties.GetMunicipality()];
        }

        private void runButton_Click(object sender, System.EventArgs e)
        {
            string uuid = new FormHelper().ReadUUIDFromTextBox(textBox1);
            string orgObjectType = (string)comboBox1.SelectedValue;
            bool raw = checkBox1.Checked;
            try
            {
                ParserUtils.WriteTemporaryFile(OrgUtils.GetOrgObject(uuid, orgObjectType, raw), HTMLUtils.TEMPORARY_DISPLAY_XML);
                webBrowser1.Navigate(Application.StartupPath + "\\" + HTMLUtils.TEMPORARY_DISPLAY_XML);
            }
            catch (Exception ex)
            {
                ParserUtils.WriteTemporaryFile(HTMLUtils.GetErrorPage(ex.Message + Environment.NewLine + ex.StackTrace), HTMLUtils.TEMPORARY_ERRORS_PAGE);
                webBrowser1.Navigate(Application.StartupPath + "\\" + HTMLUtils.TEMPORARY_ERRORS_PAGE);
            }
        }

        private void validateButton_Click(object sender, EventArgs e)
        {
            // sample uuid = "f5deadd7-2ac9-478b-b7ff-58cd0dad0b4c"
            string uuid = new FormHelper().ReadUUIDFromTextBox(textBox1);
            string orgObjectType = (string)comboBox1.SelectedValue;
            bool raw = checkBox1.Checked;

            try
            {
                OrgUtils.GetOrgObject(uuid, orgObjectType, true); // we don't really care about the result, this is mostly for validating that the object exists
                Validator validator = validatorBroker.GetValidator(orgObjectType, uuid_box.Text);
                List<string> errors = validator.Validate(uuid);
                ParserUtils.WriteTemporaryFile(HTMLUtils.GetErrorList(errors), HTMLUtils.VALIDATION_RESULT_HTML);

                webBrowser1.Navigate(Application.StartupPath + "\\" + HTMLUtils.VALIDATION_RESULT_HTML);
            }
            catch (Exception ex)
            {
                ParserUtils.WriteTemporaryFile(HTMLUtils.GetErrorPage(ex.Message), HTMLUtils.TEMPORARY_ERRORS_PAGE);
                webBrowser1.Navigate(Application.StartupPath + "\\" + HTMLUtils.TEMPORARY_ERRORS_PAGE);
            }
        }
        
        private void MainForm_FormClosed_1(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
