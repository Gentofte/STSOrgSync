using GK.AD;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SDB;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace STSOrgSyncV2
{
    // ================================================================================
    public class UserHandler : IUserHandler
    {
        readonly IServiceProvider _serviceProvider;
        readonly ILogger _logger;
        readonly ISTSOSYNCV2Config _config;
        readonly IConfiguration Configuration;

        readonly IADObjectFactory _adObjectFactory;

        // -----------------------------------------------------------------------------
        public UserHandler(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetService<ILogger<UserHandler>>();
            _config = _serviceProvider.GetService<ISTSOSYNCV2Config>();
            Configuration = configuration;

            _adObjectFactory = _serviceProvider.GetService<IADObjectFactory>();
        }

        // -----------------------------------------------------------------------------
        public async Task HandleEvent(ADX.DTO.ADEvent adEvent, CancellationToken cancellationToken)
        {
            await DoUserStuff(adEvent, cancellationToken);
        }

        // -----------------------------------------------------------------------------
        async Task DoUserStuff(ADX.DTO.ADEvent adEvent, CancellationToken cancellationToken)
        {
            try
            {
                string operation = null;

                switch (adEvent.ADEventType)
                {
                    case ADX.DTO.ADEventType.Create:
                    case ADX.DTO.ADEventType.Raw:
                    case ADX.DTO.ADEventType.Update:
                        operation = "UPDATE";
                        break;
                    case ADX.DTO.ADEventType.Delete:
                        operation = "DELETE";
                        break;
                }

                if (operation != null)
                {
                    var userDTO = adEvent.GetADObject() as ADX.DTO.User;
                    if (userDTO != null)
                    {
                        var serverName = adEvent.Sender.ServerName;
                        var uuid = userDTO.objectGuid.ToString().ToLower();

                        var user = _adObjectFactory.TryGetUser(userDTO.objectGuid, serverName);
                        if (user != null)
                        {
                            var sdb = new SDBService(Configuration.GetValue<string>("SDBUrl"));
                            var stsUser = sdb.InspectUser(uuid, user.dn);

                            Organisation.BusinessLayer.DTO.Registration.UserRegistration userReg = new Organisation.BusinessLayer.DTO.Registration.UserRegistration();
                            userReg.Person.Cpr = stsUser.CPR;
                            userReg.Person.Name = stsUser.Name;
                            userReg.Timestamp = DateTime.Now;
                            userReg.UserId = stsUser.UserId;
                            userReg.ShortKey = stsUser.UserShortKey;
                            userReg.Uuid = uuid;

                            foreach (var address in stsUser.Addresses)
                            {
                                switch (address.Type)
                                {
                                    case SDBServices.STS.DTO.AddressType.Email:
                                        userReg.Email = address._text;
                                        break;
                                    case SDBServices.STS.DTO.AddressType.Telefon:
                                        userReg.PhoneNumber = address._text;
                                        break;
                                    case SDBServices.STS.DTO.AddressType.Placering:
                                        userReg.Location = address._text;
                                        break;
                                }    
                            }

                            if (stsUser.Position != null)
                            {
                                userReg.Positions.Add(new Organisation.BusinessLayer.DTO.Registration.Position()
                                {
                                    Name = stsUser.Position._text,
                                    OrgUnitUuid = stsUser.Position.OUID.ToString().ToLower()
                                });
                            }

                            // not in SDB yet
                            userReg.RacfID = null;

                            var userDao = new Organisation.SchedulingLayer.UserDao();
                            userDao.Save(userReg, "DELETE".Equals(operation) ? Organisation.SchedulingLayer.OperationType.DELETE : Organisation.SchedulingLayer.OperationType.UPDATE, Configuration.GetValue<string>("Municipality"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            await Task.Yield();
        }
    }
}