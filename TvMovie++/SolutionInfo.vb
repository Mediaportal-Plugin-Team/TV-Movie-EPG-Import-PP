#Region "Copyright (C) 2005-2025 Team MediaPortal"

' Copyright (C) 2005-2025 Team MediaPortal
' http://www.team-mediaportal.com
' 
' MediaPortal is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 2 of the License, or
' (at your option) any later version.
' 
' MediaPortal is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with MediaPortal. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Reflection
'using MediaPortal.Common.Utils;

'
' General Information about an assembly is controlled through the following 
' set of attributes. Change these attribute values to modify the information
' associated with an assembly.
'
' These attributes apply to all assemblies in the solution
#If DEBUG Then

<Assembly: AssemblyConfiguration("Debug version")>
#Else
<Assembly: AssemblyConfiguration("")>
#End If

<Assembly: AssemblyCompany("Team MediaPortal")>
<Assembly: AssemblyProduct("MediaPortal")>
<Assembly: AssemblyCopyright("Copyright © 2005-2025 Team MediaPortal")>
<Assembly: AssemblyTrademark("")>
<Assembly: AssemblyCulture("")>

' Version information for an assembly consists of the following four values:
'
'      Major Version
'      Minor Version 
'      Build Number
'      Revision
'
' You can specify all the values or you can default the Revision and Build Numbers 
' by using the '*' as shown below:

<Assembly: AssemblyVersion("1.7.0.999")>
<Assembly: AssemblyFileVersion("1.7.0.999")>

'[assembly: CompatibleVersion("1.1.8.*", "1.1.8.*")]
