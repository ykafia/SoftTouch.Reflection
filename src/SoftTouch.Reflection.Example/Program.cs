// See https://aka.ms/new-console-template for more information
using SoftTouch.Reflection.Example;
using SoftTouch.Reflection.Core;
Console.WriteLine("Hello, World!");

static void Reflect<T>(in T data)
    where T : IReflectable
{
    Console.WriteLine(data.Get<string>("Name"));
}


var person = new Person("John", 5);

Reflect(person);