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

        // Korsbæk Compliance test global UUIDs
        private static string korsbaekKommuneUuid = "e888f84a-483b-482e-9186-8772d23a0f4e";
        private static string jobcenterUuid = "d945cf4a-9cd7-4869-83d3-6e886d54d05f";
        private static string ydelseogRaadighedUuid = "e0fa4daf-d4d7-4698-afb4-f22e7d2f4534";
        private static string folkeregisterUuid = "071847cb-a18e-41d3-9b88-c4fcd9f966f8";
        private static string oekonomiogStyringUuid = "89dd21e1-83ce-496b-91d1-ebb2d1c375a0";
        private static string kontrolgruppenUuid = "d4e5fe6f-1181-4d6a-b00a-c765445b77da";
        private static string pensionUuid = "577628f2-38e9-4382-9c7b-98e91dff704b";
        private static string beskaeftigelsesomraadetUuid = "4d4969a4-a1e6-4572-8993-db726e81114c";
        private static string jobogRessourcerUuid = "6f6e3714-62fa-4a96-9ab5-13bcdd0bffff";

        // random values are fine for these, they are not referenced later
        private static string socialogSundhedsforvaltningenUuid = Guid.NewGuid().ToString().ToLower();
        private static string ungeomraadetUuid = Guid.NewGuid().ToString().ToLower();
        private static string jobogKompetencerUuid = Guid.NewGuid().ToString().ToLower();
        private static string borgerserviceUuid = Guid.NewGuid().ToString().ToLower();
        private static string infoOmstillingUuid = Guid.NewGuid().ToString().ToLower();
        private static string boerneogKulturforvaltningenUuid = Guid.NewGuid().ToString().ToLower();
        private static string oekonomiogAnalyseUuid = Guid.NewGuid().ToString().ToLower();
        private static string familieograadgivningUuid = Guid.NewGuid().ToString().ToLower();
        private static string dagtilbudogSundhedUuid = Guid.NewGuid().ToString().ToLower();
        private static string paedagogiskPsykologiskRaadgivningUuid = Guid.NewGuid().ToString().ToLower();
        private static string familieafdelingenUuid = Guid.NewGuid().ToString().ToLower();
        private static string pPRTeam1Uuid = Guid.NewGuid().ToString().ToLower();
        private static string pPRTeam2Uuid = Guid.NewGuid().ToString().ToLower();
        private static string socialogHandicapafdelingenUuid = Guid.NewGuid().ToString().ToLower();
        private static string denBoligsocialeEnhedUuid = Guid.NewGuid().ToString().ToLower();
        private static string detPsykosocialeomraadeUuid = Guid.NewGuid().ToString().ToLower();
        private static string handicapraadgivningenUuid = Guid.NewGuid().ToString().ToLower();
        private static string korsbaekHandicaptilbudUuid = Guid.NewGuid().ToString().ToLower();
        private static string sundhedUdviklingServiceogkonomiUuid = Guid.NewGuid().ToString().ToLower();
        private static string kulturFritidogUngeUuid = Guid.NewGuid().ToString().ToLower();

        // Employees in the Korsbaek test-case that has static UUIDs (for testing purposes)
        private static string thokildHansenUuid = "fa98d62e-83d5-4d3e-a192-4512052f57d3";
        private static string jakobKarlsenUuid = "43a07fbe-3713-4093-a46d-b91277d14350";
        private static string anetteMoellerUuid = "20251ced-7617-44d2-9999-7d43ff0fca12";
        private static string lissiSederquistUuid = "b247e9ff-56ac-4317-a43c-2adb794bb3de";
        private static string dorteVinkelUuid = "21ae6961-1e3f-43b3-9e7d-4fedb214f0ff";
        private static string anitaStenbjergUuid = "d67cf40c-0d6c-4c7c-b863-792028dae4bc";
        private static string markusJensenUuid = "13022073-8665-4064-898d-2f04fca737f8";

        static void Main(string[] args)
        {
            Initializer.Init();

            // This is a small method to build some useful sample data for manuel testing
            // BuildTestData();

            // compliance testcases, do not run unless needed
            // KorsbaekData_Init();
            // KorsbaekData_511(); // flyt afdeling
            // KorsbaekData_512(); // luk afdeling
            // KorsbaekData_521(); // omplacering af medarbejder
            // KorsbaekData_522(); // afslutning af medarbejder
            // KorsbaekData_531(); // tilknytte henvendelsessted
            // KorsbaekData_532(); // ændre henvendelsessted
            // KorsbaekData_541(); // tilknytte udbetalingsenhed
            // KorsbaekData_551(); // tilknytte it-system
            // KorsbaekData_552(); // fjerne it-system

            /* ordinary tests */
            /*
            TestListAndReadOUs();
            TestListAndReadUsers();
            TestCreateAndUpdateFullUser();
            TestCreateAndUpdateFullOU();
            TestCreateDeleteUpdateUser();
            TestCreateDeleteUpdateOU();
            TestUpdateWithoutChanges();
            TestPayoutUnits();
            TestPositions();
            TestContactPlaces();
            TestUpdateAndSearch();
            TestMultipleAddresses();
            TestItSystemUsage();
            */
            //            DeltaReadTest();

            System.Environment.Exit(0);
        }

        private static void DeltaReadTest()
        {
            ///////////////// OU TEST ///////////////////////

            /*
            // 1: initial creation
            var ouReg = OUReg();
            ouReg.Uuid = "17fd6d54-1f0f-4463-b418-48c8963b65e1";
            ouReg.Name = "OU 1";
            orgUnitService.Create(ouReg);

            var ouReg2 = OUReg();
            ouReg2.Uuid = "17fd6d54-1f0f-4463-b418-48c8963b65e2";
            ouReg2.Name = "OU 2";
            orgUnitService.Create(ouReg2);
            */

            /*
            // 2: update
            var ouReg2 = OUReg();
            ouReg2.Uuid = "17fd6d54-1f0f-4463-b418-48c8963b65e2";
            ouReg2.Name = "OU 2 - updated";
            orgUnitService.Update(ouReg2);
            */

            // 3: verify using search
            List<string> ous = inspectorService.FindAllOUs();

            foreach (string ou in ous)
            {
                inspectorService.ReadOURaw(ou);
            }

            ///////////////// USER TEST ///////////////////////
            /*
                        // 1: initial creation
                        var userReg = UserReg();
                        userReg.Uuid = "be6eb2c9-f0db-41bb-ad65-37bee77d9a51";
                        userReg.UserId = "userId1";
                        userReg.Person.Name = "User 1";
                        userReg.Positions.Add(new DTO.V1_1.Position()
                        {
                            Name = "Position 1",
                            OrgUnitUuid = Uuid()

                        });
                        userService.Create(userReg);

                        var userReg2 = UserReg();
                        userReg2.Uuid = "be6eb2c9-f0db-41bb-ad65-37bee77d9a52";
                        userReg2.UserId = "userId2";
                        userReg2.Person.Name = "User 2";
                        userReg2.Positions.Add(new DTO.V1_1.Position()
                        {
                            Name = "Position 2",
                            OrgUnitUuid = Uuid()

                        });
                        userService.Create(userReg2);
            */
            // FUCK - searching for users objects that have been updated is not enough - we also need Person/Position objects.... ok, we can do that
            /*
                        // 2: update
                        var userReg2 = UserReg();
                        userReg2.Uuid = "be6eb2c9-f0db-41bb-ad65-37bee77d9a52";
                        userReg2.UserId = "userId2 - updated";
                        userReg2.Person.Name = "User 2";
                        userReg2.Positions.Add(new DTO.V1_1.Position()
                        {
                            Name = "Position 2",
                            OrgUnitUuid = Uuid()

                        });
                        userService.Update(userReg2);
            */
            /*
                        // 3: verify using search
                        List<string> users = inspectorService.FindAllUsers();
                        foreach (string user in users)
                        {
                            inspectorService.ReadUserRaw(user);
                        }

                        Console.WriteLine("Done");
            */
        }

        private static void TestMultipleAddresses()
        {
            var reg = OUReg();
            reg.Email = new Address() { Value = "email@email.com" };
            reg.Phone = new Address() { Value = "12345678" };
            orgUnitService.Update(reg);

            var ou = orgUnitService.Read(reg.Uuid);
            if (!"12345678".Equals(ou.Phone?.Value))
            {
                throw new Exception("Wrong phone");
            }
            else if (!"email@email.com".Equals(ou.Email?.Value))
            {
                throw new Exception("Wrong email");
            }
        }

        private static void TestUpdateAndSearch()
        {
            var ouReg1 = OUReg();
            ouReg1.Name = "ou1";
            orgUnitService.Update(ouReg1);

            var ouReg2 = OUReg();
            ouReg2.Name = "ou2";
            orgUnitService.Update(ouReg2);

            var userReg = UserReg();
            userReg.Person.Name = "name";            
            userReg.Positions.Add(new DTO.V1_1.Position()
            {
                Name = "position 1",
                OrgUnitUuid = ouReg1.Uuid                
            });
            userReg.Positions.Add(new DTO.V1_1.Position()
            {
                Name = "position 2",
                OrgUnitUuid = ouReg2.Uuid
            });
            userService.Update(userReg);

            userReg.Positions.Remove(userReg.Positions[0]);
            userService.Update(userReg);

            var user = userService.Read(userReg.Uuid);
            if (user.Positions.Count != 1)
            {
                throw new Exception("User position count should be 1 not " + user.Positions.Count);
            }
        }

        private static void TestListAndReadUsers()
        {
            // small hack to ensure this test passes (the search parameters will find all users in the organisation, and we need to test that it hits the required amount)
            OrganisationRegistryProperties properties = OrganisationRegistryProperties.GetInstance();
            string oldUuid = properties.MunicipalityOrganisationUUID[OrganisationRegistryProperties.GetMunicipality()];
            properties.MunicipalityOrganisationUUID[OrganisationRegistryProperties.GetMunicipality()] = Uuid();

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

            /* TODO: reenable this test once search takes Tilstand into consideration - it currently fails because of a bug @KMD */
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

            properties.MunicipalityOrganisationUUID[OrganisationRegistryProperties.GetMunicipality()] = oldUuid;
        }

        private static void TestListAndReadOUs()
        {
            // small hack to ensure this test passes (the search parameters will find all ous in the organisation, and we need to test that it hits the required amount)
            OrganisationRegistryProperties properties = OrganisationRegistryProperties.GetInstance();
            string oldUuid = properties.MunicipalityOrganisationUUID[OrganisationRegistryProperties.GetMunicipality()];
            properties.MunicipalityOrganisationUUID[OrganisationRegistryProperties.GetMunicipality()] = Uuid();

            OrgUnitRegistration registration1 = OUReg();
            registration1.Name = "magic";
            registration1.Email.Value = "email1@email.com";
            registration1.ParentOrgUnitUuid = Uuid();
            registration1.ItSystemUuids.Add(Uuid());
            registration1.ItSystemUuids.Add(Uuid());
            orgUnitService.Update(registration1);

            orgUnitService.Read(registration1.Uuid);

            OrgUnitRegistration registration2 = OUReg();
            registration2.Name = "magic";
            registration2.Email.Value = "email2@email.com";
            registration2.ParentOrgUnitUuid = Uuid();
            registration2.ItSystemUuids.Add(Uuid());
            registration2.ItSystemUuids.Add(Uuid());
            orgUnitService.Update(registration2);

            registration2.Name = "different name";
            orgUnitService.Update(registration2);

            // TODO: a KMD bug prevents this test from working...
            OrgUnitRegistration registration3 = OUReg();
            registration3.Name = "ou3";
            registration3.Email.Value = "email3@email.com";
            registration3.ParentOrgUnitUuid = Uuid();
            registration3.ItSystemUuids.Add(Uuid());
            registration3.ItSystemUuids.Add(Uuid());
            orgUnitService.Update(registration3);
            orgUnitService.Delete(registration3.Uuid, DateTime.Now);

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

            properties.MunicipalityOrganisationUUID[OrganisationRegistryProperties.GetMunicipality()] = oldUuid;
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

        // 5.1.1 - flyt en afdeling
        private static void KorsbaekData_511()
        {
            var orgUnitReg = orgUnitService.Read(kontrolgruppenUuid);
            orgUnitReg.ParentOrgUnitUuid = oekonomiogStyringUuid;
            orgUnitService.Update(orgUnitReg);
        }

        // 5.1.2 - luk afdeling
        private static void KorsbaekData_512()
        {
            orgUnitService.Delete(pensionUuid, DateTime.Now);

            var userReg = userService.Read(thokildHansenUuid);
            userReg.Positions.Clear();
            userService.Update(userReg);

            userReg = userService.Read(jakobKarlsenUuid);
            userReg.Positions.Clear();
            userService.Update(userReg);

            userReg = userService.Read(anetteMoellerUuid);
            userReg.Positions.Clear();
            userService.Update(userReg);

            userReg = userService.Read(lissiSederquistUuid);
            userReg.Positions.Clear();
            userService.Update(userReg);

            userReg = userService.Read(dorteVinkelUuid);
            userReg.Positions.Clear();
            userService.Update(userReg);
        }

        // 5.2.1 - omplacering af medarbejder
        private static void KorsbaekData_521()
        {
            var userReg = userService.Read(anitaStenbjergUuid);
            userReg.Positions.Add(new DTO.V1_1.Position { Name = "Assistent01", OrgUnitUuid = folkeregisterUuid });
            userService.Update(userReg);
        }

        // 5.2.2 - afslut medarbejder
        private static void KorsbaekData_522()
        {
            userService.Delete(markusJensenUuid, DateTime.Now);
        }

        // 5.3.1 - tilknytte henvendelsessted
        private static void KorsbaekData_531()
        {
            var orgUnitReg = orgUnitService.Read(korsbaekKommuneUuid);
            orgUnitReg.ContactPlaces.Add(new DTO.V1_1.ContactPlace()
            {
                OrgUnitUuid = jobcenterUuid,
                Tasks = new List<string> {
                    "ef0384a8-4d12-47b7-b834-17f5c56174c3",
                    "97ae950d-f1d9-4024-85f1-1ac1407292ed",
                    "356e5f68-e0af-4a80-ae13-6e1393a581ce",
                    "c0349e20-630b-4831-9239-773bc42eab3b",
                    "35601dc0-ccfe-460f-a1f4-2195a92801b3",
                    "fd3b5a52-78f0-450d-9863-c5b51fd02a6d",
                    "d67d8fda-3129-4119-be70-218c481cf891",
                    "2d99694c-9093-422f-b6e5-a92599f02c1a",
                    "54c0f130-d684-4ac5-afd9-d6b386de9f62",
                    "9144f628-5ab7-49c7-a58d-d3a52ba79b6c",
                    "c11086d8-3dd1-44c1-8339-37588f9817d2",
                    "6c89562c-9b33-4e37-8bac-58660f2af39a",
                    "5059fb7d-d7bb-4916-adb0-9ad70139a9ad",
                    "dbb1b318-3c85-11e3-9ae0-0050c2490048",
                    "dbb1b318-3c85-11e3-9ae1-0050c2490048",
                    "dbb1b318-3c85-11e3-9ae2-0050c2490048",
                    "1343e2b1-a913-11e3-9a40-0050c2490048",
                    "2c912c3b-b360-11e4-a829-0050c2490048",
                    "e3f432bb-1d99-11e6-a265-0050c2490048"
                }
            });

            orgUnitService.Update(orgUnitReg);
        }

        // 5.3.2 - ændre henvendelssted
        private static void KorsbaekData_532()
        {
            var orgUnitReg = orgUnitService.Read(jobogRessourcerUuid);
            orgUnitReg.ContactPlaces[0].OrgUnitUuid = jobcenterUuid;
            orgUnitService.Update(orgUnitReg);
        }

        // 5.4.1 - udbetalingsenhed (broken - the compliance test does not require this to be linked to any OU, which is bad)
        private static void KorsbaekData_541()
        {
            var orgUnitReg = orgUnitService.Read(ydelseogRaadighedUuid);
            orgUnitReg.LOSShortName = new Address()
            {
                ShortKey = "LOS YDRÅD",
                Value = "YDRÅD"
            };
            orgUnitService.Update(orgUnitReg);

            // TODO: as we create Udbetalingsenhed relations through a direct link, we need
            //       to hook this up to something - we pick Korsbæk Kommune
            orgUnitReg = orgUnitService.Read(korsbaekKommuneUuid);
            orgUnitReg.PayoutUnitUuid = ydelseogRaadighedUuid;
            orgUnitService.Update(orgUnitReg);
        }

        // 5.5.1 - it anvendelse
        private static void KorsbaekData_551()
        {
            var orgUnitReg = orgUnitService.Read(beskaeftigelsesomraadetUuid);
            orgUnitReg.ItSystemUuids.Add("1bd6d5d1-7e35-4c2a-9298-200c17ed64b2");
            orgUnitService.Update(orgUnitReg);

            orgUnitReg = orgUnitService.Read(jobcenterUuid);
            orgUnitReg.ItSystemUuids.Add("1bd6d5d1-7e35-4c2a-9298-200c17ed64b2");
            orgUnitService.Update(orgUnitReg);

            orgUnitReg = orgUnitService.Read(ydelseogRaadighedUuid);
            orgUnitReg.ItSystemUuids.Add("1bd6d5d1-7e35-4c2a-9298-200c17ed64b2");
            orgUnitService.Update(orgUnitReg);
        }

        // 5.5.2 - it anvendelse (nedlæg)
        private static void KorsbaekData_552()
        {
            var orgUnitReg = orgUnitService.Read(beskaeftigelsesomraadetUuid);
            orgUnitReg.ItSystemUuids.Clear();
            orgUnitService.Update(orgUnitReg);

            orgUnitReg = orgUnitService.Read(jobcenterUuid);
            orgUnitReg.ItSystemUuids.Clear();
            orgUnitService.Update(orgUnitReg);

            orgUnitReg = orgUnitService.Read(ydelseogRaadighedUuid);
            orgUnitReg.ItSystemUuids.Clear();
            orgUnitService.Update(orgUnitReg);
        }

        private static void KorsbaekData_Init()
        {
            List<OrgUnitRegistration> ous = new List<OrgUnitRegistration>();
            List<UserRegistration> users = new List<UserRegistration>();

            //////////////////// OUS //////////////////////

            var korsbaekKommune = new OrgUnitRegistration();
            korsbaekKommune.Uuid = korsbaekKommuneUuid;
            korsbaekKommune.Name = "Korsbæk Kommune";
            korsbaekKommune.ShortKey = "Korsbæk Kommune";
            ous.Add(korsbaekKommune);

            var socialogSundhedsforvaltningen = new OrgUnitRegistration();
            socialogSundhedsforvaltningen.Uuid = socialogSundhedsforvaltningenUuid;
            socialogSundhedsforvaltningen.Name = "Social og Sundhedsforvaltningen";
            socialogSundhedsforvaltningen.ShortKey = "Social og Sundhedsforvaltningen";
            socialogSundhedsforvaltningen.ParentOrgUnitUuid = korsbaekKommuneUuid;
            ous.Add(socialogSundhedsforvaltningen);

            var beskaeftigelsesomraadet = new OrgUnitRegistration();
            beskaeftigelsesomraadet.Uuid = beskaeftigelsesomraadetUuid;
            beskaeftigelsesomraadet.Name = "Beskæftigelsesområdet";
            beskaeftigelsesomraadet.ShortKey = "Beskæftigelsesområdet";
            beskaeftigelsesomraadet.ParentOrgUnitUuid = socialogSundhedsforvaltningenUuid;
            ous.Add(beskaeftigelsesomraadet);

            var ungeomraadet = new OrgUnitRegistration();
            ungeomraadet.Uuid = ungeomraadetUuid;
            ungeomraadet.Name = "Ungeområdet";
            ungeomraadet.ShortKey = "Ungeområdet";
            ungeomraadet.ParentOrgUnitUuid = beskaeftigelsesomraadetUuid;
            ous.Add(ungeomraadet);

            var jobcenter = new OrgUnitRegistration();
            jobcenter.Uuid = jobcenterUuid;
            jobcenter.Name = "Jobcenter";
            jobcenter.ShortKey = "Jobcenter";
            jobcenter.ParentOrgUnitUuid = beskaeftigelsesomraadetUuid;
            ous.Add(jobcenter);

            var ydelseogRaadighed = new OrgUnitRegistration();
            ydelseogRaadighed.Uuid = ydelseogRaadighedUuid;
            ydelseogRaadighed.Name = "Ydelse og Rådighed";
            ydelseogRaadighed.ShortKey = "Ydelse og Rådighed";
            ydelseogRaadighed.ParentOrgUnitUuid = jobcenterUuid;
            ydelseogRaadighed.PayoutUnitUuid = oekonomiogStyringUuid;
            ous.Add(ydelseogRaadighed);

            // TODO: der er ingen angivelse af hvilke Opgaver som Borgerservice er kontaktsted for, så der er indsat 0000...
            var jobogRessourcer = new OrgUnitRegistration();
            jobogRessourcer.Uuid = jobogRessourcerUuid;
            jobogRessourcer.Name = "Job og Ressourcer";
            jobogRessourcer.ShortKey = "Job og Ressourcer";
            jobogRessourcer.ParentOrgUnitUuid = jobcenterUuid;
            jobogRessourcer.ContactPlaces.Add(new DTO.V1_1.ContactPlace() { OrgUnitUuid = borgerserviceUuid, Tasks = new List<string> { "00000000-0000-4000-0000-000000000000" } });
            ous.Add(jobogRessourcer);

            var jobogKompetencer = new OrgUnitRegistration();
            jobogKompetencer.Uuid = jobogKompetencerUuid;
            jobogKompetencer.Name = "Job og Kompetencer";
            jobogKompetencer.ShortKey = "Job og Kompetencer";
            jobogKompetencer.ParentOrgUnitUuid = jobcenterUuid;
            ous.Add(jobogKompetencer);

            var borgerservice = new OrgUnitRegistration();
            borgerservice.Uuid = borgerserviceUuid;
            borgerservice.Name = "Borgerservice";
            borgerservice.ShortKey = "Borgerservice";
            borgerservice.ParentOrgUnitUuid = sundhedUdviklingServiceogkonomiUuid;
            ous.Add(borgerservice);

            // TODO: denne enhed havde ikke noget LOS navn, men det skal den have da den er udbetalende enhed
            var oekonomiogStyring = new OrgUnitRegistration();
            oekonomiogStyring.Uuid = oekonomiogStyringUuid;
            oekonomiogStyring.Name = "Økonomi og Styring";
            oekonomiogStyring.ShortKey = "Økonomi og Styring";
            oekonomiogStyring.LOSShortName = new Address { Value = "OEKOGSTYR" };
            oekonomiogStyring.ParentOrgUnitUuid = sundhedUdviklingServiceogkonomiUuid;
            ous.Add(oekonomiogStyring);

            var folkeregister = new OrgUnitRegistration();
            folkeregister.Uuid = folkeregisterUuid;
            folkeregister.Name = "Folkeregister";
            folkeregister.ShortKey = "Folkeregister";
            folkeregister.ParentOrgUnitUuid = borgerserviceUuid;
            ous.Add(folkeregister);

            var pension = new OrgUnitRegistration();
            pension.Uuid = pensionUuid;
            pension.Name = "Pension";
            pension.ShortKey = "Pension";
            pension.ParentOrgUnitUuid = borgerserviceUuid;
            ous.Add(pension);

            var kontrolgruppen = new OrgUnitRegistration();
            kontrolgruppen.Uuid = kontrolgruppenUuid;
            kontrolgruppen.Name = "Kontrolgruppen";
            kontrolgruppen.ShortKey = "Kontrolgruppen";
            kontrolgruppen.ParentOrgUnitUuid = borgerserviceUuid;
            ous.Add(kontrolgruppen);

            var infoOmstilling = new OrgUnitRegistration();
            infoOmstilling.Uuid = infoOmstillingUuid;
            infoOmstilling.Name = "Info-Omstilling";
            infoOmstilling.ShortKey = "Info-Omstilling";
            infoOmstilling.ParentOrgUnitUuid = borgerserviceUuid;
            ous.Add(infoOmstilling);

            var boerneogKulturforvaltningen = new OrgUnitRegistration();
            boerneogKulturforvaltningen.Uuid = boerneogKulturforvaltningenUuid;
            boerneogKulturforvaltningen.Name = "Børne og Kulturforvaltningen";
            boerneogKulturforvaltningen.ShortKey = "Børne og Kulturforvaltningen";
            boerneogKulturforvaltningen.ParentOrgUnitUuid = korsbaekKommuneUuid;
            ous.Add(boerneogKulturforvaltningen);

            var oekonomiogAnalyse = new OrgUnitRegistration();
            oekonomiogAnalyse.Uuid = oekonomiogAnalyseUuid;
            oekonomiogAnalyse.Name = "Økonomi og Analyse";
            oekonomiogAnalyse.ShortKey = "Økonomi og Analyse";
            oekonomiogAnalyse.ParentOrgUnitUuid = boerneogKulturforvaltningenUuid;
            ous.Add(oekonomiogAnalyse);

            var familieograadgivning = new OrgUnitRegistration();
            familieograadgivning.Uuid = familieograadgivningUuid;
            familieograadgivning.Name = "Familie og rådgivning";
            familieograadgivning.ShortKey = "Familie og rådgivning";
            familieograadgivning.ParentOrgUnitUuid = boerneogKulturforvaltningenUuid;
            ous.Add(familieograadgivning);

            var dagtilbudogSundhed = new OrgUnitRegistration();
            dagtilbudogSundhed.Uuid = dagtilbudogSundhedUuid;
            dagtilbudogSundhed.Name = "Dagtilbud og Sundhed";
            dagtilbudogSundhed.ShortKey = "Dagtilbud og Sundhed";
            dagtilbudogSundhed.ParentOrgUnitUuid = boerneogKulturforvaltningenUuid;
            ous.Add(dagtilbudogSundhed);

            var paedagogiskPsykologiskRaadgivning = new OrgUnitRegistration();
            paedagogiskPsykologiskRaadgivning.Uuid = paedagogiskPsykologiskRaadgivningUuid;
            paedagogiskPsykologiskRaadgivning.Name = "Pædagogisk Psykologisk Rådgivning";
            paedagogiskPsykologiskRaadgivning.ShortKey = "Pædagogisk Psykologisk Rådgivning";
            paedagogiskPsykologiskRaadgivning.ParentOrgUnitUuid = familieograadgivningUuid;
            ous.Add(paedagogiskPsykologiskRaadgivning);

            var familieafdelingen = new OrgUnitRegistration();
            familieafdelingen.Uuid = familieafdelingenUuid;
            familieafdelingen.Name = "Familieafdelingen";
            familieafdelingen.ShortKey = "Familieafdelingen";
            familieafdelingen.ParentOrgUnitUuid = familieograadgivningUuid;
            ous.Add(familieafdelingen);

            var pPRTeam1 = new OrgUnitRegistration();
            pPRTeam1.Uuid = pPRTeam1Uuid;
            pPRTeam1.Name = "PPR Team 1";
            pPRTeam1.ShortKey = "PPR Team 1";
            pPRTeam1.ParentOrgUnitUuid = paedagogiskPsykologiskRaadgivningUuid;
            ous.Add(pPRTeam1);

            var pPRTeam2 = new OrgUnitRegistration();
            pPRTeam2.Uuid = pPRTeam2Uuid;
            pPRTeam2.Name = "PPR Team 2";
            pPRTeam2.ShortKey = "PPR Team 2";
            pPRTeam2.ParentOrgUnitUuid = paedagogiskPsykologiskRaadgivningUuid;
            ous.Add(pPRTeam2);

            var socialogHandicapafdelingen = new OrgUnitRegistration();
            socialogHandicapafdelingen.Uuid = socialogHandicapafdelingenUuid;
            socialogHandicapafdelingen.Name = "Social og Handicapafdelingen";
            socialogHandicapafdelingen.ShortKey = "Social og Handicapafdelingen";
            socialogHandicapafdelingen.ParentOrgUnitUuid = socialogSundhedsforvaltningenUuid;
            ous.Add(socialogHandicapafdelingen);

            var denBoligsocialeEnhed = new OrgUnitRegistration();
            denBoligsocialeEnhed.Uuid = denBoligsocialeEnhedUuid;
            denBoligsocialeEnhed.Name = "Den Boligsociale Enhed";
            denBoligsocialeEnhed.ShortKey = "Den Boligsociale Enhed";
            denBoligsocialeEnhed.ParentOrgUnitUuid = socialogHandicapafdelingenUuid;
            ous.Add(denBoligsocialeEnhed);

            var detPsykosocialeomraade = new OrgUnitRegistration();
            detPsykosocialeomraade.Uuid = detPsykosocialeomraadeUuid;
            detPsykosocialeomraade.Name = "Det Psykosociale område";
            detPsykosocialeomraade.ShortKey = "Det Psykosociale område";
            detPsykosocialeomraade.ParentOrgUnitUuid = socialogHandicapafdelingenUuid;
            ous.Add(detPsykosocialeomraade);

            var handicapraadgivningen = new OrgUnitRegistration();
            handicapraadgivningen.Uuid = handicapraadgivningenUuid;
            handicapraadgivningen.Name = "Handicaprådgivningen";
            handicapraadgivningen.ShortKey = "Handicaprådgivningen";
            handicapraadgivningen.ParentOrgUnitUuid = socialogHandicapafdelingenUuid;
            ous.Add(handicapraadgivningen);

            var korsbaekHandicaptilbud = new OrgUnitRegistration();
            korsbaekHandicaptilbud.Uuid = korsbaekHandicaptilbudUuid;
            korsbaekHandicaptilbud.Name = "Korsbæk Handicaptilbud";
            korsbaekHandicaptilbud.ShortKey = "Korsbæk Handicaptilbud";
            korsbaekHandicaptilbud.ParentOrgUnitUuid = socialogHandicapafdelingenUuid;
            ous.Add(korsbaekHandicaptilbud);

            var sundhedUdviklingServiceogkonomi = new OrgUnitRegistration();
            sundhedUdviklingServiceogkonomi.Uuid = sundhedUdviklingServiceogkonomiUuid;
            sundhedUdviklingServiceogkonomi.Name = "Sundhed, Udvikling, Service og Økonomi";
            sundhedUdviklingServiceogkonomi.ShortKey = "Sundhed, Udvikling, Service og Økonomi";
            sundhedUdviklingServiceogkonomi.ParentOrgUnitUuid = socialogSundhedsforvaltningenUuid;
            ous.Add(sundhedUdviklingServiceogkonomi);

            var kulturFritidogUnge = new OrgUnitRegistration();
            kulturFritidogUnge.Uuid = kulturFritidogUngeUuid;
            kulturFritidogUnge.Name = "Kultur, Fritid og Unge";
            kulturFritidogUnge.ShortKey = "Kultur, Fritid og Unge";
            kulturFritidogUnge.ParentOrgUnitUuid = boerneogKulturforvaltningenUuid;
            ous.Add(kulturFritidogUnge);

            //////////////////// USERS //////////////////////

            var piaStenberg = new UserRegistration();
            piaStenberg.Uuid = Guid.NewGuid().ToString().ToLower();
            piaStenberg.UserId = "PIASTE";
            piaStenberg.ShortKey = "PIASTE";
            piaStenberg.Email = new Address { Value = "piaste@korsbaek.dk" };
            piaStenberg.Person.ShortKey = "P_PIASTE";
            piaStenberg.Person.Name = "Pia Stenberg";
            piaStenberg.Positions.Add(new DTO.V1_1.Position { Name = "YR. Medarb. 01", OrgUnitUuid = ydelseogRaadighedUuid });
            users.Add(piaStenberg);

            var catrineKristensen = new UserRegistration();
            catrineKristensen.Uuid = Guid.NewGuid().ToString().ToLower();
            catrineKristensen.UserId = "AMCATK";
            catrineKristensen.ShortKey = "AMCATK";
            catrineKristensen.Email = new Address { Value = "amcatk@korsbaek.dk" };
            catrineKristensen.Person.ShortKey = "P_AMCATK";
            catrineKristensen.Person.Name = "Catrine Kristensen";
            catrineKristensen.Positions.Add(new DTO.V1_1.Position { Name = "YR. Medarb. 02", OrgUnitUuid = ydelseogRaadighedUuid });
            users.Add(catrineKristensen);

            var dortheLangager = new UserRegistration();
            dortheLangager.Uuid = Guid.NewGuid().ToString().ToLower();
            dortheLangager.UserId = "BEDOLG";
            dortheLangager.ShortKey = "BEDOLG";
            dortheLangager.Email = new Address { Value = "bedolg@korsbaek.dk" };
            dortheLangager.Person.ShortKey = "P_BEDOLG";
            dortheLangager.Person.Name = "Dorthe Langager";
            dortheLangager.Positions.Add(new DTO.V1_1.Position { Name = "YR. Medarb. 03", OrgUnitUuid = ydelseogRaadighedUuid });
            users.Add(dortheLangager);

            var frederikDijon = new UserRegistration();
            frederikDijon.Uuid = Guid.NewGuid().ToString().ToLower();
            frederikDijon.UserId = "SOFCDJ";
            frederikDijon.ShortKey = "SOFCDJ";
            frederikDijon.Email = new Address { Value = "sofcdj@korsbaek.dk" };
            frederikDijon.Person.ShortKey = "P_SOFCDJ";
            frederikDijon.Person.Name = "Frederik Dijon";
            frederikDijon.Positions.Add(new DTO.V1_1.Position { Name = "YR. Medarb. 04", OrgUnitUuid = ydelseogRaadighedUuid });
            users.Add(frederikDijon);

            var frederikkeGertsen = new UserRegistration();
            frederikkeGertsen.Uuid = Guid.NewGuid().ToString().ToLower();
            frederikkeGertsen.UserId = "SOFDGE";
            frederikkeGertsen.ShortKey = "SOFDGE";
            frederikkeGertsen.Email = new Address { Value = "sofdge@korsbaek.dk" };
            frederikkeGertsen.Person.ShortKey = "P_SOFDGE";
            frederikkeGertsen.Person.Name = "Frederikke Gertsen";
            frederikkeGertsen.Positions.Add(new DTO.V1_1.Position { Name = "YR. Medarb. 05", OrgUnitUuid = ydelseogRaadighedUuid });
            users.Add(frederikkeGertsen);

            var leneBallen = new UserRegistration();
            leneBallen.Uuid = Guid.NewGuid().ToString().ToLower();
            leneBallen.UserId = "FLEBAL";
            leneBallen.ShortKey = "FLEBAL";
            leneBallen.Email = new Address { Value = "flebal@korsbaek.dk" };
            leneBallen.Person.ShortKey = "P_FLEBAL";
            leneBallen.Person.Name = "Lene Ballen";
            leneBallen.Positions.Add(new DTO.V1_1.Position { Name = "YR. Medarb. 06", OrgUnitUuid = ydelseogRaadighedUuid });
            users.Add(leneBallen);

            var henrikJacobsen = new UserRegistration();
            henrikJacobsen.Uuid = Guid.NewGuid().ToString().ToLower();
            henrikJacobsen.UserId = "HENJAC";
            henrikJacobsen.ShortKey = "HENJAC";
            henrikJacobsen.Email = new Address { Value = "henjac@korsbaek.dk" };
            henrikJacobsen.Person.ShortKey = "P_HENJAC";
            henrikJacobsen.Person.Name = "Henrik Jacobsen";
            henrikJacobsen.Positions.Add(new DTO.V1_1.Position { Name = "JR. Medarb. 01", OrgUnitUuid = jobogRessourcerUuid });
            users.Add(henrikJacobsen);

            var sofieDamager = new UserRegistration();
            sofieDamager.Uuid = Guid.NewGuid().ToString().ToLower();
            sofieDamager.UserId = "SOFJDM";
            sofieDamager.ShortKey = "SOFJDM";
            sofieDamager.Email = new Address { Value = "sofjdm@korsbaek.dk" };
            sofieDamager.Person.ShortKey = "P_SOFJDM";
            sofieDamager.Person.Name = "Sofie Damager";
            sofieDamager.Positions.Add(new DTO.V1_1.Position { Name = "JR. Medarb. 02", OrgUnitUuid = jobogRessourcerUuid });
            users.Add(sofieDamager);

            var jannieBodoe = new UserRegistration();
            jannieBodoe.Uuid = Guid.NewGuid().ToString().ToLower();
            jannieBodoe.UserId = "JANBOD";
            jannieBodoe.ShortKey = "JANBOD";
            jannieBodoe.Email = new Address { Value = "janbod@korsbaek.dk" };
            jannieBodoe.Person.ShortKey = "P_JANBOD";
            jannieBodoe.Person.Name = "Jannie Bodø";
            jannieBodoe.Positions.Add(new DTO.V1_1.Position { Name = "JR. Medarb. 03", OrgUnitUuid = jobogRessourcerUuid });
            users.Add(jannieBodoe);

            var katrineKaustisk = new UserRegistration();
            katrineKaustisk.Uuid = Guid.NewGuid().ToString().ToLower();
            katrineKaustisk.UserId = "KATKAU";
            katrineKaustisk.ShortKey = "KATKAU";
            katrineKaustisk.Email = new Address { Value = "katkau@korsbaek.dk" };
            katrineKaustisk.Person.ShortKey = "P_KATKAU";
            katrineKaustisk.Person.Name = "Katrine Kaustisk";
            katrineKaustisk.Positions.Add(new DTO.V1_1.Position { Name = "JR. Medarb. 04", OrgUnitUuid = jobogRessourcerUuid });
            users.Add(katrineKaustisk);

            var louiseDier = new UserRegistration();
            louiseDier.Uuid = Guid.NewGuid().ToString().ToLower();
            louiseDier.UserId = "LOUDIE";
            louiseDier.ShortKey = "LOUDIE";
            louiseDier.Email = new Address { Value = "loudie@korsbaek.dk" };
            louiseDier.Person.ShortKey = "P_LOUDIE";
            louiseDier.Person.Name = "Louise Dier";
            louiseDier.Positions.Add(new DTO.V1_1.Position { Name = "JR. Medarb. 05", OrgUnitUuid = jobogRessourcerUuid });
            users.Add(louiseDier);

            var anneBjerregaard = new UserRegistration();
            anneBjerregaard.Uuid = Guid.NewGuid().ToString().ToLower();
            anneBjerregaard.UserId = "ANJBJE";
            anneBjerregaard.ShortKey = "ANJBJE";
            anneBjerregaard.Email = new Address { Value = "anjbje@korsbaek.dk" };
            anneBjerregaard.Person.ShortKey = "P_ANJBJE";
            anneBjerregaard.Person.Name = "Anne Bjerregård";
            anneBjerregaard.Positions.Add(new DTO.V1_1.Position { Name = "FR. Medarb. 01", OrgUnitUuid = folkeregisterUuid });
            users.Add(anneBjerregaard);

            var anetteHemsfeldt = new UserRegistration();
            anetteHemsfeldt.Uuid = Guid.NewGuid().ToString().ToLower();
            anetteHemsfeldt.UserId = "SOFAHM";
            anetteHemsfeldt.ShortKey = "SOFAHM";
            anetteHemsfeldt.Email = new Address { Value = "sofahm@korsbaek.dk" };
            anetteHemsfeldt.Person.ShortKey = "P_SOFAHM";
            anetteHemsfeldt.Person.Name = "Anette Hemsfeldt";
            anetteHemsfeldt.Positions.Add(new DTO.V1_1.Position { Name = "FR. Medarb. 02", OrgUnitUuid = folkeregisterUuid });
            users.Add(anetteHemsfeldt);

            var charlieLarsen = new UserRegistration();
            charlieLarsen.Uuid = Guid.NewGuid().ToString().ToLower();
            charlieLarsen.UserId = "CHAMIL";
            charlieLarsen.ShortKey = "CHAMIL";
            charlieLarsen.Email = new Address { Value = "chamil@korsbaek.dk" };
            charlieLarsen.Person.ShortKey = "P_CHAMIL";
            charlieLarsen.Person.Name = "Charlie Larsen";
            charlieLarsen.Positions.Add(new DTO.V1_1.Position { Name = "FR. Medarb. 03", OrgUnitUuid = folkeregisterUuid });
            users.Add(charlieLarsen);

            var evalinaBoesen = new UserRegistration();
            evalinaBoesen.Uuid = Guid.NewGuid().ToString().ToLower();
            evalinaBoesen.UserId = "EBOLIS";
            evalinaBoesen.ShortKey = "EBOLIS";
            evalinaBoesen.Email = new Address { Value = "ebolis@korsbaek.dk" };
            evalinaBoesen.Person.ShortKey = "P_EBOLIS";
            evalinaBoesen.Person.Name = "Evalina Boesen";
            evalinaBoesen.Positions.Add(new DTO.V1_1.Position { Name = "FR. Medarb. 04", OrgUnitUuid = folkeregisterUuid });
            users.Add(evalinaBoesen);

            var nickieAagerup = new UserRegistration();
            nickieAagerup.Uuid = Guid.NewGuid().ToString().ToLower();
            nickieAagerup.UserId = "NICAAG";
            nickieAagerup.ShortKey = "NICAAG";
            nickieAagerup.Email = new Address { Value = "nicaag@korsbaek.dk" };
            nickieAagerup.Person.ShortKey = "P_NICAAG";
            nickieAagerup.Person.Name = "Nickie Aagerup";
            nickieAagerup.Positions.Add(new DTO.V1_1.Position { Name = "FR. Medarb. 05", OrgUnitUuid = folkeregisterUuid });
            users.Add(nickieAagerup);

            var lindaLassen = new UserRegistration();
            lindaLassen.Uuid = Guid.NewGuid().ToString().ToLower();
            lindaLassen.UserId = "CSFLLA";
            lindaLassen.ShortKey = "CSFLLA";
            lindaLassen.Email = new Address { Value = "csflla@korsbaek.dk" };
            lindaLassen.Person.ShortKey = "P_CSFLLA";
            lindaLassen.Person.Name = "Linda Lassen";
            lindaLassen.Positions.Add(new DTO.V1_1.Position { Name = "TL FR+Info", OrgUnitUuid = folkeregisterUuid });
            users.Add(lindaLassen);

            var anitaStenbjerg = new UserRegistration();
            anitaStenbjerg.Uuid = anitaStenbjergUuid;
            anitaStenbjerg.UserId = "CSFALS";
            anitaStenbjerg.ShortKey = "CSFALS";
            anitaStenbjerg.Email = new Address { Value = "csfals@korsbaek.dk" };
            anitaStenbjerg.Person.ShortKey = "P_CSFALS";
            anitaStenbjerg.Person.Name = "Anita Stenbjerg";
            anitaStenbjerg.Positions.Add(new DTO.V1_1.Position { Name = "Omst. Medarb. 01", OrgUnitUuid = infoOmstillingUuid });
            users.Add(anitaStenbjerg);

            var birgitteNielsen = new UserRegistration();
            birgitteNielsen.Uuid = Guid.NewGuid().ToString().ToLower();
            birgitteNielsen.UserId = "BIANNI";
            birgitteNielsen.ShortKey = "BIANNI";
            birgitteNielsen.Email = new Address { Value = "bianni@korsbaek.dk" };
            birgitteNielsen.Person.ShortKey = "P_BIANNI";
            birgitteNielsen.Person.Name = "Birgitte Nielsen";
            birgitteNielsen.Positions.Add(new DTO.V1_1.Position { Name = "Omst. Medarb. 02", OrgUnitUuid = infoOmstillingUuid });
            users.Add(birgitteNielsen);

            var brigitteRosendahl = new UserRegistration();
            brigitteRosendahl.Uuid = Guid.NewGuid().ToString().ToLower();
            brigitteRosendahl.UserId = "SOFBFR";
            brigitteRosendahl.ShortKey = "SOFBFR";
            brigitteRosendahl.Email = new Address { Value = "sofbfr@korsbaek.dk" };
            brigitteRosendahl.Person.ShortKey = "P_SOFBFR";
            brigitteRosendahl.Person.Name = "Brigitte Rosendahl";
            brigitteRosendahl.Positions.Add(new DTO.V1_1.Position { Name = "Omst. Medarb. 03", OrgUnitUuid = infoOmstillingUuid });
            users.Add(brigitteRosendahl);

            var markusJensen = new UserRegistration();
            markusJensen.Uuid = markusJensenUuid;
            markusJensen.UserId = "MAKOJE";
            markusJensen.ShortKey = "MAKOJE";
            markusJensen.Email = new Address { Value = "makoje@korsbaek.dk" };
            markusJensen.Person.ShortKey = "P_MAKOJE";
            markusJensen.Person.Name = "Markus Jensen";
            markusJensen.Positions.Add(new DTO.V1_1.Position { Name = "Chef Borgerservice", OrgUnitUuid = borgerserviceUuid });
            users.Add(markusJensen);

            var farrukhShah = new UserRegistration();
            farrukhShah.Uuid = Guid.NewGuid().ToString().ToLower();
            farrukhShah.UserId = "FARSHA";
            farrukhShah.ShortKey = "FARSHA";
            farrukhShah.Email = new Address { Value = "farsha@korsbaek.dk" };
            farrukhShah.Person.ShortKey = "P_FARSHA";
            farrukhShah.Person.Name = "Farrukh Shah";
            farrukhShah.Positions.Add(new DTO.V1_1.Position { Name = "Kontrol medarb. 01", OrgUnitUuid = kontrolgruppenUuid });
            users.Add(farrukhShah);

            var tinaKragenaes = new UserRegistration();
            tinaKragenaes.Uuid = Guid.NewGuid().ToString().ToLower();
            tinaKragenaes.UserId = "SOFTKR";
            tinaKragenaes.ShortKey = "SOFTKR";
            tinaKragenaes.Email = new Address { Value = "softkr@korsbaek.dk" };
            tinaKragenaes.Person.ShortKey = "P_SOFTKR";
            tinaKragenaes.Person.Name = "Tina Kragenæs";
            tinaKragenaes.Positions.Add(new DTO.V1_1.Position { Name = "Kontrol medarb. 02", OrgUnitUuid = kontrolgruppenUuid });
            users.Add(tinaKragenaes);

            var jannikKoberboel = new UserRegistration();
            jannikKoberboel.Uuid = Guid.NewGuid().ToString().ToLower();
            jannikKoberboel.UserId = "JAKOLA";
            jannikKoberboel.ShortKey = "JAKOLA";
            jannikKoberboel.Email = new Address { Value = "jakola@korsbaek.dk" };
            jannikKoberboel.Person.ShortKey = "P_JAKOLA";
            jannikKoberboel.Person.Name = "Jannik Koberbøl";
            jannikKoberboel.Positions.Add(new DTO.V1_1.Position { Name = "Kontrol medarb. 03", OrgUnitUuid = kontrolgruppenUuid });
            users.Add(jannikKoberboel);

            var anetteMoeller = new UserRegistration();
            anetteMoeller.Uuid = anetteMoellerUuid;
            anetteMoeller.UserId = "SOFAJN";
            anetteMoeller.ShortKey = "SOFAJN";
            anetteMoeller.Email = new Address { Value = "sofajn@korsbaek.dk" };
            anetteMoeller.Person.ShortKey = "P_SOFAJN";
            anetteMoeller.Person.Name = "Anette Møller";
            anetteMoeller.Positions.Add(new DTO.V1_1.Position { Name = "Pens. Medarb. 01", OrgUnitUuid = pensionUuid });
            users.Add(anetteMoeller);

            var lissiSederquist = new UserRegistration();
            lissiSederquist.Uuid = lissiSederquistUuid;
            lissiSederquist.UserId = "LICEKI";
            lissiSederquist.ShortKey = "LICEKI";
            lissiSederquist.Email = new Address { Value = "liceki@korsbaek.dk" };
            lissiSederquist.Person.ShortKey = "P_LICEKI";
            lissiSederquist.Person.Name = "Lissi Sederquist";
            lissiSederquist.Positions.Add(new DTO.V1_1.Position { Name = "Pens. Medarb. 02", OrgUnitUuid = pensionUuid });
            users.Add(lissiSederquist);

            var jakobKarlsen = new UserRegistration();
            jakobKarlsen.Uuid = jakobKarlsenUuid;
            jakobKarlsen.UserId = "JAKARA";
            jakobKarlsen.ShortKey = "JAKARA";
            jakobKarlsen.Email = new Address { Value = "jakara@korsbaek.dk" };
            jakobKarlsen.Person.ShortKey = "P_JAKARA";
            jakobKarlsen.Person.Name = "Jakob Karlsen";
            jakobKarlsen.Positions.Add(new DTO.V1_1.Position { Name = "Pens. Medarb. 03", OrgUnitUuid = pensionUuid });
            users.Add(jakobKarlsen);

            var thokildHansen = new UserRegistration();
            thokildHansen.Uuid = thokildHansenUuid;
            thokildHansen.UserId = "TOWEHA";
            thokildHansen.ShortKey = "TOWEHA";
            thokildHansen.Email = new Address { Value = "toweha@korsbaek.dk" };
            thokildHansen.Person.ShortKey = "P_TOWEHA";
            thokildHansen.Person.Name = "Thokild Hansen";
            thokildHansen.Positions.Add(new DTO.V1_1.Position { Name = "Pens. Medarb. 04", OrgUnitUuid = pensionUuid });
            users.Add(thokildHansen);

            var dorteVinkel = new UserRegistration();
            dorteVinkel.Uuid = dorteVinkelUuid;
            dorteVinkel.UserId = "LINVIN";
            dorteVinkel.ShortKey = "LINVIN";
            dorteVinkel.Email = new Address { Value = "linvin@korsbaek.dk" };
            dorteVinkel.Person.ShortKey = "P_LINVIN";
            dorteVinkel.Person.Name = "Dorte Vinkel";
            dorteVinkel.Positions.Add(new DTO.V1_1.Position { Name = "Pens. Medarb. 05", OrgUnitUuid = pensionUuid });
            users.Add(dorteVinkel);

            var lotteSigurdsen = new UserRegistration();
            lotteSigurdsen.Uuid = Guid.NewGuid().ToString().ToLower();
            lotteSigurdsen.UserId = "CSFLSI";
            lotteSigurdsen.ShortKey = "CSFLSI";
            lotteSigurdsen.Email = new Address { Value = "csflsi@korsbaek.dk" };
            lotteSigurdsen.Person.ShortKey = "P_CSFLSI";
            lotteSigurdsen.Person.Name = "Lotte Sigurdsen";
            lotteSigurdsen.Positions.Add(new DTO.V1_1.Position { Name = "TL Pension+Kontrol", OrgUnitUuid = pensionUuid });
            users.Add(lotteSigurdsen);

            var leneMortensen = new UserRegistration();
            leneMortensen.Uuid = Guid.NewGuid().ToString().ToLower();
            leneMortensen.UserId = "INLEMO";
            leneMortensen.ShortKey = "INLEMO";
            leneMortensen.Email = new Address { Value = "inlemo@korsbaek.dk" };
            leneMortensen.Person.ShortKey = "P_INLEMO";
            leneMortensen.Person.Name = "Lene Mortensen";
            leneMortensen.Positions.Add(new DTO.V1_1.Position { Name = "Assistenter", OrgUnitUuid = oekonomiogStyringUuid });
            users.Add(leneMortensen);

            var annePedersen = new UserRegistration();
            annePedersen.Uuid = Guid.NewGuid().ToString().ToLower();
            annePedersen.UserId = "ANNPED";
            annePedersen.ShortKey = "ANNPED";
            annePedersen.Email = new Address { Value = "annped@korsbaek.dk" };
            annePedersen.Person.ShortKey = "P_ANNPED";
            annePedersen.Person.Name = "Anne Pedersen";
            annePedersen.Positions.Add(new DTO.V1_1.Position { Name = "Assistenter", OrgUnitUuid = oekonomiogStyringUuid });
            users.Add(annePedersen);

            var dianaJensen = new UserRegistration();
            dianaJensen.Uuid = Guid.NewGuid().ToString().ToLower();
            dianaJensen.UserId = "DIEAJE";
            dianaJensen.ShortKey = "DIEAJE";
            dianaJensen.Email = new Address { Value = "dieaje@korsbaek.dk" };
            dianaJensen.Person.ShortKey = "P_DIEAJE";
            dianaJensen.Person.Name = "Diana Jensen";
            dianaJensen.Positions.Add(new DTO.V1_1.Position { Name = "Assistenter", OrgUnitUuid = oekonomiogStyringUuid });
            users.Add(dianaJensen);

            var gertrudGregersen = new UserRegistration();
            gertrudGregersen.Uuid = Guid.NewGuid().ToString().ToLower();
            gertrudGregersen.UserId = "GERGRE";
            gertrudGregersen.ShortKey = "GERGRE";
            gertrudGregersen.Email = new Address { Value = "gergre@korsbaek.dk" };
            gertrudGregersen.Person.ShortKey = "P_GERGRE";
            gertrudGregersen.Person.Name = "Gertrud Gregersen";
            gertrudGregersen.Positions.Add(new DTO.V1_1.Position { Name = "Udbetaling", OrgUnitUuid = oekonomiogStyringUuid });
            users.Add(gertrudGregersen);

            var thorValentin = new UserRegistration();
            thorValentin.Uuid = Guid.NewGuid().ToString().ToLower();
            thorValentin.UserId = "THOEVA";
            thorValentin.ShortKey = "THOEVA";
            thorValentin.Email = new Address { Value = "thoeva@korsbaek.dk" };
            thorValentin.Person.ShortKey = "P_THOEVA";
            thorValentin.Person.Name = "Thor Valentin";
            thorValentin.Positions.Add(new DTO.V1_1.Position { Name = "Udbetaling", OrgUnitUuid = oekonomiogStyringUuid });
            users.Add(thorValentin);

            var bjoernEriksen = new UserRegistration();
            bjoernEriksen.Uuid = Guid.NewGuid().ToString().ToLower();
            bjoernEriksen.UserId = "BJORGE";
            bjoernEriksen.ShortKey = "BJORGE";
            bjoernEriksen.Email = new Address { Value = "bjorge@korsbaek.dk" };
            bjoernEriksen.Person.ShortKey = "P_BJORGE";
            bjoernEriksen.Person.Name = "Bjørn Eriksen";
            bjoernEriksen.Positions.Add(new DTO.V1_1.Position { Name = "Udbetaling", OrgUnitUuid = oekonomiogStyringUuid });
            users.Add(bjoernEriksen);

            var jensNathansen = new UserRegistration();
            jensNathansen.Uuid = Guid.NewGuid().ToString().ToLower();
            jensNathansen.UserId = "JEMONA";
            jensNathansen.ShortKey = "JEMONA";
            jensNathansen.Email = new Address { Value = "jemona@korsbaek.dk" };
            jensNathansen.Person.ShortKey = "P_JEMONA";
            jensNathansen.Person.Name = "Jens Nathansen";
            jensNathansen.Positions.Add(new DTO.V1_1.Position { Name = "Debitor", OrgUnitUuid = oekonomiogStyringUuid });
            users.Add(jensNathansen);

            var lauraNielsen = new UserRegistration();
            lauraNielsen.Uuid = Guid.NewGuid().ToString().ToLower();
            lauraNielsen.UserId = "LAULNI";
            lauraNielsen.ShortKey = "LAULNI";
            lauraNielsen.Email = new Address { Value = "laulni@korsbaek.dk" };
            lauraNielsen.Person.ShortKey = "P_LAULNI";
            lauraNielsen.Person.Name = "Laura Nielsen";
            lauraNielsen.Positions.Add(new DTO.V1_1.Position { Name = "Debitor", OrgUnitUuid = oekonomiogStyringUuid });
            users.Add(lauraNielsen);

            var martinNielsen = new UserRegistration();
            martinNielsen.Uuid = Guid.NewGuid().ToString().ToLower();
            martinNielsen.UserId = "MARNIS";
            martinNielsen.ShortKey = "MARNIS";
            martinNielsen.Email = new Address { Value = "marnis@korsbaek.dk" };
            martinNielsen.Person.ShortKey = "P_MARNIS";
            martinNielsen.Person.Name = "Martin Nielsen";
            martinNielsen.Positions.Add(new DTO.V1_1.Position { Name = "Debitor", OrgUnitUuid = oekonomiogStyringUuid });
            users.Add(martinNielsen);

            // import everything
            foreach (var ou in ous)
            {
                orgUnitService.Update(ou);
            }

            foreach (var user in users)
            {
                userService.Update(user);
            }
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

            /*
            orgUnitRegistration.ItSystemUuids.Add(itSystem3);
            orgUnitService.Update(orgUnitRegistration);

            ou = inspectorService.ReadOUObject(orgUnitRegistration.Uuid);
            Validate(ou, orgUnitRegistration);

            orgUnitRegistration.ItSystemUuids.Remove(itSystem2);
            orgUnitService.Update(orgUnitRegistration);

            ou = inspectorService.ReadOUObject(orgUnitRegistration.Uuid);
            Validate(ou, orgUnitRegistration);
            */
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
                } /* TODO: not currently supported
                else if (address is Location && !address.Uuid.Equals(registration.Location.Uuid))
                {
                    throw new Exception("Location is not the same");
                } */
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
