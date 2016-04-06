CREATE TABLE [dbo].[SensorData]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [EventDateTime] DATETIME NOT NULL, 
    [SensorTypeId] INT NOT NULL, 
    [DeviceId] INT NOT NULL, 
    [Value] FLOAT NOT NULL, 
    CONSTRAINT [FK_SensorData_SensorType] FOREIGN KEY (SensorTypeId) REFERENCES SensorType(Id), 
    CONSTRAINT [FK_SensorData_Device] FOREIGN KEY (DeviceId) REFERENCES Device(Id)  
)
