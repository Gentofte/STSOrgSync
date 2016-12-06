using System.Collections.Generic;

namespace OrganisationInspector
{
    public interface Validator
    {
        List<string> Validate(string uuid);
    }
}
