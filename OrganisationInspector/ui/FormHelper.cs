using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OrganisationInspector
{
    public class FormHelper
    {
        public string ReadUUIDFromTextBox(TextBox tb)
        {
            if (tb.Text?.Length == 47)
            {
                tb.Text = ParserUtils.HexToUUID(tb.Text.Replace(" ", ""));
                return tb.Text;
            }

            return tb.Text?.ToLower();
        }

        public BindingSource GetObjectTypeComboBox()
        {
            return new BindingSource(new List<String> {"OrgUnit", "User", "Person","Address", "OrgFunction" }, null);

        }
    }
}
