var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AuditLogManager>("auditlogmanager");

builder.Build().Run();
