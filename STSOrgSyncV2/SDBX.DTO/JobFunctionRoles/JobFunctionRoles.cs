﻿using System;
using System.Runtime.Serialization;

namespace SDBServices.STS.DTO
{
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/sdbservices/sts/dto/2016/v1")]
    public class JobFunctionRoles : Collection, IJobFunctionRoles
    {
        [DataMember]
        public string userId { get; set; }

        [DataMember]
        public string commonName { get; set; }

        [DataMember]
        public Guid serial { get; set; }
    }
}
