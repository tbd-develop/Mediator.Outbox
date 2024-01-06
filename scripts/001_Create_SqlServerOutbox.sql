create table dbo.NotificationOutbox
(
    Id            int identity constraint NotificationOutbox_pk primary key,
    Type          nvarchar( max) not null,
    Content       nvarchar( max) not null,
    DateAdded     datetime default getutcdate() not null,
    DateProcessed datetime
)
    go

create index NotificationOutbox_DateAdded_index
    on dbo.NotificationOutbox (DateAdded)
    go

