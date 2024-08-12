using Finance.Application.Abstractions.Messaging;
using Finance.Domain.Abstracts;
using Finance.Infrastructure;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Reflection;

namespace Finance.ArchitectureTests;

public class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(Entity).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(IBaseCommand).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(ApplicationDbContext).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
}