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
    public class OUHandler : IOUHandler
    {
        readonly IServiceProvider _serviceProvider;
        readonly ILogger _logger;
        readonly ISTSOSYNCV2Config _config;
        readonly IConfiguration Configuration;

        readonly IADObjectFactory _adObjectFactory;

        // -----------------------------------------------------------------------------
        public OUHandler(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetService<ILogger<OUHandler>>();
            _config = _serviceProvider.GetService<ISTSOSYNCV2Config>();
            Configuration = configuration;

            _adObjectFactory = _serviceProvider.GetService<IADObjectFactory>();
        }

        // -----------------------------------------------------------------------------
        public async Task HandleEvent(ADX.DTO.ADEvent adEvent, CancellationToken cancellationToken)
        {
            await DoOUStuff(adEvent, cancellationToken);
        }

        // -----------------------------------------------------------------------------
        async Task DoOUStuff(ADX.DTO.ADEvent adEvent, CancellationToken cancellationToken)
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
                var ouDTO = adEvent.GetADObject() as ADX.DTO.OU;

                if (ouDTO != null)
                {
                    var serverName = adEvent.Sender.ServerName;
                    var uuid = ouDTO.objectGuid.ToString().ToLower();

                    var ou = _adObjectFactory.TryGetOU(ouDTO.objectGuid, serverName);
                    if (ou != null)
                    {
                        var sdb = new SDBService(Configuration.GetValue<string>("SDBUrl"));
                        var stsOu = sdb.InspectOrgUnit(uuid, ou.dn);

                        if (stsOu == null || stsOu._objectID.Equals(Guid.Empty))
                        {
                            throw new Exception("OU did not exists in SDB: " + uuid);
                        }

                        if (!stsOu.WithinScope)
                        {
                            operation = "DELETE";
                        }

                        Organisation.BusinessLayer.DTO.Registration.OrgUnitRegistration ouReg = new Organisation.BusinessLayer.DTO.Registration.OrgUnitRegistration();
                        ouReg.Name = stsOu.Name;
                        ouReg.ParentOrgUnitUuid = stsOu._parentID?.ToString()?.ToLower();
                        ouReg.PayoutUnitUuid = stsOu._payoutUnitID?.ToString()?.ToLower();
                        ouReg.ShortKey = stsOu.ShortKey;
                        ouReg.Timestamp = DateTime.Now;
                        ouReg.Type = Organisation.BusinessLayer.DTO.Registration.OrgUnitRegistration.OrgUnitType.DEPARTMENT;
                        ouReg.Uuid = uuid;

                        foreach (var address in stsOu.Addresses)
                        {
                            switch (address.Type)
                            {
                                case SDBServices.STS.DTO.AddressType.Email:
                                    ouReg.Email = address._text;
                                    break;
                                case SDBServices.STS.DTO.AddressType.EmailBemaerkning:
                                    ouReg.EmailRemarks = address._text;
                                    break;
                                case SDBServices.STS.DTO.AddressType.Telefon:
                                    ouReg.PhoneNumber = address._text;
                                    break;
                                case SDBServices.STS.DTO.AddressType.TelefonAabningstid:
                                    ouReg.PhoneOpenHours = address._text;
                                    break;
                                case SDBServices.STS.DTO.AddressType.Placering:
                                    ouReg.Location = address._text;
                                    break;
                                case SDBServices.STS.DTO.AddressType.EAN:
                                    ouReg.Ean = address._text;
                                    break;
                                case SDBServices.STS.DTO.AddressType.HenvendelseAabningstid:
                                    ouReg.ContactOpenHours = address._text;
                                    break;
                                case SDBServices.STS.DTO.AddressType.HenvendelsesAdresse:
                                    ouReg.Contact = address._text;
                                    break;
                                case SDBServices.STS.DTO.AddressType.LOSKaldenavnKort:
                                    ouReg.LOSShortName = address._text;
                                    break;
                                case SDBServices.STS.DTO.AddressType.PostAdresse:
                                    ouReg.Post = address._text;
                                    break;
                                case SDBServices.STS.DTO.AddressType.PostReturAdresse:
                                    ouReg.PostReturn = address._text;
                                    break;
                            }
                        }

                        var orgUnitDao = new Organisation.SchedulingLayer.OrgUnitDao();
                        orgUnitDao.Save(ouReg, "DELETE".Equals(operation) ? Organisation.SchedulingLayer.OperationType.DELETE : Organisation.SchedulingLayer.OperationType.UPDATE, Configuration.GetValue<string>("Municipality"));
                    }
                }
            }

            await Task.Yield();
        }
    }
}
