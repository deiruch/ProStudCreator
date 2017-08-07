DB-Setup
======

1. Sicherstellen dass SQL Server 2012 oder neuer verwendet wird.
2. SQL Server Configuration Manager öffnen und bei SQL Server-Dienste die Eigenschaften des SQL Server (MSSQLSERVER) öffnen.
3. Zum Tab FileStream navigieren und alle Checkboxen setzen.
4. SQL Server (MSSQLSERVER) neu starten.
5. FileGroup und File erstellen.

```
ALTER database ProStudCreator
ADD FILEGROUP fsfg_ProStudCreator
CONTAINS FILESTREAM
GO

ALTER database ProStudCreator
ADD FILE
(NAME= 'fs_ProStudCreator',
FILENAME = 'C:\ProStudCreator\fs_ProstudCreator')
TO FILEGROUP fsfg_ProStudCreator
GO

FILESTREAM_ON [fsfg_ProStudCreator]
```

8. Filestream funktioniert nur mit integrated Security (Windows autentifizierung.) -> SQL Login für `DefaultAppPool` erstellen.
9. Sicherstellen das der IIS User (´DefaultAppPool´) zugriffsrechte auf `C:\ProStudCreator\fs_ProstudCreator` hat.




Local-Setup
======

1. Visual Studio, SQL Server Management Studio und SQL Server 2012 oder neuer (keine Express Editionen) herunterladen.
2. Projekt in VS clonen.
3. [FileStream einschalten](https://docs.microsoft.com/en-us/sql/relational-databases/blob/enable-and-configure-filestream)
4. Datenbank an SQL Server anschliessen (attach)

```
USE [master]
GO
CREATE DATABASE [FileStreamDB] ON 
( FILENAME = N'C:\FileStreamDB\FileStreamDB.mdf' ),
( FILENAME = N'C:\FileStreamDB\FileStreamDB_log.ldf' ),
FILEGROUP [FileStreamGroup] CONTAINS FILESTREAM DEFAULT 
( NAME = N'FileStreamDB_FSData', FILENAME = N'C:\FileStreamDB\FileStreamData' )
FOR ATTACH
GO
```

5. Telerik-Setup durchführen



Telerik-Setup
======

[Anleitung von Telerik](http://docs.telerik.com/devtools/aspnet-ajax/general-information/adding-the-telerik-controls-to-your-project)
