Welcome to Ranet.UILibrary.OLAP project

It is Open Source project which is hosted at
http://code.google.com/p/ranet-uilibrary-olap/
http://ranetuilibraryolap.codeplex.com/
where you can find all source code.

You are welcome to report bugs and propositions at
http://code.google.com/p/ranet-uilibrary-olap/issues/list
http://groups.google.com/group/ranet-uilibrary-olap
http://ranetuilibraryolap.codeplex.com/WorkItem/AdvancedList.aspx
http://ranetuilibraryolap.codeplex.com/Thread/List.aspx


To use libraries you need to have:

On client:
   * Silverlight 3.0 compatible browser (i.e. Mozilla Firefox, Google Chrome, Microsoft Internet Explorer etc.)

On server:
   * Microsoft Internet information server (IIS)
   * Microsoft SQL Server 2005/2008 with Analysis services
   * .NET 3.5 
                       
On development computer:
   * .NET 3.5 (with MS Build) 
   * Silverlight SDK 3.0 (more info at http://silverlight.net/getstarted/)
   * Silverlight Toolkit October 2009 (http://silverlight.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=30514) 
   * Adomd client (Microsoft.AnalysisServices.AdomdClient.dll)
   * (optionally) WiX - for installer packages build (http://wix.sourceforge.net/)
   * (optionally) Microsoft Visual studio 2008 SP1 with Silverlight tools 3.0 installed
   * (optionally) SharpDevelop 3.1 (http://www.icsharpcode.net/OpenSource/SD/Download/)
   * (optionally) Ranet.BuildTools-1.0  (TFS build integration)

You need to build the sample application before run it.
You can use Visual Studio development webserver or can deploy it to IIS.
Set "Startup project" to %InstallDir%\Samples\UITest.Web\UITest.Web.csproj.

Please set correct OLAPConnectionString and Web Services virtual paths on "Config" tab or in Web.config.

The sample application supposes that you have installed Microsoft sample datawarehouse database 
AdventureWorksDW, you can find it at http://msftdbprodsamples.codeplex.com/.
Of course, you can use any other OLAP database.

Ranet.UILibrary.OLAP provides user interface for building MDX queries, parsing and executing MDX queries, browsing result sets and even changing cube data. Ranet.UILibrary.OLAP works under Firefox, Chrome, IE, with SQL Server Analysis Services 2005/2008, uses .NET 3.5 and Silverlight 3.0.

MDX Visual Designer overview

The MDX Visual Designer is a user control for building MDX queries. MDX Visual Designer provides browsing cube metadata. From the cube Metadata tab you can drag KPIs, Measures, hierarchies, dimensions onto the Filters, Columns, Rows and Data. The MDX Visual Designer automatically generates MDX query which can be edited manually. Design settings can be stored and restored. When you execute the query, the Query result pane displays the results for the MDX query using PivotGrid user control.
PivotGrid overview

PivotGrid is a user control for browsing result sets after executing MDX query. PivotGrid has a number of useful features, like:

    * Format cells according to cube settings (BACKCOLOR, FORECOLOR, etc.)
    * User settings for cell formatting using icons, indicators, font settings.
    * Edit cube data
    * Drilldown, collapse and expand data
    * Navigation through the history of the queries
    * Rotate axes
    * Hide empty rows and columns
    * Copy data between levels
    * Allocation between selected cells
    * Scaling table
    * Hints for cells
    * Export to MS Excel
    * Accessing data according to the privileges in SQL Server Analysis Services 

PivotGrid provides two modes of editing data:

    * Direct editing
    * Using cache 

Before using please read Readme file: http://code.google.com/p/ranet-uilibrary-olap/wiki/Readme
Ranet.UILibrary.OLAP video tutorial: http://www.youtube.com/watch?v=dDNa3D8RO4Y

You can find more info about project owner at http://galaktika.ru/en/
    
Commercial license and technical support for this product is available.

Call or email at the information shown below:

US, Canada and EU

Galantis, Inc
3555 Harbor Gateway South, Suite B, Costa Mesa, CA 92626
Office: +1-206-420-3807
Fax: +1-206-420-3807
Cell: +1-714-408-3200
Ron Clevenger
RonClevenger@galantis.com

Russia, Belarus, Ukraine, etc

Galaktika Corporation
http://www.galaktika.ru
FE TopSoft
Office: 375-17-294-9999, 375-17-294-9988, ext.1519
Cell: +375-29-770-2114
Anatoly Volodko
Anatoly.V.Volodko@galaktika.by
