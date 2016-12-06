using Organisation.BusinessLayer;
using System;

namespace OrganisationInspector
{
    public static class OrgUtils
    {
        public static string GetOrgObject(string uuid, string type, Boolean raw)
        {
            if (type.Equals("User"))
            {
                return GetUser(uuid, raw);
            }
            else if (type.Equals("Person"))
            {
                return GetPerson(uuid, raw);
            }
            else if (type.Equals("Address"))
            {
                return GetAddress(uuid, raw);
            }
            else if (type.Equals("OrgFunction"))
            {
                return GetOrgFunction(uuid, raw);
            }
            else
            {
                return GetOrgUnit(uuid, raw);
            }
        }

        public static string GetUser(string uuid, bool raw)
        {
            InspectorService service = new InspectorService();

            if (raw)
            {
                return service.ReadUserRaw(uuid);
            }

            User user = service.ReadUserObject(uuid);

            return ParserUtils.SerializeObject(user);
        }

        public static string GetPerson(string uuid, bool raw)
        {
            InspectorService service = new InspectorService();

            if (raw)
            {
                return service.ReadPersonRaw(uuid);
            }

            Person person = service.ReadPersonObject(uuid);

            return ParserUtils.SerializeObject(person);
        }

        public static string GetAddress(string uuid, bool raw)
        {
            InspectorService service = new InspectorService();

            if (raw)
            {
                return service.ReadAddressRaw(uuid);
            }

            AddressHolder address = service.ReadAddressObject(uuid);

            return ParserUtils.SerializeObject(address);
        }

        public static string GetOrgUnit(string uuid, bool raw)
        {
            InspectorService service = new InspectorService();

            if (raw)
            {
                return service.ReadOURaw(uuid);
            }

            OU orgUnit = service.ReadOUObject(uuid);

            return ParserUtils.SerializeObject(orgUnit);
        }

        public static string GetOrgFunction(string uuid, bool raw)
        {
            InspectorService service = new InspectorService();

            if (raw)
            {
                return service.ReadFunctionRaw(uuid);
            }

            Function orgFunction = service.ReadFunctionObject(uuid);

            return ParserUtils.SerializeObject(orgFunction);
        }
    }
}
