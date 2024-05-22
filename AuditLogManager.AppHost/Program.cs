var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AuditLogManager>("auditlogmanager");

builder.AddProject<Projects.AuditLogManager_Sample_AspNetCore>("auditlogmanager-sample-aspnetcore");

builder.Build().Run();
