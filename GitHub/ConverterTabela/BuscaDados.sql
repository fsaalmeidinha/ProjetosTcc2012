USE [FinanceInvest]
GO

/****** Object:  StoredProcedure [dbo].[SP_BUSCA_DADOS]    Script Date: 04/24/2012 22:59:01 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_BUSCA_DADOS]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SP_BUSCA_DADOS]
GO

USE [FinanceInvest]
GO

/****** Object:  StoredProcedure [dbo].[SP_BUSCA_DADOS]    Script Date: 04/24/2012 22:59:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- AUTHOR:		<AUTHOR,,NAME>
-- CREATE DATE: <CREATE DATE,,>
-- DESCRIPTION:	<DESCRIPTION,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_BUSCA_DADOS]
	@NOMEPAPEL NVARCHAR(20)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @SQL NVARCHAR(500)
			
	SET @SQL = N'SELECT * FROM ' + @NOMEPAPEL
	EXEC SP_EXECUTESQL @SQL
	
END

GO


