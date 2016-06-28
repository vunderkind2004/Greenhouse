CREATE TABLE [dbo].[SensorData] (
    [Id]            INT        IDENTITY (1, 1) NOT NULL,
    [EventDateTime] DATETIME   NOT NULL,
    [SensorId]      INT        NOT NULL,
    [Value]         FLOAT (53) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SensorData_Sensor] FOREIGN KEY ([SensorId]) REFERENCES [dbo].[Sensor] ([Id])
);



GO
CREATE NONCLUSTERED INDEX [IX_EventTime]
    ON [dbo].[SensorData]([EventDateTime] ASC);

