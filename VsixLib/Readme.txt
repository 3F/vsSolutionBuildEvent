

The main VSPackage uses the VsixLib for including additional libraries from GAC etc.

In general, the new VS2015 is already includes by default some libraries (as dependencies) into .vsix even if the "Copy Local" set as false! 
for example: Microsoft.VisualStudio.OLE.Interop.dll, Microsoft.VisualStudio.Shell.Interop.dll, etc.
It different for other versions! (+ MSBuild tools) As solution we use like this:

   <Content Include="bin\*.dll">
     <Visible>false</Visible>
     <Link>%(Filename)%(Extension)</Link>
     <IncludeInVSIX>true</IncludeInVSIX>
   </Content>

However, we should use list of files instead of mask like above - *.dll (e.g.: Include="Lib1.dll;Lib2.dll;Lib3.dll..."), 
because the VS will 'see' this folder is too early and some libraries may not be included in vsix (partially is means), but definitely should included after first build operation.

Therefore, we use VsixLib for equality between versions of VS (providing References into vsix) and for similar providing instead of including in main project.


/Use List.targets