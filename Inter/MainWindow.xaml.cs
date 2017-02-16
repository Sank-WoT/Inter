namespace Inter
{
using System;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Permissions;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string[]> nameP = new List<string[]>();
        List<string> typeP = new List<string>();
        // Список доступных типов
        static List<string> AvailableTypes = new List<string> { "string", "int", "double", "bool" };

        // Свойства для хранения значений при парсинге типов int и double
        // Значения для типа double
        static double DoubleValue { get; set; }
        // Значения для типа int
        static int IntValue { get; set; }

        // Значения для типа int
        static bool BoolValue { get; set; }

        // Свойство для определения типа рассматриваемого поля
        // 0 - double, 1 - int, 2 - string
        static int whatType { get; set; }

        // Функция получения типа переменной для куска распарсенной строки
        static Type GetElementType(string inputString)
        {
            // создаем обращениеы
            Type type = null;

            // Ввиду специфичности полных имен типов, выбран самый предпочтительный
            // вариант формирования последних, исходя из имеющегося списка
            if (inputString.Equals("string"))
                type = typeof(string);

            if (inputString.Equals("int"))
                type = typeof(int);

            if (inputString.Equals("double"))
                type = typeof(double);

            if (inputString.Equals("bool"))
                type = typeof(bool);
            // вернуть тип
            return Type.GetType(type.FullName, false, true);
        }

        // Функция удаления лишних пробелов в названии переменной
        static string RemoveSpaces(string inputString)
        {
            inputString = inputString.Replace("  ", string.Empty);
            inputString = inputString.Trim().Replace(" ", string.Empty);

            return inputString;
        }

        // Функция отображения результатов создания полей
        public string ShowResults(FieldInfo[] fi, Object ob)
        {
            string complete = "";
            if (fi.Length > 0)
            {
                // Вывод результатов по основным полям
                Console.WriteLine("\nAvailable fields:");
                for (int i = 0; i < fi.Length; i++)
                {
                    
                    Console.WriteLine("Name            : {0}", fi[i].Name);
                    Console.WriteLine("Value           : {0}", fi[i].GetValue(ob));
                    Console.WriteLine("Declaring Type  : {0}", fi[i].DeclaringType);
                    Console.WriteLine("IsPublic        : {0}", fi[i].IsPublic);
                    Console.WriteLine("MemberType      : {0}", fi[i].MemberType);
                    Console.WriteLine("FieldType       : {0}", fi[i].FieldType); 
                    complete += $"Name            : {fi[i].Name}\n Value           : {fi[i].GetValue(ob)}\n Declaring Type  : {fi[i].DeclaringType}\n IsPublic        : {fi[i].IsPublic}\n MemberType      : {fi[i].MemberType}\n FieldType       : {fi[i].FieldType}\n";
                }
               
            }
            else
                complete = "No fields found!";
            return complete;
        } 

        // Функция установки значений для созданных полей
        /*
        static void SetParameters(FieldInfo[] fi, Object ob)
        {
            if (fi.Length > 0)
            {
                // Проходимся по доступным полям и спрашиваем, нужно ли задать значение
                for (int i = 0; i < fi.Length; i++)
                {
                    Console.Write("\nSet parameter to '{0}'? ", fi[i].Name);

                    string answer = Console.ReadLine();

                    // Если нужно, считываем строку.
                    if (answer.Equals("y") || answer.Equals("Y"))
                    {
                        Console.Write("Value = ");
                        string value = Console.ReadLine();

                        // И парсим для получения значения в зависимости от типа
                        // входной строки, исключая возможные ошибки
                        if (ParseInputValues(value, fi[i].FieldType.ToString()))
                        {
                            // В зависимости от типа, заносим в рассматриваемое поле введенное значение
                            switch (whatType)
                            {
                                case 1:
                                    fi[i].SetValue(ob, IntValue);
                                    break;
                                case 2:
                                    fi[i].SetValue(ob, value);
                                    break;

                                case 3:
                                    fi[i].SetValue(ob, BoolValue);
                                    break;

                                default:
                                    fi[i].SetValue(ob, DoubleValue);
                                    break;
                            }
                        }
                    }
                }

                Console.WriteLine();
            }
            else
                Console.WriteLine("No fields found!");
        }
        */

        // Функция-парсер входной строки
        static IDictionary<string, string> ParseInputString(string inputString, char separator)
        {
            // Коллекция значений полей. Ключ в данном случае является уникальным значением переменной.
            IDictionary<string, string> dynamicPair = new Dictionary<string, string>();

            // Массив разбиений исходной строки. Разделитель - вводится пользователем.
            string[] splittedInputString = inputString.Split(separator);

            // Проходимся по списку доступных типов
            foreach (string type in AvailableTypes)
            {
                // Получаем разбиение исходной строки
                foreach (string tmpString in splittedInputString)
                {
                    // Если в разбиении присутствует доступный тип, вытягиваем подстроку, содержащую название
                    // переменной, исключая пробелы. Для соблюдения правил кода, была реализована прикрутка "_"
                    // в случае, если название переменной начинается с числа.
                    if (tmpString.Contains(type))
                    {
                        string tmpSubString = tmpString.Substring(tmpString.IndexOf(type, 0) + type.Length + 1);
                        tmpSubString = RemoveSpaces(tmpSubString);

                        if (Char.IsNumber(tmpSubString.ToCharArray()[0]))
                            tmpSubString = tmpSubString.Insert(0, "_");

                        // Если перменной с рассматриваемым именем не существует, заносим данные по анализируемой
                        // подстроке в коллекцию
                        if (!dynamicPair.ContainsKey(tmpSubString))
                            dynamicPair.Add(tmpSubString, type);
                    }
                }
            }

            return dynamicPair;
        }

        // Функция-парсер строки, вводимой для установки значения рассматриваемого поля
        static bool ParseInputValues(string inputString, string fName)
        {
            bool parseResult = false;

            // В зависимости от типа проводим анализ входной строки с занесением
            // соответствующих значений в свойства исходного класса
            try
            {
                if (fName.Equals(typeof(double).FullName))
                {
                    double parseValue;
                    parseResult = double.TryParse(inputString, out parseValue);

                    DoubleValue = parseValue;
                    whatType = 0;
                }

                if (fName.Equals(typeof(int).FullName))
                {
                    int parseValue;
                    parseResult = int.TryParse(inputString, out parseValue);

                    IntValue = parseValue;
                    whatType = 1;
                }

                if (fName.Equals(typeof(string).FullName))
                {
                    whatType = 2;
                    parseResult = true;
                }

                if (fName.Equals(typeof(bool).FullName))
                {
                    bool parseValue;
                    parseResult = bool.TryParse(inputString, out parseValue);

                    BoolValue = parseValue;
                    whatType = 3;
                }

            }
            catch (Exception)
            {
                Console.WriteLine("Incorrect value!");
            }

            return parseResult;
        }

        // Функция динамического создания элементов при помощи рефлексии
        public static Type CreateDynamicClassType(AppDomain currentDomain, string inputString, char separator)
        {
            // Заносим в коллекцию распарсенную входную строку
            IDictionary<string, string> parsedValueString = ParseInputString(inputString, separator);

            // Список созданных полей. По сути в данной версии ПО выполняет только функцию хранения.
            List<FieldBuilder> fbList = new List<FieldBuilder>();

            // Создаем Assembly.
            AssemblyName DynamicAssemblyName = new AssemblyName();
            DynamicAssemblyName.Name = "DynamicVariablesAssembly";
            AssemblyBuilder DynamicAssembly =
                           currentDomain.DefineDynamicAssembly(DynamicAssemblyName, AssemblyBuilderAccess.Run);

            // Создание динамического модуля в динамической Assembly.
            ModuleBuilder DynamicModuleBuilder = DynamicAssembly.DefineDynamicModule("DynamicVariablesModule");

            // Определяем public класс "DynamicVariablesClass" в Assembly.
            TypeBuilder DynamicTypeBuilder = DynamicModuleBuilder.DefineType("DynamicVariablesClass", TypeAttributes.Public);

            // Если чего-то напарсили, создаем поля.
            if (parsedValueString.Count > 0)
            {
                // Проходимся по коллекции
                foreach (KeyValuePair<string, string> kvp in parsedValueString)
                {
                    // Получаем тип создаваемого поля
                    Type type = GetElementType(kvp.Value);

                    // Если тип существует, определяем public поле заданного типа с заданным именем
                    if (type != null)
                        fbList.Add(DynamicTypeBuilder.DefineField(kvp.Key, type, FieldAttributes.Public));
                }
            }

            return DynamicTypeBuilder.CreateType();
        }

   
        public MainWindow()
        {
            InitializeComponent();
        }

        public void CreatePerem(string kod)
        {
            try
            {
                // Разделитель
                char separator = ' ';

                // Вводим разделитель для разбора строки
                while (separator.Equals(' '))
                {
                    Console.Write("Input string separator (except space): ");
                    separator = ';';

                    if (separator.Equals(' '))
                        Console.WriteLine("Icorrect separator!");
                }

                // Вводим исходную строку
                Console.WriteLine("\nInput string (format: [type][random symbol][variable name][separator - {0}]): ", separator);
                string inputString = kod;
                // Получаем тип
                Type dynamicVariablesType = CreateDynamicClassType(Thread.GetDomain(), inputString, separator);

                // Создаем объект
                Object dynamicVariablesObject = Activator.CreateInstance(dynamicVariablesType);

                // Получаем список доступных полей
                FieldInfo[] fi = dynamicVariablesType.GetFields();

                // Предлагаем задать параметры
                // SetParameters(fi, dynamicVariablesObject);

                // Выводим результаты

                Complete.Text += ShowResults(fi, dynamicVariablesObject);
            }
            catch (Exception eX)
            {
                Console.WriteLine("Exception caught: " + eX.Message);
            }
        } 

       [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        private void Run_Click(object sender, RoutedEventArgs e)
        {
            Error.Text = "";
            Dictionary<string, string> peremen = new Dictionary<string, string>();
            Terminal ter = new Terminal(Main.Text);
            // отсекаем
            ter.otsek(Error);
            List<string> podStr;
            podStr = ter.getpodStr();
            podStr = ter.VarGet(Error, podStr);
            if(null != podStr)
            {
                podStr = ter.PeremandType(Error, podStr);
            }
            Console.WriteLine("Проверка");
            foreach (string s in podStr)
            {
                Console.WriteLine(s);
            }
            if (null != podStr)
            {
                podStr = ter.ot(Error, podStr);
            }
            Console.WriteLine("Проверка 2");

            List<string[]> nameP = new List<string[]>();
            List<string> typeP = new List<string>();

            for (int s = 0; s < podStr.Count; s++)
            {
                string[] namePeremen;
                if (0 == s%2)
                {
                    // массив переменных
                    namePeremen = podStr[s].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    nameP.Add(namePeremen);
                    foreach (string name in namePeremen)
                    {
                        Console.WriteLine($"переменная {name}");
                    }
                }

                else
                {
                    podStr[s] = podStr[s].Trim(',');
                    podStr[s] = podStr[s].Trim();
                    Console.WriteLine($"тип {podStr[s]}");
                    typeP.Add(podStr[s]);
                }
            }

            for(int a = 0; a < nameP.Count; a++ )
            {
                string[] namePereme = nameP[a];
                foreach(string name in namePereme)
                {
                  CreatePerem($"{typeP[a]} {name}");
                }   
            }
        }

    }
}
