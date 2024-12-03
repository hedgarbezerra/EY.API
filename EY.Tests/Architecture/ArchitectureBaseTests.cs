using EY.API;
using EY.Business;
using EY.Domain;
using EY.Infrastructure;
using EY.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EY.Tests.Architecture
{
    public class ArchitectureBaseTests
    {
        protected const string SolutionPrefix = "EY.";

        protected static Assembly TestsAssembly = Assembly.GetExecutingAssembly();
        protected static Assembly DomainAssembly = typeof(DomainAssemblyBinder).Assembly;
        protected static Assembly BusinessAssembly = typeof(BusinessAssemblyBinder).Assembly;
        protected static Assembly InfrastructureAssembly = typeof(InfrastructureAssemblyBinder).Assembly;
        protected static Assembly SharedAssembly = typeof(SharedAssemblyBinder).Assembly;
        protected static Assembly WebApiAssembly = typeof(Program).Assembly;

        protected static IReadOnlyList<Assembly> Assemblies = [DomainAssembly, BusinessAssembly, InfrastructureAssembly, SharedAssembly];
    }
}
