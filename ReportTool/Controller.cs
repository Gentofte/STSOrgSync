using Organisation.BusinessLayer;
using System.Collections.Generic;
using System.Linq;
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
            OrganisationRegistryProperties.Municipality = cvr;
            properties.MunicipalityOrganisationUUID[OrganisationRegistryProperties.GetMunicipality()] = orgUuid;

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

            var ous = inspectorService.ReadOUHierarchy(ReadAddresses.YES, ReadParentDetails.NO, ReadPayoutUnit.YES, ReadPositions.YES, ReadItSystems.YES, ReadContactPlaces.YES);
            log.Text += "Fandt " + ous.Count + " enheder" + System.Environment.NewLine;

            log.Text += "Søger efter brugere: ";
            var userUuids = inspectorService.FindAllUsers(ous).Distinct().ToList();
            var users = ReadUsers(userUuids);

            log.Text += "Bygger model af data i hukommelsen..." + System.Environment.NewLine;
            OU root = GetRootOUFromList(ous);
            if (root == null)
            {
                log.Text += "No OUs found!" + System.Environment.NewLine;
                return null;
            }

            TreeNode<OU> tree = BuildTree(root, ous);

            List<OUItSystem> itSystems = new List<OUItSystem>();

            List<OUModel> ouModels = new List<OUModel>();
            foreach (OU ou in ous)
            {
                OUModel ouModel = new OUModel();
                ouModel.Uuid = ou.Uuid;
                ouModel.Name = ou.Name;
                ouModel.Status = ou.Status.ToString();
                ouModel.AddressDetails = GetDetailAddresses(ou.Addresses);
                ouModel.EmployeesDetails = GetDetailEmployees(ou, users);
                ouModel.BrugervendtNoegle = ou.ShortKey;
                ouModels.Add(ouModel);

                if (ou.ItSystems != null)
                {
                    foreach (string x in ou.ItSystems)
                    {
                        bool found = false;

                        foreach (OUItSystem it in itSystems)
                        {
                            if (it.Uuid.Equals(x))
                            {
                                found = true;
                                it.Enheder.Add(ou.Name);
                            }
                        }

                        if (!found)
                        {
                            OUItSystem it = new OUItSystem();
                            it.Uuid = x;
                            it.Enheder.Add(ou.Name);

                            itSystems.Add(it);
                        }
                    }
                }
            }

            List<UserModel> userModels = new List<UserModel>();
            foreach (User user in users)
            {
                UserModel userModel = new UserModel();
                userModel.Name = user.Person?.Name;
                userModel.Status = user.Status.ToString();
                userModel.Uuid = user.Uuid;
                userModel.UserId = user.UserId;
                userModel.ShortKey = user.ShortKey;
                userModel.Cpr = user.Person?.Cpr;
                userModel.AddressDetails = GetDetailAddresses(user.Addresses);
                userModels.Add(userModel);
            }

            List<PayoutUnitModel> payoutUnitModels = new List<PayoutUnitModel>();
            foreach (OU ou in ous)
            {
                if (ou.PayoutOU != null)
                {
                    foreach (OU payoutUnit in ous)
                    {
                        if (payoutUnit.Uuid.Equals(ou.PayoutOU.Uuid))
                        {
                            PayoutUnitModel payoutUnitModel = new PayoutUnitModel();
                            payoutUnitModel.UnitName = ou.Name;
                            payoutUnitModel.UnitUuid = ou.Uuid;
                            payoutUnitModel.PayoutUnitName = payoutUnit.Name;
                            payoutUnitModel.PayoutUnitUuid = payoutUnit.Uuid;

                            foreach (AddressHolder address in payoutUnit.Addresses)
                            {
                                if (address is LOSShortName)
                                {
                                    payoutUnitModel.PayoutUnitLOSShortKey = address.Value;
                                    break;
                                }
                            }

                            payoutUnitModels.Add(payoutUnitModel);
                            break;
                        }
                    }
                }
            }

            List<ContactPlacesModel> contactPlacesModels = new List<ContactPlacesModel>();
            foreach (OU ou in ous)
            {
                if (ou.ContactPlaces != null)
                {
                    foreach (ContactPlace cp in ou.ContactPlaces)
                    {
                        foreach (OU contactPlace in ous)
                        {
                            if (contactPlace.Uuid.Equals(cp.OrgUnit.Uuid))
                            {
                                ContactPlacesModel cpModel = new ContactPlacesModel();
                                cpModel.ContactUnitName = contactPlace.Name;
                                cpModel.ContactUnitUuid = contactPlace.Uuid;
                                cpModel.UnitName = ou.Name;
                                cpModel.UnitUuid = ou.Uuid;
                                cpModel.Opgaver = cp.Tasks;

                                contactPlacesModels.Add(cpModel);
                                break;
                            }
                        }
                    }
                }
            }

            Model model = new Model();
            model.AsciiTreeRepresentation = tree.PrintPretty("", false);
            model.OUs = ouModels;
            model.Users = userModels;
            model.PayoutUnits = payoutUnitModels;
            model.ContactPlaces = contactPlacesModels;
            model.ItSystems = itSystems;

            return model;
        }

        public string Parse(Model model)
        {
            return Engine.Razor.RunCompile(File.ReadAllText("templates/template.html"), "templateKey", null, model); ;
        }

        public void ParseAndWrite(Model model, string outputFileName)
        {
            log.AppendText("Writing output to: " + System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\\" + outputFileName);
            string htmlPage = Engine.Razor.RunCompile(File.ReadAllText("templates/template.html"), "templateKey", null, model);
            File.WriteAllText(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\\" + outputFileName, htmlPage);
        }

        private string GetDetailAddresses(List<AddressHolder> addresses)
        {
            StringBuilder sb = new StringBuilder();

            if (addresses != null)
            {
                foreach (AddressHolder address in addresses)
                {
                    sb.AppendLine(address.GetType().Name + " = " + address.Value);
                }
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

                    sb.AppendLine("\t" + position.Name + ", besat af " + employeeName);
                }
                else
                {
                    sb.AppendLine(position.Name + ", ubesat stilling");
                }
            }

            return sb.ToString();
        }

        public List<User> ReadUsers(List<string> users)
        {
            log.Text += "Fandt " + users.Count + " brugere" + System.Environment.NewLine;
            log.Text += "Læser brugere: [                         ]"; // 25 spaces

            long pixels = 0;

            List<User> result = new List<User>();
            foreach (string uuid in users)
            {
                User user = inspectorService.ReadUserObject(uuid);
                result.Add(user);

                if (((100 * result.Count) / users.Count) / 4 > pixels)
                {
                    pixels += 1;
                    if (pixels > 25)
                    {
                        pixels = 25;
                    }
                    RefreshProgressBar(pixels);
                }
            }
            RefreshProgressBar(25);

            log.Text += System.Environment.NewLine;

            return result;
        }

        private void RefreshProgressBar(long pixels)
        {
            int idx = log.Text.LastIndexOf("[");
            string prefix = log.Text.Substring(0, idx);

            prefix += "[";

            for (long i = 0; i < pixels; i++)
            {
                prefix += "*";
            }

            for (long i = pixels; i < 25; i++)
            {
                prefix += " ";
            }

            prefix += "]";

            log.Text = prefix;
            log.Invalidate();
            log.Update();
            log.Refresh();

            Application.DoEvents();
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
                if (ou.ParentOU == null && (ou.Status.Equals(Status.ACTIVE) || ou.Status.Equals(Status.UNKNOWN)))
                {
                    return ou;
                }
            }

            return null;
        }
    }
}
