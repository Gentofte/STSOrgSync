using Organisation.BusinessLayer;
using Organisation.BusinessLayer.DTO.V1_1;
using Organisation.SchedulingLayer;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Organisation.ServiceLayer
{
    public class UserV11Controller : ApiController
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ServiceMessages messages;
        private UserDao userDao;
        private UserService userService;

        public UserV11Controller()
        {
            messages = new ServiceMessages();
            userDao = new UserDao();
            userService = new UserService();
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
            user.Uuid = uuid;

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

        [HttpGet]
        public IHttpActionResult Read(string uuid)
        {
            if (string.IsNullOrEmpty(uuid))
            {
                return BadRequest();
            }

            try
            {
                UserRegistration registration = userService.Read(uuid);
                if (registration == null)
                {
                    return NotFound();
                }

                return Ok(registration);
            }
            catch (Exception ex)
            {
                log.Error(messages.GetUserMessage(messages.READ_FAILED), ex);

                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult List()
        {
            try
            {
                List<string> result = userService.List();

                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(messages.GetUserMessage(messages.READ_FAILED), ex);

                return InternalServerError();
            }
        }

        private bool ValidateUser(UserRegistration user)
        {
            if (string.IsNullOrEmpty(user.Person.Name) ||  string.IsNullOrEmpty(user.UserId) || string.IsNullOrEmpty(user.Uuid))
            {
                return false;
            }

            if (user.Positions == null ||user.Positions.Count == 0)
            {
                return false;
            }

            foreach (BusinessLayer.DTO.V1_1.Position position in user.Positions)
            {
                if (string.IsNullOrEmpty(position.Name) ||string.IsNullOrEmpty(position.OrgUnitUuid))
                {
                    return false;
                }
            }

            return true;
        }
    }
}