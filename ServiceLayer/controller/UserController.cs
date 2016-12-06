using Organisation.BusinessLayer;
using Organisation.SchedulingLayer;
using System;
using System.Web.Http;

namespace Organisation.ServiceLayer
{
    public class UserController : ApiController
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ServiceMessages messages;
        private UserDao userDao;

        public UserController()
        {
            Initializer.Init();
            messages = new ServiceMessages();
            userDao = new UserDao();
        }

        [HttpPost]
        public IHttpActionResult Update(UserRegistration user)
        {
            bool isValid = ValidateUser(user);

            if (isValid)
            {
                try
                {
                    userDao.Save(user, OperationType.UPDATE);

                    return Ok(user);
                }
                catch (Exception ex)
                {
                    log.Error(messages.GetUserMessage(messages.UPDATE_FAILED), ex);
                }
            }

            return BadRequest();
        }

        [HttpDelete]
        public IHttpActionResult Delete(string uuid)
        {
            UserRegistrationExtended user = new UserRegistrationExtended();
            user.UserUuid = uuid;

            try
            {
                userDao.Save(user, OperationType.DELETE);

                return Ok(uuid);
            }
            catch (Exception ex)
            {
                log.Error(messages.GetUserMessage(messages.DELETE_FAILED), ex);
            }

            return BadRequest();
        }

        private bool ValidateUser(UserRegistration user)
        {
            // Mandatory fields - PersonName, PositionName, PositionOrgUnitUUID, UserID
            if (String.IsNullOrEmpty(user.PersonName) || String.IsNullOrEmpty(user.PositionName) ||
                String.IsNullOrEmpty(user.PositionOrgUnitUuid) || String.IsNullOrEmpty(user.UserId)
                || String.IsNullOrEmpty(user.UserUuid))
            {
                return false;
            }

            return true;
        }
    }
}