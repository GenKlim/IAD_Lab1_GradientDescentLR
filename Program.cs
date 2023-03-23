using static System.Reflection.Assembly;
using static System.IO.Path;

namespace ГрадиентныйСпускЛР {
    internal class Program {
        static void Main() {
            const string path = "ex1data1.txt";
            if (LoadData(GetDirectoryName(GetExecutingAssembly().Location) + "\\" + path, out List<List<double>> data)) {
                Random rnd = new Random();
                double thetta1 = rnd.Next(0, 100);
                double thetta2 = rnd.Next(0, 100);
                //Обучающая выборка.
                List<double> x = new List<double>(); //{ 1, 2, 3, 4 }
                List<double> y = new List<double>(); //{ 5, 7, 9, 12 }
                List<double> r = new List<double>();
                //Контрольная выборка.
                List<double> xControl = new List<double>();
                List<double> yControl = new List<double>();
                List<double> rControl = new List<double>();
                //Деление на обучающую и тестовую выборки (3:1).
                int CountTrain = (int)(data.Count * 0.75);
                for (int i = 0; i < CountTrain; i++) {
                    x.Add(data[i][0]); y.Add(data[i][1]);
                }
                for (int i = CountTrain; i < data.Count; i++) {
                    xControl.Add(data[i][0]); yControl.Add(data[i][1]);
                }
                //Средние квадратические ошибки.
                double? MSE = null;
                double? MSEControl = null;
                //Скорость обучения.
                double learningRate = 0.01;
                //Критерии остановки.
                double Epsilon = 0.0001;
                int iterations = 50000;
                //Градиентный спуск.
                for (int i = 0; i < iterations; i++) {
                    double theta1Derivative = 1.0 / x.Count * x.Zip(y, (x, y) => thetta2 * x + thetta1 - y).Sum();
                    double theta2Derivative = 1.0 / x.Count * x.Zip(y, (x, y) => (thetta2 * x + thetta1 - y) * x).Sum();
                    thetta2 -= learningRate * theta2Derivative;
                    thetta1 -= learningRate * theta1Derivative;
                    if (Math.Abs(theta1Derivative) < Epsilon && Math.Abs(theta2Derivative) < Epsilon) break;
                }
                //Высчитывание ошибки.
                if (x.Count == y.Count) {
                    MSE = 0.0;
                    MSEControl = 0.0;
                    for (int i = 0; i < x.Count; i++) {
                        r.Add(thetta1 + (thetta2 * x[i]));
                        MSE += Math.Pow(r[i] - y[i], 2);
                    }
                    for (int i = 0; i < xControl.Count; i++) {
                        rControl.Add(thetta1 + (thetta2 * xControl[i]));
                        MSEControl += Math.Pow(rControl[i] - yControl[i], 2);
                    }
                    MSE /= r.Count;
                    MSEControl /= rControl.Count;
                }
                else {
                    for (int i = 0; i < x.Count; i++) {
                        r.Add(thetta1 + (thetta2 * x[i]));
                        rControl.Add(thetta1 + (thetta2 * xControl[i]));
                    }
                }
                Console.WriteLine("Результаты по обучению (" + CountTrain + "):\ny: " + string.Join("; ", y) + ".\n\nr: " + string.Join("; ", r) + ".");
                Console.WriteLine("\nРезультаты по тесту (" + (data.Count - CountTrain) + "):\ny: " + string.Join("; ", yControl) + ".\n\nr: " + string.Join("; ", rControl) + ".");
                Console.WriteLine("\n===============================");
                Console.WriteLine("\nНаклон (О2): " + thetta2 + ".");
                Console.WriteLine("Смещение (О1): " + thetta1 + ".");
                Console.WriteLine("\nОбщая ошибка (MSE) обучающей выборки: " + (MSE.HasValue ? MSE.Value : "Неопределена") + ".\nОбщая ошибка (MSE) контрольной выборки: " + (MSEControl.HasValue ? MSEControl.Value : "Неопределена") + ".");
            }
            else {
                Console.WriteLine("Не удалось загрузить \"" + path + "\".");
            }
            Console.WriteLine("\nДля продолжения нажмите на любую клавишу...");
            Console.ReadKey();
        }
        public static bool LoadData(string dataPath, out List<List<double>> data) {
            data = new List<List<double>>();
            if (!File.Exists(dataPath)) return false;
            string[] rawData = File.ReadAllLines(dataPath);
            foreach (var line in rawData) {
                var sp = line.Split(",");
                List<double> values = new List<double>();
                foreach (var el in sp) values.Add(ParseDouble(el));
                data.Add(values);
            }
            return true;
        }
        static double ParseDouble(string str) => double.Parse(str.Replace(".", ","));
    }
}