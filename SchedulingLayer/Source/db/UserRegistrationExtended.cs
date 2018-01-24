using Organisation.BusinessLayer.DTO.V1_1;

namespace Organisation.SchedulingLayer
{
    public class UserRegistrationExtended : UserRegistration
    {
        public long Id { get; set; }
        public OperationType Operation { get; set; }   
    } 
}
