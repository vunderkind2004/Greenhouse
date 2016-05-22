CREATE TABLE [dbo].[SensorData]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [EventDateTime] DATETIME NOT NULL, 
    [SensorId] INT NOT NULL, 
    [Value] FLOAT NOT NULL, 
    CONSTRAINT [FK_SensorData_Sensor] FOREIGN KEY (SensorId) REFERENCES Sensor(Id), 
)
