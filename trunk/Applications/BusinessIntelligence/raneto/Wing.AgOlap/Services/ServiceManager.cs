using System;
using System.Net;
using System.ServiceModel;

namespace Ranet.AgOlap.Services
{
	/// <summary>
	/// Class for create and initialize services.
	/// </summary>
	public static class ServiceManager
	{
		/// <summary>
		/// Creates new service proxy instance.
		/// </summary>
		/// <typeparam name="T">Type of service proxy. Must implement TChannel interface.</typeparam>
		/// <typeparam name="TChannel">Type of service contract interface.</typeparam>
		/// <returns></returns>
		public static T CreateService<T, TChannel>()
			where T : ClientBase<TChannel>, TChannel, new()
			where TChannel : class
		{
			return CreateService<T, TChannel>(null, TimeSpan.MinValue);
		}

		/// <summary>
		/// Creates new service proxy instance.
		/// </summary>
		/// <typeparam name="T">Type of service proxy. Must implement TChannel interface.</typeparam>
		/// <typeparam name="TChannel">Type of service contract interface.</typeparam>
		/// <param name="baseUrl">Base url.</param>
		/// <returns></returns>
		public static T CreateService<T, TChannel>(string baseUrl)
			where T : ClientBase<TChannel>, TChannel, new()
			where TChannel : class
		{
			return CreateService<T, TChannel>(baseUrl, TimeSpan.MinValue);
		}

		/// <summary>
		/// Creates new service proxy instance.
		/// </summary>
		/// <typeparam name="T">Type of service proxy. Must implement TChannel interface.</typeparam>
		/// <typeparam name="TChannel">Type of service contract interface.</typeparam>
		/// <param name="baseUrl">Base url.</param>
		/// <param name="timeout">Timeout.</param>
		/// <returns></returns>
		public static T CreateService<T, TChannel>(string baseUrl, TimeSpan timeout)
			where T : ClientBase<TChannel>, TChannel, new()
			where TChannel : class
		{
			T client = new T();
			InitUrl<T, TChannel>(client, baseUrl);
			if (timeout != TimeSpan.MinValue)
			{
				client.InnerChannel.OperationTimeout = timeout;
			}

			return client;
		}

		private static string m_BaseAddress;
		/// <summary>
		/// Gets base address for current application.
		/// </summary>
		public static string BaseAddress
		{
			get
			{
				if (string.IsNullOrEmpty(m_BaseAddress))
				{
					m_BaseAddress = new Uri(System.Windows.Application.Current.Host.Source, @"../").AbsoluteUri;
				}

				return m_BaseAddress;
			}
		}

		private static void InitUrl<T, TChannel>(T client, string baseUrl)
			where T : ClientBase<TChannel>, TChannel, new()
			where TChannel : class
		{
			Uri uri = client.Endpoint.Address.Uri;
			string srvName = uri.LocalPath.Remove(0, 1);
			int slashIndex = srvName.LastIndexOf('/');
			var baseAddress = baseUrl;

			if (string.IsNullOrEmpty(baseAddress))
			{
				baseAddress = BaseAddress;
			}

			if (slashIndex > 0)
			{
				srvName = srvName.Substring(slashIndex + 1, srvName.Length - slashIndex - 1);
			}

			client.Endpoint.Address = new EndpointAddress(new Uri(new Uri(baseAddress), srvName));

			if (client.Endpoint.Address.Uri.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase))
			{
				if (client.Endpoint.Binding is BasicHttpBinding)
				{
					var basicBinding = (BasicHttpBinding)client.Endpoint.Binding;
					client.Endpoint.Binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport)
					{
						EnableHttpCookieContainer = basicBinding.EnableHttpCookieContainer,
						MaxBufferSize = basicBinding.MaxBufferSize,
						MaxReceivedMessageSize = basicBinding.MaxReceivedMessageSize,
						TextEncoding = basicBinding.TextEncoding,
						Name = basicBinding.Name
					};
				}
				else
				{
					// TODO: CustomBinding, PollingDuplexHttpBinding
					// This should never happen
					throw new InvalidOperationException(
							string.Format("Unknown endpoint binding type '{0}'", client.Endpoint.Binding.GetType()));
				}
			}
		}
	}
}
