using Organisation.BusinessLayer;
using Organisation.SchedulingLayer;
using System;
using System.Web.Http;

namespace Organisation.ServiceLayer
{
    /* DEPRECATED until KOMBIT redecides
    public class ItSystemController : ApiController
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ServiceMessages Messages;
        private ItSystemDao ItSystemDao;

        public ItSystemController()
        {
            Messages = new ServiceMessages();
            ItSystemDao = new ItSystemDao();
        }

        [HttpPost]
        public IHttpActionResult Update(ItSystemRegistration itSystem)
        {
            bool isValid = Validate(itSystem);

            if (isValid)
            {
                try
                {
                    ItSystemDao.Save(itSystem, OperationType.UPDATE);

                    return Ok(itSystem);
                }
                catch (Exception ex)
                {
                    log.Error(Messages.GetItSystemMessage(Messages.UPDATE_FAILED), ex);
                }
            }

            return BadRequest();
        }

        [HttpDelete]
        public IHttpActionResult Delete(string uuid)
        {
            ItSystemRegistrationExtended itSystem = new ItSystemRegistrationExtended();
            itSystem.Uuid = uuid;

            try
            {
                ItSystemDao.Save(itSystem, OperationType.DELETE);

                return Ok(uuid);
            }
            catch (Exception ex)
            {
                log.Error(Messages.GetItSystemMessage(Messages.DELETE_FAILED), ex);
            }

            return BadRequest();
        }

        private bool Validate(ItSystemRegistration itsystem)
        {
            if (string.IsNullOrEmpty(itsystem.Uuid))
            {
                return false;
            }

            return true;
        }
    }
    */
}