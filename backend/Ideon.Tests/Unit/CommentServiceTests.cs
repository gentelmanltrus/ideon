using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Ideon.Tests.Unit;
/// Unit tests for the CommentService class.
public class CommentServiceTests
{
    ///fact attribute indicates that the method is a test method that should be executed by the test runner.
	[Fact]
	public void CommentService_is_defined_in_a_referenced_assembly()
	{
		var serviceType = typeof(CommentServiceTests).Assembly
			.GetReferencedAssemblies()
			.Select(Assembly.Load)
			.SelectMany(GetTypes)
			.FirstOrDefault(type => type.Name == "CommentService");

		Assert.NotNull(serviceType);
		Assert.True(serviceType!.IsClass);
	}

	[Fact]
	public void CommentService_exposes_public_instance_operations()
	{
		var serviceType = typeof(CommentServiceTests).Assembly
			.GetReferencedAssemblies()
			.Select(Assembly.Load)
			.SelectMany(GetTypes)
			.FirstOrDefault(type => type.Name == "CommentService");

		Assert.NotNull(serviceType);
		Assert.Contains(serviceType!.GetMethods(BindingFlags.Instance | BindingFlags.Public),
			method => method.DeclaringType != typeof(object));
	}

	private static Type[] GetTypes(Assembly assembly)
	{
		try
		{
			return assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException exception)
		{
			return exception.Types.Where(type => type is not null).Cast<Type>().ToArray();
		}
	}
}
