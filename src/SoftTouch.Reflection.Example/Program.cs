// See https://aka.ms/new-console-template for more information
using SoftTouch.Reflection.Example;
using SoftTouch.Reflection.Core;
Console.WriteLine("Hello, World!");

static void Reflect<T>(ref T data)
    where T : struct, IReflectable
{
    data.Set("Name", "Jane");
    Console.WriteLine(data.Get<string>("Name"));
}


var person = new Person("John", 5);

foreach(var p in Person.Setters)
    Console.WriteLine(p);

Reflect(ref person);

Console.WriteLine(person.Name);