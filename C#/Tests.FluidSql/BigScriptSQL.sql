-- End to end scenario script

USE [Test];

DECLARE @IntValue INT = 0;
DECLARE @StrValue NVARCHAR(MAX);
DECLARE @StrValue2 NVARCHAR(100);

SET @StrValue = 'Foo';
SELECT  @StrValue2 = 'Bar';

-- Table/index

IF ( OBJECT_ID('dbo.FooTable') IS NULL )
    CREATE TABLE [dbo].[FooTable]
        (
          [somekey] BIGINT NOT NULL
                           IDENTITY ,
          [BigInt] BIGINT ,
          [Binary] BINARY(100) ,
          [Bit] BIT NOT NULL ,
          [Char] CHAR(100) ,
          [DateTime] DATETIME DEFAULT ( GETDATE() ) ,
          [Decimal] DECIMAL(12, 2) ,
          [Float] FLOAT ,
          [Image] IMAGE ,
          [Int] INT ,
          [Money] MONEY ,
          [NChar] NCHAR(100) ,
          [NText] NTEXT ,
          [NVarChar] NVARCHAR(100) ,
          [Real] REAL ,
          [UniqueIdentifier] UNIQUEIDENTIFIER ,
          [SmallDateTime] SMALLDATETIME ,
          [SmallInt] SMALLINT ,
          [SmallMoney] SMALLMONEY ,
          [Text] TEXT ,
          [Timestamp] TIMESTAMP ,
          [TinyInt] TINYINT ,
          [VarBinary] VARBINARY(100) ,
          [VarChar] VARCHAR(100) ,
          [Variant] SQL_VARIANT ,
          [Xml] XML ,
          [Date] DATE ,
          [Time] TIME ,
          [DateTime2] DATETIME2(7) ,
          [DateTimeOffset] DATETIMEOFFSET(2) ,
          PRIMARY KEY CLUSTERED ( [somekey] ASC, [Bit] DESC ) ,
          INDEX [IX_Secondary] NONCLUSTERED /*HASH*/ ( [DateTime] )
        );
    

IF ( OBJECT_ID('tempdb..FooTableTemp') IS NULL )
    CREATE TABLE #FooTableTemp
        (
          [id] INT PRIMARY KEY
                   IDENTITY
                   NOT NULL
        );

DECLARE @FooTableVar TABLE
    (
      [id] INT PRIMARY KEY
               IDENTITY
               NOT NULL
    );






IF ( OBJECT_ID('tempdb..FooTableTemp') IS NOT NULL )
    DROP TABLE #FooTableTemp

IF ( OBJECT_ID('dbo.FooTable') IS NOT NULL )
    DROP TABLE [dbo].[FooTable]	