//тут у нас либы всякие
using WordTableGenerator;//подключаю своё пространство имён 
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

using TestTask;
using DocumentFormat.OpenXml.Bibliography;
class Program
{



    //инициация переменной для подгрузки именования уровней
    static string xmlFilePath = "";
    static async Task Main(string[] args)
    {
        string url = "https://fias.nalog.ru/WebServices/Public/GetLastDownloadFileInfo"; // урлка для того чтобы на неё гет кинуть
        string directoryName = "ExtractedFiles"; // имя папки для распаковки
        string versionDataParce = "";

        List<List<MegaElem>> vectors = new List<List<MegaElem>>();

        // Заполняем список 17 пустыми векторами, 17 векторов для каждого уровня
        for (int iter = 0; iter < 17; iter++)
        {
            vectors.Add(new List<MegaElem>());
        }


        try
        {
            using (HttpClient client = new HttpClient())
            {
                Console.WriteLine("идет гет запрос");
                // делаем гет запрос
                var response = await client.GetStringAsync(url);

                // парсим джсонку
                Console.WriteLine("парсинг json");
                var json = JObject.Parse(response);
                string garXmlDeltaUrl = json["GarXMLDeltaURL"].ToString();
                versionDataParce = json["TextVersion"].ToString().Replace("БД ФИАС от ", "");
                

                // качаем зипку
                Console.WriteLine("старт скачивания");
                var zipFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "download.zip");
                using (var zipStream = await client.GetStreamAsync(garXmlDeltaUrl))
                using (var fileStream = new FileStream(zipFilePath, FileMode.Create, FileAccess.Write))
                {
                    await zipStream.CopyToAsync(fileStream);
                }
                Console.WriteLine("создание папки для распаковки если существует");
                // Создание папки для распаковки
                string extractPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directoryName);
                Directory.CreateDirectory(extractPath); // Создание папки, если она не существует

                // Проверка, если папка не пуста, удаление всех файлов в ней это для того чтобы не крашило если там уже есть файлы
                if (Directory.EnumerateFileSystemEntries(extractPath).Any())
                {
                    Console.WriteLine("удаление файлов из папки");
                    DirectoryInfo di = new DirectoryInfo(extractPath);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }


                }
                Console.WriteLine("разархивирование");
                // Разархивирование ZIP-файла в созданную папку
                ZipFile.ExtractToDirectory(zipFilePath, extractPath);

                // Удаление загруженного ZIP-файла
                File.Delete(zipFilePath);

                Console.WriteLine($"Файл успешно загружен и разархивирован в '{extractPath}'.");
            }
            // тута получам путь до файла с левелами
            string[] files = Directory.GetFiles("ExtractedFiles", "AS_OBJECT_LEVELS*.XML");
            if (files.Length > 0)
            {
                xmlFilePath = files[0];
                Console.WriteLine("Найден файл: " + xmlFilePath);
            }
            else
            {
                Console.WriteLine("Файл не найден.");
            }

            // Загружаем XML-документ 
            XDocument xDocument = XDocument.Load(xmlFilePath);

            // Создаем словарь для хранения уровней и имен
            Dictionary<string, string> levelNameDictionary = new Dictionary<string, string>();

            // Извлекаем данные из XML
            foreach (var element in xDocument.Descendants("OBJECTLEVEL"))
            {
                string level = element.Attribute("LEVEL")?.Value;
                string name = element.Attribute("NAME")?.Value;

                if (level != null && name != null)
                {
                    levelNameDictionary[level] = name; // Добавляем в словарь
                }
            }

            string baseDirectory = "ExtractedFiles"; // Укажите путь к основной папке
            string[] badLevels = ["10", "11", "12", "9", "17"]; //10 - здание, 11 - помещение, 12 - помещения в помещении, 9 - земельный участок, 17 - машина-место

           

            // Проход по папкам от 01 до 99
            for (int i = 1; i <= 99; i++)
            {
                // Console.WriteLine($"обработка {i} папки"); //дебаг вывод, выводит папку которая сейчас обрабатывается
                string folderName = i.ToString("D2"); // форматирует число в строку с ведущим нулем
                string folderPath = Path.Combine(baseDirectory, folderName); //делает новый путь файла путем сращивания базовой дериктории и имени папки
                

                if (Directory.Exists(folderPath))
                {
                    //поиск XML-файла, берет первый в алфавитном порядке по маске 
                    var xmlFile = Directory.GetFiles(folderPath, "AS_ADDR_OBJ_*.xml").OrderBy(f => f).FirstOrDefault();

                    if (xmlFile != null)
                    {
                        try
                        {
                            //загружаем XML-документ
                            XDocument xDocument2 = XDocument.Load(xmlFile);


                            //извлечение данных, тут создается список объектов MegaElem в который записывается вся нужная инфа с XML файла
                            var objects = from obj in xDocument2.Descendants("OBJECT")
                                          select new MegaElem
                                          {
                                              Level = (string)obj.Attribute("LEVEL"),
                                              Name = (string)obj.Attribute("NAME"),
                                              IsActive = (int)obj.Attribute("ISACTIVE"),
                                              ShortName = (string)obj.Attribute("TYPENAME")
                                          };


                            //проходит по всем элементам и добавляет их в вектора в соответсвии с их уровнем
                            foreach (var megaElem in objects)
                            {
                                //отбор активных и подходящих ( в тз было указано по домам, квартирам, земельным участкам и машиноместам информацию указывать не нужно, дословно я не нашел таких уровней, по этому подобрал более подходящие)
                                if (megaElem.IsActive != 0 && !badLevels.Contains(megaElem.Level))
                                { 
                                    //поскольку уровень не может быть меньше 1 смело вычетаю из него 1 чтобы получить адрес вектора для подходящего уровня
                                    int id = int.Parse(megaElem.Level) - 1;
                                    vectors[id].Add(megaElem);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка при обработке файла {xmlFile}: {ex.Message}");
                        }
                    }

                }

            }


            //создаётся объект класса для генерации отчета в ворд документе
            var word_generator = new TableCreator();
            //используется метод класса который создает файл, в метод передается вектор с векторами объектов MegaElem, levelNameDictionary хранит в себе расшифровку каждого уровня
            word_generator.CreateWordTables(vectors, levelNameDictionary,"Report.docx", versionDataParce);
            /*
            foreach (var elems in vectors)
            {


                 вывод для дебага, выводит уровень, isactive, имя и короткое имя всех объектов которые были спаршены выше
                foreach (var deep_elem in elems)
                {
                    Console.WriteLine($"Уровень: {deep_elem.Level}, активность: {deep_elem.IsActive}, короткое имя: {deep_elem.ShortName}, имя: {deep_elem.Name}");
                }
                
        }
            */
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}
