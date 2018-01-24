using System;
using Organisation.BusinessLayer.DTO.V1_1;
using Organisation.IntegrationLayer;
using System.Collections.Generic;

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

            // This is a small method to build some useful sample data for manuel testing
            //BuildTestData();
            TestListAndReadOUs();
            TestListAndReadUsers();
            TestCreateAndUpdateFullUser();
            TestCreateAndUpdateFullOU();
            TestCreateDeleteUpdateUser();
            TestCreateDeleteUpdateOU();
            TestUpdateWithoutChanges();
            TestItSystemUsage();
            TestPayoutUnits();
            TestPositions();
            TestContactPlaces();

            System.Environment.Exit(0);
        }

        private static void TestListAndReadUsers()
        {
            // small hack to ensure this test passes (the search parameters will find all users in the organisation, and we need to test that it hits the required amount)
            OrganisationRegistryProperties properties = OrganisationRegistryProperties.GetInstance();
            string oldUuid = properties.MunicipalityOrganisationUUID;
            properties.MunicipalityOrganisationUUID = Uuid();

            UserRegistration registration1 = UserReg();
            registration1.UserId = "userId1";
            registration1.Email.Value = "email1@email.com";
            registration1.Person.Name = "Name of Person 1";
            registration1.Positions.Add(new DTO.V1_1.Position()
            {
                Name = "Position 1",
                OrgUnitUuid = Uuid()
            });
            registration1.Positions.Add(new DTO.V1_1.Position()
            {
                Name = "Position 2",
                OrgUnitUuid = Uuid()
            });
            userService.Update(registration1);

            UserRegistration registration2 = UserReg();
            registration2.UserId = "userId2";
            registration2.Email.Value = "email2@email.com";
            registration2.Person.Name = "Name of Person 2";
            registration2.Positions.Add(new DTO.V1_1.Position()
            {
                Name = "Position 3",
                OrgUnitUuid = Uuid()
            });
            registration2.Positions.Add(new DTO.V1_1.Position()
            {
                Name = "Position 4",
                OrgUnitUuid = Uuid()
            });
            userService.Update(registration2);

            /* TODO: reenable this test once search takes Tilstand into consideration - it currently fails because of a bug @KMD
            UserRegistration registration3 = UserReg();
            registration3.UserId = "userId3";
            registration3.Email.Value = "email3@email.com";
            registration3.Person.Name = "Name of Person 3";
            registration3.Positions.Add(new DTO.V1_1.Position()
            {
                Name = "Position 5",
                OrgUnitUuid = Uuid()
            });
            userService.Update(registration3);
            userService.Delete(registration3.Uuid, DateTime.Now);
            */

            List<string> users = userService.List();
            if (users.Count != 2)
            {
                throw new Exception("List() returned " + users.Count + " users, but 2 was expected");
            }

            foreach (var uuid in users)
            {
                UserRegistration registration = userService.Read(uuid);

                if (uuid.Equals(registration1.Uuid))
                {
                    if (!registration1.UserId.Equals(registration.UserId))
                    {
                        throw new Exception("userId does not match");
                    }

                    if (!registration1.Person.Name.Equals(registration.Person.Name))
                    {
                        throw new Exception("Name does not match");
                    }

                    if (!registration1.Email.Value.Equals(registration.Email.Value))
                    {
                        throw new Exception("Email does not match");
                    }

                    if (registration1.Positions.Count != registration.Positions.Count)
                    {
                        throw new Exception("Amount of positions does not match");
                    }

                    foreach (var position in registration1.Positions)
                    {
                        bool found = false;

                        foreach (var readPosition in registration.Positions)
                        {
                            if (readPosition.Name.Equals(position.Name) && readPosition.OrgUnitUuid.Equals(position.OrgUnitUuid))
                            {
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            throw new Exception("Missing position");
                        }
                    }
                }
                else if (uuid.Equals(registration2.Uuid))
                {
                    if (!registration2.UserId.Equals(registration.UserId))
                    {
                        throw new Exception("userId does not match");
                    }

                    if (!registration2.Person.Name.Equals(registration.Person.Name))
                    {
                        throw new Exception("Name does not match");
                    }

                    if (!registration2.Email.Value.Equals(registration.Email.Value))
                    {
                        throw new Exception("Email does not match");
                    }

                    if (registration2.Positions.Count != registration.Positions.Count)
                    {
                        throw new Exception("Amount of positions does not match");
                    }

                    foreach (var position in registration2.Positions)
                    {
                        bool found = false;

                        foreach (var readPosition in registration.Positions)
                        {
                            if (readPosition.Name.Equals(position.Name) && readPosition.OrgUnitUuid.Equals(position.OrgUnitUuid))
                            {
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            throw new Exception("Missing position");
                        }
                    }
                }
                else
                {
                    throw new Exception("List returned the uuid of an unexpected user");
                }
            }

            properties.MunicipalityOrganisationUUID = oldUuid;
        }

        private static void TestListAndReadOUs()
        {
            // small hack to ensure this test passes (the search parameters will find all ous in the organisation, and we need to test that it hits the required amount)
            OrganisationRegistryProperties properties = OrganisationRegistryProperties.GetInstance();
            string oldUuid = properties.MunicipalityOrganisationUUID;
            properties.MunicipalityOrganisationUUID = Uuid();

            OrgUnitRegistration registration1 = OUReg();
            registration1.Name = "ou1";
            registration1.Email.Value = "email1@email.com";
            registration1.ParentOrgUnitUuid = Uuid();
            registration1.ItSystemUuids.Add(Uuid());
            registration1.ItSystemUuids.Add(Uuid());
            orgUnitService.Update(registration1);

            OrgUnitRegistration registration2 = OUReg();
            registration2.Name = "ou2";
            registration2.Email.Value = "email2@email.com";
            registration2.ParentOrgUnitUuid = Uuid();
            registration2.ItSystemUuids.Add(Uuid());
            registration2.ItSystemUuids.Add(Uuid());
            orgUnitService.Update(registration2);

            /* TODO: reenable this test once search takes Tilstand into consideration - it currently fails because of a bug @KMD
            OrgUnitRegistration registration3 = OUReg();
            registration3.Name = "ou3";
            registration3.Email.Value = "email3@email.com";
            registration3.ParentOrgUnitUuid = Uuid();
            registration3.ItSystemUuids.Add(Uuid());
            registration3.ItSystemUuids.Add(Uuid());
            orgUnitService.Update(registration3);
            orgUnitService.Delete(registration3, DateTime.Now);
            */

            List<string> ous = orgUnitService.List();
            if (ous.Count != 2)
            {
                throw new Exception("List() returned " + ous.Count + " ous, but 2 was expected");
            }

            foreach (var uuid in ous)
            {
                OrgUnitRegistration registration = orgUnitService.Read(uuid);

                if (uuid.Equals(registration1.Uuid))
                {
                    if (!registration1.Name.Equals(registration.Name))
                    {
                        throw new Exception("Name does not match");
                    }

                    if (!registration1.ParentOrgUnitUuid.Equals(registration.ParentOrgUnitUuid))
                    {
                        throw new Exception("ParentOU UUID does not match");
                    }

                    if (!registration1.Email.Value.Equals(registration.Email.Value))
                    {
                        throw new Exception("Email does not match");
                    }

                    if (registration1.ItSystemUuids.Count != registration.ItSystemUuids.Count)
                    {
                        throw new Exception("Amount of ItSystems does not match");
                    }

                    foreach (var itSystem in registration1.ItSystemUuids)
                    {
                        bool found = false;

                        foreach (var readItSystems in registration.ItSystemUuids)
                        {
                            if (readItSystems.Equals(itSystem))
                            {
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            throw new Exception("Missing ItSystem");
                        }
                    }
                }
                else if (uuid.Equals(registration2.Uuid))
                {
                    if (!registration2.Name.Equals(registration.Name))
                    {
                        throw new Exception("Name does not match");
                    }

                    if (!registration2.ParentOrgUnitUuid.Equals(registration.ParentOrgUnitUuid))
                    {
                        throw new Exception("ParentOU UUID does not match");
                    }

                    if (!registration2.Email.Value.Equals(registration.Email.Value))
                    {
                        throw new Exception("Email does not match");
                    }

                    if (registration2.ItSystemUuids.Count != registration.ItSystemUuids.Count)
                    {
                        throw new Exception("Amount of ItSystems does not match");
                    }

                    foreach (var itSystem in registration2.ItSystemUuids)
                    {
                        bool found = false;

                        foreach (var readItSystems in registration.ItSystemUuids)
                        {
                            if (readItSystems.Equals(itSystem))
                            {
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            throw new Exception("Missing ItSystem");
                        }
                    }
                }
                else
                {
                    throw new Exception("List returned the uuid of an unexpected ou");
                }
            }

            properties.MunicipalityOrganisationUUID = oldUuid;
        }

        private static void TestContactPlaces()
        {
            OrgUnitRegistration registration = OUReg();
            registration.ContactPlaces.Add(new DTO.V1_1.ContactPlace()
            {
                OrgUnitUuid = Guid.NewGuid().ToString().ToLower(),
                Tasks = new List<string>() { Guid.NewGuid().ToString().ToLower(), Guid.NewGuid().ToString().ToLower() }
            });

            orgUnitService.Update(registration);
            OU ou = inspectorService.ReadOUObject(registration.Uuid);

            // 1 contact place with 2 KLE's
            Validate(ou, registration);

            registration.ContactPlaces.Add(new DTO.V1_1.ContactPlace()
            {
                OrgUnitUuid = Guid.NewGuid().ToString().ToLower(),
                Tasks = new List<string>() { Guid.NewGuid().ToString().ToLower(), Guid.NewGuid().ToString().ToLower() }
            });

            orgUnitService.Update(registration);
            ou = inspectorService.ReadOUObject(registration.Uuid);

            // 2 contact places (1 new) with 2 KLE's, same KLE as before in the old one, and new in the new one
            Validate(ou, registration);

            registration.ContactPlaces.Clear();
            orgUnitService.Update(registration);
            ou = inspectorService.ReadOUObject(registration.Uuid);

            // removed the two old contact places, so now we have zero
            Validate(ou, registration);

            registration.ContactPlaces.Add(new DTO.V1_1.ContactPlace()
            {
                OrgUnitUuid = Guid.NewGuid().ToString().ToLower(),
                Tasks = new List<string>() { Guid.NewGuid().ToString().ToLower(), Guid.NewGuid().ToString().ToLower() }
            });
            orgUnitService.Update(registration);
            ou = inspectorService.ReadOUObject(registration.Uuid);

            // added a new one
            Validate(ou, registration);

            registration.ContactPlaces[0].Tasks.Clear();
            registration.ContactPlaces[0].Tasks.Add(Guid.NewGuid().ToString().ToLower());

            orgUnitService.Update(registration);
            ou = inspectorService.ReadOUObject(registration.Uuid);

            // changed the tasks on the single contact place
            Validate(ou, registration);
        }

        private static void TestPositions()
        {
            string orgUnitUuid1 = Uuid();
            string orgUnitUuid2 = Uuid();
            string orgUnitUuid3 = Uuid();

            // simple employement
            UserRegistration registration = UserReg();
            registration.Positions.Add(new DTO.V1_1.Position()
            {
                Name = "Position 1",
                OrgUnitUuid = orgUnitUuid1
            });

            userService.Update(registration);
            User user = inspectorService.ReadUserObject(registration.Uuid);
            Validate(user, registration);

            // fire from current position, but give two new positions instead
            registration.Positions.Clear();
            registration.Positions.Add(new DTO.V1_1.Position()
            {
                Name = "Position 2",
                OrgUnitUuid = orgUnitUuid2
            });
            registration.Positions.Add(new DTO.V1_1.Position()
            {
                Name = "Position 3",
                OrgUnitUuid = orgUnitUuid3
            });

            userService.Update(registration);
            user = inspectorService.ReadUserObject(registration.Uuid);
            Validate(user, registration);

            // now fire one of those positions
            registration.Positions.Clear();
            registration.Positions.Add(new DTO.V1_1.Position()
            {
                Name = "Position 2",
                OrgUnitUuid = orgUnitUuid2
            });
            userService.Update(registration);
            user = inspectorService.ReadUserObject(registration.Uuid);
            Validate(user, registration);

            // fresh user, this time we want to control the UUID of the positions
            string positionUuid1 = Uuid();
            string positionUuid2 = Uuid();
            string positionUuid3 = Uuid();

            // start with two jobs
            registration = UserReg();
            registration.Positions.Add(new DTO.V1_1.Position()
            {
                Name = "Position 1",
                OrgUnitUuid = orgUnitUuid1,
                Uuid = positionUuid1
            });
            registration.Positions.Add(new DTO.V1_1.Position()
            {
                Name = "Position 2",
                OrgUnitUuid = orgUnitUuid2,
                Uuid = positionUuid2
            });

            userService.Update(registration);
            user = inspectorService.ReadUserObject(registration.Uuid);
            Validate(user, registration);

            // modify one of these positions
            registration.Positions.Clear();
            registration.Positions.Add(new DTO.V1_1.Position()
            {
                Name = "Position 2",
                OrgUnitUuid = orgUnitUuid2,
                Uuid = positionUuid2
            });
            registration.Positions.Add(new DTO.V1_1.Position()
            {
                Name = "Position 3",
                OrgUnitUuid = orgUnitUuid3,
                Uuid = positionUuid1 // reusing position 1 to point to new OU
            });

            userService.Update(registration);
            user = inspectorService.ReadUserObject(registration.Uuid);
            Validate(user, registration);
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
            user.Person.Name = "Mads Langkilde";
            user.UserId = "mlk";
            DTO.V1_1.Position position = new DTO.V1_1.Position();
            user.Positions.Add(position);
            position.Name = "Mayor";
            position.OrgUnitUuid = root.Uuid;
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

            OrgUnitRegistration fireDepartment = OUReg();
            fireDepartment.Name = "Brandvæsenet";
            fireDepartment.ParentOrgUnitUuid = administration.Uuid;
            orgUnitService.Update(fireDepartment);

            user = UserReg();
            user.Person.Name = "Bente Blankocheck";
            user.UserId = "bbcheck";
            position = new DTO.V1_1.Position()
            {
                Name = "Leder",
                OrgUnitUuid = economics.Uuid
            };
            user.Positions.Add(position);
            position = new DTO.V1_1.Position()
            {
                Name = "Brandmand",
                OrgUnitUuid = fireDepartment.Uuid
            };
            user.Positions.Add(position);
            userService.Update(user);

            user = UserReg();
            user.Person.Name = "Morten Massepenge";
            user.UserId = "mpenge";
            position = new DTO.V1_1.Position();
            user.Positions.Add(position);
            position.Name = "Økonomimedarbejder";
            position.OrgUnitUuid = economics.Uuid;
            userService.Update(user);

            OrgUnitRegistration borgerservice = OUReg();
            borgerservice.Name = "Borgerservice";
            borgerservice.LOSShortName.Value = "BS"; // BorgerService is a payout unit
            borgerservice.ParentOrgUnitUuid = administration.Uuid;
            borgerservice.ItSystemUuids.Add("09dfb4a5-aa5c-47d9-91db-b8780dcba37e");
            orgUnitService.Update(borgerservice);

            user = UserReg();
            user.Person.Name = "Karen Hjælpsom";
            user.UserId = "khj";
            user.Email.Value = "khj@mail.dk";
            position = new DTO.V1_1.Position();
            user.Positions.Add(position);
            position.Name = "Sagsbehandler";
            position.OrgUnitUuid = borgerservice.Uuid;
            userService.Update(user);

            user = UserReg();
            user.Person.Name = "Søren Sørensen";
            user.UserId = "ssø";
            user.Phone.Value = "12345678";
            position = new DTO.V1_1.Position();
            user.Positions.Add(position);
            position.Name = "Sagsbehandler";
            position.OrgUnitUuid = borgerservice.Uuid;
            userService.Update(user);

            user = UserReg();
            user.Person.Name = "Viggo Mortensen";
            user.UserId = "vmort";
            user.Person.Cpr = "0101010101";
            position = new DTO.V1_1.Position();
            user.Positions.Add(position);
            position.Name = "Sagsbehandler";
            position.OrgUnitUuid = borgerservice.Uuid;
            userService.Update(user);

            OrgUnitRegistration jobcenter = OUReg();
            jobcenter.Name = "Jobcenter Centralkontor";
            jobcenter.PayoutUnitUuid = borgerservice.Uuid;
            jobcenter.Ean.Value = "12312312312312";
            jobcenter.EmailRemarks.Value = "Some remark";
            jobcenter.ParentOrgUnitUuid = administration.Uuid;
            orgUnitService.Update(jobcenter);

            user = UserReg();
            user.Person.Name = "Johan Jensen";
            user.UserId = "jojens";
            position = new DTO.V1_1.Position();
            user.Positions.Add(position);
            position.Name = "Leder";
            position.OrgUnitUuid = jobcenter.Uuid;
            userService.Update(user);

            OrgUnitRegistration jobcenter1 = OUReg();
            jobcenter1.Name = "Jobcenter Vest";
            jobcenter1.ParentOrgUnitUuid = jobcenter.Uuid;
            jobcenter1.ContactPlaces.Add(new DTO.V1_1.ContactPlace()
            {
                OrgUnitUuid = jobcenter.Uuid,
                Tasks = new List<string>() { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() }
            });
            orgUnitService.Update(jobcenter1);

            user = UserReg();
            user.Person.Name = "Julie Jensen";
            user.UserId = "juljen";
            position = new DTO.V1_1.Position();
            user.Positions.Add(position);
            position.Name = "Sagsbehandler";
            position.OrgUnitUuid = jobcenter1.Uuid;
            userService.Update(user);

            OrgUnitRegistration jobcenter2 = OUReg();
            jobcenter2.Name = "Jobcenter Øst";
            jobcenter2.ParentOrgUnitUuid = jobcenter.Uuid;
            jobcenter2.ContactPlaces.Add(new DTO.V1_1.ContactPlace()
            {
                OrgUnitUuid = jobcenter.Uuid,
                Tasks = new List<string>() { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() }
            });
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
            user.Person.Name = "Steven Sørensen";
            user.UserId = "ssøren";
            position = new DTO.V1_1.Position();
            user.Positions.Add(position);
            position.Name = "Administrator";
            position.OrgUnitUuid = operations.Uuid;
            userService.Update(user);

            user = UserReg();
            user.Person.Name = "Marie Marolle";
            user.UserId = "marolle";
            position = new DTO.V1_1.Position();
            user.Positions.Add(position);
            position.Name = "Administrator";
            position.OrgUnitUuid = operations.Uuid;
            userService.Update(user);

            OrgUnitRegistration projects = OUReg();
            projects.Name = "Udvikling og projekter";
            projects.ParentOrgUnitUuid = itDepartment.Uuid;
            orgUnitService.Update(projects);

            user = UserReg();
            user.Person.Name = "Henrik Jeppesen";
            user.UserId = "jeppe";
            position = new DTO.V1_1.Position();
            user.Positions.Add(position);
            position.Name = "Projektleder";
            position.OrgUnitUuid = projects.Uuid;
            userService.Update(user);
        }

        private static void TestUpdateWithoutChanges()
        {
            OrgUnitRegistration orgUnitRegistration = OUReg();
            orgUnitService.Update(orgUnitRegistration);
            orgUnitService.Update(orgUnitRegistration);

            UserRegistration userRegistration = UserReg();
            DTO.V1_1.Position position = new DTO.V1_1.Position();
            userRegistration.Positions.Add(position);
            position.Name = "PositionNameValue";
            position.OrgUnitUuid = orgUnitRegistration.Uuid;

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
            DTO.V1_1.Position position = new DTO.V1_1.Position();
            registration.Positions.Add(position);
            position.Name = "PositionNameValue";
            position.OrgUnitUuid = parentRegistration.Uuid;

            userService.Update(registration);
            userService.Delete(registration.Uuid, DateTime.Now.AddMinutes(-3));

            registration.Timestamp = DateTime.Now.AddMinutes(-1);
            userService.Update(registration);

            User user = inspectorService.ReadUserObject(registration.Uuid);
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
            registration.ItSystemUuids.Add(Uuid());
            registration.ItSystemUuids.Add(Uuid());
            registration.ContactPlaces.Add(new DTO.V1_1.ContactPlace()
            {
                OrgUnitUuid = parentReg.Uuid,
                Tasks = new List<string>() { Guid.NewGuid().ToString().ToLower(), Guid.NewGuid().ToString().ToLower() }
            });
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
            registration.ItSystemUuids.Add(Uuid());
            registration.ContactPlaces.Add(new DTO.V1_1.ContactPlace()
            {
                OrgUnitUuid = parentReg2.Uuid,
                Tasks = new List<string>() { Guid.NewGuid().ToString().ToLower(), Guid.NewGuid().ToString().ToLower() }
            });
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

            OrgUnitRegistration parentReg3 = OUReg();
            orgUnitService.Update(parentReg3);

            UserRegistration registration = UserReg();
            registration.Email.Value = "EmailValue";
            registration.Location.Value = "LocationValue";
            registration.Person.Cpr = "0000000000";
            registration.Person.Name = "PersonNameValue";
            registration.Phone.Value = "PhoneValue";
            DTO.V1_1.Position position = new DTO.V1_1.Position();
            registration.Positions.Add(position);
            position.Name = "PositionNameValue";
            position.OrgUnitUuid = parentReg.Uuid;

            DTO.V1_1.Position position2 = new DTO.V1_1.Position();
            registration.Positions.Add(position2);
            position2.Name = "PositionNameValue3";
            position2.OrgUnitUuid = parentReg3.Uuid;

            registration.UserId = "UserIdValue";
            userService.Update(registration);

            User user = inspectorService.ReadUserObject(registration.Uuid);
            Validate(user, registration);

            registration.Email.Value = "EmailValue2";
            registration.Location.Value = "LocationValue2";
            registration.Person.Cpr = "0000000001";
            registration.Person.Name = "PersonNameValue2";
            registration.Phone.Value = "PhoneValue2";
            position = new DTO.V1_1.Position();
            registration.Positions.Clear();
            registration.Positions.Add(position);
            position.Name = "PositionNameValue2";
            position.OrgUnitUuid = parentReg2.Uuid;
            registration.UserId = "UserIdValue2";
            userService.Update(registration);

            user = inspectorService.ReadUserObject(registration.Uuid);
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
            registration.Uuid = Uuid();
            registration.UserId = "DefaultUserID";
            registration.Person.Name = "DefaultPersonName";

            return registration;
        }

        private static string Uuid()
        {
            return Guid.NewGuid().ToString().ToLower();
        }

        private static void Validate(User user, UserRegistration registration)
        {
            if (!string.Equals(user.Person?.Cpr, registration.Person.Cpr))
            {
                throw new Exception("CPR is not the same");
            }

            if (!string.Equals(user.Person?.Name, registration.Person.Name))
            {
                throw new Exception("Name is not the same");
            }

            if (!string.Equals(user.UserId, registration.UserId))
            {
                throw new Exception("UserId is not the same");
            }

            foreach (var positionInOrg in user.Positions)
            {
                bool found = false;

                foreach (var positionInLocal in registration.Positions)
                {
                    if (positionInOrg.OU.Uuid.Equals(positionInLocal.OrgUnitUuid) && positionInOrg.Name.Equals(positionInLocal.Name))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    throw new Exception("Position in organisation '" + positionInOrg.Uuid + "' did not exist in local registration");
                }
            }

            foreach (var positionInLocal in registration.Positions)
            {
                bool found = false;

                foreach (var positionInOrg in user.Positions)
                {
                    if (positionInOrg.OU.Uuid.Equals(positionInLocal.OrgUnitUuid) && positionInOrg.Name.Equals(positionInLocal.Name))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    throw new Exception("Position in local registration '" + positionInLocal.Uuid + "' did not exist in organisation");
                }
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

            // TODO: validation the other way around as well
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

            // TODO: validation the other way around as well
            if (registration.ContactPlaces != null)
            {
                foreach (DTO.V1_1.ContactPlace contactPlace in registration.ContactPlaces)
                {
                    bool found = false;

                    if (orgUnit.ContactPlaces != null)
                    {
                        foreach (ContactPlace orgContactPlace in orgUnit.ContactPlaces)
                        {
                            if (orgContactPlace.OrgUnit.Uuid.Equals(contactPlace.OrgUnitUuid))
                            {
                                if (orgContactPlace.Tasks == null || contactPlace.Tasks == null)
                                {
                                    if (orgContactPlace.Tasks == contactPlace.Tasks)
                                    {
                                        found = true; // both are null - which is kinda of a match
                                    }
                                }
                                else // both a non-null
                                {
                                    if (orgContactPlace.Tasks.Count == contactPlace.Tasks.Count) // if not same size, not match
                                    {
                                        bool allMatch = true;

                                        foreach (string task in orgContactPlace.Tasks)
                                        {
                                            if (!contactPlace.Tasks.Contains(task))
                                            {
                                                allMatch = false;
                                            }
                                        }

                                        if (allMatch)
                                        {
                                            found = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (!found)
                    {
                        throw new Exception("ContactPlace that points to OU = " + contactPlace.OrgUnitUuid + " should be in Organisation, but it is not!");
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
