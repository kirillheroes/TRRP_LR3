using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace Server
{
	public class CatInfoServiceCore : CatInfoService.CatInfoServiceBase
	{
		private readonly object _locker;
		public CatInfoServiceCore()
		{
			//создаём новый объект блокировщика,
			//необходимый для блокировки потенциального выполнения действия несколькими потоками
			_locker = new object();
		}

		public override Task<CatInfoString> getCatInfo(Cat request, ServerCallContext context)
		{
			string response_text = ConstuctResponseMsg(request);
			var resulting_string = new CatInfoString { OutputText = response_text };

			lock (_locker) //блокируем вывод в консоль, чтобы доступ был у одного потока одновременно
			{
				OutputResultsInfo(request, response_text);
			}
			return Task.FromResult(resulting_string);
		}

		private void OutputResultsInfo(Cat cat, string result_string)
		{
			Console.WriteLine();
			Console.WriteLine($"Имя: {cat.Name}");
			Console.WriteLine($"Возраст: {Convert.ToString(cat.Age)}");
			Console.WriteLine($"Порода: {cat.Kind}");
			Console.WriteLine($"Пол: {Convert.ToString(cat.Sex)}");

			Console.WriteLine();
			Console.WriteLine("Результат:");
			Console.WriteLine(result_string);

			Console.WriteLine();
			Console.WriteLine("Для окончания работы нажмите любую клавишу...");
		}

		private string ConstuctResponseMsg(Cat request)
		{
			string name = Convert.ToString(request.Name);
			int age = request.Age;
			string kind = Convert.ToString(request.Kind);
			string sex = request.Sex;

			if (sex == "Кошечка")
				return "Ваша кошка " + name + " с породой " + kind + " уже в возрасте " + Convert.ToString(age) + "-х лет!";
			else
				return "Этот " + Convert.ToString(age) + "-летний кот " + name + " относится к породе " + kind;
		}
	}
}
