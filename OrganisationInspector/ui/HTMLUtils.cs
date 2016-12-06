using System;
using System.Collections.Generic;
using System.Text;

namespace OrganisationInspector
{
    public static class HTMLUtils
    {
        public const string TEMPORARY_ERRORS_PAGE = "errors.html";
        public const string VALIDATION_RESULT_HTML = "validation_result.html";
        public const string TEMPORARY_DISPLAY_XML = "display.xml";

        public static string GetErrorPage(String message)
        {
            return "<h1>Error</h1>" + "<p>" + message + "</p>";
        }

        public static string GetErrorList(List<string> errors)
        {
            if (errors.Count == 0)
            {
                return "<h1> Object complies with all the validation rules.</h1>";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (String error in errors)
            {
                sb.Append("<li>" + error + "</li>");
            }
            sb.Append("</ul>");

            return sb.ToString();
        }
    }
}
