var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AuditLogManager>("audit-log-manager");

builder.AddProject<Projects.AuditLogManager_Sample_AspNetCore>("aspnetcore-audit-log-manager-sample");

builder.Build().Run();
