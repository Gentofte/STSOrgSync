namespace SDBServices.STS.DTO
{
    /// <summary>
    /// An enumeration of supported address types for STS users and OUs.
    /// 
    /// Each element states if it supports users and/or OUs.
    /// </summary>
    public enum AddressType
    {
        /// <summary>
        /// The phoner number of the user or OU.
        /// 
        /// This element is valid both for users and OUs.
        /// </summary>
        Telefon,

        /// <summary>
        /// The email address of the user or OU
        /// 
        /// This element is valid both for users and OUs.
        /// </summary>
        Email,

        /// <summary>
        /// The physical location of the user or OU.
        /// 
        /// This element is valid both for users and OUs.
        /// </summary>
        Placering,

        /// <summary>
        /// If this OU is a PayoutUnit, it must have a reference to the corresponding unit in LOS. The value to give here is the Kaldenavn Kort value from LOS.
        /// 
        /// Please note that as a side-effect of registering this information on the OU, it will be created as a PayoutUnit, so do not put this value on ordinary OUs.
        /// 
        /// This element is only valid for OUs.
        /// </summary>
        LOSKaldenavnKort,

        /// <summary>
        /// The EAN number of this OU (if it has one).
        /// 
        /// This element is only valid for OUs.
        /// </summary>
        EAN,

        /// <summary>
        /// This is the post-address of this OU. 
        /// 
        /// Note that KOMBIT requires this to be a reference to DAR (Danmarks Adresse Register), and it is not well specified how this value should be entered.
        /// (Leave empty for now).
        /// 
        /// This element is only valid for OUs.
        /// </summary>
        PostAdresse,

        /// <summary>
        /// For contact purposes, it is possible to register at which days and which hours of the day, that this OU is open for (phone) business.
        /// 
        /// This element is only valid for OUs.
        /// </summary>
        TelefonAabningstid,

        /// <summary>
        /// For contact purposes, it is possible to register at which days and hours of the day, that this OU is open for business.
        /// 
        /// (Leave empty for now).
        /// 
        /// This element is only valid for OUs.
        /// </summary>
        HenvendelseAabningstid,

        /// <summary>
        /// Denoting the return address for mail.
        /// 
        /// This element is only valid for OUs.
        /// </summary>
        PostReturAdresse,

        /// <summary>
        /// Denoting comments for email contact.
        /// 
        /// This element is only valid for OUs.
        /// </summary>
        EmailBemaerkning,

        /// <summary>
        /// Denoting an address, where people can meet up to get personal assistance.
        /// 
        /// This element is only valid for OUs.
        /// </summary>
        HenvendelsesAdresse
    }
}
