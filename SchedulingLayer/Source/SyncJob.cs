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
                HandleOUs(out ouCount);
                HandleUsers(out userCount);
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

                // wait 5 minutes, then 15 minutes and finally 30 minutes between each run
                nextRun = DateTime.Now.AddMinutes(5 * errorCount);

                log.Error("Failed to run scheduler, sleeping until: " + nextRun.ToString("MM/dd/yyyy HH:mm"), ex);
            }

            if (userCount > 0 || ouCount > 0)
            {
                log.Info("Scheduler completed " + userCount + " user(s) and " + ouCount + " ou(s)");
            }
        }

        public static void HandleUsers(out long count)
        {
            UserService service = new UserService();
            UserDao dao = new UserDao();
            count = 0;

            UserRegistrationExtended user = null;
            while ((user = dao.GetOldestEntry()) != null)
            {
                try
                {
                    if (user.Operation.Equals(OperationType.DELETE))
                    {
                        service.Delete(user.Uuid, user.Timestamp);
                    }
                    else
                    {
                        service.Update(user);
                    }

                    count++;
                    dao.Delete(user.Id);

                    errorCount = 0;
                }
                catch (TemporaryFailureException ex)
                {
                    log.Error("Could not handle user '" + user.Uuid + "' at the moment, will try later");
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.Error("Could not handle user '" + user.Uuid + "'", ex);
                    dao.Delete(user.Id);
                }
            }
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

        public static void HandleOUs(out long count)
        {
            OrgUnitService service = new OrgUnitService();
            OrgUnitDao dao = new OrgUnitDao();
            count = 0;

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
                    dao.Delete(ou.Id);

                    errorCount = 0;
                }
                catch (TemporaryFailureException ex)
                {
                    log.Error("Could not handle ou '" + ou.Uuid + "' at the moment, will try later");
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.Error("Could not handle ou '" + ou.Uuid + "'", ex);
                    dao.Delete(ou.Id);
                }
            }
        }
    }
}
