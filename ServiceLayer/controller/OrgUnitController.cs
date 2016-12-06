﻿using Organisation.BusinessLayer;
using Organisation.SchedulingLayer;
using System;
using System.Web.Http;

namespace Organisation.ServiceLayer
{
    public class OrgUnitController : ApiController
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ServiceMessages Messages;
        private OrgUnitDao OrgUnitDao;

        public OrgUnitController()
        {
            Messages = new ServiceMessages();
            OrgUnitDao = new OrgUnitDao();
        }

        [HttpPost]
        public IHttpActionResult Update(OrgUnitRegistration ou)
        {
            bool isValid = ValidateOU(ou);

            if (isValid)
            {
                try
                {
                    OrgUnitDao.Save(ou, OperationType.UPDATE);
                    return Ok(ou);
                }
                catch (Exception ex)
                {
                    log.Error(Messages.GetUserMessage(Messages.UPDATE_FAILED), ex);
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
                OrgUnitDao.Save(toRemove, OperationType.DELETE);

                return Ok();
            }
            catch (Exception ex)
            {
                log.Error(Messages.GetUserMessage(Messages.DELETE_FAILED), ex);
            }

            return BadRequest();
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