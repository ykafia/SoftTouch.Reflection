# SoftTouch.Reflection

A simple reflection source generator, avoids boxing and is AOT compatible.

First, types that should be reflected must be partial and have the Reflectable attribute.
```csharp
using SoftTouch.Reflection.Core;

namespace SoftTouch.Reflection.Example;

[Reflectable]
public partial struct Person
{
    public string Name { get; set; }
    public int Age { get; }
    public House PersonHouse { get; set; }

    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }
}

[Reflectable]
public partial class House
{
    public string Address { get; }
    internal int Number { get; set; }

}
```

This will implement the interface IReflectable and add some methods and members to access and modify properties of the data.
```csharp
using SoftTouch.Reflection.Example;
using SoftTouch.Reflection.Core;

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
```
