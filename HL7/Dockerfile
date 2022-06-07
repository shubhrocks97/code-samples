FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY ["ONQHL7.QueueDataProcessor/ONQHL7.QueueDataProcessor.csproj", "ONQHL7.QueueDataProcessor/"]
COPY ["ONQHL7.GlobalDb/ONQHL7.GlobalDb.csproj", "ONQHL7.GlobalDb/"]
COPY ["ONQHL7.PluginInterfaces/ONQHL7.PluginInterfaces.csproj", "ONQHL7.PluginInterfaces/"]

RUN dotnet restore "ONQHL7.QueueDataProcessor/ONQHL7.QueueDataProcessor.csproj"

#RUN mkdir -p Logfiles
#RUN mkdir -p HL7Messages
#RUN mkdir -p PluginDllFiles
#RUN mkdir -p MessageTemplate


#COPY ["ONQHL7.MessageGenerator_V1/bin/Debug/net5.0/ONQHL7.MessageGenerator_V1.dll", "../PluginDllFiles"]
#COPY ["ONQHL7.NotificationSender_V1/bin/Debug/net5.0/ONQHL7.ONQHL7.NotificationSender_V1.dll", "../PluginDllFiles"]
#COPY ["ONQHL7.NotificationSender_V1/ONQHL7.MessageGenerator_V1/MessageTemplate/COVID_message_negative.txt", "../MessageTemplate"]
#COPY ["ONQHL7.NotificationSender_V1/ONQHL7.MessageGenerator_V1/MessageTemplate/COVID_message_positive.txt", "../MessageTemplate"]


COPY . .
WORKDIR "/src/ONQHL7.QueueDataProcessor"
RUN dotnet build "ONQHL7.QueueDataProcessor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ONQHL7.QueueDataProcessor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["ONQHL7.MessageGenerator_V1.dll", "wwwroot/PluginDllFiles/"]
COPY ["ONQHL7.NotificationSender_V1.dll", "wwwroot/PluginDllFiles/"]
COPY ["COVID_message_negative.txt", "wwwroot/MessageTemplate/"]
COPY ["COVID_message_positive.txt", "wwwroot/MessageTemplate/"]
WORKDIR /app/wwwroot/Logfiles
WORKDIR /app/wwwroot/HL7Messages
WORKDIR /app
ENTRYPOINT ["dotnet", "ONQHL7.QueueDataProcessor.dll"]