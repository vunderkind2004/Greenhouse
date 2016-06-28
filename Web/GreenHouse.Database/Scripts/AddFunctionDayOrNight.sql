IF OBJECT_ID (N'dbo.DayOrNight', N'FN') IS NOT NULL
    DROP FUNCTION dbo.DayOrNight;
GO
CREATE FUNCTION dbo.DayOrNight (@DATE datetime, @sunsetHour int, @sunrizeHour int)
RETURNS datetime
WITH EXECUTE AS CALLER
AS
BEGIN
     DECLARE @hh int;
     DECLARE @DateOut datetime;
     SET @hh= DATEPART(hh,@DATE);
     SET @DateOut =(CONVERT(DATETIME,str(DATEPART(yyyy,@DATE)) + '-' + str(DATEPART(mm,@DATE)) + '-' + str(DATEPART(dd,@DATE))));
     IF (@hh>=@sunsetHour) 
         SET @DateOut = @DateOut + 1;  
     ELSE
		IF (@hh>=@sunrizeHour)
			SET @DateOut = @DateOut + 0.5;  		   	
     RETURN(@DateOut);
END;
GO
SET DATEFIRST 1;
SELECT dbo.DayOrNight(CONVERT(DATETIME,'12/26/2004 19:59:59',101),20,8) AS 'day or night';