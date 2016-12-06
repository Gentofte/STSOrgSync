using Organisation.BusinessLayer;
using System;

namespace Organisation.SchedulingLayer
{
    internal class SyncJob
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static DateTime nextRun = DateTime.MinValue;
        private static long errorCount = 0;

        public static void Run()
        {
            long userCount = 0, ouCount = 0; // itSystemCount = 0;

            if (DateTime.Compare(DateTime.Now, nextRun) < 0)
            {
                return;
            }

            try
            {
                log.Debug("Scheduler started synchronizing objects from queue");
                // IT systems have been decrecated for now
//                itSystemCount = HandleItSystems();
                userCount = HandleUsers();
                ouCount = HandleOUs();
//                log.Debug("Scheduler finished: itSystems=" + itSystemCount + ", users=" + userCount + ", ous=" + ouCount);
                log.Debug("Scheduler finished: users=" + userCount + ", ous=" + ouCount);

                errorCount = 0;
            }
            catch (Exception ex)
            {
                switch (errorCount)
                {
                    case 0:
                        errorCount = 1;
                        break;
                    case 1:
                        errorCount = 3;
                        break;
                    case 3:
                        errorCount = 6;
                        break;
                }

                // wait 10 minutes, then 30 minutes and finally 60 minutes between each run
                nextRun = DateTime.Now.AddMinutes(10 * errorCount);

                log.Error("Failed to run scheduler, sleeping until: " + nextRun.ToString("MM/dd/yyyy HH:mm"), ex);
            }
        }

        public static long HandleUsers()
        {
            UserService service = new UserService();
            UserDao dao = new UserDao();
            long count = 0;

            UserRegistrationExtended user = null;
            while ((user = dao.GetOldestEntry()) != null)
            {
                try
                {
                    if (user.Operation.Equals(OperationType.DELETE))
                    {
                        service.Delete(user.UserUuid, user.Timestamp);
                    }
                    else
                    {
                        service.Update(user);
                    }

                    count++;
                    dao.Delete(user.UserUuid);
                }
                catch (TemporaryFailureException ex)
                {
                    log.Error("Could not handle user '" + user.UserUuid + "' at the moment, will try later" , ex);
                    break;
                }
                catch (Exception ex)
                {
                    log.Error("Could not handle user '" + user.UserUuid + "'", ex);
                    dao.Delete(user.UserUuid);
                }
            }

            return count;
        }

        /*
        public static long HandleItSystems()
        {
            ItSystemService service = new ItSystemService();
            ItSystemDao dao = new ItSystemDao();
            long count = 0;

            ItSystemRegistrationExtended itSystem = null;
            while ((itSystem = dao.GetOldestEntry()) != null)
            {
                try
                {
                    if (itSystem.Operation.Equals(OperationType.DELETE))
                    {
                        service.Delete(itSystem.Uuid, itSystem.Timestamp);
                    }
                    else
                    {
                        service.Update(itSystem);
                    }

                    count++;

                    dao.Delete(itSystem.Uuid);
                }
                catch (TemporaryFailureException ex)
                {
                    log.Error("Could not handle ItSystem '" + itSystem.Uuid + "' at the moment, will try later", ex);
                    break;
                }
                catch (Exception ex)
                {
                    log.Error("Could not handle ItSystem '" + itSystem.Uuid + "'", ex);
                    dao.Delete(itSystem.Uuid);
                }
            }

            return count;
        }
        */

        public static long HandleOUs()
        {
            OrgUnitService service = new OrgUnitService();
            OrgUnitDao dao = new OrgUnitDao();
            long count = 0;

            OrgUnitRegistrationExtended ou = null;
            while ((ou = dao.GetOldestEntry()) != null)
            {
                try
                {
                    if (ou.Operation.Equals(OperationType.DELETE))
                    {
                        service.Delete(ou.Uuid, ou.Timestamp);
                    }
                    else
                    {
                        service.Update(ou);
                    }

                    count++;

                    dao.Delete(ou.Uuid);
                }
                catch (TemporaryFailureException ex)
                {
                    log.Error("Could not handle ou '" + ou.Uuid + "' at the moment, will try later" , ex);
                    break;
                }
                catch (Exception ex)
                {
                    log.Error("Could not handle ou '" + ou.Uuid + "'", ex);
                    dao.Delete(ou.Uuid);
                }
            }

            return count;
        }
    }
}
