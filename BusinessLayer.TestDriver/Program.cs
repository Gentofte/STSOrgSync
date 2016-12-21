using System;

namespace Organisation.BusinessLayer.TestDriver
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static OrgUnitService orgUnitService = new OrgUnitService();
        private static UserService userService = new UserService();
        private static InspectorService inspectorService = new InspectorService();

        static void Main(string[] args)
        {
            Initializer.Init();

            //BuildKorsbaek();

            // TODO: this is a small method to build some useful sample data for manuel testing
            //BuildTestData();

            TestCreateAndUpdateFullUser();
            TestCreateAndUpdateFullOU();
            TestCreateDeleteUpdateUser();
            TestCreateDeleteUpdateOU();
            TestUpdateWithoutChanges();
            TestItSystemUsage();
            TestPayoutUnits();

            Environment.Exit(0);
        }

        private static void BuildKorsbaek()
        {
            OrgUnitRegistration korsbaekKommune = new OrgUnitRegistration();
            korsbaekKommune.ShortKey = "Korsbæk Kommune";
            korsbaekKommune.Name = "Korsbæk Kommune";
            korsbaekKommune.Uuid = "b9e45b18-b6ba-4434-bce4-844214aaaae8";
            orgUnitService.Create(korsbaekKommune);

            OrgUnitRegistration socialOgSundhedsforvaltningen = new OrgUnitRegistration();
            socialOgSundhedsforvaltningen.ShortKey = "Social og Sundhedsforvaltningen";
            socialOgSundhedsforvaltningen.Name = "Social og Sundhedsforvaltningen";
            socialOgSundhedsforvaltningen.Uuid = "9e491092-be32-4a74-a8b6-a3868386958d";
            socialOgSundhedsforvaltningen.ParentOrgUnitUuid = korsbaekKommune.Uuid;
            orgUnitService.Create(socialOgSundhedsforvaltningen);

            OrgUnitRegistration beskaeftigelsesOmraadet = new OrgUnitRegistration();
            beskaeftigelsesOmraadet.ShortKey = "Beskæftigelsesområdet";
            beskaeftigelsesOmraadet.Name = "Beskæftigelsesområdet";
            beskaeftigelsesOmraadet.Uuid = "6bbd954b-ef5f-426e-b387-aa92a0acc8ac";
            beskaeftigelsesOmraadet.ParentOrgUnitUuid = socialOgSundhedsforvaltningen.Uuid;
            orgUnitService.Create(beskaeftigelsesOmraadet);

            OrgUnitRegistration sundhedUdviklingServiceOgOekonomi = new OrgUnitRegistration();
            sundhedUdviklingServiceOgOekonomi.ShortKey = "Sundhed, Udvikling, Service og Økonomi";
            sundhedUdviklingServiceOgOekonomi.Name = "Sundhed, Udvikling, Service og Økonomi";
            sundhedUdviklingServiceOgOekonomi.Uuid = "e0fb33b7-ee59-43f4-af5b-faf67de2c549";
            sundhedUdviklingServiceOgOekonomi.ParentOrgUnitUuid = socialOgSundhedsforvaltningen.Uuid;
            orgUnitService.Create(sundhedUdviklingServiceOgOekonomi);

            OrgUnitRegistration ungeOmraadet = new OrgUnitRegistration();
            ungeOmraadet.ShortKey = "Ungeområdet";
            ungeOmraadet.Name = "Ungeområdet";
            ungeOmraadet.Uuid = "e5044026-9fc9-485a-a3bb-428a004c0483";
            ungeOmraadet.ParentOrgUnitUuid = beskaeftigelsesOmraadet.Uuid;
            orgUnitService.Create(ungeOmraadet);

            OrgUnitRegistration jobcenter = new OrgUnitRegistration();
            jobcenter.ShortKey = "Jobcenter";
            jobcenter.Name = "Jobcenter";
            jobcenter.Uuid = "e265f56e-031f-46b5-a403-6ded21e8d3e2";
            jobcenter.ParentOrgUnitUuid = beskaeftigelsesOmraadet.Uuid;
            orgUnitService.Create(jobcenter);

            OrgUnitRegistration ydelseOgRaadighed = new OrgUnitRegistration();
            ydelseOgRaadighed.ShortKey = "Ydelse og Rådighed";
            ydelseOgRaadighed.Name = "Ydelse og Rådighed";
            ydelseOgRaadighed.Uuid = "bd145a83-5386-40c8-b0e6-4fa5f6298c95";
            ydelseOgRaadighed.ParentOrgUnitUuid = jobcenter.Uuid;
            orgUnitService.Create(ydelseOgRaadighed);

            OrgUnitRegistration jobOgRessourcer = new OrgUnitRegistration();
            jobOgRessourcer.ShortKey = "Job og Ressourcer";
            jobOgRessourcer.Name = "Job og Ressourcer";
            jobOgRessourcer.Uuid = "b1f926f5-47fd-4b3e-a4fc-01f21bc5d7e1";
            jobOgRessourcer.ParentOrgUnitUuid = jobcenter.Uuid;
            orgUnitService.Create(jobOgRessourcer);

            OrgUnitRegistration jobOgKompetencer = new OrgUnitRegistration();
            jobOgKompetencer.ShortKey = "Job og Ressourcer";
            jobOgKompetencer.Name = "Job og Ressourcer";
            jobOgKompetencer.Uuid = "03d0923b-fa25-4991-84ed-b060379c7a31";
            jobOgKompetencer.ParentOrgUnitUuid = jobcenter.Uuid;
            orgUnitService.Create(jobOgKompetencer);

            UserRegistration userRegistration = new UserRegistration();
            userRegistration.UserUuid = "fb5281bd-2d00-4426-97ae-927f7eddf8aa";
            userRegistration.UserShortKey = "PIASTE";
            userRegistration.UserId = "PIASTE";
            userRegistration.PersonName = "Pia Stenberg";
            userRegistration.Email.Value = "piaste@korsbaek.dk";
            userRegistration.PositionName = "YR. Medarb. 01";
            userRegistration.PositionShortKey = "YR. Medarb. 01";
            userRegistration.PositionUuid = "0b9dcfc6-fcbe-4856-a4ac-de0c979ec47a";
            userRegistration.PositionOrgUnitUuid = ydelseOgRaadighed.Uuid;
            userService.Create(userRegistration);

            userRegistration = new UserRegistration();
            userRegistration.PositionUuid = "a3a1a3c8-7519-43c1-ad49-2cbb284632ad";
            userRegistration.PositionName = "YR. Medarb. 02";
            userRegistration.PositionShortKey = "YR. Medarb. 02";
            userRegistration.PositionOrgUnitUuid = ydelseOgRaadighed.Uuid;
            userRegistration.UserUuid = "18cddc45-ccb4-4753-8b42-6a9f62dc2d83";
            userRegistration.UserShortKey = "AMCATK";
            userRegistration.UserId = "AMCATK";
            userRegistration.PersonName = "Catrine Kristensen";
            userRegistration.Email.Value = "amcatk@korsbaek.dk";
            userService.Create(userRegistration);

            userRegistration = new UserRegistration();
            userRegistration.PositionUuid = "99d0d6be-1ca9-4367-8fe6-6ec761dfe3a5";
            userRegistration.PositionName = "YR. Medarb. 03";
            userRegistration.PositionShortKey = "YR. Medarb. 03";
            userRegistration.PositionOrgUnitUuid = ydelseOgRaadighed.Uuid;
            userRegistration.UserUuid = "530d688e-ff9b-4437-8c9b-1dbb651e13e2";
            userRegistration.UserShortKey = "BEDOLG";
            userRegistration.UserId = "BEDOLG";
            userRegistration.PersonName = "Dorthe Langager";
            userRegistration.Email.Value = "bedolg@korsbaek.dk";
            userService.Create(userRegistration);

            userRegistration = new UserRegistration();
            userRegistration.PositionUuid = "8a82772e-ac4e-438e-8a95-d495a6cbc90d";
            userRegistration.PositionName = "JR. Medarb. 01";
            userRegistration.PositionShortKey = "JR. Medarb. 01";
            userRegistration.PositionOrgUnitUuid = jobOgRessourcer.Uuid;
            userRegistration.UserUuid = "f8985e8f-f4da-413e-b819-febdd7767b94";
            userRegistration.UserShortKey = "HENJAC";
            userRegistration.UserId = "HENJAC";
            userRegistration.PersonName = "Henrik Jacobsen";
            userRegistration.Email.Value = "henjac@korsbaek.dk";
            userService.Create(userRegistration);

            userRegistration = new UserRegistration();
            userRegistration.PositionUuid = "7b0ace17-d54e-47bf-9c44-1353b134252d";
            userRegistration.PositionName = "JR. Medarb. 02";
            userRegistration.PositionShortKey = "JR. Medarb. 02";
            userRegistration.PositionOrgUnitUuid = jobOgRessourcer.Uuid;
            userRegistration.UserUuid = "ba4710c3-0304-457a-9924-f80f8d8563eb";
            userRegistration.UserShortKey = "SOFJDM";
            userRegistration.UserId = "SOFJDM";
            userRegistration.PersonName = "Sofie Damager";
            userRegistration.Email.Value = "sofjdm@korsbaek.dk";
            userService.Create(userRegistration);
        }

        private static void TestPayoutUnits()
        {
            OrgUnitRegistration payoutUnit1 = OUReg();
            payoutUnit1.Name = "Udbetalingsenhed 1";
            payoutUnit1.LOSShortName.Value = "UDE1";
            orgUnitService.Update(payoutUnit1);

            OrgUnitRegistration payoutUnit2 = OUReg();
            payoutUnit2.Name = "Udbetalingsenhed 2";
            payoutUnit2.LOSShortName.Value = "UDE2";
            orgUnitService.Update(payoutUnit2);

            OrgUnitRegistration unit = OUReg();
            unit.Name = "Aktiv enhed";
            orgUnitService.Update(unit);

            OU ou = inspectorService.ReadOUObject(unit.Uuid);
            Validate(ou, unit);

            unit.PayoutUnitUuid = payoutUnit1.Uuid;
            orgUnitService.Update(unit);

            ou = inspectorService.ReadOUObject(unit.Uuid);
            Validate(ou, unit);

            // lock another active unit onto this payout unit
            OrgUnitRegistration unit2 = OUReg();
            unit2.Name = "Aktiv enhed 2";
            unit2.PayoutUnitUuid = payoutUnit1.Uuid;
            orgUnitService.Update(unit2);

            unit.PayoutUnitUuid = payoutUnit2.Uuid;
            orgUnitService.Update(unit);

            ou = inspectorService.ReadOUObject(unit.Uuid);
            Validate(ou, unit);

            // test that the other active units kept its old reference
            ou = inspectorService.ReadOUObject(unit2.Uuid);
            Validate(ou, unit2);

            unit.PayoutUnitUuid = null;
            orgUnitService.Update(unit);

            ou = inspectorService.ReadOUObject(unit.Uuid);
            Validate(ou, unit);

            unit.PayoutUnitUuid = payoutUnit1.Uuid;
            orgUnitService.Update(unit);

            ou = inspectorService.ReadOUObject(unit.Uuid);
            Validate(ou, unit);

            orgUnitService.Delete(payoutUnit1.Uuid, DateTime.Now.AddMinutes(-2));

            // special test-case - when deling the payout unit, the active unit should still point to the payout unit (deleted or otherwise). It is the callers responsibility to update the active units as well
            ou = inspectorService.ReadOUObject(unit.Uuid);
            Validate(ou, unit);
        }

        private static void BuildTestData()
        {
            OrgUnitRegistration root = OUReg();
            root.Name = "Fiskebæk Kommune";
            orgUnitService.Update(root);

            UserRegistration user = UserReg();
            user.PersonName = "Mads Langkilde";
            user.UserId = "mlk";
            user.PositionName = "Mayor";
            user.PositionOrgUnitUuid = root.Uuid;
            userService.Update(user);

            OrgUnitRegistration administration = OUReg();
            administration.Name = "Administration";
            administration.ParentOrgUnitUuid = root.Uuid;
            administration.ItSystemUuids.Add("95d4db6e-219a-4d72-a6c7-f20f02e850b1");
            administration.ItSystemUuids.Add("93f1f053-7c6c-48e8-8146-752c8b9f74b5");
            administration.ItSystemUuids.Add("09dfb4a5-aa5c-47d9-91db-b8780dcba37e");
            orgUnitService.Update(administration);

            OrgUnitRegistration economics = OUReg();
            economics.Name = "Økonomi";
            economics.ParentOrgUnitUuid = administration.Uuid;
            orgUnitService.Update(economics);

            user = UserReg();
            user.PersonName = "Bente Blankocheck";
            user.UserId = "bbcheck";
            user.PositionName = "Leder";
            user.PositionOrgUnitUuid = economics.Uuid;
            userService.Update(user);

            user = UserReg();
            user.PersonName = "Morten Massepenge";
            user.UserId = "mpenge";
            user.PositionName = "Økonomimedarbejder";
            user.PositionOrgUnitUuid = economics.Uuid;
            userService.Update(user);

            OrgUnitRegistration borgerservice = OUReg();
            borgerservice.Name = "Borgerservice";
            borgerservice.LOSShortName.Value = "BS"; // BorgerService is a payout unit
            borgerservice.ParentOrgUnitUuid = administration.Uuid;
            borgerservice.ItSystemUuids.Add("09dfb4a5-aa5c-47d9-91db-b8780dcba37e");
            orgUnitService.Update(borgerservice);

            user = UserReg();
            user.PersonName = "Karen Hjælpsom";
            user.UserId = "khj";
            user.Email.Value = "khj@mail.dk";
            user.PositionName = "Sagsbehandler";
            user.PositionOrgUnitUuid = borgerservice.Uuid;
            userService.Update(user);

            user = UserReg();
            user.PersonName = "Søren Sørensen";
            user.UserId = "ssø";
            user.Phone.Value = "12345678";
            user.PositionName = "Sagsbehandler";
            user.PositionOrgUnitUuid = borgerservice.Uuid;
            userService.Update(user);

            user = UserReg();
            user.PersonName = "Viggo Mortensen";
            user.UserId = "vmort";
            user.PersonCpr = "0101010101";
            user.PositionName = "Sagsbehandler";
            user.PositionOrgUnitUuid = borgerservice.Uuid;
            userService.Update(user);

            OrgUnitRegistration jobcenter = OUReg();
            jobcenter.Name = "Jobcenter Centralkontor";
            jobcenter.PayoutUnitUuid = borgerservice.Uuid;
            jobcenter.Ean.Value = "12312312312312";
            jobcenter.EmailRemarks.Value = "Some remark";
            jobcenter.ParentOrgUnitUuid = administration.Uuid;
            orgUnitService.Update(jobcenter);

            user = UserReg();
            user.PersonName = "Johan Jensen";
            user.UserId = "jojens";
            user.PositionName = "Leder";
            user.PositionOrgUnitUuid = jobcenter.Uuid;
            userService.Update(user);

            OrgUnitRegistration jobcenter1 = OUReg();
            jobcenter1.Name = "Jobcenter Vest";
            jobcenter1.ParentOrgUnitUuid = jobcenter.Uuid;
            orgUnitService.Update(jobcenter1);

            user = UserReg();
            user.PersonName = "Julie Jensen";
            user.UserId = "juljen";
            user.PositionName = "Sagsbehandler";
            user.PositionOrgUnitUuid = jobcenter1.Uuid;
            userService.Update(user);

            OrgUnitRegistration jobcenter2 = OUReg();
            jobcenter2.Name = "Jobcenter Øst";
            jobcenter2.ParentOrgUnitUuid = jobcenter.Uuid;
            orgUnitService.Update(jobcenter2);

            OrgUnitRegistration itDepartment = OUReg();
            itDepartment.Name = "IT Department";
            itDepartment.PayoutUnitUuid = borgerservice.Uuid;
            itDepartment.ParentOrgUnitUuid = root.Uuid;
            orgUnitService.Update(itDepartment);

            OrgUnitRegistration operations = OUReg();
            operations.Name = "Drift og operation";
            operations.ParentOrgUnitUuid = itDepartment.Uuid;
            orgUnitService.Update(operations);

            user = UserReg();
            user.PersonName = "Steven Sørensen";
            user.UserId = "ssøren";
            user.PositionName = "Administrator";
            user.PositionOrgUnitUuid = operations.Uuid;
            userService.Update(user);

            user = UserReg();
            user.PersonName = "Marie Marolle";
            user.UserId = "marolle";
            user.PositionName = "Administrator";
            user.PositionOrgUnitUuid = operations.Uuid;
            userService.Update(user);

            OrgUnitRegistration projects = OUReg();
            projects.Name = "Udvikling og projekter";
            projects.ParentOrgUnitUuid = itDepartment.Uuid;
            orgUnitService.Update(projects);

            user = UserReg();
            user.PersonName = "Henrik Jeppesen";
            user.UserId = "jeppe";
            user.PositionName = "Projektleder";
            user.PositionOrgUnitUuid = projects.Uuid;
            userService.Update(user);
        }

        private static void TestUpdateWithoutChanges()
        {
            OrgUnitRegistration orgUnitRegistration = OUReg();
            orgUnitService.Update(orgUnitRegistration);
            orgUnitService.Update(orgUnitRegistration);

            UserRegistration userRegistration = UserReg();
            userRegistration.PositionName = "PositionNameValue";
            userRegistration.PositionOrgUnitUuid = orgUnitRegistration.Uuid;
            userService.Update(userRegistration);
            userService.Update(userRegistration);
        }

        private static void TestItSystemUsage()
        {
            string itSystem1 = Guid.NewGuid().ToString().ToLower();
            string itSystem2 = Guid.NewGuid().ToString().ToLower();
            string itSystem3 = Guid.NewGuid().ToString().ToLower();

            OrgUnitRegistration orgUnitRegistration = OUReg();
            orgUnitRegistration.ItSystemUuids.Add(itSystem1);
            orgUnitRegistration.ItSystemUuids.Add(itSystem2);
            orgUnitService.Update(orgUnitRegistration);

            OU ou = inspectorService.ReadOUObject(orgUnitRegistration.Uuid);
            Validate(ou, orgUnitRegistration);

            orgUnitRegistration.ItSystemUuids.Add(itSystem3);
            orgUnitService.Update(orgUnitRegistration);

            ou = inspectorService.ReadOUObject(orgUnitRegistration.Uuid);
            Validate(ou, orgUnitRegistration);

            orgUnitRegistration.ItSystemUuids.Remove(itSystem2);
            orgUnitService.Update(orgUnitRegistration);

            ou = inspectorService.ReadOUObject(orgUnitRegistration.Uuid);
            Validate(ou, orgUnitRegistration);
        }

        private static void TestCreateDeleteUpdateOU()
        {
            OrgUnitRegistration registration = OUReg();
            registration.Timestamp = DateTime.Now.AddMinutes(-5);
            orgUnitService.Update(registration);

            orgUnitService.Delete(registration.Uuid, DateTime.Now.AddMinutes(-3));

            registration.Timestamp = DateTime.Now.AddMinutes(-1);
            orgUnitService.Update(registration);

            OU ou = inspectorService.ReadOUObject(registration.Uuid);
            Validate(ou, registration);
        }

        private static void TestCreateDeleteUpdateUser()
        {
            OrgUnitRegistration parentRegistration = OUReg();
            orgUnitService.Update(parentRegistration);

            UserRegistration registration = UserReg();
            registration.PositionName = "PositionNameValue";
            registration.PositionOrgUnitUuid = parentRegistration.Uuid;
            registration.Timestamp = DateTime.Now.AddMinutes(-5);

            userService.Update(registration);
            userService.Delete(registration.UserUuid, DateTime.Now.AddMinutes(-3));

            registration.Timestamp = DateTime.Now.AddMinutes(-1);
            userService.Update(registration);

            User user = inspectorService.ReadUserObject(registration.UserUuid);
            Validate(user, registration);
        }

        private static void TestCreateAndUpdateFullOU()
        {
            // create parent OUs
            OrgUnitRegistration parentReg = OUReg();
            orgUnitService.Update(parentReg);

            OrgUnitRegistration parentReg2 = OUReg();
            orgUnitService.Update(parentReg2);

            OrgUnitRegistration registration = OUReg();
            registration.Name = "Some Random OU Name";
            registration.ParentOrgUnitUuid = parentReg.Uuid;
            registration.ContactOpenHours.Value = "ContactOpenHoursValue";
            registration.Ean.Value = "EanValue";
            registration.Email.Value = "EmailValue";
            registration.EmailRemarks.Value = "EmailRemark";
            registration.Contact.Value = "Contact";
            registration.PostReturn.Value = "PostReturn";
            registration.ItSystemUuids = null; // TODO: cannot do this until we have official UUIDs from KOMBIT
            registration.Location.Value = "LocationValue";
            registration.LOSShortName.Value = "LOSShortNameValue";
            registration.PayoutUnitUuid = parentReg.Uuid;
            registration.Phone.Value = "PhoneValue";
            registration.PhoneOpenHours.Value = "PhoneOpenHoursValue";
            registration.Post.Value = "PostValue";
            orgUnitService.Update(registration);

            OU ou = inspectorService.ReadOUObject(registration.Uuid);
            Validate(ou, registration);

            registration.Name = "Some Random OU Name 2";
            registration.ParentOrgUnitUuid = parentReg2.Uuid;
            registration.ContactOpenHours.Value = "ContactOpenHoursValue2";
            registration.Ean.Value = "EanValue2";
            registration.Email.Value = "EmailValue2";
            registration.EmailRemarks.Value = "EmailOpenHoursValue2";
            registration.Contact.Value = "ContactValue2";
            registration.PostReturn.Value = "PostReturnValue2";
            registration.ItSystemUuids = null; // TODO: cannot do this until we have official UUIDs from KOMBIT
            registration.Location.Value = "LocationValue2";
            registration.LOSShortName.Value = "LOSShortNameValue2";
            registration.PayoutUnitUuid = parentReg2.Uuid;
            registration.Phone.Value = "PhoneValue2";
            registration.PhoneOpenHours.Value = "PhoneOpenHoursValue2";
            registration.Post.Value = "PostValue2";
            orgUnitService.Update(registration);

            ou = inspectorService.ReadOUObject(registration.Uuid);
            Validate(ou, registration);
        }

        private static void TestCreateAndUpdateFullUser()
        {
            // create parent OUs
            OrgUnitRegistration parentReg = OUReg();
            orgUnitService.Update(parentReg);

            OrgUnitRegistration parentReg2 = OUReg();
            orgUnitService.Update(parentReg2);

            UserRegistration registration = UserReg();
            registration.Email.Value = "EmailValue";
            registration.Location.Value = "LocationValue";
            registration.PersonCpr = "0000000000";
            registration.PersonName = "PersonNameValue";
            registration.Phone.Value = "PhoneValue";
            registration.PositionName = "PositionNameValue";
            registration.PositionOrgUnitUuid = parentReg.Uuid;
            registration.UserId = "UserIdValue";
            userService.Update(registration);

            User user = inspectorService.ReadUserObject(registration.UserUuid);
            Validate(user, registration);

            registration.Email.Value = "EmailValue2";
            registration.Location.Value = "LocationValue2";
            registration.PersonCpr = "0000000001";
            registration.PersonName = "PersonNameValue2";
            registration.Phone.Value = "PhoneValue2";
            registration.PositionName = "PositionNameValue2";
            registration.PositionOrgUnitUuid = parentReg.Uuid;
            registration.UserId = "UserIdValue2";
            userService.Update(registration);

            user = inspectorService.ReadUserObject(registration.UserUuid);
            Validate(user, registration);
        }

        private static OrgUnitRegistration OUReg()
        {
            OrgUnitRegistration registration = new OrgUnitRegistration();
            registration.Uuid = Uuid();
            registration.Name = "Default OU Name";

            return registration;
        }

        private static UserRegistration UserReg()
        {
            UserRegistration registration = new UserRegistration();
            registration.UserUuid = Uuid();
            registration.UserId = "DefaultUserID";
            registration.PersonName = "DefaultPersonName";

            return registration;
        }

        private static string Uuid()
        {
            return Guid.NewGuid().ToString().ToLower();
        }

        private static void Validate(User user, UserRegistration registration)
        {
            if (!string.Equals(user.Person?.Cpr, registration.PersonCpr))
            {
                throw new Exception("CPR is not the same");
            }

            if (!string.Equals(user.Person?.Name, registration.PersonName))
            {
                throw new Exception("Name is not the same");
            }

            if (!string.Equals(user.UserId, registration.UserId))
            {
                throw new Exception("UserId is not the same");
            }

            if (!string.Equals(user.Position?.Name, registration.PositionName))
            {
                throw new Exception("PositionName is not the same");
            }

            if (!string.Equals(user.Position?.OU?.Uuid, registration.PositionOrgUnitUuid))
            {
                throw new Exception("PositionOU reference is not the same");
            }

            // bit one sided, but probably enough for rough testing
            foreach (var address in user.Addresses)
            {
                if (address is Email && !address.Uuid.Equals(registration.Email.Uuid))
                {
                    throw new Exception("Email is not the same");
                }
                else if (address is Location && !address.Uuid.Equals(registration.Location.Uuid))
                {
                    throw new Exception("Location is not the same");
                }
                else if (address is Phone && !address.Uuid.Equals(registration.Phone.Uuid))
                {
                    throw new Exception("Phone is not the same");
                }
            }

        }

        private static void Validate(OU orgUnit, OrgUnitRegistration registration)
        {
            if (!string.Equals(orgUnit.ParentOU?.Uuid, registration.ParentOrgUnitUuid))
            {
                throw new Exception("ParentOU reference is not the same");
            }

            if (!string.Equals(orgUnit.Name, registration.Name))
            {
                throw new Exception("Name is not the same");
            }

            if (!string.Equals(orgUnit.PayoutOU?.Uuid, registration.PayoutUnitUuid))
            {
                throw new Exception("PayoutUnit reference is not the same");
            }

            if (registration.ItSystemUuids != null)
            {
                foreach (string uuid in registration.ItSystemUuids)
                {
                    bool found = false;

                    if (orgUnit.ItSystems != null)
                    {
                        foreach (string orgUuid in orgUnit.ItSystems)
                        {
                            if (orgUuid.Equals(uuid))
                            {
                                found = true;
                            }
                        }
                    }

                    if (!found)
                    {
                        throw new Exception("ItSystem with uuid " + uuid + " should be in Organisation, but is not!");
                    }
                }
            }

            if (orgUnit.ItSystems != null)
            {
                foreach (string uuid in orgUnit.ItSystems)
                {
                    bool found = false;

                    if (registration.ItSystemUuids != null)
                    {
                        foreach (string orgUuid in registration.ItSystemUuids)
                        {
                            if (orgUuid.Equals(uuid))
                            {
                                found = true;
                            }
                        }
                    }

                    if (!found)
                    {
                        throw new Exception("ItSystem with uuid " + uuid + " found in Organisation, but it should not be there!");
                    }
                }
            }

            if (!string.Equals(orgUnit.PayoutOU?.Uuid, registration.PayoutUnitUuid))
            {
                throw new Exception("PayoutUnit reference is not the same");
            }

            // bit one sided, but probably enough for rough testing
            foreach (var address in orgUnit.Addresses)
            {
                if (address is Email && !address.Uuid.Equals(registration.Email.Uuid))
                {
                    throw new Exception("Email is not the same");
                }
                else if (address is Phone && !address.Uuid.Equals(registration.Phone.Uuid))
                {
                    throw new Exception("Phone is not the same");
                }
                else if (address is Location && !address.Uuid.Equals(registration.Location.Uuid))
                {
                    throw new Exception("Location is not the same");
                }
                else if (address is LOSShortName && !address.Uuid.Equals(registration.LOSShortName.Uuid))
                {
                    throw new Exception("LOSShortName is not the same");
                }
                else if (address is PhoneHours && !address.Uuid.Equals(registration.PhoneOpenHours.Uuid))
                {
                    throw new Exception("PhoneHours is not the same");
                }
                else if (address is EmailRemarks && !address.Uuid.Equals(registration.EmailRemarks.Uuid))
                {
                    throw new Exception("EmailRemarks is not the same");
                }
                else if (address is Contact && !address.Uuid.Equals(registration.Contact.Uuid))
                {
                    throw new Exception("Contact is not the same");
                }
                else if (address is PostReturn && !address.Uuid.Equals(registration.PostReturn.Uuid))
                {
                    throw new Exception("PostReturn is not the same");
                }
                else if (address is Ean && !address.Uuid.Equals(registration.Ean.Uuid))
                {
                    throw new Exception("Ean is not the same");
                }
                else if (address is ContactHours && !address.Uuid.Equals(registration.ContactOpenHours.Uuid))
                {
                    throw new Exception("ContactHours is not the same");
                }
            }
        }
    }
}
