using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Ideon.Tests.Unit;

public class CollaborationRequestServiceTests
{
	[Fact]
	public void CollaborationRequestService_IsAvailable()
	{
		var serviceType = FindServiceType();

		Assert.NotNull(serviceType);
		Assert.True(serviceType!.IsClass);
	}

	[Fact]
	public void CollaborationRequestService_ExposesPublicOperations()
	{
		var serviceType = FindServiceType();

		Assert.NotNull(serviceType);
		Assert.Contains(serviceType!.GetMethods(BindingFlags.Instance | BindingFlags.Public),
			method => method.DeclaringType != typeof(object));
	}

	private static Type? FindServiceType()
	{
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			try
			{
				var match = assembly.GetTypes().FirstOrDefault(type =>
					type.Name == "CollaborationRequestService");
				if (match is not null)
				{
					return match;
				}
			}
			catch (ReflectionTypeLoadException exception)
			{
				var match = exception.Types.FirstOrDefault(type =>
					type?.Name == "CollaborationRequestService");
				if (match is not null)
				{
					return match;
				}
			}
		}

		return null;
	}
}
