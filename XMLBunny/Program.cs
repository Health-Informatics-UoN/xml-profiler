var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

Console.WriteLine("Enter XML File Name: ");

var fileName = Console.ReadLine();

var filePath = path + $"/{fileName}";

