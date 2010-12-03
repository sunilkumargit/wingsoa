USE [master]
GO
/****** Object:  Database [FlexERPDW]    Script Date: 12/02/2010 22:45:21 ******/
CREATE DATABASE [FlexERPDW] ON  PRIMARY 
( NAME = N'FlexERPDW', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\FlexERPDW.mdf' , SIZE = 34816KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'FlexERPDW_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\FlexERPDW_log.ldf' , SIZE = 241216KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [FlexERPDW] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [FlexERPDW].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [FlexERPDW] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [FlexERPDW] SET ANSI_NULLS OFF
GO
ALTER DATABASE [FlexERPDW] SET ANSI_PADDING OFF
GO
ALTER DATABASE [FlexERPDW] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [FlexERPDW] SET ARITHABORT OFF
GO
ALTER DATABASE [FlexERPDW] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [FlexERPDW] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [FlexERPDW] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [FlexERPDW] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [FlexERPDW] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [FlexERPDW] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [FlexERPDW] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [FlexERPDW] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [FlexERPDW] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [FlexERPDW] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [FlexERPDW] SET  DISABLE_BROKER
GO
ALTER DATABASE [FlexERPDW] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [FlexERPDW] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [FlexERPDW] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [FlexERPDW] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [FlexERPDW] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [FlexERPDW] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [FlexERPDW] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [FlexERPDW] SET  READ_WRITE
GO
ALTER DATABASE [FlexERPDW] SET RECOVERY FULL
GO
ALTER DATABASE [FlexERPDW] SET  MULTI_USER
GO
ALTER DATABASE [FlexERPDW] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [FlexERPDW] SET DB_CHAINING OFF
GO
EXEC sys.sp_db_vardecimal_storage_format N'FlexERPDW', N'ON'
GO
USE [FlexERPDW]
GO
/****** Object:  Table [dbo].[ClienteDim]    Script Date: 12/02/2010 22:45:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClienteDim](
	[PK_Cliente] [int] NOT NULL,
	[Nome] [nvarchar](50) NULL,
	[UF] [nvarchar](50) NULL,
	[Regiao] [nvarchar](50) NULL,
	[Classe] [nvarchar](50) NULL,
 CONSTRAINT [PK_ClienteDim] PRIMARY KEY CLUSTERED 
(
	[PK_Cliente] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClienteDim', @level2type=N'COLUMN',@level2name=N'PK_Cliente'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Cliente' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClienteDim', @level2type=N'COLUMN',@level2name=N'PK_Cliente'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClienteDim', @level2type=N'COLUMN',@level2name=N'Nome'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Nome' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClienteDim', @level2type=N'COLUMN',@level2name=N'Nome'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClienteDim'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVTable', @value=N'ClienteDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClienteDim'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClienteDim'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClienteDim', @level2type=N'CONSTRAINT',@level2name=N'PK_ClienteDim'
GO
/****** Object:  Table [dbo].[ClassificaoFinanDim]    Script Date: 12/02/2010 22:45:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClassificaoFinanDim](
	[Grupo] [nvarchar](50) NULL,
	[Subgrupo] [nvarchar](50) NULL,
	[Classificacao] [nvarchar](50) NULL,
	[PK_ClassificacaoFinan] [nvarchar](50) NOT NULL,
	[Grupo_Name] [nvarchar](50) NULL,
	[Subgrupo_Name] [nvarchar](50) NULL,
	[Classificacao_Name] [nvarchar](50) NULL,
 CONSTRAINT [PK_ClassificaoFinanDim] PRIMARY KEY CLUSTERED 
(
	[PK_ClassificacaoFinan] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClassificaoFinanDim', @level2type=N'COLUMN',@level2name=N'Grupo'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Grupo' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClassificaoFinanDim', @level2type=N'COLUMN',@level2name=N'Grupo'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClassificaoFinanDim', @level2type=N'COLUMN',@level2name=N'Subgrupo'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Subgrupo' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClassificaoFinanDim', @level2type=N'COLUMN',@level2name=N'Subgrupo'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClassificaoFinanDim', @level2type=N'COLUMN',@level2name=N'Classificacao'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Classificacao' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClassificaoFinanDim', @level2type=N'COLUMN',@level2name=N'Classificacao'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClassificaoFinanDim', @level2type=N'COLUMN',@level2name=N'PK_ClassificacaoFinan'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'ClassificacaoFinan' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClassificaoFinanDim', @level2type=N'COLUMN',@level2name=N'PK_ClassificacaoFinan'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClassificaoFinanDim', @level2type=N'COLUMN',@level2name=N'Grupo_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Grupo_Name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClassificaoFinanDim', @level2type=N'COLUMN',@level2name=N'Grupo_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClassificaoFinanDim', @level2type=N'COLUMN',@level2name=N'Subgrupo_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Subgrupo_Name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClassificaoFinanDim', @level2type=N'COLUMN',@level2name=N'Subgrupo_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClassificaoFinanDim', @level2type=N'COLUMN',@level2name=N'Classificacao_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Classificacao_Name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClassificaoFinanDim', @level2type=N'COLUMN',@level2name=N'Classificacao_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClassificaoFinanDim'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVTable', @value=N'ClassificaoFinanDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClassificaoFinanDim'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClassificaoFinanDim'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ClassificaoFinanDim', @level2type=N'CONSTRAINT',@level2name=N'PK_ClassificaoFinanDim'
GO
/****** Object:  Table [dbo].[TituloCRDim]    Script Date: 12/02/2010 22:45:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TituloCRDim](
	[PK_TituloCR] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_TituloCRDim] PRIMARY KEY CLUSTERED 
(
	[PK_TituloCR] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TituloCRDim', @level2type=N'COLUMN',@level2name=N'PK_TituloCR'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'TituloCR' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TituloCRDim', @level2type=N'COLUMN',@level2name=N'PK_TituloCR'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TituloCRDim'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVTable', @value=N'TituloCRDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TituloCRDim'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TituloCRDim'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TituloCRDim', @level2type=N'CONSTRAINT',@level2name=N'PK_TituloCRDim'
GO
/****** Object:  Table [dbo].[TituloCPDim]    Script Date: 12/02/2010 22:45:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TituloCPDim](
	[PK_TituloCP] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_TituloCPDim] PRIMARY KEY CLUSTERED 
(
	[PK_TituloCP] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TituloCPDim', @level2type=N'COLUMN',@level2name=N'PK_TituloCP'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'TituloCP' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TituloCPDim', @level2type=N'COLUMN',@level2name=N'PK_TituloCP'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TituloCPDim'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVTable', @value=N'TituloCPDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TituloCPDim'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TituloCPDim'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TituloCPDim', @level2type=N'CONSTRAINT',@level2name=N'PK_TituloCPDim'
GO
/****** Object:  Table [dbo].[FormaPagtoDim]    Script Date: 12/02/2010 22:45:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormaPagtoDim](
	[FormaPagamento] [nvarchar](50) NULL,
	[PK_FormaPagto] [int] NOT NULL,
 CONSTRAINT [PK_FormaPagtoDim] PRIMARY KEY CLUSTERED 
(
	[PK_FormaPagto] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FormaPagtoDim', @level2type=N'COLUMN',@level2name=N'FormaPagamento'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'FormaPagamento' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FormaPagtoDim', @level2type=N'COLUMN',@level2name=N'FormaPagamento'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FormaPagtoDim', @level2type=N'COLUMN',@level2name=N'PK_FormaPagto'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'FormaPagto' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FormaPagtoDim', @level2type=N'COLUMN',@level2name=N'PK_FormaPagto'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FormaPagtoDim'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVTable', @value=N'FormaPagtoDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FormaPagtoDim'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FormaPagtoDim'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FormaPagtoDim', @level2type=N'CONSTRAINT',@level2name=N'PK_FormaPagtoDim'
GO
/****** Object:  Table [dbo].[EmpresaDim]    Script Date: 12/02/2010 22:45:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmpresaDim](
	[PK_Empresa] [int] NOT NULL,
	[Nome] [nvarchar](50) NULL,
 CONSTRAINT [PK_EmpresaDim] PRIMARY KEY CLUSTERED 
(
	[PK_Empresa] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'EmpresaDim', @level2type=N'COLUMN',@level2name=N'PK_Empresa'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Empresa' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'EmpresaDim', @level2type=N'COLUMN',@level2name=N'PK_Empresa'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'EmpresaDim', @level2type=N'COLUMN',@level2name=N'Nome'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Nome' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'EmpresaDim', @level2type=N'COLUMN',@level2name=N'Nome'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'EmpresaDim'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVTable', @value=N'EmpresaDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'EmpresaDim'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'EmpresaDim'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'EmpresaDim', @level2type=N'CONSTRAINT',@level2name=N'PK_EmpresaDim'
GO
/****** Object:  Table [dbo].[DateDim]    Script Date: 12/02/2010 22:45:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DateDim](
	[PK_Date] [datetime] NOT NULL,
	[Date_Name] [nvarchar](50) NULL,
	[Year] [datetime] NULL,
	[Year_Name] [nvarchar](50) NULL,
	[Trimester] [datetime] NULL,
	[Trimester_Name] [nvarchar](50) NULL,
	[Month] [datetime] NULL,
	[Month_Name] [nvarchar](50) NULL,
	[Day_Of_Year] [int] NULL,
	[Day_Of_Year_Name] [nvarchar](50) NULL,
	[Day_Of_Trimester] [int] NULL,
	[Day_Of_Trimester_Name] [nvarchar](50) NULL,
	[Day_Of_Month] [int] NULL,
	[Day_Of_Month_Name] [nvarchar](50) NULL,
	[Month_Of_Year] [int] NULL,
	[Month_Of_Year_Name] [nvarchar](50) NULL,
	[Month_Of_Trimester] [int] NULL,
	[Month_Of_Trimester_Name] [nvarchar](50) NULL,
	[Trimester_Of_Year] [int] NULL,
	[Trimester_Of_Year_Name] [nvarchar](50) NULL,
 CONSTRAINT [PK_DateDim] PRIMARY KEY CLUSTERED 
(
	[PK_Date] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'PK_Date'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Date' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'PK_Date'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Date_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Date_Name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Date_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Year'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Year'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Year_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Year_Name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Year_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Trimester'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Trimester' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Trimester'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Trimester_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Trimester_Name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Trimester_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Month'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Month' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Month'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Month_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Month_Name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Month_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Day_Of_Year'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Day_Of_Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Day_Of_Year'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Day_Of_Year_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Day_Of_Year_Name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Day_Of_Year_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Day_Of_Trimester'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Day_Of_Trimester' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Day_Of_Trimester'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Day_Of_Trimester_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Day_Of_Trimester_Name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Day_Of_Trimester_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Day_Of_Month'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Day_Of_Month' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Day_Of_Month'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Day_Of_Month_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Day_Of_Month_Name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Day_Of_Month_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Month_Of_Year'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Month_Of_Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Month_Of_Year'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Month_Of_Year_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Month_Of_Year_Name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Month_Of_Year_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Month_Of_Trimester'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Month_Of_Trimester' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Month_Of_Trimester'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Month_Of_Trimester_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Month_Of_Trimester_Name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Month_Of_Trimester_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Trimester_Of_Year'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Trimester_Of_Year' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Trimester_Of_Year'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Trimester_Of_Year_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'Trimester_Of_Year_Name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'COLUMN',@level2name=N'Trimester_Of_Year_Name'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVTable', @value=N'DateDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DateDim', @level2type=N'CONSTRAINT',@level2name=N'PK_DateDim'
GO
/****** Object:  Table [dbo].[Contas_a_Receber]    Script Date: 12/02/2010 22:45:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contas_a_Receber](
	[FK_Data_Emissao] [datetime] NULL,
	[FK_Forma_de_Pagamento] [int] NULL,
	[FK_Data_de_Vencimento] [datetime] NULL,
	[FK_Empresa] [int] NULL,
	[FK_Cliente] [int] NULL,
	[VlrNomRec] [float] NULL,
	[FK_Classificacao_Financeira] [nvarchar](50) NULL,
	[FK_Titulo_a_Receber] [nvarchar](50) NULL,
	[VlrAbRec] [float] NULL,
	[VlrAtrasoRec] [float] NULL,
	[VlrRec] [float] NULL,
	[VlrEncRec] [float] NULL,
	[VlrDescRec] [float] NULL
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CR_-_Titulos] ON [dbo].[Contas_a_Receber] 
(
	[FK_Forma_de_Pagamento] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CR_-_Titulos1] ON [dbo].[Contas_a_Receber] 
(
	[FK_Data_de_Vencimento] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FinCR] ON [dbo].[Contas_a_Receber] 
(
	[FK_Data_Emissao] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FinCR1] ON [dbo].[Contas_a_Receber] 
(
	[FK_Forma_de_Pagamento] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FinCR2] ON [dbo].[Contas_a_Receber] 
(
	[FK_Data_de_Vencimento] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FinCR3] ON [dbo].[Contas_a_Receber] 
(
	[FK_Empresa] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FinCR4] ON [dbo].[Contas_a_Receber] 
(
	[FK_Cliente] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FinCR5] ON [dbo].[Contas_a_Receber] 
(
	[FK_Classificacao_Financeira] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FinCR6] ON [dbo].[Contas_a_Receber] 
(
	[FK_Titulo_a_Receber] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FinCR7] ON [dbo].[Contas_a_Receber] 
(
	[FK_Classificacao_Financeira] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FinCR8] ON [dbo].[Contas_a_Receber] 
(
	[FK_Titulo_a_Receber] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'FK_Data_Emissao'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'FK_Data_Emissao' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'FK_Data_Emissao'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'FK_Forma_de_Pagamento'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'FK_Forma_Pagamento' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'FK_Forma_de_Pagamento'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'FK_Data_de_Vencimento'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'FK_Data_Vencimento' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'FK_Data_de_Vencimento'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'FK_Empresa'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'FK_Empresa' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'FK_Empresa'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'FK_Cliente'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'FK_Cliente' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'FK_Cliente'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'VlrNomRec'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'VlrParcReceber' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'VlrNomRec'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'FK_Classificacao_Financeira'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'FK_ClassificaoFinanDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'FK_Classificacao_Financeira'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'FK_Titulo_a_Receber'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'FK_TituloCRDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'FK_Titulo_a_Receber'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'VlrAbRec'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'VlrAbParcReceber' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'VlrAbRec'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'VlrAtrasoRec'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'VlrAtrasoParcReceber' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'VlrAtrasoRec'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'VlrRec'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'VlrRec' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'VlrRec'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'VlrEncRec'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'VlrEncRec' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'VlrEncRec'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'VlrDescRec'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'VlrDescRec' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'COLUMN',@level2name=N'VlrDescRec'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVTable', @value=N'FinCR' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'INDEX',@level2name=N'IX_CR_-_Titulos'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'INDEX',@level2name=N'IX_CR_-_Titulos1'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'INDEX',@level2name=N'IX_FinCR'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'INDEX',@level2name=N'IX_FinCR1'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'INDEX',@level2name=N'IX_FinCR2'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'INDEX',@level2name=N'IX_FinCR3'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'INDEX',@level2name=N'IX_FinCR4'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'INDEX',@level2name=N'IX_FinCR5'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'INDEX',@level2name=N'IX_FinCR6'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'INDEX',@level2name=N'IX_FinCR7'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'INDEX',@level2name=N'IX_FinCR8'
GO
/****** Object:  Table [dbo].[Contas_a_Pagar]    Script Date: 12/02/2010 22:45:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contas_a_Pagar](
	[VlrNomPg] [float] NULL,
	[FK_Forma_de_Pagamento] [int] NULL,
	[FK_Data_de_Vencimento] [datetime] NULL,
	[FK_Data_Emissao] [datetime] NULL,
	[FK_Empresa] [int] NULL,
	[FK_Cliente] [int] NULL,
	[FK_Classificacao_Financeira] [nvarchar](50) NULL,
	[FK_Titulo_a_Pagar] [nvarchar](50) NULL,
	[VlrAbPg] [float] NULL,
	[VlrAtrasoPg] [float] NULL,
	[VlrPago] [float] NULL,
	[VlrEncPg] [float] NULL,
	[VlrDescPg] [float] NULL
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CP_-_Titulos] ON [dbo].[Contas_a_Pagar] 
(
	[FK_Forma_de_Pagamento] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CP_-_Titulos1] ON [dbo].[Contas_a_Pagar] 
(
	[FK_Data_de_Vencimento] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FinCP] ON [dbo].[Contas_a_Pagar] 
(
	[FK_Data_Emissao] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FinCP1] ON [dbo].[Contas_a_Pagar] 
(
	[FK_Forma_de_Pagamento] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FinCP10] ON [dbo].[Contas_a_Pagar] 
(
	[FK_Titulo_a_Pagar] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FinCP2] ON [dbo].[Contas_a_Pagar] 
(
	[FK_Data_de_Vencimento] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FinCP3] ON [dbo].[Contas_a_Pagar] 
(
	[FK_Empresa] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FinCP4] ON [dbo].[Contas_a_Pagar] 
(
	[FK_Cliente] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FinCP5] ON [dbo].[Contas_a_Pagar] 
(
	[FK_Empresa] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FinCP6] ON [dbo].[Contas_a_Pagar] 
(
	[FK_Cliente] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FinCP7] ON [dbo].[Contas_a_Pagar] 
(
	[FK_Classificacao_Financeira] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FinCP8] ON [dbo].[Contas_a_Pagar] 
(
	[FK_Titulo_a_Pagar] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FinCP9] ON [dbo].[Contas_a_Pagar] 
(
	[FK_Classificacao_Financeira] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'VlrNomPg'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'ValorNominal' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'VlrNomPg'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'FK_Forma_de_Pagamento'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'FK_Forma_Pagamento' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'FK_Forma_de_Pagamento'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'FK_Data_de_Vencimento'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'FK_Data_Vencimento' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'FK_Data_de_Vencimento'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'FK_Data_Emissao'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'FK_Data_Emissao' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'FK_Data_Emissao'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'FK_Empresa'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'FK_EmpresaDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'FK_Empresa'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'FK_Cliente'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'FK_ClienteDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'FK_Cliente'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'FK_Classificacao_Financeira'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'FK_ClassificaoFinanDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'FK_Classificacao_Financeira'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'FK_Titulo_a_Pagar'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'FK_TituloCPDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'FK_Titulo_a_Pagar'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'VlrAbPg'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'VlrAbParcPagar' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'VlrAbPg'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'VlrAtrasoPg'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'VlrAtrasoParcPagar' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'VlrAtrasoPg'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'VlrPago'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'VlrPago' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'VlrPago'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'VlrEncPg'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'VlrEncPg' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'VlrEncPg'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'VlrDescPg'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVColumn', @value=N'VlrDescPg' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'COLUMN',@level2name=N'VlrDescPg'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVTable', @value=N'FinCP' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'INDEX',@level2name=N'IX_CP_-_Titulos'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'INDEX',@level2name=N'IX_CP_-_Titulos1'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'INDEX',@level2name=N'IX_FinCP'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'INDEX',@level2name=N'IX_FinCP1'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'INDEX',@level2name=N'IX_FinCP10'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'INDEX',@level2name=N'IX_FinCP2'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'INDEX',@level2name=N'IX_FinCP3'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'INDEX',@level2name=N'IX_FinCP4'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'INDEX',@level2name=N'IX_FinCP5'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'INDEX',@level2name=N'IX_FinCP6'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'INDEX',@level2name=N'IX_FinCP7'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'INDEX',@level2name=N'IX_FinCP8'
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'INDEX',@level2name=N'IX_FinCP9'
GO
/****** Object:  ForeignKey [Contas_a_Receber-ClassificaoFinanDim]    Script Date: 12/02/2010 22:45:21 ******/
ALTER TABLE [dbo].[Contas_a_Receber]  WITH CHECK ADD  CONSTRAINT [Contas_a_Receber-ClassificaoFinanDim] FOREIGN KEY([FK_Classificacao_Financeira])
REFERENCES [dbo].[ClassificaoFinanDim] ([PK_ClassificacaoFinan])
GO
ALTER TABLE [dbo].[Contas_a_Receber] CHECK CONSTRAINT [Contas_a_Receber-ClassificaoFinanDim]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-ClassificaoFinanDim'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVRelation', @value=N'Contas_a_Receber-ClassificaoFinanDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-ClassificaoFinanDim'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-ClassificaoFinanDim'
GO
/****** Object:  ForeignKey [Contas_a_Receber-ClienteDim]    Script Date: 12/02/2010 22:45:21 ******/
ALTER TABLE [dbo].[Contas_a_Receber]  WITH CHECK ADD  CONSTRAINT [Contas_a_Receber-ClienteDim] FOREIGN KEY([FK_Cliente])
REFERENCES [dbo].[ClienteDim] ([PK_Cliente])
GO
ALTER TABLE [dbo].[Contas_a_Receber] CHECK CONSTRAINT [Contas_a_Receber-ClienteDim]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-ClienteDim'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVRelation', @value=N'Contas_a_Receber-ClienteDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-ClienteDim'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-ClienteDim'
GO
/****** Object:  ForeignKey [Contas_a_Receber-DateDim]    Script Date: 12/02/2010 22:45:21 ******/
ALTER TABLE [dbo].[Contas_a_Receber]  WITH CHECK ADD  CONSTRAINT [Contas_a_Receber-DateDim] FOREIGN KEY([FK_Data_Emissao])
REFERENCES [dbo].[DateDim] ([PK_Date])
GO
ALTER TABLE [dbo].[Contas_a_Receber] CHECK CONSTRAINT [Contas_a_Receber-DateDim]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-DateDim'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVRelation', @value=N'Contas_a_Receber-DateDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-DateDim'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-DateDim'
GO
/****** Object:  ForeignKey [Contas_a_Receber-DateDim1]    Script Date: 12/02/2010 22:45:21 ******/
ALTER TABLE [dbo].[Contas_a_Receber]  WITH CHECK ADD  CONSTRAINT [Contas_a_Receber-DateDim1] FOREIGN KEY([FK_Data_de_Vencimento])
REFERENCES [dbo].[DateDim] ([PK_Date])
GO
ALTER TABLE [dbo].[Contas_a_Receber] CHECK CONSTRAINT [Contas_a_Receber-DateDim1]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-DateDim1'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVRelation', @value=N'Contas_a_Receber-DateDim1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-DateDim1'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-DateDim1'
GO
/****** Object:  ForeignKey [Contas_a_Receber-EmpresaDim]    Script Date: 12/02/2010 22:45:21 ******/
ALTER TABLE [dbo].[Contas_a_Receber]  WITH CHECK ADD  CONSTRAINT [Contas_a_Receber-EmpresaDim] FOREIGN KEY([FK_Empresa])
REFERENCES [dbo].[EmpresaDim] ([PK_Empresa])
GO
ALTER TABLE [dbo].[Contas_a_Receber] CHECK CONSTRAINT [Contas_a_Receber-EmpresaDim]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-EmpresaDim'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVRelation', @value=N'Contas_a_Receber-EmpresaDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-EmpresaDim'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-EmpresaDim'
GO
/****** Object:  ForeignKey [Contas_a_Receber-FormaPagtoDim]    Script Date: 12/02/2010 22:45:21 ******/
ALTER TABLE [dbo].[Contas_a_Receber]  WITH CHECK ADD  CONSTRAINT [Contas_a_Receber-FormaPagtoDim] FOREIGN KEY([FK_Forma_de_Pagamento])
REFERENCES [dbo].[FormaPagtoDim] ([PK_FormaPagto])
GO
ALTER TABLE [dbo].[Contas_a_Receber] CHECK CONSTRAINT [Contas_a_Receber-FormaPagtoDim]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-FormaPagtoDim'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVRelation', @value=N'Contas_a_Receber-FormaPagtoDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-FormaPagtoDim'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-FormaPagtoDim'
GO
/****** Object:  ForeignKey [Contas_a_Receber-TituloCRDim]    Script Date: 12/02/2010 22:45:21 ******/
ALTER TABLE [dbo].[Contas_a_Receber]  WITH CHECK ADD  CONSTRAINT [Contas_a_Receber-TituloCRDim] FOREIGN KEY([FK_Titulo_a_Receber])
REFERENCES [dbo].[TituloCRDim] ([PK_TituloCR])
GO
ALTER TABLE [dbo].[Contas_a_Receber] CHECK CONSTRAINT [Contas_a_Receber-TituloCRDim]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-TituloCRDim'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVRelation', @value=N'Contas_a_Receber-TituloCRDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-TituloCRDim'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Receber', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Receber-TituloCRDim'
GO
/****** Object:  ForeignKey [Contas_a_Pagar-ClassificaoFinanDim]    Script Date: 12/02/2010 22:45:21 ******/
ALTER TABLE [dbo].[Contas_a_Pagar]  WITH CHECK ADD  CONSTRAINT [Contas_a_Pagar-ClassificaoFinanDim] FOREIGN KEY([FK_Classificacao_Financeira])
REFERENCES [dbo].[ClassificaoFinanDim] ([PK_ClassificacaoFinan])
GO
ALTER TABLE [dbo].[Contas_a_Pagar] CHECK CONSTRAINT [Contas_a_Pagar-ClassificaoFinanDim]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-ClassificaoFinanDim'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVRelation', @value=N'Contas_a_Pagar-ClassificaoFinanDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-ClassificaoFinanDim'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-ClassificaoFinanDim'
GO
/****** Object:  ForeignKey [Contas_a_Pagar-ClienteDim]    Script Date: 12/02/2010 22:45:21 ******/
ALTER TABLE [dbo].[Contas_a_Pagar]  WITH CHECK ADD  CONSTRAINT [Contas_a_Pagar-ClienteDim] FOREIGN KEY([FK_Cliente])
REFERENCES [dbo].[ClienteDim] ([PK_Cliente])
GO
ALTER TABLE [dbo].[Contas_a_Pagar] CHECK CONSTRAINT [Contas_a_Pagar-ClienteDim]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-ClienteDim'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVRelation', @value=N'Contas_a_Pagar-ClienteDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-ClienteDim'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-ClienteDim'
GO
/****** Object:  ForeignKey [Contas_a_Pagar-DateDim]    Script Date: 12/02/2010 22:45:21 ******/
ALTER TABLE [dbo].[Contas_a_Pagar]  WITH CHECK ADD  CONSTRAINT [Contas_a_Pagar-DateDim] FOREIGN KEY([FK_Data_Emissao])
REFERENCES [dbo].[DateDim] ([PK_Date])
GO
ALTER TABLE [dbo].[Contas_a_Pagar] CHECK CONSTRAINT [Contas_a_Pagar-DateDim]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-DateDim'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVRelation', @value=N'Contas_a_Pagar-DateDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-DateDim'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-DateDim'
GO
/****** Object:  ForeignKey [Contas_a_Pagar-DateDim1]    Script Date: 12/02/2010 22:45:21 ******/
ALTER TABLE [dbo].[Contas_a_Pagar]  WITH CHECK ADD  CONSTRAINT [Contas_a_Pagar-DateDim1] FOREIGN KEY([FK_Data_de_Vencimento])
REFERENCES [dbo].[DateDim] ([PK_Date])
GO
ALTER TABLE [dbo].[Contas_a_Pagar] CHECK CONSTRAINT [Contas_a_Pagar-DateDim1]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-DateDim1'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVRelation', @value=N'Contas_a_Pagar-DateDim1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-DateDim1'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-DateDim1'
GO
/****** Object:  ForeignKey [Contas_a_Pagar-EmpresaDim]    Script Date: 12/02/2010 22:45:21 ******/
ALTER TABLE [dbo].[Contas_a_Pagar]  WITH CHECK ADD  CONSTRAINT [Contas_a_Pagar-EmpresaDim] FOREIGN KEY([FK_Empresa])
REFERENCES [dbo].[EmpresaDim] ([PK_Empresa])
GO
ALTER TABLE [dbo].[Contas_a_Pagar] CHECK CONSTRAINT [Contas_a_Pagar-EmpresaDim]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-EmpresaDim'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVRelation', @value=N'Contas_a_Pagar-EmpresaDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-EmpresaDim'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-EmpresaDim'
GO
/****** Object:  ForeignKey [Contas_a_Pagar-FormaPagtoDim]    Script Date: 12/02/2010 22:45:21 ******/
ALTER TABLE [dbo].[Contas_a_Pagar]  WITH CHECK ADD  CONSTRAINT [Contas_a_Pagar-FormaPagtoDim] FOREIGN KEY([FK_Forma_de_Pagamento])
REFERENCES [dbo].[FormaPagtoDim] ([PK_FormaPagto])
GO
ALTER TABLE [dbo].[Contas_a_Pagar] CHECK CONSTRAINT [Contas_a_Pagar-FormaPagtoDim]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-FormaPagtoDim'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVRelation', @value=N'Contas_a_Pagar-FormaPagtoDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-FormaPagtoDim'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-FormaPagtoDim'
GO
/****** Object:  ForeignKey [Contas_a_Pagar-TituloCPDim]    Script Date: 12/02/2010 22:45:21 ******/
ALTER TABLE [dbo].[Contas_a_Pagar]  WITH CHECK ADD  CONSTRAINT [Contas_a_Pagar-TituloCPDim] FOREIGN KEY([FK_Titulo_a_Pagar])
REFERENCES [dbo].[TituloCPDim] ([PK_TituloCP])
GO
ALTER TABLE [dbo].[Contas_a_Pagar] CHECK CONSTRAINT [Contas_a_Pagar-TituloCPDim]
GO
EXEC sys.sp_addextendedproperty @name=N'AllowGen', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-TituloCPDim'
GO
EXEC sys.sp_addextendedproperty @name=N'DSVRelation', @value=N'Contas_a_Pagar-TituloCPDim' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-TituloCPDim'
GO
EXEC sys.sp_addextendedproperty @name=N'Project', @value=N'f995dd3d-fc0a-4d73-af19-b577380e9bc5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contas_a_Pagar', @level2type=N'CONSTRAINT',@level2name=N'Contas_a_Pagar-TituloCPDim'
GO
