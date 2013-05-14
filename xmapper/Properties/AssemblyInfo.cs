using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle("xmapper")]
[assembly: AssemblyDescription("XML object serialization framework")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("xmapper")]
[assembly: AssemblyCopyright("Copyright © Leon Breedt 2010-2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("0.2.10.0")]
[assembly: AssemblyFileVersion("0.2.10.0")]

#if DEBUG
[assembly: InternalsVisibleTo("xmapper.test")]
#else
[assembly: AssemblyKeyFile(@"..\xmapper.snk")]
#endif
