# SqlCeDiffer
Tool to create a sql difference script between 2 Sql Ce 4 files (.sdf)

## How To Use
Create a .cmd with the following details in it:

cd <path to sqlcediffer.exe>
sqlcediffer.exe -source:<path to latest schema file> -target:<path to old schema file> -outputPath:<path to save the diff script to>


If you want you an do this via MSBuild as follows:

<Target Name="Build">
		<Message Text="Starting Sql Differencing" />
		<Exec Command="<path to sql Diff>\sqlcediffer.exe -source:<new sdf file> -target:<old sdf file> -outputPath:c:\xxxx.sql" />
</Target>