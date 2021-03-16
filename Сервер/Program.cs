using Grpc.Core;
using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;
using System;
using System.Threading.Tasks;
using System.Configuration;

namespace Server
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			//считываем IP-адрес и порт для сервера
			var IP_from_config = ConfigurationManager.AppSettings.Get("IP_address");
			int PORT_from_config = int.Parse(ConfigurationManager.AppSettings.Get("port"));

			//настраиваем сервер
			var reflectionService = new ReflectionServiceImpl(CatInfoService.Descriptor, ServerReflection.Descriptor);
			Grpc.Core.Server server = new Grpc.Core.Server
			{
				Services = 
				{
					//устанавливаем обработчик для нашего сервиса
					//им будет являться обработчик из proto3
					CatInfoService.BindService(new CatInfoServiceCore()),
					ServerReflection.BindService(reflectionService)
				}, 
				//записываем наш порт и IP
				Ports = { new ServerPort(IP_from_config, PORT_from_config, ServerCredentials.Insecure) }
			};

			//запускаем сервер
			Console.WriteLine("Запускаю сервер...");
			try
            {
				server.Start();
			}

            catch (Exception e)
            {
				//вывод ошибок на консоль в случае неудачного запуска
				//пример возникновения: неправильный порт и/или IP
				Console.WriteLine(e.Message);
				Console.ReadKey();
			}

			//в случае успешного запуска

			Console.WriteLine("Сервер запущен!");

			//выводим адрес и порт, чтобы убедиться в их корректности
			Console.WriteLine($"Адрес: {IP_from_config}");
			Console.WriteLine($"Порт: {PORT_from_config}");
			Console.WriteLine("Ожидаю данные от Клиентской части... ...");

			//работаем бесконечно...
			//для окончания необходимо нажать любую клавишу, установив фокус на консоль сервера
			Console.WriteLine("Для окончания работы нажмите любую клавишу...");
			Console.ReadKey();

			Console.WriteLine();
			Console.WriteLine("Закрываю сервер (асинхронно) ... ...");
            try
            {
				await server.ShutdownAsync();
			}
			catch (Exception e)
			{
				//на всякий случай возникновения ошибки
				Console.WriteLine(e.Message);
				Console.ReadKey();
			}
			
			Console.WriteLine("Сервер успешно закрыт!");
			Console.ReadKey();
		}
	}
}
