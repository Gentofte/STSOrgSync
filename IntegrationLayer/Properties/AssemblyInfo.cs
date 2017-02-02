using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Org")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Org")]
[assembly: AssemblyCopyright("Copyright ©  2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// make the BusinessLayer, SchedulingLayer and Report Tool friends
[assembly: InternalsVisibleTo("BusinessLayer"),
           InternalsVisibleTo("BusinessLayer.TestDriver"),
           InternalsVisibleTo("SchedulingLayer"),
           InternalsVisibleTo("ServiceLayer"),
           InternalsVisibleTo("ReportTool"),
           InternalsVisibleTo("OrganisationInspector")]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("771d4d55-8cd3-4af4-8c28-ea094925d68a")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

