using Organisation.BusinessLayer;
using Organisation.SchedulingLayer;
using System;
using System.Net;
using System.Web.Http;

namespace Organisation.ServiceLayer
{
    public class HealthController : ApiController
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private UserDao userDao;
        private HealthService healthService;

        public HealthController()
        {
            userDao = new UserDao();
            healthService = new HealthService();
        }

        [HttpGet]
        public IHttpActionResult Health()
        {
            HealthStatus status = healthService.Status();

            try
            {
                userDao.GetOldestEntry();
            }
            catch (Exception ex)
            {
                log.Error("Connection to database failed!", ex);
                status.DBStatus = false;
            }

            if (status.Up())
            {
                return Ok(status);
            }

            return Content(HttpStatusCode.InternalServerError, status);
        }
    }
}