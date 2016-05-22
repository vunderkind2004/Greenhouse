CREATE TABLE [dbo].[Sensor]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL, 
	[SensorTypeId] INT NOT NULL,
    [Location] NVARCHAR(MAX) NULL, 
    [DeviceId] INT NOT NULL, 
    CONSTRAINT [FK_Sensor_SensorType] FOREIGN KEY ([SensorTypeId]) REFERENCES SensorType(Id),
	CONSTRAINT [FK_Sensor_Device] FOREIGN KEY (DeviceId) REFERENCES Device(Id)
    
)
