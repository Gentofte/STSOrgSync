using Organisation.BusinessLayer;
using Organisation.BusinessLayer.DTO.V1_1;
using Organisation.SchedulingLayer;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Organisation.ServiceLayer
{
    public class OrgUnitV11Controller : ApiController
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ServiceMessages messages;
        private OrgUnitDao orgUnitDao;
        private OrgUnitService orgUnitService;

        public OrgUnitV11Controller()
        {
            messages = new ServiceMessages();
            orgUnitDao = new OrgUnitDao();
            orgUnitService = new OrgUnitService();
        }

        [HttpPost]
        public IHttpActionResult Update(OrgUnitRegistration ou)
        {
            bool isValid = ValidateOU(ou);

            if (isValid)
            {
                try
                {
                    orgUnitDao.Save(ou, OperationType.UPDATE);
                    return Ok(ou);
                }
                catch (Exception ex)
                {
                    log.Error(messages.GetOuMessage(messages.UPDATE_FAILED), ex);
                }
            }

            return BadRequest();
        }

        [HttpDelete]
        public IHttpActionResult Remove(string uuid)
        {
            OrgUnitRegistrationExtended toRemove = new OrgUnitRegistrationExtended();
            toRemove.Uuid = uuid;

            try
            {
                orgUnitDao.Save(toRemove, OperationType.DELETE);

                return Ok();
            }
            catch (Exception ex)
            {
                log.Error(messages.GetOuMessage(messages.DELETE_FAILED), ex);
            }

            return BadRequest();
        }

        [HttpGet]
        public IHttpActionResult Read(string uuid)
        {
            if (string.IsNullOrEmpty(uuid))
            {
                return BadRequest();
            }

            try
            {
                OrgUnitRegistration registration = orgUnitService.Read(uuid);
                if (registration == null)
                {
                    return NotFound();
                }

                return Ok(registration);
            }
            catch (Exception ex)
            {
                log.Error(messages.GetOuMessage(messages.READ_FAILED), ex);

                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult List()
        {
            try
            {
                List<string> result = orgUnitService.List();

                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(messages.GetOuMessage(messages.READ_FAILED), ex);

                return InternalServerError();
            }
        }

        private bool ValidateOU(OrgUnitRegistration ou)
        {
            if (String.IsNullOrEmpty(ou.Name) || String.IsNullOrEmpty(ou.Uuid))
            {
                return false;
            }

            return true;
        }
    }
}