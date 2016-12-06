using Organisation.BusinessLayer;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using RazorEngine;
using RazorEngine.Templating;
using Organisation.IntegrationLayer;
using RazorEngine.Configuration;
using System.Windows.Forms;

namespace Organisation.ReportTool
{
    public class Controller
    {
        private InspectorService inspectorService;
        private RichTextBox log;

        public Controller(string cvr, string orgUuid, RichTextBox log)
        {
            this.log = log;

            // overwrite settings from registry - this must happen before calling Init() as that will start fetching tokens
            OrganisationRegistryProperties properties = OrganisationRegistryProperties.GetInstance();
            properties.Municipality = cvr;
            properties.MunicipalityOrganisationUUID = orgUuid;

            Initializer.Init();

            this.inspectorService = new InspectorService();

            var config = new TemplateServiceConfiguration();
            config.CachingProvider = new DefaultCachingProvider(t => { });
            var service = RazorEngineService.Create(config);
            Engine.Razor = service;
        }

        public Model BuildModel()
        {
            
            log.Text+= "Søger efter enheder: ";
            var ous = ReadOUs(inspectorService.FindAllOUs());
            log.Text += "Søger efter brugere: ";
            var users = ReadUsers(inspectorService.FindAllUsers());

            log.Text += "Bygger model af data i hukommelsen..." + System.Environment.NewLine;
            OU root = GetRootOUFromList(ous);
            if (root == null)
            {
                log.Text += "No OUs found!" + System.Environment.NewLine;
                return null;
            }

            TreeNode<OU> tree = BuildTree(root, ous);

            List<OUModel> ouModels = new List<OUModel>();
            foreach (OU ou in ous)
            {
                OUModel ouModel = new OUModel();
                ouModel.Uuid = ou.Uuid;
                ouModel.Name = ou.Name;
                ouModel.AddressDetails = GetDetailAddresses(ou.Addresses);
                ouModel.EmployeesDetails = GetDetailEmployees(ou, users);
                ouModel.BrugervendtNoegle = ou.ShortKey;
                ouModels.Add(ouModel);
            }

            List<UserModel> userModels = new List<UserModel>();
            foreach (User user in users)
            {
                UserModel userModel = new UserModel();
                userModel.Name = user.Person.Name;
                userModel.Uuid = user.Uuid;
                userModel.UserId = user.UserId;
                userModel.ShortKey = user.ShortKey;
                userModel.Cpr = user.Person.Cpr;
                userModel.AddressDetails = GetDetailAddresses(user.Addresses);
                userModels.Add(userModel);
            }

            Model model = new Model();
            model.AsciiTreeRepresentation = tree.PrintPretty("", false);
            model.OUs = ouModels;
            model.Users = userModels;

            return model;
        }

        public string Parse(Model model)
        {
            return Engine.Razor.RunCompile(File.ReadAllText("templates/template.html"), "templateKey", null, model); ;
        }

        public void ParseAndWrite(Model model, string outputFileName)
        {
            log.AppendText("Writing output");
            string htmlPage = Engine.Razor.RunCompile(File.ReadAllText("templates/template.html"), "templateKey", null, model);
            File.WriteAllText(outputFileName, htmlPage);
        }

        private string GetDetailAddresses(List<AddressHolder> addresses)
        {
            StringBuilder sb = new StringBuilder();

            foreach (AddressHolder address in addresses)
            {
                sb.AppendLine("\t" + address.GetType().Name + " = " + address.Value);
            }

            return sb.ToString();
        }

        private string GetDetailEmployees(OU ou, List<User> users)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Position position in ou.Positions)
            {
                if (position.User?.Uuid != null)
                {
                    string employeeName = "<Ikke registreret bruger>";
                    foreach (User user in users)
                    {
                        if (user.Uuid.Equals(position.User.Uuid))
                        {
                            employeeName = user.Person.Name;
                            break;
                        }
                    }

                    sb.AppendLine("\t" + position.Name + " (" + position.ShortKey + "), besat af " + employeeName + " (" + position.User.Uuid + ")");
                }
                else
                {
                    sb.AppendLine("\t" + position.Name + " (" + position.ShortKey + "), ubesat stilling");
                }
            }

            return sb.ToString();
        }

        public List<User> ReadUsers(List<string> users)
        {
            log.Text += "Fandt " + users.Count + " brugere" + System.Environment.NewLine;
            log.Text += "Læser brugere: ";

            long pixelStep = users.Count / 10 + 1;
            long count = 0;

            List<User> result = new List<User>();
            foreach (string uuid in users)
            {
                User user = inspectorService.ReadUserObject(uuid);
                result.Add(user);

                if (count++ % pixelStep == 0)
                {
                    log.Text += "*";
                }
            }

            log.Text += System.Environment.NewLine;

            return result;
        }

        public List<OU> ReadOUs(List<string> ous)
        {
            log.Text += "Fandt " + ous.Count + " enheder" + System.Environment.NewLine;
            log.Text += "Læser enheder: ";

            long pixelStep = ous.Count / 10 + 1;
            long count = 0;

            List<OU> result = new List<OU>();
            foreach (string uuid in ous)
            {
                OU ou = inspectorService.ReadOUObject(uuid);
                result.Add(ou);

                if (count++ % pixelStep == 0)
                {
                    log.Text += "*";
                }
            }

            log.Text += System.Environment.NewLine;

            return result;
        }

        private List<OU> GetChilds(OU ou, List<OU> ous)
        {
            string uuid = ou.Uuid;
            List<OU> result = new List<OU>();
            foreach (OU anOU in ous)
            {
                if (uuid.Equals(anOU?.ParentOU?.Uuid))
                {
                    result.Add(anOU);
                }
            }

            return result;
        }

        private TreeNode<OU> BuildTree(OU root, List<OU> flatStructure)
        {
            TreeNode<OU> result = new TreeNode<OU>(root);

            foreach (OU child in GetChilds(root, flatStructure))
            {
                result.AddTreeNodeChild(BuildTree(child, flatStructure));
            }

            return result;
        }

        private OU GetRootOUFromList(List<OU> ous)
        {
            foreach (OU ou in ous)
            {
                if (ou.ParentOU == null)
                {
                    return ou;
                }
            }

            return null;
        }
    }
}
