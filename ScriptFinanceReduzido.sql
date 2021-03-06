USE [FinanceReduzido]
GO
/****** Object:  Table [dbo].[CotacaoDolar]    Script Date: 08/09/2012 14:59:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CotacaoDolar](
	[id] [int] NOT NULL,
	[data] [smalldatetime] NULL,
	[valor] [decimal](10, 8) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PETR4]    Script Date: 08/09/2012 14:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PETR4](
	[ID] [int] NOT NULL,
	[IDCONTEUDO] [int] NULL,
	[NOMERESUMIDO] [varchar](12) NULL,
	[PRECOABERTURA] [decimal](7, 2) NULL,
	[PRECOMAX] [decimal](7, 2) NULL,
	[PRECOMIN] [decimal](7, 2) NULL,
	[PRECOMED] [decimal](7, 2) NULL,
	[TOTALNEGO] [int] NULL,
	[QUANTIDADETOTALNEGO] [int] NULL,
	[VALORTOTALNEGO] [decimal](15, 2) NULL,
	[DATAGERACAO] [smalldatetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
